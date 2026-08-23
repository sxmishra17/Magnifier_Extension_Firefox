// popup.js – Settings panel with multi-language localization

(function () {
  "use strict";

  const zoomSlider    = document.getElementById("zoomSlider");
  const zoomBadge     = document.getElementById("zoomBadge");
  const sizeSlider    = document.getElementById("sizeSlider");
  const sizeBadge     = document.getElementById("sizeBadge");
  const enableToggle  = document.getElementById("enableToggle");
  const posRadios     = document.querySelectorAll("input[name='lensPos']");
  const shapeRadios   = document.querySelectorAll("input[name='lensShape']");
  const openPdfViewer = document.getElementById("openPdfViewer");
  const langSelect    = document.getElementById("langSelect");

  const SIZE_KEYS    = { 1: "small", 2: "medium", 3: "large" };
  const SIZE_TO_INT  = { small: 1, medium: 2, large: 3 };
  const INT_TO_SIZE  = { 1: "small", 2: "medium", 3: "large" };

  let currentSettings = {
    zoom: 1.5,
    lensSize: "medium",
    lensPosition: "right",
    lensShape: "rect",
    language: "auto"
  };

  // Populate Language dropdown
  if (langSelect && window.I18N) {
    langSelect.innerHTML = "";
    for (const [code, name] of Object.entries(window.I18N.SUPPORTED_LANGUAGES)) {
      const opt = document.createElement("option");
      opt.value = code;
      opt.textContent = name;
      langSelect.appendChild(opt);
    }
  }

  // ── Load current settings on popup open ─────────────────────────────────

  browser.tabs
    .query({ active: true, currentWindow: true })
    .then((tabs) => {
      const tabId = tabs && tabs[0] ? tabs[0].id : null;
      const fromStorage = browser.storage.sync.get({
        zoom: 1.5,
        lensSize: "medium",
        lensPosition: "right",
        lensShape: "rect",
        language: "auto"
      });
      const fromBg = tabId != null
        ? browser.runtime.sendMessage({ type: "get-tab-enabled", tabId })
        : Promise.resolve({ enabled: false });

      return Promise.all([fromStorage, fromBg]).then(([s, bg]) => {
        currentSettings = s;

        // Apply language
        if (window.I18N) {
          window.I18N.setLanguage(s.language || "auto");
          if (langSelect) langSelect.value = s.language || "auto";
          applyTranslations();
        }

        zoomSlider.value      = s.zoom;
        enableToggle.checked  = bg.enabled;
        zoomBadge.textContent = formatZoom(s.zoom);
        updateSliderTrack(s.zoom);

        const sizeInt       = SIZE_TO_INT[s.lensSize] || 2;
        sizeSlider.value    = sizeInt;
        updateSizeBadge(sizeInt);
        updateSizeTrack(sizeInt);

        posRadios.forEach((r)  => { r.checked = r.value === s.lensPosition; });
        shapeRadios.forEach((r) => { r.checked = r.value === s.lensShape; });
      });
    })
    .catch(() => {});

  // ── Apply Translations ──────────────────────────────────────────────────

  function applyTranslations() {
    if (!window.I18N) return;
    const t = window.I18N.t;

    const setTxt = (id, key) => {
      const el = document.getElementById(id);
      if (el) el.textContent = t(key);
    };

    setTxt("txtAppTitle", "appTitle");
    setTxt("txtSubtitle", "subtitle");
    setTxt("txtZoomLevel", "zoomLevel");
    setTxt("txtLensSize", "lensSize");
    setTxt("txtSizeSmall", "small");
    setTxt("txtSizeMedium", "medium");
    setTxt("txtSizeLarge", "large");
    setTxt("txtLensShape", "lensShape");
    setTxt("txtCircle", "circle");
    setTxt("txtRectangle", "rectangle");
    setTxt("txtLensPos", "lensPosition");
    setTxt("txtLanguage", "language");
    setTxt("txtPressHotkey", "pressHotkey");
    setTxt("txtDownloadDesktop", "downloadDesktop");
    setTxt("txtSavedAuto", "savedAuto");
    if (openPdfViewer) openPdfViewer.textContent = t("openPdfViewer");

    const curSizeInt = parseInt(sizeSlider.value, 10) || 2;
    updateSizeBadge(curSizeInt);
  }

  function updateSizeBadge(sizeInt) {
    const key = SIZE_KEYS[sizeInt] || "medium";
    if (window.I18N) {
      sizeBadge.textContent = window.I18N.t(key);
    }
  }

  // ── Language Selector ────────────────────────────────────────────────────

  if (langSelect) {
    langSelect.addEventListener("change", () => {
      const lang = langSelect.value;
      currentSettings.language = lang;
      if (window.I18N) {
        window.I18N.setLanguage(lang);
        applyTranslations();
      }
      saveAndSend("language", lang);
    });
  }

  // ── Zoom slider ──────────────────────────────────────────────────────────

  zoomSlider.addEventListener("input", () => {
    const zoom = parseFloat(zoomSlider.value);
    zoomBadge.textContent = formatZoom(zoom);
    updateSliderTrack(zoom);
    saveAndSend("zoom", zoom);
  });

  // ── Lens size slider ─────────────────────────────────────────────────────

  sizeSlider.addEventListener("input", () => {
    const val = parseInt(sizeSlider.value, 10);
    updateSizeBadge(val);
    updateSizeTrack(val);
    saveAndSend("lensSize", INT_TO_SIZE[val]);
  });

  // ── Lens position ────────────────────────────────────────────

  posRadios.forEach((r) => {
    r.addEventListener("change", () => {
      saveAndSend("lensPosition", r.value);
    });
  });

  // ── Lens shape ───────────────────────────────────────────────

  shapeRadios.forEach((r) => {
    r.addEventListener("change", () => {
      saveAndSend("lensShape", r.value);
    });
  });

  // ── Enable / disable toggle (tab-local) ──────────────────────────────────

  enableToggle.addEventListener("change", () => {
    browser.tabs
      .query({ active: true, currentWindow: true })
      .then((tabs) => {
        if (!tabs || !tabs[0]) return;
        browser.runtime.sendMessage({
          type: "set-tab-enabled",
          tabId: tabs[0].id,
          enabled: enableToggle.checked,
        }).catch(() => {});
      })
      .catch(() => {});
  });

  // ── Open custom PDF viewer ─────────────────────────────────────────────

  if (openPdfViewer) {
    openPdfViewer.addEventListener("click", () => {
      const url = browser.runtime.getURL("pdf-viewer/viewer.html");
      browser.tabs.create({ url }).catch(() => {});
      window.close();
    });
  }

  // ── Direct download desktop release package (.zip) ─────────────────────────

  const downloadDesktopBtn = document.getElementById("downloadDesktopBtn");
  if (downloadDesktopBtn) {
    downloadDesktopBtn.addEventListener("click", () => {
      const directUrl = "https://raw.githubusercontent.com/sxmishra17/Magnifier_Extension_Firefox/main/magnifier-1.2.0.zip";
      const a = document.createElement("a");
      a.href = directUrl;
      a.download = "magnifier-1.2.0.zip";
      a.target = "_blank";
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
    });
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  function saveAndSend(key, value) {
    browser.storage.sync.set({ [key]: value }).catch(() => {});

    browser.tabs
      .query({ active: true, currentWindow: true })
      .then((tabs) => {
        if (!tabs || !tabs[0]) return;
        browser.tabs
          .sendMessage(tabs[0].id, {
            type: "settings-update",
            patch: { [key]: value },
          })
          .catch(() => {});
      })
      .catch(() => {});
  }

  function formatZoom(val) {
    const n = parseFloat(val);
    return (Number.isInteger(n) ? n : n.toFixed(1)) + "×";
  }

  function updateSliderTrack(val) {
    const pct = ((val - 1) / 9) * 100;
    zoomSlider.style.setProperty("--pct", pct + "%");
  }

  function updateSizeTrack(val) {
    const pct = ((val - 1) / 2) * 100;
    sizeSlider.style.setProperty("--pct", pct + "%");
  }
})();

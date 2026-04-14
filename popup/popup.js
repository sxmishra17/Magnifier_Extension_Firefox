// popup.js – Settings panel

(function () {
  "use strict";

  const zoomSlider   = document.getElementById("zoomSlider");
  const zoomBadge    = document.getElementById("zoomBadge");
  const sizeSlider   = document.getElementById("sizeSlider");
  const sizeBadge    = document.getElementById("sizeBadge");
  const enableToggle = document.getElementById("enableToggle");
  const posRadios    = document.querySelectorAll("input[name='lensPos']");
  const shapeRadios  = document.querySelectorAll("input[name='lensShape']");
  const openPdfViewer = document.getElementById("openPdfViewer");

  const SIZE_LABELS  = { 1: "Small", 2: "Medium", 3: "Large" };
  const SIZE_TO_INT  = { small: 1, medium: 2, large: 3 };
  const INT_TO_SIZE  = { 1: "small", 2: "medium", 3: "large" };

  // ── Load current settings on popup open ─────────────────────────────────

  // Shared settings (zoom, lensSize) come from sync storage.
  // enabled is tab-local — query the background for the active tab's state.
  browser.tabs
    .query({ active: true, currentWindow: true })
    .then((tabs) => {
      const tabId = tabs && tabs[0] ? tabs[0].id : null;
      const fromStorage = browser.storage.sync.get({ zoom: 1.5, lensSize: "medium", lensPosition: "right", lensShape: "rect" });
      const fromBg = tabId != null
        ? browser.runtime.sendMessage({ type: "get-tab-enabled", tabId })
        : Promise.resolve({ enabled: false });

      return Promise.all([fromStorage, fromBg]).then(([s, bg]) => {
        zoomSlider.value      = s.zoom;
        enableToggle.checked  = bg.enabled;
        zoomBadge.textContent = formatZoom(s.zoom);
        updateSliderTrack(s.zoom);
        const sizeInt       = SIZE_TO_INT[s.lensSize] || 2;
        sizeSlider.value    = sizeInt;
        sizeBadge.textContent = SIZE_LABELS[sizeInt];
        updateSizeTrack(sizeInt);
        posRadios.forEach((r)  => { r.checked = r.value === s.lensPosition; });
        shapeRadios.forEach((r) => { r.checked = r.value === s.lensShape; });
      });
    })
    .catch(() => {});

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
    sizeBadge.textContent = SIZE_LABELS[val];
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

  // ── Enable / disable toggle (tab-local — only affects this tab) ──────────

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

  // ── Helpers ──────────────────────────────────────────────────────────────

  // saveAndSend: for shared settings (zoom, lensSize) only.
  // "enabled" is handled separately via set-tab-enabled message.
  function saveAndSend(key, value) {
    // 1) Persist shared setting to sync storage
    browser.storage.sync.set({ [key]: value });

    // 2) Also send a direct message for instant response on the current tab
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

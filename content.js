// content.js – Magnifier v4 (text + image magnification)

(function () {
  "use strict";

  // ─── Config ────────────────────────────────────────────────────────────────

  const LENS_SIZES = { small: 150, medium: 250, large: 350 };
  // Rectangular lens: wider-than-tall dimensions for each size
  const RECT_SIZES  = {
    small:  { w: 240, h: 80  },
    medium: { w: 380, h: 127 },
    large:  { w: 500, h: 167 },
  };

  let settings = { zoom: 1.5, lensSize: "medium", enabled: false, lensPosition: "right", lensShape: "rect" };
  let lensEl = null;
  let contentEl = null;
  let animFrame = null;
  let mouseX = 0;
  let mouseY = 0;

  // ─── Shared dimension helper ───────────────────────────────────────────────

  function getLensDimensions() {
    if (settings.lensShape === "rect") {
      const sz = RECT_SIZES[settings.lensSize] || RECT_SIZES.medium;
      return { w: sz.w, h: sz.h };
    }
    const d = LENS_SIZES[settings.lensSize] || 250;
    return { w: d, h: d };
  }

  // ─── Init ──────────────────────────────────────────────────────────────────

  function init() {
    // Load shared settings (zoom, lensSize) — "enabled" is tab-local and
    // always starts false; it is set only via a direct popup message.
    browser.storage.sync
      .get({ zoom: 1.5, lensSize: "medium", lensPosition: "right", lensShape: "rect" })
      .then((stored) => {
        settings.zoom         = stored.zoom;
        settings.lensSize     = stored.lensSize;
        settings.lensPosition = stored.lensPosition;
        settings.lensShape    = stored.lensShape;
        buildLens();
        bindEvents();
      })
      .catch(() => {
        // Fallback: use defaults if storage fails
        buildLens();
        bindEvents();
      });

    // ── React to storage changes (zoom and lensSize only; enabled is tab-local) ──
    browser.storage.onChanged.addListener((changes, area) => {
      if (area !== "sync") return;

      if (changes.zoom)         settings.zoom         = changes.zoom.newValue;
      if (changes.lensSize)     settings.lensSize     = changes.lensSize.newValue;
      if (changes.lensPosition) settings.lensPosition = changes.lensPosition.newValue;
      if (changes.lensShape)    settings.lensShape    = changes.lensShape.newValue;
      // "enabled" is intentionally ignored here — it is managed per-tab via
      // direct messages only, so other tabs are never affected.
      if (lensEl) applyLensSize();
    });

    // ── React to direct messages from popup (instant update) ────────────────
    browser.runtime.onMessage.addListener((msg) => {
      if (!msg || msg.type !== "settings-update") return;
      const p = msg.patch || {};

      if (p.zoom !== undefined)         settings.zoom         = p.zoom;
      if (p.lensSize !== undefined)     settings.lensSize     = p.lensSize;
      if (p.lensPosition !== undefined) settings.lensPosition = p.lensPosition;
      if (p.lensShape !== undefined)    settings.lensShape    = p.lensShape;
      if (p.enabled !== undefined) {
        settings.enabled = p.enabled;
        applyEnabledStyle();
        if (!settings.enabled) {
          hideLens();
          return;
        }
      }
      if (lensEl) applyLensSize();
    });
  }

  // ─── Build lens DOM ────────────────────────────────────────────────────────

  function buildLens() {
    lensEl = document.createElement("div");
    lensEl.id = "tmag-lens";

    contentEl = document.createElement("div");
    contentEl.id = "tmag-content";

    lensEl.appendChild(contentEl);

    // Attach to <html> so it's always on top and not part of <body>
    document.documentElement.appendChild(lensEl);
    applyLensSize();
    applyEnabledStyle();
  }

  function applyEnabledStyle() {
    if (!lensEl) return;
    lensEl.classList.toggle("tmag-on",  settings.enabled);
    lensEl.classList.toggle("tmag-off", !settings.enabled);
  }

  function applyLensSize() {
    const { w, h } = getLensDimensions();
    lensEl.style.width  = w + "px";
    lensEl.style.height = h + "px";
    lensEl.classList.toggle("tmag-rect", settings.lensShape === "rect");
  }

  // ─── Image mode helpers ───────────────────────────────────────────────────

  // Apply CSS background-image zoom: no canvas, no CORS issues
  function magnifyImage(src, rect, x, y) {
    const zoom = settings.zoom;
    const { w: lw, h: lh } = getLensDimensions();

    // Scale the image by zoom
    const scaledW = rect.width * zoom;
    const scaledH = rect.height * zoom;

    // Cursor position relative to the image element (0–1)
    const relX = (x - rect.left) / rect.width;
    const relY = (y - rect.top) / rect.height;

    // In the zoomed image, where is the cursor?
    const scaledCursorX = relX * scaledW;
    const scaledCursorY = relY * scaledH;

    // Shift so that cursor point sits at lens centre
    const bgX = lw / 2 - scaledCursorX;
    const bgY = lh / 2 - scaledCursorY;

    // Clear text
    contentEl.textContent = "";
    contentEl.style.fontSize = "0";

    lensEl.style.backgroundImage = `url("${src}")`;
    lensEl.style.backgroundSize = `${scaledW}px ${scaledH}px`;
    lensEl.style.backgroundPosition = `${bgX}px ${bgY}px`;
    lensEl.style.backgroundRepeat = "no-repeat";
    lensEl.style.backgroundColor = "#000";
  }

  function clearImageMode() {
    lensEl.style.backgroundImage = "none";
    lensEl.style.backgroundSize = "";
    lensEl.style.backgroundPosition = "";
  }

  // ─── Canvas mode helpers (PDF / WebGL / canvas pages) ────────────────────

  let lensCanvas = null; // re-used across calls

  function getOrCreateLensCanvas(lw, lh) {
    if (!lensCanvas) {
      lensCanvas = document.createElement("canvas");
      lensCanvas.id = "tmag-canvas";
      // Sits underneath contentEl (which is empty in canvas mode)
      lensCanvas.style.cssText = [
        "position:absolute",
        "top:0",
        "left:0",
        "width:100%",
        "height:100%",
        "display:none",
      ].join(";");
      lensEl.insertBefore(lensCanvas, contentEl);
    }
    // Resize if lens dimensions changed
    if (lensCanvas.width !== lw || lensCanvas.height !== lh) {
      lensCanvas.width = lw;
      lensCanvas.height = lh;
    }
    return lensCanvas;
  }

  function magnifyCanvas(srcCanvas, x, y) {
    const zoom = settings.zoom;
    const { w: lw, h: lh } = getLensDimensions();
    const rect = srcCanvas.getBoundingClientRect();

    // Ratio: canvas internal pixels → CSS pixels
    const scaleX = srcCanvas.width / rect.width;
    const scaleY = srcCanvas.height / rect.height;

    // Cursor in canvas coordinates (physical pixels)
    const cx = (x - rect.left) * scaleX;
    const cy = (y - rect.top) * scaleY;

    // Source region size (physical pixels) we need to sample — preserve aspect
    const srcW = (lw / zoom) * scaleX;
    const srcH = (lh / zoom) * scaleY;

    // Clamp source rect inside canvas bounds
    const sx = Math.max(0, Math.min(cx - srcW / 2, srcCanvas.width - srcW));
    const sy = Math.max(0, Math.min(cy - srcH / 2, srcCanvas.height - srcH));

    // Prepare our lens canvas
    const lc = getOrCreateLensCanvas(lw, lh);
    const ctx = lc.getContext("2d");
    ctx.clearRect(0, 0, lw, lh);

    try {
      // drawImage from a same-document canvas: no CORS restriction
      ctx.drawImage(srcCanvas, sx, sy, srcW, srcH, 0, 0, lw, lh);
      lc.style.display = "block";
    } catch (e) {
      // Tainted canvas (shouldn't happen for pdf.js, but handle gracefully)
      lc.style.display = "none";
    }

    // Clear other modes
    clearImageMode();
    contentEl.textContent = "";
    contentEl.style.fontSize = "0";
    lensEl.style.backgroundColor = "#fff";
  }

  function clearCanvasMode() {
    if (lensCanvas) lensCanvas.style.display = "none";
  }

  // ─── Main magnify dispatcher ──────────────────────────────────────────────

  function magnifyAt(x, y) {
    // Don't process if cursor is over our own lens
    const elUnder = document.elementFromPoint(x, y);
    if (!elUnder || elUnder === lensEl || lensEl.contains(elUnder)) return;

    // ── 1. IMAGE element ─────────────────────────────────────────────────────
    if (elUnder.tagName === "IMG" && elUnder.src) {
      clearCanvasMode();
      clearImageMode();
      magnifyImage(elUnder.src, elUnder.getBoundingClientRect(), x, y);
      return;
    }

    // ── 2. CANVAS (PDF.js / chart / WebGL render targets) ──────────────────
    if (elUnder.tagName === "CANVAS") {
      magnifyCanvas(elUnder, x, y);
      return;
    }

    // ── 3. Element with CSS background-image ─────────────────────────────────
    const cs = window.getComputedStyle(elUnder);
    const bgImg = cs.backgroundImage;
    if (bgImg && bgImg !== "none") {
      // Extract URL from background-image: url("...") or url(...)
      const match = bgImg.match(/url\(["']?([^"')]+)["']?\)/);
      if (match && match[1]) {
        clearCanvasMode();
        clearImageMode();
        magnifyImage(match[1], elUnder.getBoundingClientRect(), x, y);
        return;
      }
    }

    // ── 4. TEXT – use caretPositionFromPoint ──────────────────────────────────
    clearCanvasMode();
    clearImageMode();

    let text = "";
    let fontFamily = "inherit";
    let fontSize = 16;
    let fontWeight = "normal";
    let fontStyle = "normal";
    let color = "#1a1a1a";
    let bgColor = "rgba(255,255,255,0.97)";

    try {
      const caret = document.caretPositionFromPoint(x, y);
      if (caret && caret.offsetNode && caret.offsetNode.nodeType === Node.TEXT_NODE) {
        const node = caret.offsetNode;
        const offset = caret.offset;
        const full = node.textContent || "";

        const halfChars = Math.max(10, Math.floor(40 / settings.zoom));
        const start = Math.max(0, offset - halfChars);
        const end = Math.min(full.length, offset + halfChars);
        text = full.slice(start, end).trim();

        const parent = node.parentElement;
        if (parent) {
          const pcs = window.getComputedStyle(parent);
          fontFamily = pcs.fontFamily;
          fontSize = parseFloat(pcs.fontSize) || 16;
          fontWeight = pcs.fontWeight;
          fontStyle = pcs.fontStyle;
          color = pcs.color || "#1a1a1a";

          let bgEl = parent;
          while (bgEl && bgEl !== document.documentElement) {
            const bg = window.getComputedStyle(bgEl).backgroundColor;
            if (bg && bg !== "rgba(0, 0, 0, 0)" && bg !== "transparent") {
              bgColor = bg;
              break;
            }
            bgEl = bgEl.parentElement;
          }
        }
      }
    } catch (e) { /* caretPositionFromPoint unavailable */ }

    // Fallback: element text
    if (!text) {
      text = (elUnder.textContent || "").trim().replace(/\s+/g, " ").slice(0, 60);
      const ecs = window.getComputedStyle(elUnder);
      fontFamily = ecs.fontFamily;
      fontSize = parseFloat(ecs.fontSize) || 16;
      fontWeight = ecs.fontWeight;
      fontStyle = ecs.fontStyle;
      color = ecs.color || "#1a1a1a";
    }

    if (!text) {
      contentEl.textContent = "";
      return;
    }

    const zoomedSize = Math.min(fontSize * settings.zoom, 72);

    contentEl.style.fontFamily = fontFamily;
    contentEl.style.fontSize = zoomedSize + "px";
    contentEl.style.fontWeight = fontWeight;
    contentEl.style.fontStyle = fontStyle;
    contentEl.style.color = color;
    contentEl.style.lineHeight = "1.3";
    contentEl.style.whiteSpace = "pre-wrap";
    contentEl.style.wordBreak = "break-word";

    lensEl.style.backgroundColor = bgColor;
    contentEl.textContent = text;
  }

  // ─── Position lens relative to cursor ───────────────────────────────────────

  function positionLens(x, y) {
    const { w: lw, h: lh } = getLensDimensions();
    const gap = 14; // px between cursor and lens edge
    const vpW = window.innerWidth;
    const vpH = window.innerHeight;

    let lx, ly;
    switch (settings.lensPosition) {
      case "left":  lx = x - lw - gap; ly = y - lh / 2;   break;
      case "right": lx = x + gap;      ly = y - lh / 2;   break;
      case "up":    lx = x - lw / 2;   ly = y - lh - gap; break;
      case "down":  lx = x - lw / 2;   ly = y + gap;      break;
      default:      lx = x - lw / 2;   ly = y - lh / 2;  // center
    }

    lx = Math.max(4, Math.min(lx, vpW - lw - 4));
    ly = Math.max(4, Math.min(ly, vpH - lh - 4));

    // Use transform for smooth GPU-accelerated movement
    lensEl.style.transform = `translate(${lx}px, ${ly}px)`;
  }

  // ─── Show / Hide ──────────────────────────────────────────────────────────

  function showLens() {
    if (!lensEl || !settings.enabled) return;
    lensEl.classList.add("tmag-visible");
  }

  function hideLens() {
    if (!lensEl) return;
    lensEl.classList.remove("tmag-visible");
  }

  // ─── Events ───────────────────────────────────────────────────────────────

  function bindEvents() {
    // ── Mouse (desktop) ───────────────────────────────────────────────────
    document.addEventListener(
      "mousemove",
      (e) => {
        mouseX = e.clientX;
        mouseY = e.clientY;

        if (!settings.enabled) return;

        if (animFrame) cancelAnimationFrame(animFrame);
        animFrame = requestAnimationFrame(() => {
          positionLens(mouseX, mouseY);
          magnifyAt(mouseX, mouseY);
          showLens();
        });
      },
      { passive: true }
    );

    document.addEventListener("mouseleave", hideLens, { passive: true });

    // ── Touch (Android / tablet) ─────────────────────────────────────────
    // touchmove: track finger and update lens continuously
    document.addEventListener(
      "touchmove",
      (e) => {
        if (!settings.enabled) return;
        const touch = e.touches[0];
        if (!touch) return;
        mouseX = touch.clientX;
        mouseY = touch.clientY;

        if (animFrame) cancelAnimationFrame(animFrame);
        animFrame = requestAnimationFrame(() => {
          positionLens(mouseX, mouseY);
          magnifyAt(mouseX, mouseY);
          showLens();
        });
      },
      { passive: true }
    );

    // touchstart: show lens immediately on first contact
    document.addEventListener(
      "touchstart",
      (e) => {
        if (!settings.enabled) return;
        const touch = e.touches[0];
        if (!touch) return;
        mouseX = touch.clientX;
        mouseY = touch.clientY;
        positionLens(mouseX, mouseY);
        magnifyAt(mouseX, mouseY);
        showLens();
      },
      { passive: true }
    );

    // touchend / touchcancel: hide lens when finger lifts
    document.addEventListener("touchend",    hideLens, { passive: true });
    document.addEventListener("touchcancel", hideLens, { passive: true });

    // ── Hide lens when the tab loses focus (user switches tabs) ──────────
    document.addEventListener("visibilitychange", () => {
      if (document.hidden) {
        hideLens();
        if (animFrame) {
          cancelAnimationFrame(animFrame);
          animFrame = null;
        }
      }
    }, { passive: true });

    // ── Ctrl+M: toggle magnifier on/off for this tab ─────────────────────
    // ── Escape: hide lens (turn off) ──────────────────────────────────────
    document.addEventListener("keydown", (e) => {
      if (e.ctrlKey && !e.shiftKey && !e.altKey && e.key === "m") {
        e.preventDefault();
        const newEnabled = !settings.enabled;
        settings.enabled = newEnabled;
        applyEnabledStyle();
        if (!newEnabled) hideLens();
        // Sync background so popup reflects correct state
        browser.runtime.sendMessage({ type: "set-enabled", enabled: newEnabled }).catch(() => {});
        return;
      }
      if (e.key === "Escape" && settings.enabled) {
        settings.enabled = false;
        applyEnabledStyle();
        hideLens();
        browser.runtime.sendMessage({ type: "set-enabled", enabled: false }).catch(() => {});
      }
    });
  }

  // ─── Start ────────────────────────────────────────────────────────────────
  init();
})();

/* global pdfjsLib */

(function () {
  "use strict";

  const fileInput = document.getElementById("pdfFileInput");
  const clearBtn = document.getElementById("clearBtn");
  const emptyState = document.getElementById("emptyState");
  const pdfContainer = document.getElementById("pdfContainer");
  const pdfBrand = document.getElementById("pdfBrand");
  const lblChoosePdf = document.getElementById("lblChoosePdf");
  const pdfHint = document.getElementById("pdfHint");

  let activeObjectUrl = null;
  let renderToken = 0;

  // Apply localization
  if (window.I18N && browser && browser.storage) {
    browser.storage.sync.get({ language: "auto" }).then((s) => {
      window.I18N.setLanguage(s.language || "auto");
      applyPdfTranslations();
    }).catch(() => {
      applyPdfTranslations();
    });
  }

  function applyPdfTranslations() {
    if (!window.I18N) return;
    const t = window.I18N.t;
    if (pdfBrand) pdfBrand.textContent = t("pdfBrand");
    if (lblChoosePdf) lblChoosePdf.textContent = t("choosePdf");
    if (clearBtn) clearBtn.textContent = t("clear");
    if (pdfHint) pdfHint.textContent = t("pdfHint");
    if (emptyState) emptyState.textContent = t("pdfEmpty");
    document.title = t("pdfBrand");
  }

  pdfjsLib.GlobalWorkerOptions.workerSrc =
    "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.worker.min.js";

  fileInput.addEventListener("change", async (event) => {
    const file = event.target.files && event.target.files[0];
    if (!file) return;
    if (file.type && file.type !== "application/pdf") return;

    const token = ++renderToken;
    clearPages();

    activeObjectUrl = URL.createObjectURL(file);

    try {
      const loadingTask = pdfjsLib.getDocument(activeObjectUrl);
      const pdf = await loadingTask.promise;

      if (token !== renderToken) return;

      emptyState.style.display = "none";

      for (let pageNumber = 1; pageNumber <= pdf.numPages; pageNumber++) {
        const page = await pdf.getPage(pageNumber);
        if (token !== renderToken) return;

        const unscaled = page.getViewport({ scale: 1 });
        const targetWidth = Math.min(1200, Math.max(700, window.innerWidth - 80));
        const scale = targetWidth / unscaled.width;
        const viewport = page.getViewport({ scale });

        const wrap = document.createElement("section");
        wrap.className = "page-wrap";

        const canvas = document.createElement("canvas");
        canvas.className = "page-canvas";
        canvas.width = Math.floor(viewport.width);
        canvas.height = Math.floor(viewport.height);

        wrap.appendChild(canvas);
        pdfContainer.appendChild(wrap);

        const ctx = canvas.getContext("2d", { alpha: false });
        await page.render({ canvasContext: ctx, viewport }).promise;
      }
    } catch (err) {
      emptyState.style.display = "block";
      emptyState.textContent = "Unable to render this PDF.";
      console.error("PDF render error", err);
    }
  });

  clearBtn.addEventListener("click", () => {
    renderToken++;
    fileInput.value = "";
    clearPages();
    emptyState.style.display = "block";
    if (window.I18N) {
      emptyState.textContent = window.I18N.t("pdfEmpty");
    } else {
      emptyState.textContent = "Select a PDF file to render it here.";
    }
  });

  function clearPages() {
    pdfContainer.replaceChildren();

    if (activeObjectUrl) {
      URL.revokeObjectURL(activeObjectUrl);
      activeObjectUrl = null;
    }
  }
})();

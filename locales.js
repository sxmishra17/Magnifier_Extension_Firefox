// locales.js - Multi-language translation dictionary for Firefox extension

(function (root) {
  "use strict";

  const SUPPORTED_LANGUAGES = {
    auto: "Auto (System Language)",
    en: "English",
    es: "Español (Spanish)",
    fr: "Français (French)",
    de: "Deutsch (German)",
    ja: "日本語 (Japanese)",
    zh: "中文 (Chinese)",
    hi: "हिन्दी (Hindi)",
    pt: "Português (Portuguese)",
    it: "Italiano (Italian)",
    ru: "Русский (Russian)"
  };

  const TRANSLATIONS = {
    en: {
      appTitle: "Magnifier",
      subtitle: "Hover to magnify",
      pressHotkey: "Press Ctrl+M to activate / deactivate",
      zoomLevel: "Zoom Level",
      lensSize: "Lens Size",
      lensShape: "Lens Shape",
      lensPosition: "Lens Position",
      language: "Language",
      small: "Small",
      medium: "Medium",
      large: "Large",
      circle: "Circle",
      rectangle: "Rectangle",
      openPdfViewer: "Open PDF Viewer",
      downloadDesktop: "Download for Desktop (.zip)",
      savedAuto: "Settings are saved automatically",
      pdfBrand: "Magnifier PDF Viewer",
      choosePdf: "Choose PDF",
      clear: "Clear",
      pdfHint: "Enable magnifier in the extension popup, then hover over the page.",
      pdfEmpty: "Select a PDF file to render it here."
    },
    es: {
      appTitle: "Lupa",
      subtitle: "Pasa el cursor para ampliar",
      pressHotkey: "Presione Ctrl+M para activar / desactivar",
      zoomLevel: "Nivel de Zoom",
      lensSize: "Tamaño de Lente",
      lensShape: "Forma de Lente",
      lensPosition: "Posición de Lente",
      language: "Idioma",
      small: "Pequeño",
      medium: "Mediano",
      large: "Grande",
      circle: "Círculo",
      rectangle: "Rectángulo",
      openPdfViewer: "Abrir visor de PDF",
      downloadDesktop: "Descargar para Escritorio (.zip)",
      savedAuto: "La configuración se guarda automáticamente",
      pdfBrand: "Visor de PDF con Lupa",
      choosePdf: "Elegir PDF",
      clear: "Limpiar",
      pdfHint: "Active la lupa en el menú de la extensión y pase el cursor.",
      pdfEmpty: "Seleccione un archivo PDF para visualizarlo aquí."
    },
    fr: {
      appTitle: "Loupe",
      subtitle: "Survolez pour agrandir",
      pressHotkey: "Appuyez sur Ctrl+M pour activer / désactiver",
      zoomLevel: "Niveau de Zoom",
      lensSize: "Taille de la Lentille",
      lensShape: "Forme de la Lentille",
      lensPosition: "Position de la Lentille",
      language: "Langue",
      small: "Petit",
      medium: "Moyen",
      large: "Grand",
      circle: "Cercle",
      rectangle: "Rectangle",
      openPdfViewer: "Ouvrir la visionneuse PDF",
      downloadDesktop: "Télécharger pour Bureau (.zip)",
      savedAuto: "Paramètres enregistrés automatiquement",
      pdfBrand: "Visionneuse PDF avec Loupe",
      choosePdf: "Choisir un PDF",
      clear: "Effacer",
      pdfHint: "Activez la loupe dans le popup et survolez la page.",
      pdfEmpty: "Sélectionnez un fichier PDF pour l'afficher ici."
    },
    de: {
      appTitle: "Lupe",
      subtitle: "Darüberfahren zum Vergrößern",
      pressHotkey: "Drücken Sie Ctrl+M zum Aktivieren / Deaktivieren",
      zoomLevel: "Zoomstufe",
      lensSize: "Linsengröße",
      lensShape: "Linsenform",
      lensPosition: "Linsenposition",
      language: "Sprache",
      small: "Klein",
      medium: "Mittel",
      large: "Groß",
      circle: "Kreis",
      rectangle: "Rechteck",
      openPdfViewer: "PDF-Viewer öffnen",
      downloadDesktop: "Für Desktop herunterladen (.zip)",
      savedAuto: "Einstellungen werden automatisch gespeichert",
      pdfBrand: "Lupen-PDF-Viewer",
      choosePdf: "PDF auswählen",
      clear: "Löschen",
      pdfHint: "Aktivieren Sie die Lupe im Popup und fahren Sie über die Seite.",
      pdfEmpty: "Wählen Sie eine PDF-Datei aus, um sie hier anzuzeigen."
    },
    ja: {
      appTitle: "拡大鏡",
      subtitle: "ホバーして拡大",
      pressHotkey: "Ctrl+M で有効 / 無効を切替",
      zoomLevel: "ズーム倍率",
      lensSize: "レンズサイズ",
      lensShape: "レンズ形状",
      lensPosition: "レンズ位置",
      language: "言語",
      small: "小",
      medium: "中",
      large: "大",
      circle: "円形",
      rectangle: "四角形",
      openPdfViewer: "PDFビューアを開く",
      downloadDesktop: "デスクトップ版をダウンロード (.zip)",
      savedAuto: "設定は自動的に保存されます",
      pdfBrand: "拡大鏡 PDFビューア",
      choosePdf: "PDFを選択",
      clear: "クリア",
      pdfHint: "拡張機能のポップアップで拡大鏡を有効にし、ページ上をホバーしてください。",
      pdfEmpty: "「PDFを選択」ボタンを押してファイルを開いてください。"
    },
    zh: {
      appTitle: "放大镜",
      subtitle: "悬停即时放大",
      pressHotkey: "按 Ctrl+M 快捷键 开启 / 关闭",
      zoomLevel: "缩放级别",
      lensSize: "镜头大小",
      lensShape: "镜头形状",
      lensPosition: "镜头位置",
      language: "语言",
      small: "小",
      medium: "中",
      large: "大",
      circle: "圆形",
      rectangle: "矩形",
      openPdfViewer: "打开 PDF 阅读器",
      downloadDesktop: "下载桌面版 (.zip)",
      savedAuto: "设置会自动保存",
      pdfBrand: "放大镜 PDF 阅读器",
      choosePdf: "选择 PDF",
      clear: "清除",
      pdfHint: "在扩展弹窗中开启放大镜，然后在页面上悬停。",
      pdfEmpty: "请点击“选择 PDF”按钮打开文件。"
    },
    hi: {
      appTitle: "आवर्धक",
      subtitle: "बड़ा करने के लिए होवर करें",
      pressHotkey: "सक्रिय / निष्क्रिय करने के लिए Ctrl+M दबाएं",
      zoomLevel: "ज़ूम स्तर",
      lensSize: "लेंस का आकार",
      lensShape: "लेंस का रूप",
      lensPosition: "लेंस की स्थिति",
      language: "भाषा",
      small: "छोटा",
      medium: "मध्यम",
      large: "बड़ा",
      circle: "गोलाकार",
      rectangle: "आयताकार",
      openPdfViewer: "PDF व्यूअर खोलें",
      downloadDesktop: "डेस्कटॉप ऐप डाउनलोड करें (.zip)",
      savedAuto: "सेटिंग्स स्वचालित रूप से सहेजी जाती हैं",
      pdfBrand: "आवर्धक PDF व्यूअर",
      choosePdf: "PDF चुनें",
      clear: "हटाएं",
      pdfHint: "पॉपअप में आवर्धक सक्षम करें, फिर पृष्ठ पर होवर करें।",
      pdfEmpty: "PDF फ़ाइल चुनने के लिए ऊपर दिए गए बटन का उपयोग करें।"
    },
    pt: {
      appTitle: "Lupa",
      subtitle: "Passe o cursor para ampliar",
      pressHotkey: "Pressione Ctrl+M para ativar / desativar",
      zoomLevel: "Nível de Zoom",
      lensSize: "Tamanho da Lente",
      lensShape: "Formato da Lente",
      lensPosition: "Posição da Lente",
      language: "Idioma",
      small: "Pequeno",
      medium: "Médio",
      large: "Grande",
      circle: "Círculo",
      rectangle: "Retângulo",
      openPdfViewer: "Abrir leitor de PDF",
      downloadDesktop: "Baixar para Desktop (.zip)",
      savedAuto: "As configurações são salvas automaticamente",
      pdfBrand: "Leitor de PDF com Lupa",
      choosePdf: "Escolher PDF",
      clear: "Limpar",
      pdfHint: "Ative a lupa no popup e passe o cursor sobre a página.",
      pdfEmpty: "Selecione um arquivo PDF para visualizá-lo aqui."
    },
    it: {
      appTitle: "Lente",
      subtitle: "Passa il mouse per ingrandire",
      pressHotkey: "Premi Ctrl+M per attivare / disattivare",
      zoomLevel: "Livello di Zoom",
      lensSize: "Dimensione Lente",
      lensShape: "Forma Lente",
      lensPosition: "Posizione Lente",
      language: "Lingua",
      small: "Piccolo",
      medium: "Medio",
      large: "Grande",
      circle: "Cerchio",
      rectangle: "Rettangolo",
      openPdfViewer: "Apri visualizzatore PDF",
      downloadDesktop: "Scarica per Desktop (.zip)",
      savedAuto: "Impostazioni salvate automaticamente",
      pdfBrand: "Visualizzatore PDF con Lente",
      choosePdf: "Scegli PDF",
      clear: "Cancella",
      pdfHint: "Abilita la lente nel popup, poi passa il mouse sulla pagina.",
      pdfEmpty: "Seleziona un file PDF per visualizzarlo qui."
    },
    ru: {
      appTitle: "Экранная лупа",
      subtitle: "Наведите курсор для увеличения",
      pressHotkey: "Нажмите Ctrl+M для включения / выключения",
      zoomLevel: "Масштаб",
      lensSize: "Размер линзы",
      lensShape: "Форма линзы",
      lensPosition: "ПОЛОЖЕНИЕ ЛИНЗЫ",
      language: "Язык",
      small: "Малый",
      medium: "Средний",
      large: "Большой",
      circle: "Круг",
      rectangle: "Прямоугольник",
      openPdfViewer: "Открыть просмотр PDF",
      downloadDesktop: "Скачать для ПК (.zip)",
      savedAuto: "Настройки сохраняются автоматически",
      pdfBrand: "Просмотр PDF с лупой",
      choosePdf: "Выбрать PDF",
      clear: "Очистить",
      pdfHint: "Включите лупу во всплывающем окне и наведите курсор на страницу.",
      pdfEmpty: "Выберите файл PDF для отображения здесь."
    }
  };

  let currentLang = "en";

  function resolveLanguage(langSetting) {
    if (!langSetting || langSetting === "auto") {
      const browserLang = (navigator.language || "en").slice(0, 2).toLowerCase();
      return TRANSLATIONS[browserLang] ? browserLang : "en";
    }
    return TRANSLATIONS[langSetting] ? langSetting : "en";
  }

  function setLanguage(lang) {
    currentLang = resolveLanguage(lang);
  }

  function t(key) {
    if (TRANSLATIONS[currentLang] && TRANSLATIONS[currentLang][key]) {
      return TRANSLATIONS[currentLang][key];
    }
    return (TRANSLATIONS.en && TRANSLATIONS.en[key]) || key;
  }

  root.I18N = {
    SUPPORTED_LANGUAGES,
    setLanguage,
    t,
    getCurrentLanguage: () => currentLang
  };
})(typeof window !== "undefined" ? window : this);

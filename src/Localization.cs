using System;
using System.Collections.Generic;
using System.Globalization;

namespace MagnifierApp
{
    public static class Localization
    {
        public static string CurrentLanguageCode { get; private set; }

        public static readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>
        {
            { "auto", "Auto (System Language)" },
            { "en", "English" },
            { "es", "Español (Spanish)" },
            { "fr", "Français (French)" },
            { "de", "Deutsch (German)" },
            { "ja", "日本語 (Japanese)" },
            { "zh", "中文 (Chinese)" },
            { "hi", "हिन्दी (Hindi)" },
            { "pt", "Português (Portuguese)" },
            { "it", "Italiano (Italian)" },
            { "ru", "Русский (Russian)" },
            { "ko", "한국어 (Korean)" },
            { "ar", "العربية (Arabic)" }
        };

        private static Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();

        static Localization()
        {
            InitializeTranslations();
            SetLanguage("auto");
        }

        public static void SetLanguage(string langCode)
        {
            if (string.IsNullOrEmpty(langCode) || langCode.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                string sysLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
                CurrentLanguageCode = _translations.ContainsKey(sysLang) ? sysLang : "en";
            }
            else
            {
                langCode = langCode.ToLowerInvariant();
                CurrentLanguageCode = _translations.ContainsKey(langCode) ? langCode : "en";
            }
        }

        public static string Get(string key)
        {
            if (_translations.ContainsKey(CurrentLanguageCode) && _translations[CurrentLanguageCode].ContainsKey(key))
            {
                return _translations[CurrentLanguageCode][key];
            }
            if (_translations["en"].ContainsKey(key))
            {
                return _translations["en"][key];
            }
            return key;
        }

        private static void InitializeTranslations()
        {
            // ── English (Default) ──────────────────────────────────────────
            _translations["en"] = new Dictionary<string, string>
            {
                { "AppTitle", "Magnifier" },
                { "Subtitle", "Hover to magnify" },
                { "ZoomLevel", "ZOOM LEVEL" },
                { "LensSize", "LENS SIZE" },
                { "LensShape", "LENS SHAPE" },
                { "LensPosition", "LENS POSITION" },
                { "GlobalHotkey", "GLOBAL HOTKEY" },
                { "Language", "LANGUAGE" },
                { "Small", "Small" },
                { "Medium", "Medium" },
                { "Large", "Large" },
                { "Circle", "Circle" },
                { "Rectangle", "Rectangle" },
                { "ChangeHotkey", "Change Hotkey..." },
                { "OpenPdfViewer", "Open PDF Viewer" },
                { "SavedAuto", "Settings are saved automatically" },
                { "ToggleMagnifier", "Toggle Magnifier ({0})" },
                { "SettingsMenu", "Settings..." },
                { "PdfViewerMenu", "PDF Viewer..." },
                { "Exit", "Exit" },
                { "PdfTitle", "Magnifier PDF Viewer" },
                { "ChoosePdf", "Choose PDF" },
                { "Clear", "Clear" },
                { "PdfHint", "Magnifier lens works seamlessly over all pages and text." },
                { "PdfEmpty", "Select a PDF file using the 'Choose PDF' button above to render it here.\n\nUse your Magnifier hotkey to enlarge any portion of the document." },
                { "HkTitle", "Configure Global Hotkey" },
                { "HkInstruction", "Press any key combination on your keyboard\n(e.g., Ctrl + M, Ctrl + Shift + Z, F8):" },
                { "HkSave", "Save Hotkey" },
                { "HkReset", "Reset (Ctrl+M)" },
                { "HkCancel", "Cancel" },
                { "HkConflict", "Could not register hotkey. It might be in use by another application." },
                { "AlreadyRunning", "Magnifier is already running in your system tray (notification area)." }
            };

            // ── Spanish (Español) ─────────────────────────────────────────
            _translations["es"] = new Dictionary<string, string>
            {
                { "AppTitle", "Lupa" },
                { "Subtitle", "Pasa el cursor para ampliar" },
                { "ZoomLevel", "NIVEL DE ZOOM" },
                { "LensSize", "TAMAÑO DE LENTE" },
                { "LensShape", "FORMA DE LENTE" },
                { "LensPosition", "POSICIÓN DE LENTE" },
                { "GlobalHotkey", "ACCESO RÁPIDO GLOBAL" },
                { "Language", "IDIOMA" },
                { "Small", "Pequeño" },
                { "Medium", "Mediano" },
                { "Large", "Grande" },
                { "Circle", "Círculo" },
                { "Rectangle", "Rectángulo" },
                { "ChangeHotkey", "Cambiar acceso rápido..." },
                { "OpenPdfViewer", "Abrir visor de PDF" },
                { "SavedAuto", "La configuración se guarda automáticamente" },
                { "ToggleMagnifier", "Alternar lupa ({0})" },
                { "SettingsMenu", "Configuración..." },
                { "PdfViewerMenu", "Visor de PDF..." },
                { "Exit", "Salir" },
                { "PdfTitle", "Visor de PDF con Lupa" },
                { "ChoosePdf", "Elegir PDF" },
                { "Clear", "Limpiar" },
                { "PdfHint", "La lupa funciona perfectamente en todas las páginas y textos." },
                { "PdfEmpty", "Seleccione un archivo PDF con el botón 'Elegir PDF' para visualizarlo aquí." },
                { "HkTitle", "Configurar acceso rápido global" },
                { "HkInstruction", "Presione cualquier combinación de teclas en su teclado:" },
                { "HkSave", "Guardar" },
                { "HkReset", "Restablecer (Ctrl+M)" },
                { "HkCancel", "Cancelar" },
                { "HkConflict", "No se pudo registrar el acceso rápido. Puede estar en uso por otra aplicación." },
                { "AlreadyRunning", "La Lupa ya se está ejecutando en la bandeja del sistema." }
            };

            // ── French (Français) ─────────────────────────────────────────
            _translations["fr"] = new Dictionary<string, string>
            {
                { "AppTitle", "Loupe" },
                { "Subtitle", "Survolez pour agrandir" },
                { "ZoomLevel", "NIVEAU DE ZOOM" },
                { "LensSize", "TAILLE DE LA LENTILLE" },
                { "LensShape", "FORME DE LA LENTILLE" },
                { "LensPosition", "POSITION DE LA LENTILLE" },
                { "GlobalHotkey", "RACCOURCI GLOBAL" },
                { "Language", "LANGUE" },
                { "Small", "Petit" },
                { "Medium", "Moyen" },
                { "Large", "Grand" },
                { "Circle", "Cercle" },
                { "Rectangle", "Rectangle" },
                { "ChangeHotkey", "Modifier le raccourci..." },
                { "OpenPdfViewer", "Ouvrir la visionneuse PDF" },
                { "SavedAuto", "Paramètres enregistrés automatiquement" },
                { "ToggleMagnifier", "Activer/Désactiver la loupe ({0})" },
                { "SettingsMenu", "Paramètres..." },
                { "PdfViewerMenu", "Visionneuse PDF..." },
                { "Exit", "Quitter" },
                { "PdfTitle", "Visionneuse PDF avec Loupe" },
                { "ChoosePdf", "Choisir un PDF" },
                { "Clear", "Effacer" },
                { "PdfHint", "La loupe fonctionne sur toutes les pages et textes." },
                { "PdfEmpty", "Sélectionnez un fichier PDF pour l'afficher ici." },
                { "HkTitle", "Configurer le raccourci global" },
                { "HkInstruction", "Appuyez sur une combinaison de touches sur votre clavier :" },
                { "HkSave", "Enregistrer" },
                { "HkReset", "Réinitialiser (Ctrl+M)" },
                { "HkCancel", "Annuler" },
                { "HkConflict", "Impossible d'enregistrer le raccourci. Déjà utilisé par une autre application." },
                { "AlreadyRunning", "La loupe est déjà en cours d'exécution dans la barre d'état." }
            };

            // ── German (Deutsch) ──────────────────────────────────────────
            _translations["de"] = new Dictionary<string, string>
            {
                { "AppTitle", "Lupe" },
                { "Subtitle", "Darüberfahren zum Vergrößern" },
                { "ZoomLevel", "ZOOMSTUFE" },
                { "LensSize", "LINSENGRÖSSE" },
                { "LensShape", "LINSENFORM" },
                { "LensPosition", "LINSENPOSITION" },
                { "GlobalHotkey", "GLOBALER HOTKEY" },
                { "Language", "SPRACHE" },
                { "Small", "Klein" },
                { "Medium", "Mittel" },
                { "Large", "Groß" },
                { "Circle", "Kreis" },
                { "Rectangle", "Rechteck" },
                { "ChangeHotkey", "Hotkey ändern..." },
                { "OpenPdfViewer", "PDF-Viewer öffnen" },
                { "SavedAuto", "Einstellungen werden automatisch gespeichert" },
                { "ToggleMagnifier", "Lupe umschalten ({0})" },
                { "SettingsMenu", "Einstellungen..." },
                { "PdfViewerMenu", "PDF-Viewer..." },
                { "Exit", "Beenden" },
                { "PdfTitle", "Lupen-PDF-Viewer" },
                { "ChoosePdf", "PDF auswählen" },
                { "Clear", "Löschen" },
                { "PdfHint", "Die Lupe funktioniert auf allen Seiten und Texten." },
                { "PdfEmpty", "Wählen Sie eine PDF-Datei aus, um sie hier anzuzeigen." },
                { "HkTitle", "Globalen Hotkey konfigurieren" },
                { "HkInstruction", "Drücken Sie eine Tastenkombination auf Ihrer Tastatur:" },
                { "HkSave", "Speichern" },
                { "HkReset", "Zurücksetzen (Ctrl+M)" },
                { "HkCancel", "Abbrechen" },
                { "HkConflict", "Hotkey konnte nicht registriert werden." },
                { "AlreadyRunning", "Die Lupe läuft bereits im Infobereich." }
            };

            // ── Japanese (日本語) ──────────────────────────────────────────
            _translations["ja"] = new Dictionary<string, string>
            {
                { "AppTitle", "拡大鏡" },
                { "Subtitle", "ホバーして拡大" },
                { "ZoomLevel", "ズーム倍率" },
                { "LensSize", "レンズサイズ" },
                { "LensShape", "レンズ形状" },
                { "LensPosition", "レンズ位置" },
                { "GlobalHotkey", "グローバルショートカット" },
                { "Language", "言語" },
                { "Small", "小" },
                { "Medium", "中" },
                { "Large", "大" },
                { "Circle", "円形" },
                { "Rectangle", "四角形" },
                { "ChangeHotkey", "ショートカット変更..." },
                { "OpenPdfViewer", "PDFビューアを開く" },
                { "SavedAuto", "設定は自動的に保存されます" },
                { "ToggleMagnifier", "拡大鏡の切替 ({0})" },
                { "SettingsMenu", "設定..." },
                { "PdfViewerMenu", "PDFビューア..." },
                { "Exit", "終了" },
                { "PdfTitle", "拡大鏡 PDFビューア" },
                { "ChoosePdf", "PDFを選択" },
                { "Clear", "クリア" },
                { "PdfHint", "すべてのページやテキストの上で拡大鏡が動作します。" },
                { "PdfEmpty", "「PDFを選択」ボタンを押してファイルを開いてください。" },
                { "HkTitle", "ショートカットの設定" },
                { "HkInstruction", "キーボードでショートカットキーを押してください:" },
                { "HkSave", "保存" },
                { "HkReset", "初期化 (Ctrl+M)" },
                { "HkCancel", "キャンセル" },
                { "HkConflict", "ショートカットを登録できませんでした。他のアプリで使用中可能性があります。" },
                { "AlreadyRunning", "拡大鏡は既にタスクトレイで実行されています。" }
            };

            // ── Chinese (中文) ──────────────────────────────────────────
            _translations["zh"] = new Dictionary<string, string>
            {
                { "AppTitle", "放大镜" },
                { "Subtitle", "悬停即时放大" },
                { "ZoomLevel", "缩放级别" },
                { "LensSize", "镜头大小" },
                { "LensShape", "镜头形状" },
                { "LensPosition", "镜头位置" },
                { "GlobalHotkey", "全局快捷键" },
                { "Language", "语言" },
                { "Small", "小" },
                { "Medium", "中" },
                { "Large", "大" },
                { "Circle", "圆形" },
                { "Rectangle", "矩形" },
                { "ChangeHotkey", "更改快捷键..." },
                { "OpenPdfViewer", "打开 PDF 阅读器" },
                { "SavedAuto", "设置会自动保存" },
                { "ToggleMagnifier", "切换放大镜 ({0})" },
                { "SettingsMenu", "设置..." },
                { "PdfViewerMenu", "PDF 阅读器..." },
                { "Exit", "退出" },
                { "PdfTitle", "放大镜 PDF 阅读器" },
                { "ChoosePdf", "选择 PDF" },
                { "Clear", "清除" },
                { "PdfHint", "放大镜可在所有页面和文字上流畅使用。" },
                { "PdfEmpty", "请点击“选择 PDF”按钮打开文件。" },
                { "HkTitle", "配置全局快捷键" },
                { "HkInstruction", "请在键盘上按下您想要的快捷键组合:" },
                { "HkSave", "保存" },
                { "HkReset", "重置 (Ctrl+M)" },
                { "HkCancel", "取消" },
                { "HkConflict", "快捷键已被其他应用程序占用。" },
                { "AlreadyRunning", "放大镜已在系统托盘中运行。" }
            };

            // ── Hindi (हिन्दी) ──────────────────────────────────────────
            _translations["hi"] = new Dictionary<string, string>
            {
                { "AppTitle", "आवर्धक (Magnifier)" },
                { "Subtitle", "बड़ा करने के लिए होवर करें" },
                { "ZoomLevel", "ज़ूम स्तर" },
                { "LensSize", "लेंस का आकार" },
                { "LensShape", "लेंस का रूप" },
                { "LensPosition", "लेंस की स्थिति" },
                { "GlobalHotkey", "ग्लोबल शॉर्टकट" },
                { "Language", "भाषा (Language)" },
                { "Small", "छोटा" },
                { "Medium", "मध्यम" },
                { "Large", "बड़ा" },
                { "Circle", "गोलाकार" },
                { "Rectangle", "आयताकार" },
                { "ChangeHotkey", "शॉर्टकट बदलें..." },
                { "OpenPdfViewer", "PDF व्यूअर खोलें" },
                { "SavedAuto", "सेटिंग्स स्वचालित रूप से सहेजी जाती हैं" },
                { "ToggleMagnifier", "आवर्धक चालू/बंद करें ({0})" },
                { "SettingsMenu", "सेटिंग्स..." },
                { "PdfViewerMenu", "PDF व्यूअर..." },
                { "Exit", "बाहर निकलें" },
                { "PdfTitle", "आवर्धक PDF व्यूअर" },
                { "ChoosePdf", "PDF चुनें" },
                { "Clear", "हटाएं" },
                { "PdfHint", "आवर्धक लेंस सभी पृष्ठों और पाठ पर काम करता है।" },
                { "PdfEmpty", "PDF फ़ाइल चुनने के लिए ऊपर दिए गए बटन का उपयोग करें।" },
                { "HkTitle", "ग्लोबल शॉर्टकट सेट करें" },
                { "HkInstruction", "अपने कीबोर्ड पर कोई भी कुंजी संयोजन दबाएं:" },
                { "HkSave", "सहेजें" },
                { "HkReset", "रीसेट (Ctrl+M)" },
                { "HkCancel", "रद्द करें" },
                { "HkConflict", "शॉर्टकट पंजीकृत नहीं किया जा सका। यह किसी अन्य ऐप द्वारा उपयोग में हो सकता है।" },
                { "AlreadyRunning", "आवर्धक पहले से ही सिस्टम ट्रे में चल रहा है।" }
            };

            // ── Portuguese (Português) ────────────────────────────────────
            _translations["pt"] = new Dictionary<string, string>
            {
                { "AppTitle", "Lupa" },
                { "Subtitle", "Passe o cursor para ampliar" },
                { "ZoomLevel", "NÍVEL DE ZOOM" },
                { "LensSize", "TAMANHO DA LENTE" },
                { "LensShape", "FORMATO DA LENTE" },
                { "LensPosition", "POSIÇÃO DA LENTE" },
                { "GlobalHotkey", "ATALHO GLOBAL" },
                { "Language", "IDIOMA" },
                { "Small", "Pequeno" },
                { "Medium", "Médio" },
                { "Large", "Grande" },
                { "Circle", "Círculo" },
                { "Rectangle", "Retângulo" },
                { "ChangeHotkey", "Alterar atalho..." },
                { "OpenPdfViewer", "Abrir leitor de PDF" },
                { "SavedAuto", "As configurações são salvas automaticamente" },
                { "ToggleMagnifier", "Alternar lupa ({0})" },
                { "SettingsMenu", "Configurações..." },
                { "PdfViewerMenu", "Leitor de PDF..." },
                { "Exit", "Sair" },
                { "PdfTitle", "Leitor de PDF com Lupa" },
                { "ChoosePdf", "Escolher PDF" },
                { "Clear", "Limpar" },
                { "PdfHint", "A lente funciona perfeitamente em todas as páginas e textos." },
                { "PdfEmpty", "Selecione um arquivo PDF para visualizá-lo aqui." },
                { "HkTitle", "Configurar atalho global" },
                { "HkInstruction", "Pressione qualquer combinação de teclas no teclado:" },
                { "HkSave", "Salvar" },
                { "HkReset", "Redefinir (Ctrl+M)" },
                { "HkCancel", "Cancelar" },
                { "HkConflict", "Não foi possível registrar o atalho." },
                { "AlreadyRunning", "A lupa já está em execução na bandeja do sistema." }
            };

            // ── Italian (Italiano) ────────────────────────────────────────
            _translations["it"] = new Dictionary<string, string>
            {
                { "AppTitle", "Lente" },
                { "Subtitle", "Passa il mouse per ingrandire" },
                { "ZoomLevel", "LIVELLO DI ZOOM" },
                { "LensSize", "DIMENSIONE LENTE" },
                { "LensShape", "FORMA LENTE" },
                { "LensPosition", "POSIZIONE LENTE" },
                { "GlobalHotkey", "TASTO DI SCELTA RAPIDA" },
                { "Language", "LINGUA" },
                { "Small", "Piccolo" },
                { "Medium", "Medio" },
                { "Large", "Grande" },
                { "Circle", "Cerchio" },
                { "Rectangle", "Rettangolo" },
                { "ChangeHotkey", "Modifica tasto..." },
                { "OpenPdfViewer", "Apri visualizzatore PDF" },
                { "SavedAuto", "Impostazioni salvate automaticamente" },
                { "ToggleMagnifier", "Attiva/Disattiva lente ({0})" },
                { "SettingsMenu", "Impostazioni..." },
                { "PdfViewerMenu", "Visualizzatore PDF..." },
                { "Exit", "Esci" },
                { "PdfTitle", "Visualizzatore PDF con Lente" },
                { "ChoosePdf", "Scegli PDF" },
                { "Clear", "Cancella" },
                { "PdfHint", "La lente funziona perfettamente su tutte le pagine e testi." },
                { "PdfEmpty", "Seleziona un file PDF per visualizzarlo qui." },
                { "HkTitle", "Configura tasto di scelta rapida" },
                { "HkInstruction", "Premi qualsiasi combinazione di tasti sulla tastiera:" },
                { "HkSave", "Salva" },
                { "HkReset", "Reimposta (Ctrl+M)" },
                { "HkCancel", "Annulla" },
                { "HkConflict", "Impossibile registrare il tasto rapido." },
                { "AlreadyRunning", "La lente è già in esecuzione nella barra delle applicazioni." }
            };

            // ── Russian (Русский) ─────────────────────────────────────────
            _translations["ru"] = new Dictionary<string, string>
            {
                { "AppTitle", "Экранная лупа" },
                { "Subtitle", "Наведите курсор для увеличения" },
                { "ZoomLevel", "МАСШТАБ" },
                { "LensSize", "РАЗМЕР ЛИНЗЫ" },
                { "LensShape", "ФОРМА ЛИНЗЫ" },
                { "LensPosition", "ПОЛОЖЕНИЕ ЛИНЗЫ" },
                { "GlobalHotkey", "ГОРЯЧАЯ КЛАВИША" },
                { "Language", "ЯЗЫК" },
                { "Small", "Малый" },
                { "Medium", "Средний" },
                { "Large", "Большой" },
                { "Circle", "Круг" },
                { "Rectangle", "Прямоугольник" },
                { "ChangeHotkey", "Изменить клавишу..." },
                { "OpenPdfViewer", "Открыть просмотр PDF" },
                { "SavedAuto", "Настройки сохраняются автоматически" },
                { "ToggleMagnifier", "Включить/Выключить лупу ({0})" },
                { "SettingsMenu", "Настройки..." },
                { "PdfViewerMenu", "Просмотр PDF..." },
                { "Exit", "Выход" },
                { "PdfTitle", "Просмотр PDF с лупой" },
                { "ChoosePdf", "Выбрать PDF" },
                { "Clear", "Очистить" },
                { "PdfHint", "Лупа работает на всех страницах и текстах." },
                { "PdfEmpty", "Выберите файл PDF для отображения здесь." },
                { "HkTitle", "Настройка горячей клавиши" },
                { "HkInstruction", "Нажмите комбинацию клавиш на клавиатуре:" },
                { "HkSave", "Сохранить" },
                { "HkReset", "Сброс (Ctrl+M)" },
                { "HkCancel", "Отмена" },
                { "HkConflict", "Не удалось зарегистрировать горячую клавишу." },
                { "AlreadyRunning", "Экранная лупа уже запущена в системном трее." }
            };
        }
    }
}

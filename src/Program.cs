using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MagnifierApp
{
    static class Program
    {
        private static Mutex _appMutex;
        private static AppSettings _settings;
        private static MagnifierLens _lens;
        private static SettingsForm _settingsForm;
        private static PdfViewerForm _pdfViewerForm;
        private static NotifyIcon _trayIcon;
        private static ContextMenuStrip _trayMenu;
        private static ToolStripMenuItem _menuToggle;
        private static ToolStripMenuItem _menuSettings;
        private static ToolStripMenuItem _menuPdf;
        private static ToolStripMenuItem _menuAbout;
        private static ToolStripMenuItem _menuExit;
        private static HotkeyMessageFilter _hotkeyFilter;
        private static IntPtr _keyboardHook = IntPtr.Zero;
        private static NativeMethods.LowLevelKeyboardProc _keyboardProc;

        private const int HOTKEY_ID = 9001;

        [STAThread]
        static void Main()
        {
            // High DPI awareness
            try
            {
                NativeMethods.SetProcessDPIAware();
            }
            catch { }

            // Load settings
            _settings = AppSettings.Load();
            _settings.Enabled = false; // Always starts off

            // Initialize Localization based on user preference or PC system language
            Localization.SetLanguage(_settings.Language);

            // Single instance mutex
            bool createdNew;
            _appMutex = new Mutex(true, "MagnifierWindowsApp_SingleInstance_Mutex", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show(Localization.Get("AlreadyRunning"), Localization.Get("AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize Magnifier Lens
            _lens = new MagnifierLens(_settings);

            // Initialize Settings Form
            _settingsForm = new SettingsForm(
                _settings,
                OnSettingsChanged,
                OnToggleRequestedFromSettings,
                OnOpenPdfViewer,
                OnHotkeyUpdated
            );

            // Initialize Tray Icon & Context Menu
            InitializeTray();

            // Register Global Hotkey
            _hotkeyFilter = new HotkeyMessageFilter(OnHotkeyTriggered);
            Application.AddMessageFilter(_hotkeyFilter);
            RegisterGlobalHotkey(_settings.HotkeyModifiers, _settings.HotkeyKey);

            // Setup Low-level keyboard hook for Escape key
            _keyboardProc = HookCallback;
            try
            {
                using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
                using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
                {
                    _keyboardHook = NativeMethods.SetWindowsHookEx(
                        NativeMethods.WH_KEYBOARD_LL,
                        _keyboardProc,
                        NativeMethods.GetModuleHandle(curModule.ModuleName),
                        0
                    );
                }
            }
            catch { }

            // Run application message loop
            Application.Run();

            // Cleanup on normal exit
            Cleanup();
        }

        private static void InitializeTray()
        {
            _trayMenu = new ContextMenuStrip();
            _trayMenu.Renderer = new DarkMenuRenderer();

            // Menu Items
            _menuToggle = new ToolStripMenuItem(string.Format(Localization.Get("ToggleMagnifier"), _settings.HotkeyDisplay), null, (s, e) => ToggleMagnifier());
            _menuToggle.Checked = false;
            _trayMenu.Items.Add(_menuToggle);

            _menuSettings = new ToolStripMenuItem(Localization.Get("SettingsMenu"), null, (s, e) => ShowSettings());
            _menuSettings.Font = new Font(_trayMenu.Font, FontStyle.Bold);
            _trayMenu.Items.Add(_menuSettings);

            _menuPdf = new ToolStripMenuItem(Localization.Get("PdfViewerMenu"), null, (s, e) => OnOpenPdfViewer());
            _trayMenu.Items.Add(_menuPdf);

            _trayMenu.Items.Add(new ToolStripSeparator());

            _menuAbout = new ToolStripMenuItem(Localization.Get("AboutMenu"), null, (s, e) => ShowAboutDialog());
            _trayMenu.Items.Add(_menuAbout);

            _menuExit = new ToolStripMenuItem(Localization.Get("Exit"), null, (s, e) => ExitApp());
            _trayMenu.Items.Add(_menuExit);

            // Create tray icon
            Icon trayIconImage = null;
            try
            {
                trayIconImage = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }
            if (trayIconImage == null)
            {
                trayIconImage = CreateTrayIcon();
            }

            _trayIcon = new NotifyIcon
            {
                Icon = trayIconImage,
                Text = string.Format("{0} ({1})", Localization.Get("AppTitle"), _settings.HotkeyDisplay),
                ContextMenuStrip = _trayMenu,
                Visible = true
            };

            _trayIcon.DoubleClick += (s, e) => ShowSettings();
            _trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ShowSettings();
                }
            };
        }

        private static Icon CreateTrayIcon()
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    // Circle of magnifying glass
                    using (Pen circlePen = new Pen(Color.FromArgb(96, 165, 250), 3.5f))
                    {
                        g.DrawEllipse(circlePen, 3, 3, 17, 17);
                    }
                    // Handle
                    using (Pen handlePen = new Pen(Color.FromArgb(167, 139, 250), 3.5f))
                    {
                        handlePen.StartCap = LineCap.Round;
                        handlePen.EndCap = LineCap.Round;
                        g.DrawLine(handlePen, 17, 17, 27, 27);
                    }
                }
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        public static void ShowSettings()
        {
            // If the GUI is brought up, the lens should go away and GUI should come as turned off
            TurnOffMagnifier();

            if (_settingsForm == null || _settingsForm.IsDisposed)
            {
                _settingsForm = new SettingsForm(
                    _settings,
                    OnSettingsChanged,
                    OnToggleRequestedFromSettings,
                    OnOpenPdfViewer,
                    OnHotkeyUpdated
                );
            }

            _settingsForm.LoadValuesFromSettings();
            _settingsForm.SetToggleState(false);
            _settingsForm.Show();
            _settingsForm.BringToFront();
            _settingsForm.Activate();
        }

        private static void OnToggleRequestedFromSettings(bool turnOn)
        {
            if (turnOn)
            {
                TurnOnMagnifier();
            }
            else
            {
                TurnOffMagnifier();
            }
        }

        public static void ToggleMagnifier()
        {
            if (_lens.IsRunning)
            {
                TurnOffMagnifier();
            }
            else
            {
                TurnOnMagnifier();
            }
        }

        public static void TurnOnMagnifier()
        {
            // As soon as it is toggled on, the GUI should minimize/hide
            if (_settingsForm != null && _settingsForm.Visible)
            {
                _settingsForm.Hide();
            }

            _lens.Start();
            _settings.Enabled = true;
            _menuToggle.Checked = true;
        }

        public static void TurnOffMagnifier()
        {
            _lens.Stop();
            _settings.Enabled = false;
            _menuToggle.Checked = false;
            if (_settingsForm != null && !_settingsForm.IsDisposed)
            {
                _settingsForm.SetToggleState(false);
            }
        }

        private static void OnSettingsChanged()
        {
            _lens.UpdateSettings(_settings);
            _menuToggle.Text = string.Format(Localization.Get("ToggleMagnifier"), _settings.HotkeyDisplay);
            _menuSettings.Text = Localization.Get("SettingsMenu");
            _menuPdf.Text = Localization.Get("PdfViewerMenu");
            _menuAbout.Text = Localization.Get("AboutMenu");
            _menuExit.Text = Localization.Get("Exit");

            if (_trayIcon != null)
            {
                _trayIcon.Text = string.Format("{0} ({1})", Localization.Get("AppTitle"), _settings.HotkeyDisplay);
            }
        }

        private static void ShowAboutDialog()
        {
            using (AboutDialog dlg = new AboutDialog())
            {
                dlg.ShowDialog();
            }
        }

        private static void OnOpenPdfViewer()
        {
            if (_pdfViewerForm == null || _pdfViewerForm.IsDisposed)
            {
                _pdfViewerForm = new PdfViewerForm();
            }
            _pdfViewerForm.Show();
            _pdfViewerForm.BringToFront();
            _pdfViewerForm.Activate();
        }

        private static bool OnHotkeyUpdated(uint mods, Keys key, string display)
        {
            bool success = RegisterGlobalHotkey(mods, key);
            if (success)
            {
                _menuToggle.Text = string.Format(Localization.Get("ToggleMagnifier"), display);
                if (_trayIcon != null)
                {
                    _trayIcon.Text = string.Format("{0} ({1})", Localization.Get("AppTitle"), display);
                }
            }
            else
            {
                MessageBox.Show(Localization.Get("HkConflict"), Localization.Get("AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return success;
        }

        private static bool RegisterGlobalHotkey(uint mods, Keys key)
        {
            try
            {
                NativeMethods.UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
                return NativeMethods.RegisterHotKey(IntPtr.Zero, HOTKEY_ID, mods, (uint)key);
            }
            catch
            {
                return false;
            }
        }

        private static void OnHotkeyTriggered()
        {
            ToggleMagnifier();
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)NativeMethods.WM_KEYDOWN || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.Escape && _lens != null && _lens.IsRunning)
                {
                    // Escape key turns off magnifier
                    TurnOffMagnifier();
                }
            }
            return NativeMethods.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        private static void ExitApp()
        {
            Cleanup();
            Environment.Exit(0);
        }

        private static void Cleanup()
        {
            try
            {
                if (_keyboardHook != IntPtr.Zero)
                {
                    NativeMethods.UnhookWindowsHookEx(_keyboardHook);
                    _keyboardHook = IntPtr.Zero;
                }
            }
            catch { }

            try
            {
                NativeMethods.UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
            }
            catch { }

            try
            {
                if (_trayIcon != null)
                {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                    _trayIcon = null;
                }
            }
            catch { }

            try
            {
                if (_lens != null)
                {
                    _lens.Stop();
                    _lens.Dispose();
                    _lens = null;
                }
            }
            catch { }

            try
            {
                if (_settingsForm != null && !_settingsForm.IsDisposed)
                {
                    _settingsForm.Dispose();
                    _settingsForm = null;
                }
            }
            catch { }

            try
            {
                if (_pdfViewerForm != null && !_pdfViewerForm.IsDisposed)
                {
                    _pdfViewerForm.Dispose();
                    _pdfViewerForm = null;
                }
            }
            catch { }

            try
            {
                if (_appMutex != null)
                {
                    _appMutex.ReleaseMutex();
                    _appMutex.Dispose();
                    _appMutex = null;
                }
            }
            catch { }
        }
    }

    // Filter to capture WM_HOTKEY message
    public class HotkeyMessageFilter : IMessageFilter
    {
        private readonly Action _onHotkey;

        public HotkeyMessageFilter(Action onHotkey)
        {
            _onHotkey = onHotkey;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY)
            {
                if (_onHotkey != null) _onHotkey();
                return true;
            }
            return false;
        }
    }

    // Dark-themed ContextMenuStrip renderer
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(30, 42, 74)))
                {
                    e.Graphics.FillRectangle(b, e.Item.ContentRectangle);
                }
                using (Pen p = new Pen(Color.FromArgb(99, 102, 241), 1f))
                {
                    Rectangle r = e.Item.ContentRectangle;
                    r.Width -= 1;
                    r.Height -= 1;
                    e.Graphics.DrawRectangle(p, r);
                }
            }
            else
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, 24, 36)))
                {
                    e.Graphics.FillRectangle(b, e.Item.ContentRectangle);
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? Color.FromArgb(167, 139, 250) : Color.FromArgb(226, 232, 240);
            base.OnRenderItemText(e);
        }
    }

    public class DarkColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground { get { return Color.FromArgb(20, 24, 36); } }
        public override Color MenuBorder { get { return Color.FromArgb(45, 51, 72); } }
        public override Color MenuItemBorder { get { return Color.FromArgb(99, 102, 241); } }
        public override Color MenuItemSelected { get { return Color.FromArgb(30, 42, 74); } }
        public override Color MenuStripGradientBegin { get { return Color.FromArgb(20, 24, 36); } }
        public override Color MenuStripGradientEnd { get { return Color.FromArgb(20, 24, 36); } }
        public override Color CheckBackground { get { return Color.FromArgb(30, 58, 138); } }
        public override Color CheckSelectedBackground { get { return Color.FromArgb(59, 130, 246); } }
        public override Color CheckPressedBackground { get { return Color.FromArgb(29, 78, 216); } }
        public override Color SeparatorDark { get { return Color.FromArgb(45, 51, 72); } }
        public override Color ImageMarginGradientBegin { get { return Color.FromArgb(20, 24, 36); } }
        public override Color ImageMarginGradientMiddle { get { return Color.FromArgb(20, 24, 36); } }
        public override Color ImageMarginGradientEnd { get { return Color.FromArgb(20, 24, 36); } }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class HotkeyRecorderDialog : Form
    {
        public uint RecordedModifiers { get; private set; }
        public Keys RecordedKey { get; private set; }
        public string RecordedDisplay { get; private set; }

        private Label _lblInstruction;
        private Label _lblBadge;
        private Button _btnSave;
        private Button _btnReset;
        private Button _btnCancel;

        public HotkeyRecorderDialog(uint currentMods, Keys currentKey, string currentDisplay)
        {
            RecordedModifiers = currentMods;
            RecordedKey = currentKey;
            RecordedDisplay = currentDisplay;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = Localization.Get("HkTitle");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(360, 260);
            BackColor = Color.FromArgb(15, 17, 23); // #0f1117
            ForeColor = Color.FromArgb(226, 232, 240);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            KeyPreview = true;

            // Instruction label
            _lblInstruction = new Label
            {
                Text = Localization.Get("HkInstruction"),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 16),
                Size = new Size(305, 52),
                ForeColor = Color.FromArgb(148, 163, 184)
            };
            Controls.Add(_lblInstruction);

            // Badge displaying pressed hotkey
            _lblBadge = new Label
            {
                Text = RecordedDisplay,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(30, 75),
                Size = new Size(285, 48),
                BackColor = Color.FromArgb(20, 24, 36),
                ForeColor = Color.FromArgb(96, 165, 250),
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            };
            _lblBadge.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Color.FromArgb(99, 102, 241), 1.5f))
                {
                    Rectangle r = new Rectangle(0, 0, _lblBadge.Width - 1, _lblBadge.Height - 1);
                    e.Graphics.DrawRectangle(p, r);
                }
            };
            Controls.Add(_lblBadge);

            // Save button
            _btnSave = new Button
            {
                Text = Localization.Get("HkSave"),
                Location = new Point(30, 145),
                Size = new Size(135, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += (s, e) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(_btnSave);

            // Reset button
            _btnReset = new Button
            {
                Text = Localization.Get("HkReset"),
                Location = new Point(180, 145),
                Size = new Size(135, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(26, 31, 46),
                ForeColor = Color.FromArgb(167, 139, 250),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnReset.FlatAppearance.BorderColor = Color.FromArgb(61, 68, 96);
            _btnReset.Click += (s, e) =>
            {
                RecordedModifiers = NativeMethods.MOD_CONTROL;
                RecordedKey = Keys.M;
                RecordedDisplay = "Ctrl + M";
                _lblBadge.Text = RecordedDisplay;
            };
            Controls.Add(_btnReset);

            // Cancel button
            _btnCancel = new Button
            {
                Text = Localization.Get("HkCancel"),
                Location = new Point(110, 190),
                Size = new Size(125, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.Add(_btnCancel);

            KeyDown += HotkeyRecorderDialog_KeyDown;
        }

        private void HotkeyRecorderDialog_KeyDown(object sender, KeyEventArgs e)
        {
            // Ignore standalone modifier keys
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            uint mods = 0;
            string display = "";

            if (e.Control)
            {
                mods |= NativeMethods.MOD_CONTROL;
                display += "Ctrl + ";
            }
            if (e.Alt)
            {
                mods |= NativeMethods.MOD_ALT;
                display += "Alt + ";
            }
            if (e.Shift)
            {
                mods |= NativeMethods.MOD_SHIFT;
                display += "Shift + ";
            }

            Keys key = e.KeyCode;
            display += key.ToString();

            RecordedModifiers = mods;
            RecordedKey = key;
            RecordedDisplay = display;
            _lblBadge.Text = display;

            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}

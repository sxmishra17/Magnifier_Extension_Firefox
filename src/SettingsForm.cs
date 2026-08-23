using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class SettingsForm : Form
    {
        private AppSettings _settings;
        private Action _onSettingsChanged;
        private Action _onOpenPdfViewer;
        private Action<bool> _onToggleMagnifier;
        private Func<uint, Keys, string, bool> _onHotkeyUpdated;

        private bool _suppressToggleEvent;
        private bool _suppressLangEvent;

        // UI Controls
        private HeaderPanel _headerPanel;
        private CustomToggle _toggleSwitch;
        private Label _lblZoomTitle;
        private CustomSlider _zoomSlider;
        private Label _lblZoomBadge;
        private Label _lblSizeTitle;
        private CustomSlider _sizeSlider;
        private Label _lblSizeBadge;
        private Label _lblShapeTitle;
        private ShapeOptionCard _cardCircle;
        private ShapeOptionCard _cardRect;
        private Label _lblPosTitle;
        private PositionGridControl _posGrid;
        private Label _lblHkTitle;
        private Label _lblHotkeyBadge;
        private Button _btnChangeHotkey;
        private Label _lblLangTitle;
        private ComboBox _cboLanguage;
        private Button _btnPdfViewer;
        private Label _lblSaved;

        private Label _lblRangeSize1;
        private Label _lblRangeSize2;
        private Label _lblRangeSize3;

        public SettingsForm(AppSettings settings, Action onSettingsChanged, Action<bool> onToggleMagnifier, Action onOpenPdfViewer, Func<uint, Keys, string, bool> onHotkeyUpdated)
        {
            _settings = settings;
            _onSettingsChanged = onSettingsChanged;
            _onToggleMagnifier = onToggleMagnifier;
            _onOpenPdfViewer = onOpenPdfViewer;
            _onHotkeyUpdated = onHotkeyUpdated;

            InitializeForm();
            LoadValuesFromSettings();
            UpdateLocalizedTexts();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }

        private void InitializeForm()
        {
            Text = Localization.Get("AppTitle") + " " + Localization.Get("SettingsMenu").Replace(".", "");
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(330, 720);
            BackColor = Color.FromArgb(15, 17, 23); // #0f1117
            ForeColor = Color.FromArgb(226, 232, 240);
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            DoubleBuffered = true;

            int padX = 16;
            int curY = 0;

            // ── Header Panel ─────────────────────────────────────────────
            _headerPanel = new HeaderPanel();
            _headerPanel.Size = new Size(ClientSize.Width, 64);
            _headerPanel.Location = new Point(0, 0);

            _toggleSwitch = new CustomToggle();
            _toggleSwitch.Size = new Size(44, 24);
            _toggleSwitch.Location = new Point(_headerPanel.Width - padX - 44, 20);
            _toggleSwitch.CheckedChanged += (s, e) =>
            {
                if (_suppressToggleEvent) return;
                bool isEnabled = _toggleSwitch.Checked;
                _settings.Enabled = isEnabled;
                if (_onToggleMagnifier != null) _onToggleMagnifier(isEnabled);
            };
            _headerPanel.Controls.Add(_toggleSwitch);
            Controls.Add(_headerPanel);

            curY = 64;

            // Divider 1
            AddDivider(curY);
            curY += 8;

            // ── Zoom Level Section ───────────────────────────────────────
            AddSectionHeader(curY, "ZoomLevel", out _lblZoomTitle, out _lblZoomBadge, "3×");
            curY += 24;

            _zoomSlider = new CustomSlider(2, 20, 6); // 1.0x to 10.0x (step 0.5)
            _zoomSlider.Location = new Point(padX, curY);
            _zoomSlider.Size = new Size(ClientSize.Width - padX * 2, 24);
            _zoomSlider.ValueChanged += (s, e) =>
            {
                float z = _zoomSlider.Value * 0.5f;
                _settings.Zoom = z;
                _lblZoomBadge.Text = (z % 1 == 0 ? ((int)z).ToString() : z.ToString("0.0")) + "×";
                NotifySettingsChanged();
            };
            Controls.Add(_zoomSlider);
            curY += 24;

            AddRangeLabels(curY, "1×", "5×", "10×");
            curY += 18;

            // Divider 2
            AddDivider(curY);
            curY += 8;

            // ── Lens Size Section ────────────────────────────────────────
            AddSectionHeader(curY, "LensSize", out _lblSizeTitle, out _lblSizeBadge, Localization.Get("Medium"));
            curY += 24;

            _sizeSlider = new CustomSlider(1, 3, 2);
            _sizeSlider.Location = new Point(padX, curY);
            _sizeSlider.Size = new Size(ClientSize.Width - padX * 2, 24);
            _sizeSlider.ValueChanged += (s, e) =>
            {
                string sz = "medium";
                if (_sizeSlider.Value == 1) sz = "small";
                else if (_sizeSlider.Value == 3) sz = "large";
                _settings.LensSize = sz;
                _lblSizeBadge.Text = sz == "small" ? Localization.Get("Small") : (sz == "large" ? Localization.Get("Large") : Localization.Get("Medium"));
                NotifySettingsChanged();
            };
            Controls.Add(_sizeSlider);
            curY += 24;

            AddSizeRangeLabels(curY, out _lblRangeSize1, out _lblRangeSize2, out _lblRangeSize3);
            curY += 18;

            // Divider 3
            AddDivider(curY);
            curY += 8;

            // ── Lens Shape Section ───────────────────────────────────────
            _lblShapeTitle = new Label
            {
                Text = Localization.Get("LensShape"),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Location = new Point(padX, curY),
                Size = new Size(180, 16)
            };
            Controls.Add(_lblShapeTitle);
            curY += 22;

            int cardW = (ClientSize.Width - padX * 2 - 10) / 2;
            _cardCircle = new ShapeOptionCard(Localization.Get("Circle"), true);
            _cardCircle.Location = new Point(padX, curY);
            _cardCircle.Size = new Size(cardW, 64);
            _cardCircle.Click += (s, e) => SelectShape("circle");

            _cardRect = new ShapeOptionCard(Localization.Get("Rectangle"), false);
            _cardRect.Location = new Point(padX + cardW + 10, curY);
            _cardRect.Size = new Size(cardW, 64);
            _cardRect.Click += (s, e) => SelectShape("rect");

            Controls.Add(_cardCircle);
            Controls.Add(_cardRect);
            curY += 72;

            // Divider 4
            AddDivider(curY);
            curY += 8;

            // ── Lens Position Section ────────────────────────────────────
            _lblPosTitle = new Label
            {
                Text = Localization.Get("LensPosition"),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Location = new Point(padX, curY),
                Size = new Size(180, 16)
            };
            Controls.Add(_lblPosTitle);
            curY += 20;

            _posGrid = new PositionGridControl();
            _posGrid.Location = new Point((ClientSize.Width - 126) / 2, curY);
            _posGrid.Size = new Size(126, 126);
            _posGrid.PositionSelected += (s, pos) =>
            {
                _settings.LensPosition = pos;
                NotifySettingsChanged();
            };
            Controls.Add(_posGrid);
            curY += 134;

            // Divider 5
            AddDivider(curY);
            curY += 8;

            // ── Hotkey Section ───────────────────────────────────────────
            _lblHkTitle = new Label
            {
                Text = Localization.Get("GlobalHotkey"),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Location = new Point(padX, curY),
                Size = new Size(140, 16)
            };
            Controls.Add(_lblHkTitle);

            _lblHotkeyBadge = new Label
            {
                Text = _settings.HotkeyDisplay,
                ForeColor = Color.FromArgb(96, 165, 250),
                BackColor = Color.FromArgb(20, 24, 36),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(ClientSize.Width - padX - 85, curY - 2),
                Size = new Size(85, 20)
            };
            Controls.Add(_lblHotkeyBadge);
            curY += 22;

            _btnChangeHotkey = new Button
            {
                Text = Localization.Get("ChangeHotkey"),
                Location = new Point(padX, curY),
                Size = new Size(ClientSize.Width - padX * 2, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(20, 24, 36),
                ForeColor = Color.FromArgb(167, 139, 250),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnChangeHotkey.FlatAppearance.BorderColor = Color.FromArgb(45, 51, 72);
            _btnChangeHotkey.Click += BtnChangeHotkey_Click;
            Controls.Add(_btnChangeHotkey);
            curY += 34;

            // Divider 6
            AddDivider(curY);
            curY += 8;

            // ── Language Selector Section ────────────────────────────────
            _lblLangTitle = new Label
            {
                Text = Localization.Get("Language"),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Location = new Point(padX, curY),
                Size = new Size(140, 16)
            };
            Controls.Add(_lblLangTitle);
            curY += 20;

            _cboLanguage = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(padX, curY),
                Size = new Size(ClientSize.Width - padX * 2, 28),
                BackColor = Color.FromArgb(20, 24, 36),
                ForeColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            foreach (var kvp in Localization.SupportedLanguages)
            {
                _cboLanguage.Items.Add(new LanguageComboItem(kvp.Key, kvp.Value));
            }

            _cboLanguage.SelectedIndexChanged += CboLanguage_SelectedIndexChanged;
            Controls.Add(_cboLanguage);
            curY += 36;

            // Divider 7
            AddDivider(curY);
            curY += 8;

            // ── Footer Section ───────────────────────────────────────────
            _btnPdfViewer = new Button
            {
                Text = Localization.Get("OpenPdfViewer"),
                Location = new Point(padX, curY),
                Size = new Size(ClientSize.Width - padX * 2, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 58, 138),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnPdfViewer.FlatAppearance.BorderColor = Color.FromArgb(59, 130, 246);
            _btnPdfViewer.Click += (s, e) =>
            {
                if (_onOpenPdfViewer != null) _onOpenPdfViewer();
            };
            Controls.Add(_btnPdfViewer);
            curY += 40;

            _lblSaved = new Label
            {
                Text = Localization.Get("SavedAuto"),
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(padX, curY),
                Size = new Size(ClientSize.Width - padX * 2, 16)
            };
            Controls.Add(_lblSaved);
            curY += 24;

            ClientSize = new Size(330, curY);
        }

        private void CboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressLangEvent) return;
            LanguageComboItem selected = _cboLanguage.SelectedItem as LanguageComboItem;
            if (selected != null)
            {
                _settings.Language = selected.Code;
                Localization.SetLanguage(selected.Code);
                UpdateLocalizedTexts();
                NotifySettingsChanged();
            }
        }

        public void UpdateLocalizedTexts()
        {
            Text = Localization.Get("AppTitle") + " " + Localization.Get("SettingsMenu").Replace(".", "");
            _headerPanel.Invalidate();

            _lblZoomTitle.Text = Localization.Get("ZoomLevel");
            _lblSizeTitle.Text = Localization.Get("LensSize");
            _lblShapeTitle.Text = Localization.Get("LensShape");
            _lblPosTitle.Text = Localization.Get("LensPosition");
            _lblHkTitle.Text = Localization.Get("GlobalHotkey");
            _lblLangTitle.Text = Localization.Get("Language");

            string sz = _settings.LensSize;
            _lblSizeBadge.Text = sz == "small" ? Localization.Get("Small") : (sz == "large" ? Localization.Get("Large") : Localization.Get("Medium"));

            _lblRangeSize1.Text = Localization.Get("Small");
            _lblRangeSize2.Text = Localization.Get("Medium");
            _lblRangeSize3.Text = Localization.Get("Large");

            _cardCircle.UpdateShapeName(Localization.Get("Circle"));
            _cardRect.UpdateShapeName(Localization.Get("Rectangle"));

            _btnChangeHotkey.Text = Localization.Get("ChangeHotkey");
            _btnPdfViewer.Text = Localization.Get("OpenPdfViewer");
            _lblSaved.Text = Localization.Get("SavedAuto");
        }

        private void AddDivider(int y)
        {
            Panel div = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(ClientSize.Width - 20, 1),
                BackColor = Color.FromArgb(45, 51, 72)
            };
            Controls.Add(div);
        }

        private void AddSectionHeader(int y, string key, out Label titleLabel, out Label badge, string badgeText)
        {
            titleLabel = new Label
            {
                Text = Localization.Get(key),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                Location = new Point(16, y),
                Size = new Size(180, 16)
            };
            Controls.Add(titleLabel);

            badge = new Label
            {
                Text = badgeText,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(29, 78, 216),
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(ClientSize.Width - 16 - 54, y - 2),
                Size = new Size(54, 20)
            };
            Controls.Add(badge);
        }

        private void AddRangeLabels(int y, string l1, string l2, string l3)
        {
            int padX = 16;
            int w = (ClientSize.Width - padX * 2) / 3;

            Label lbl1 = new Label { Text = l1, ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleLeft };
            Label lbl2 = new Label { Text = l2, ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX + w, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleCenter };
            Label lbl3 = new Label { Text = l3, ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX + w * 2, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleRight };

            Controls.Add(lbl1);
            Controls.Add(lbl2);
            Controls.Add(lbl3);
        }

        private void AddSizeRangeLabels(int y, out Label lbl1, out Label lbl2, out Label lbl3)
        {
            int padX = 16;
            int w = (ClientSize.Width - padX * 2) / 3;

            lbl1 = new Label { Text = Localization.Get("Small"), ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleLeft };
            lbl2 = new Label { Text = Localization.Get("Medium"), ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX + w, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleCenter };
            lbl3 = new Label { Text = Localization.Get("Large"), ForeColor = Color.FromArgb(71, 85, 105), Font = new Font("Segoe UI", 7.5f), Location = new Point(padX + w * 2, y), Size = new Size(w, 14), TextAlign = ContentAlignment.MiddleRight };

            Controls.Add(lbl1);
            Controls.Add(lbl2);
            Controls.Add(lbl3);
        }

        public void LoadValuesFromSettings()
        {
            _suppressToggleEvent = true;
            _toggleSwitch.Checked = _settings.Enabled;
            _suppressToggleEvent = false;

            int zVal = (int)Math.Round(_settings.Zoom * 2);
            _zoomSlider.Value = Math.Max(2, Math.Min(20, zVal));
            _lblZoomBadge.Text = (_settings.Zoom % 1 == 0 ? ((int)_settings.Zoom).ToString() : _settings.Zoom.ToString("0.0")) + "×";

            int szVal = _settings.LensSize == "small" ? 1 : (_settings.LensSize == "large" ? 3 : 2);
            _sizeSlider.Value = szVal;
            string sz = _settings.LensSize;
            _lblSizeBadge.Text = sz == "small" ? Localization.Get("Small") : (sz == "large" ? Localization.Get("Large") : Localization.Get("Medium"));

            SelectShape(_settings.LensShape, false);
            _posGrid.SelectedPosition = _settings.LensPosition;
            _lblHotkeyBadge.Text = _settings.HotkeyDisplay;

            _suppressLangEvent = true;
            for (int i = 0; i < _cboLanguage.Items.Count; i++)
            {
                LanguageComboItem item = _cboLanguage.Items[i] as LanguageComboItem;
                if (item != null && item.Code.Equals(_settings.Language, StringComparison.OrdinalIgnoreCase))
                {
                    _cboLanguage.SelectedIndex = i;
                    break;
                }
            }
            if (_cboLanguage.SelectedIndex < 0 && _cboLanguage.Items.Count > 0)
            {
                _cboLanguage.SelectedIndex = 0;
            }
            _suppressLangEvent = false;
        }

        public void SetToggleState(bool active)
        {
            _suppressToggleEvent = true;
            _toggleSwitch.Checked = active;
            _suppressToggleEvent = false;
        }

        private void SelectShape(string shape, bool triggerChange = true)
        {
            _settings.LensShape = shape;
            _cardCircle.IsSelected = (shape == "circle");
            _cardRect.IsSelected = (shape == "rect");
            if (triggerChange) NotifySettingsChanged();
        }

        private void NotifySettingsChanged()
        {
            _settings.Save();
            if (_onSettingsChanged != null) _onSettingsChanged();
        }

        private void BtnChangeHotkey_Click(object sender, EventArgs e)
        {
            using (HotkeyRecorderDialog dlg = new HotkeyRecorderDialog(_settings.HotkeyModifiers, _settings.HotkeyKey, _settings.HotkeyDisplay))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (_onHotkeyUpdated != null)
                    {
                        bool ok = _onHotkeyUpdated(dlg.RecordedModifiers, dlg.RecordedKey, dlg.RecordedDisplay);
                        if (ok)
                        {
                            _settings.HotkeyModifiers = dlg.RecordedModifiers;
                            _settings.HotkeyKey = dlg.RecordedKey;
                            _settings.HotkeyDisplay = dlg.RecordedDisplay;
                            _lblHotkeyBadge.Text = dlg.RecordedDisplay;
                            _settings.Save();
                        }
                    }
                }
            }
        }
    }

    public class LanguageComboItem
    {
        public string Code { get; private set; }
        public string DisplayName { get; private set; }

        public LanguageComboItem(string code, string displayName)
        {
            Code = code;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    // ─── Custom UI Controls with Rich Aesthetics ────────────────────────────

    public class HeaderPanel : Panel
    {
        public HeaderPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background gradient (#1a1f2e -> #141824)
            using (LinearGradientBrush bg = new LinearGradientBrush(ClientRectangle, Color.FromArgb(26, 31, 46), Color.FromArgb(20, 24, 36), 135f))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            // Draw Logo (Magnifying glass with blue-purple gradient)
            int iconX = 16;
            int iconY = 16;

            using (Pen circlePen = new Pen(Color.FromArgb(96, 165, 250), 3.5f))
            {
                g.DrawEllipse(circlePen, iconX + 2, iconY + 2, 18, 18);
            }
            using (Pen handlePen = new Pen(Color.FromArgb(167, 139, 250), 3.5f))
            {
                handlePen.StartCap = LineCap.Round;
                handlePen.EndCap = LineCap.Round;
                g.DrawLine(handlePen, iconX + 16, iconY + 16, iconX + 26, iconY + 26);
            }

            // Title "Magnifier" (localized)
            string title = Localization.Get("AppTitle");
            using (Font titleFont = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
            {
                g.DrawString(title, titleFont, titleBrush, iconX + 38, 12);
            }

            // Subtitle "Hover to magnify" (localized)
            string subtitle = Localization.Get("Subtitle");
            using (Font subFont = new Font("Segoe UI", 8.2f, FontStyle.Regular))
            using (Brush subBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                g.DrawString(subtitle, subFont, subBrush, iconX + 38, 34);
            }

            base.OnPaint(e);
        }
    }

    public class CustomToggle : Control
    {
        private bool _checked;
        public event EventHandler CheckedChanged;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    Invalidate();
                    if (CheckedChanged != null) CheckedChanged(this, EventArgs.Empty);
                }
            }
        }

        public CustomToggle()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int diameter = Height - 1;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, diameter, diameter, 90, 180);
                path.AddArc(Width - diameter - 1, 0, diameter, diameter, 270, 180);
                path.CloseFigure();

                if (_checked)
                {
                    using (LinearGradientBrush bg = new LinearGradientBrush(rect, Color.FromArgb(59, 130, 246), Color.FromArgb(139, 92, 246), 135f))
                    {
                        g.FillPath(bg, path);
                    }
                    using (Pen p = new Pen(Color.FromArgb(59, 130, 246), 1.5f))
                    {
                        g.DrawPath(p, path);
                    }
                    // Knob on right
                    int knobSize = Height - 8;
                    int knobX = Width - knobSize - 4;
                    using (Brush knobBrush = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(knobBrush, knobX, 4, knobSize, knobSize);
                    }
                }
                else
                {
                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(45, 51, 72)))
                    {
                        g.FillPath(bg, path);
                    }
                    using (Pen p = new Pen(Color.FromArgb(61, 68, 96), 1.5f))
                    {
                        g.DrawPath(p, path);
                    }
                    // Knob on left
                    int knobSize = Height - 8;
                    using (Brush knobBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                    {
                        g.FillEllipse(knobBrush, 4, 4, knobSize, knobSize);
                    }
                }
            }
        }
    }

    public class CustomSlider : Control
    {
        private int _min;
        private int _max;
        private int _val;
        private bool _isDragging;

        public event EventHandler ValueChanged;

        public int Value
        {
            get { return _val; }
            set
            {
                int clamped = Math.Max(_min, Math.Min(_max, value));
                if (_val != clamped)
                {
                    _val = clamped;
                    Invalidate();
                    if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        public CustomSlider(int min, int max, int defaultVal)
        {
            _min = min;
            _max = max;
            _val = defaultVal;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                UpdateValueFromMouse(e.X);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                UpdateValueFromMouse(e.X);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnMouseUp(e);
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int pad = 8;
            int trackWidth = Width - pad * 2;
            float ratio = Math.Max(0f, Math.Min(1f, (float)(mouseX - pad) / trackWidth));
            int newVal = (int)Math.Round(_min + ratio * (_max - _min));
            Value = newVal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int pad = 8;
            int trackHeight = 6;
            int trackY = (Height - trackHeight) / 2;
            int trackWidth = Width - pad * 2;

            float ratio = (float)(_val - _min) / (_max - _min);
            int thumbX = pad + (int)(ratio * trackWidth);
            int thumbY = Height / 2;
            int thumbRadius = 8;

            // Background track
            using (SolidBrush darkTrack = new SolidBrush(Color.FromArgb(45, 51, 72)))
            {
                Rectangle bgRect = new Rectangle(pad, trackY, trackWidth, trackHeight);
                g.FillRectangle(darkTrack, bgRect);
            }

            // Filled progress track (blue gradient)
            int fillW = thumbX - pad;
            if (fillW > 0)
            {
                using (LinearGradientBrush fillTrack = new LinearGradientBrush(new Rectangle(pad, trackY, fillW, trackHeight), Color.FromArgb(59, 130, 246), Color.FromArgb(139, 92, 246), 0f))
                {
                    g.FillRectangle(fillTrack, pad, trackY, fillW, trackHeight);
                }
            }

            // Thumb
            Rectangle thumbRect = new Rectangle(thumbX - thumbRadius, thumbY - thumbRadius, thumbRadius * 2, thumbRadius * 2);
            using (LinearGradientBrush thumbBrush = new LinearGradientBrush(thumbRect, Color.FromArgb(96, 165, 250), Color.FromArgb(167, 139, 250), 135f))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }
            using (Pen thumbBorder = new Pen(Color.FromArgb(15, 17, 23), 2f))
            {
                g.DrawEllipse(thumbBorder, thumbRect);
            }
        }
    }

    public class ShapeOptionCard : Control
    {
        public string ShapeName { get; private set; }
        public bool IsCircle { get; private set; }
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    Invalidate();
                }
            }
        }

        public ShapeOptionCard(string shapeName, bool isCircle)
        {
            ShapeName = shapeName;
            IsCircle = isCircle;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Cursor = Cursors.Hand;
        }

        public void UpdateShapeName(string shapeName)
        {
            ShapeName = shapeName;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (_isSelected)
            {
                using (LinearGradientBrush bg = new LinearGradientBrush(rect, Color.FromArgb(30, 42, 74), Color.FromArgb(35, 27, 61), 135f))
                {
                    g.FillRectangle(bg, rect);
                }
                using (Pen borderPen = new Pen(Color.FromArgb(99, 102, 241), 1.5f))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }
            else
            {
                using (SolidBrush bg = new SolidBrush(Color.FromArgb(20, 24, 36)))
                {
                    g.FillRectangle(bg, rect);
                }
                using (Pen borderPen = new Pen(Color.FromArgb(45, 51, 72), 1.5f))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }

            // Shape preview icon
            int iconY = 10;
            if (IsCircle)
            {
                int d = 26;
                int iconX = (Width - d) / 2;
                Rectangle iconRect = new Rectangle(iconX, iconY, d, d);
                Color shapeColor = _isSelected ? Color.FromArgb(167, 139, 250) : Color.FromArgb(59, 130, 246);
                using (Pen p = new Pen(shapeColor, 2.2f))
                {
                    g.DrawEllipse(p, iconRect);
                }
            }
            else
            {
                int w = 38;
                int h = 22;
                int iconX = (Width - w) / 2;
                Rectangle iconRect = new Rectangle(iconX, iconY + 2, w, h);
                Color shapeColor = _isSelected ? Color.FromArgb(167, 139, 250) : Color.FromArgb(59, 130, 246);
                using (Pen p = new Pen(shapeColor, 2.2f))
                {
                    g.DrawRectangle(p, iconRect);
                }
            }

            // Label
            using (Font f = new Font("Segoe UI", 8.2f, _isSelected ? FontStyle.Bold : FontStyle.Regular))
            using (Brush textBrush = new SolidBrush(_isSelected ? Color.FromArgb(167, 139, 250) : Color.FromArgb(148, 163, 184)))
            {
                SizeF sz = g.MeasureString(ShapeName, f);
                g.DrawString(ShapeName, f, textBrush, (Width - sz.Width) / 2, 42);
            }
        }
    }

    public class PositionGridControl : Control
    {
        private string _selectedPosition = "right";
        public event EventHandler<string> PositionSelected;

        public string SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                if (_selectedPosition != value)
                {
                    _selectedPosition = value;
                    Invalidate();
                }
            }
        }

        public PositionGridControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int cellW = 38;
            int gap = 6;
            int stride = cellW + gap;

            string[,] grid = new string[,] {
                { null,    "up",     null },
                { "left",  "center", "right" },
                { null,    "down",   null }
            };

            int col = e.X / stride;
            int row = e.Y / stride;

            if (col >= 0 && col < 3 && row >= 0 && row < 3)
            {
                string pos = grid[row, col];
                if (pos != null)
                {
                    SelectedPosition = pos;
                    if (PositionSelected != null) PositionSelected(this, pos);
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cellW = 38;
            int gap = 6;
            int stride = cellW + gap;

            string[,] grid = new string[,] {
                { null,    "up",     null },
                { "left",  "center", "right" },
                { null,    "down",   null }
            };

            string[,] symbols = new string[,] {
                { "",  "↑",  "" },
                { "←", "⊙",  "→" },
                { "",  "↓",  "" }
            };

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    string pos = grid[r, c];
                    if (pos == null) continue;

                    int x = c * stride;
                    int y = r * stride;
                    Rectangle rect = new Rectangle(x, y, cellW, cellW);
                    bool active = (pos == _selectedPosition);

                    if (active)
                    {
                        using (LinearGradientBrush bg = new LinearGradientBrush(rect, Color.FromArgb(30, 42, 74), Color.FromArgb(35, 27, 61), 135f))
                        {
                            g.FillRectangle(bg, rect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(99, 102, 241), 1.5f))
                        {
                            g.DrawRectangle(p, rect);
                        }
                    }
                    else
                    {
                        using (SolidBrush bg = new SolidBrush(Color.FromArgb(20, 24, 36)))
                        {
                            g.FillRectangle(bg, rect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(45, 51, 72), 1.2f))
                        {
                            g.DrawRectangle(p, rect);
                        }
                    }

                    // Arrow symbol
                    string sym = symbols[r, c];
                    using (Font f = new Font("Segoe UI", 12f, FontStyle.Bold))
                    using (Brush b = new SolidBrush(active ? Color.FromArgb(167, 139, 250) : Color.FromArgb(100, 116, 139)))
                    {
                        SizeF sz = g.MeasureString(sym, f);
                        g.DrawString(sym, f, b, x + (cellW - sz.Width) / 2, y + (cellW - sz.Height) / 2);
                    }
                }
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class PdfViewerForm : Form
    {
        private WebBrowser _webBrowser;
        private Label _emptyStateLabel;
        private Button _btnChoosePdf;
        private Button _btnClear;
        private Label _lblBrand;
        private Label _lblHint;
        private Panel _toolbarPanel;

        public PdfViewerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = Localization.Get("PdfTitle");
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(960, 720);
            BackColor = Color.FromArgb(15, 18, 27); // #0f121b
            ForeColor = Color.FromArgb(231, 236, 255);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Top Toolbar
            _toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(10, 14, 24),
                Padding = new Padding(12, 8, 12, 8)
            };
            _toolbarPanel.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(50, 70, 110), 1f))
                {
                    e.Graphics.DrawLine(p, 0, _toolbarPanel.Height - 1, _toolbarPanel.Width, _toolbarPanel.Height - 1);
                }
            };
            Controls.Add(_toolbarPanel);

            // Brand
            _lblBrand = new Label
            {
                Text = Localization.Get("PdfTitle"),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 236, 255),
                Location = new Point(14, 14),
                AutoSize = true
            };
            _toolbarPanel.Controls.Add(_lblBrand);

            // Choose PDF button
            _btnChoosePdf = new Button
            {
                Text = Localization.Get("ChoosePdf"),
                Location = new Point(220, 10),
                Size = new Size(110, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 34, 67),
                ForeColor = Color.FromArgb(219, 230, 255),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnChoosePdf.FlatAppearance.BorderColor = Color.FromArgb(70, 110, 180);
            _btnChoosePdf.Click += BtnChoosePdf_Click;
            _toolbarPanel.Controls.Add(_btnChoosePdf);

            // Clear button
            _btnClear = new Button
            {
                Text = Localization.Get("Clear"),
                Location = new Point(340, 10),
                Size = new Size(75, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(44, 26, 47),
                ForeColor = Color.FromArgb(240, 200, 240),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnClear.FlatAppearance.BorderColor = Color.FromArgb(120, 60, 110);
            _btnClear.Click += BtnClear_Click;
            _toolbarPanel.Controls.Add(_btnClear);

            // Hint
            _lblHint = new Label
            {
                Text = Localization.Get("PdfHint"),
                ForeColor = Color.FromArgb(153, 167, 207),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                Location = new Point(435, 16),
                AutoSize = true
            };
            _toolbarPanel.Controls.Add(_lblHint);

            // Empty state label
            _emptyStateLabel = new Label
            {
                Text = Localization.Get("PdfEmpty"),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                ForeColor = Color.FromArgb(165, 176, 216),
                Dock = DockStyle.Fill
            };
            Controls.Add(_emptyStateLabel);

            // WebBrowser for PDF rendering
            _webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                Visible = false,
                ScriptErrorsSuppressed = true
            };
            Controls.Add(_webBrowser);
        }

        private void BtnChoosePdf_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                ofd.Title = Localization.Get("ChoosePdf");
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    LoadPdf(ofd.FileName);
                }
            }
        }

        private void LoadPdf(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    _emptyStateLabel.Visible = false;
                    _webBrowser.Visible = true;
                    _webBrowser.Navigate(new Uri(filePath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening PDF: " + ex.Message, Localization.Get("PdfTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _webBrowser.Navigate("about:blank");
            _webBrowser.Visible = false;
            _emptyStateLabel.Visible = true;
        }
    }
}

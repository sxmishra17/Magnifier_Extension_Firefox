using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = Localization.Get("AboutTitle");
            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(380, 260);
            BackColor = Color.FromArgb(28, 25, 23); // Warm charcoal #1c1917
            ForeColor = Color.FromArgb(245, 245, 244);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Title Label
            Label lblTitle = new Label
            {
                Text = "YuvaTech Magnifier",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138), // #fef08a
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            // Version & Company Label
            Label lblDetails = new Label
            {
                Text = "Version 1.2.0\nDeveloped by YuvaTech\n\nReal-time screen & document magnification lens.",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(214, 211, 209),
                Location = new Point(22, 54),
                Size = new Size(320, 70)
            };
            Controls.Add(lblDetails);

            // GitHub Link Button
            Button btnGitHub = new Button
            {
                Text = "🌐 Visit GitHub Repository",
                Location = new Point(22, 134),
                Size = new Size(320, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(234, 88, 12),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGitHub.FlatAppearance.BorderSize = 0;
            btnGitHub.Click += (s, e) =>
            {
                try
                {
                    Process.Start("https://github.com/sxmishra17/Magnifier_Extension_Firefox");
                }
                catch { }
            };
            Controls.Add(btnGitHub);

            // Close Button
            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(242, 178),
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(44, 40, 36),
                ForeColor = Color.FromArgb(245, 245, 244),
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(87, 83, 78);
            Controls.Add(btnClose);

            AcceptButton = btnClose;
        }
    }
}

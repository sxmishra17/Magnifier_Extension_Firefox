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
            BackColor = Color.FromArgb(28, 25, 23); // Warm charcoal #1c1917
            ForeColor = Color.FromArgb(245, 245, 244);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Company Title LinkLabel
            LinkLabel lblTitle = new LinkLabel
            {
                Text = "Yuvatech Solution USA, LLC",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                LinkColor = Color.FromArgb(254, 240, 138), // #fef08a
                ActiveLinkColor = Color.FromArgb(245, 158, 11), // #f59e0b
                VisitedLinkColor = Color.FromArgb(254, 240, 138),
                Location = new Point(20, 20),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            lblTitle.LinkClicked += (s, e) =>
            {
                try
                {
                    Process.Start("https://yuvatechsolutionsusa.com");
                }
                catch { }
            };
            Controls.Add(lblTitle);

            // Version & Description
            Label lblDetails = new Label
            {
                Text = "Magnifier Desktop v1.3.0\nDeveloped by Yuvatech Solution USA, LLC\nReal-time screen & document magnification lens.",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(214, 211, 209),
                Location = new Point(22, 50),
                Size = new Size(350, 52)
            };
            Controls.Add(lblDetails);

            int btnW = 350;
            int btnH = 30;
            int curY = 110;

            // Visit Us Button (Company Website)
            Button btnWebsite = new Button
            {
                Text = "🌐 Visit Us: yuvatechsolutionsusa.com",
                Location = new Point(22, curY),
                Size = new Size(btnW, btnH),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(2, 132, 199), // Ocean Blue #0284c7
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnWebsite.FlatAppearance.BorderSize = 0;
            btnWebsite.Click += (s, e) =>
            {
                try
                {
                    Process.Start("https://yuvatechsolutionsusa.com");
                }
                catch { }
            };
            Controls.Add(btnWebsite);
            curY += 36;

            // View Other Extensions Button
            Button btnExtensions = new Button
            {
                Text = "🧩 View Other Extensions on Firefox",
                Location = new Point(22, curY),
                Size = new Size(btnW, btnH),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(124, 58, 237), // Purple #7c3aed
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExtensions.FlatAppearance.BorderSize = 0;
            btnExtensions.Click += (s, e) =>
            {
                try
                {
                    Process.Start("https://addons.mozilla.org/en-US/firefox/user/14938505/");
                }
                catch { }
            };
            Controls.Add(btnExtensions);
            curY += 36;

            // GitHub Link Button
            Button btnGitHub = new Button
            {
                Text = "💻 Visit GitHub Repository",
                Location = new Point(22, curY),
                Size = new Size(btnW, btnH),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(234, 88, 12), // Amber/Orange #ea580c
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
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
            curY += 42;

            // Close Button
            Button btnClose = new Button
            {
                Text = "Close",
                Location = new Point(272, curY),
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(44, 40, 36),
                ForeColor = Color.FromArgb(245, 245, 244),
                DialogResult = DialogResult.OK
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(87, 83, 78);
            Controls.Add(btnClose);
            curY += 38;

            ClientSize = new Size(394, curY);

            AcceptButton = btnClose;
        }
    }
}

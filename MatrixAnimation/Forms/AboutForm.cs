using System.Reflection;

namespace MatrixAnimation.Forms
{
    /// <summary>
    /// About dialog. Shows app info and a small preview header.
    /// </summary>
    public class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "About Matrix Animation";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 280);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(8, 12, 14);
            ForeColor = Color.FromArgb(220, 255, 230);
            Font = new Font("Consolas", 9F);

            var title = new Label
            {
                Text = "MATRIX ANIMATION",
                Font = new Font("Consolas", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 120),
                AutoSize = true,
                Location = new Point(20, 18),
            };
            Controls.Add(title);

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            var lblVer = new Label
            {
                Text = $"Version {ver?.Major}.{ver?.Minor}.{ver?.Build}",
                Font = new Font("Consolas", 10F),
                ForeColor = Color.FromArgb(0, 200, 100),
                AutoSize = true,
                Location = new Point(22, 50),
            };
            Controls.Add(lblVer);

            var lblDesc = new Label
            {
                Text =
                    "A neon-style Matrix rain animation built with C# and\r\n" +
                    "SkiaSharp. Features include:\r\n\r\n" +
                    "  * Smooth, GPU-friendly 2D rendering with SkiaSharp\r\n" +
                    "  * Six color themes and neon glow trails\r\n" +
                    "  * Adjustable speed, density, font, glow, mutation rate\r\n" +
                    "  * Fullscreen mode with click ripple effects\r\n" +
                    "  * Persisted user settings",
                Location = new Point(22, 80),
                Size = new Size(380, 130),
                ForeColor = Color.FromArgb(200, 255, 220),
                Font = new Font("Segoe UI", 9F),
            };
            Controls.Add(lblDesc);

            var lblCredit = new Label
            {
                Text = "Made with <3 by Jacky the Code Bender",
                Font = new Font("Consolas", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 200, 160),
                AutoSize = true,
                Location = new Point(22, 222),
            };
            Controls.Add(lblCredit);

            var btnOk = new Button
            {
                Text = "OK",
                Location = new Point(ClientSize.Width - 90, ClientSize.Height - 40),
                Size = new Size(70, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 40, 44),
                ForeColor = Color.FromArgb(220, 255, 230),
                DialogResult = DialogResult.OK,
            };
            btnOk.FlatAppearance.BorderColor = Color.FromArgb(0, 200, 100);
            Controls.Add(btnOk);
            AcceptButton = btnOk;
            CancelButton = btnOk;
        }
    }
}

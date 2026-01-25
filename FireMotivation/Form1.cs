using System;
using System.Windows.Forms;

namespace FireMotivation
{
    public partial class Form1 : Form
    {
        private FireControl fireCtrl;

        public Form1()
        {
            InitializeComponent();
            
            this.Text = "Fire Motivation - Jacky Code Bender";
            this.Size = new System.Drawing.Size(800, 600);
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None; // Full screen experience
            this.KeyPreview = true;

            // Tambahkan control Api
            fireCtrl = new FireControl();
            fireCtrl.Dock = DockStyle.Fill;
            this.Controls.Add(fireCtrl);

            // Keluar jika tombol ESC ditekan
            this.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Escape) Application.Exit();
            };
        }
    }
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NiceViz
{
    public partial class Form1 : Form
    {
        // Timer untuk animasi (60 FPS)
        private System.Windows.Forms.Timer animationTimer;
        
        // Mode Visualisasi: 0=Bars, 1=Wave, 2=Circle
        private int currentMode = 0;
        private string[] modeNames = { "Spectrum Bars", "Digital Wave", "Neon Pulse" };
        
        // Data simulasi audio
        private float[] audioData;
        private Random rng;
        
        // Warna-warna keren ala Winamp/Cyberpunk
        private Color colorPrimary = Color.Cyan;
        private Color colorSecondary = Color.Magenta;
        private Color colorBackground = Color.FromArgb(10, 10, 20); // Dark Blue-Black

        public Form1()
        {
            InitializeCustomComponents();
            
            // Setup data buffer (misal 64 band)
            audioData = new float[64];
            rng = new Random();
            
            // Konfigurasi Form
            this.Text = "NiceViz - Audio Visualizer (Jacky Style)";
            this.Size = new Size(800, 500);
            this.BackColor = colorBackground;
            this.DoubleBuffered = true; // Penting supaya tidak kedip-kedip (flicker)
            this.StartPosition = FormStartPosition.CenterScreen;

            // Timer setup
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void InitializeCustomComponents()
        {
            // Tombol Ganti Mode
            Button btnChangeMode = new Button();
            btnChangeMode.Text = "Switch Mode";
            btnChangeMode.Size = new Size(120, 40);
            btnChangeMode.Location = new Point(10, 10);
            btnChangeMode.ForeColor = Color.White;
            btnChangeMode.BackColor = Color.FromArgb(50, 50, 70);
            btnChangeMode.FlatStyle = FlatStyle.Flat;
            btnChangeMode.Click += (s, e) => {
                currentMode = (currentMode + 1) % 3;
            };
            this.Controls.Add(btnChangeMode);

            // Label Info
            Label lblInfo = new Label();
            lblInfo.Text = "Mode: " + modeNames[currentMode];
            lblInfo.ForeColor = Color.Yellow;
            lblInfo.Location = new Point(140, 20);
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Consolas", 12, FontStyle.Bold);
            
            // Update label saat mode berubah (kita check di onpaint saja biar simple)
            this.Paint += (s, e) => { lblInfo.Text = "Mode: " + modeNames[currentMode]; };
            this.Controls.Add(lblInfo);
        }

        // Logic Simulasi Audio: Membuat data acak yang terlihat "koheren" seperti musik
        private void SimulateAudioData()
        {
            for (int i = 0; i < audioData.Length; i++)
            {
                // Nilai target acak (0.0 sampai 1.0)
                float target = (float)rng.NextDouble();
                
                // Sedikit smoothing agar tidak terlalu jittery (gerak kasar)
                // Kita geser nilai lama mendekati nilai baru
                audioData[i] = (audioData[i] * 0.7f) + (target * 0.3f);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            SimulateAudioData();
            this.Invalidate(); // Paksa gambar ulang (Memicu event OnPaint)
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gambar visualisasi berdasarkan mode
            switch (currentMode)
            {
                case 0: DrawBars(g); break;
                case 1: DrawWave(g); break;
                case 2: DrawCircle(g); break;
            }
        }

        // === VISUALIZER 1: SPECTRUM BARS ===
        private void DrawBars(Graphics g)
        {
            float barWidth = (float)this.ClientSize.Width / audioData.Length;
            float maxHeight = this.ClientSize.Height * 0.8f;
            int bottomY = this.ClientSize.Height;

            for (int i = 0; i < audioData.Length; i++)
            {
                float val = audioData[i];
                float h = val * maxHeight;
                
                // Gunakan gradasi warna
                Color barColor = LerpColor(colorPrimary, colorSecondary, val);
                using (Brush brush = new SolidBrush(barColor))
                {
                    g.FillRectangle(brush, i * barWidth, bottomY - h, barWidth - 2, h);
                }
            }
        }

        // === VISUALIZER 2: DIGITAL WAVE ===
        private void DrawWave(Graphics g)
        {
            // Titik tengah layar
            float centerY = this.ClientSize.Height / 2;
            float stepX = (float)this.ClientSize.Width / (audioData.Length - 1);
            
            List<PointF> points = new List<PointF>();

            for (int i = 0; i < audioData.Length; i++)
            {
                float val = audioData[i] - 0.5f; // Range -0.5 sampai 0.5
                float amplitude = 200; // Tinggi gelombang
                
                float x = i * stepX;
                float y = centerY + (val * amplitude);
                points.Add(new PointF(x, y));
            }

            using (Pen pen = new Pen(Color.LimeGreen, 3))
            {
                g.DrawLines(pen, points.ToArray());
            }

            // Efek bayangan (Glow simple)
            using (Pen penGlow = new Pen(Color.FromArgb(50, 0, 255, 0), 10))
            {
                g.DrawLines(penGlow, points.ToArray());
            }
        }

        // === VISUALIZER 3: NEON CIRCLE (PUSAT) ===
        private void DrawCircle(Graphics g)
        {
            float centerX = this.ClientSize.Width / 2;
            float centerY = this.ClientSize.Height / 2;
            
            // Ambil rata-rata bass (frekuensi rendah) untuk ukuran lingkaran
            float avgBass = 0;
            for(int i=0; i<10; i++) avgBass += audioData[i];
            avgBass /= 10;

            float radius = 50 + (avgBass * 200);

            // Gambar lingkaran luar
            using (Pen pen = new Pen(colorSecondary, 5))
            {
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Gambar lingkaran dalam (isi)
            using (Brush brush = new SolidBrush(Color.FromArgb(100, colorPrimary)))
            {
                float innerRadius = radius * 0.8f;
                g.FillEllipse(brush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Partikel kecil di sekeliling (dekorasi)
            for(int i=0; i<audioData.Length; i+=2)
            {
                float angle = (i / (float)audioData.Length) * 360;
                float dist = radius + (audioData[i] * 50);
                float px = centerX + (float)(Math.Cos(angle * Math.PI / 180) * dist);
                float py = centerY + (float)(Math.Sin(angle * Math.PI / 180) * dist);
                
                g.FillRectangle(Brushes.White, px, py, 4, 4);
            }
        }

        // Helper untuk mencampur warna
        private Color LerpColor(Color s, Color e, float k)
        {
            var bk = (255 - 255 * k); // Opacity variation if needed, or simple RGB lerp
            var r = s.R + (e.R - s.R) * k;
            var g = s.G + (e.G - s.G) * k;
            var b = s.B + (e.B - s.B) * k;
            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }
    }
}

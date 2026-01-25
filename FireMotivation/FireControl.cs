using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FireMotivation
{
    // Custom Control untuk menangani animasi Api
    public class FireControl : Control
    {
        private System.Windows.Forms.Timer renderTimer;
        private Bitmap? fireBitmap;
        private int[] heatMap;
        private int fireWidth;
        private int fireHeight;
        private Color[] palette;
        
        // Text Animation Variables
        private string[] quotes = new string[]
        {
            "JANGAN MENYERAH!",
            "TERUSLAH MEMBARA",
            "MIMPI BUTUH AKSI",
            "GAGAL ITU BIASA",
            "BANGKIT ITU LUAR BIASA",
            "BELIEVE IN YOURSELF",
            "KEEP THE FIRE ALIVE",
            "YOU ARE UNSTOPPABLE",
            "FOCUS ON YOUR GOAL",
            "BE THE LIGHT"
        };
        
        private int currentQuoteIndex = 0;
        private int textAlpha = 0;
        private int textState = 0; // 0: FadeIn, 1: Hold, 2: FadeOut
        private int holdTimer = 0;
        private const int FADE_SPEED = 5;
        private const int HOLD_DURATION = 100; // frames

        public FireControl()
        {
            // Mengaktifkan Double Buffering agar tidak kedip (flicker)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.UserPaint | 
                          ControlStyles.OptimizedDoubleBuffer | 
                          ControlStyles.ResizeRedraw, true);

            this.BackColor = Color.Black;
            
            // Inisialisasi Palette Warna Api
            GeneratePalette();
            
            // Timer untuk loop animasi (sekitar 30-40 FPS cukup untuk efek api)
            renderTimer = new System.Windows.Forms.Timer();
            renderTimer.Interval = 16; // ~60 FPS
            renderTimer.Tick += (s, e) => UpdateFrame();
            renderTimer.Start();
        }

        private void InitializeFire(int width, int height)
        {
            // Kita buat resolusi api lebih kecil dari layar asli agar efek pixelnya terlihat (retro style)
            // dan performanya jauh lebih ringan.
            fireWidth = width / 4; 
            fireHeight = height / 4; 

            if (fireWidth < 1) fireWidth = 10;
            if (fireHeight < 1) fireHeight = 10;

            heatMap = new int[fireWidth * fireHeight];
            fireBitmap = new Bitmap(fireWidth, fireHeight, PixelFormat.Format32bppPArgb);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InitializeFire(this.Width, this.Height);
        }

        private void GeneratePalette()
        {
            palette = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                // HSL to RGB conversion simplified for Fire
                // Logic: 0 = Black, 255 = White
                // Gradient: Black -> Red -> Orange -> Yellow -> White
                
                int r = 0, g = 0, b = 0;
                
                // Red component
                r = i * 3;
                if (r > 255) r = 255;
                
                // Green component (starts appearing later to create orange/yellow)
                if (i > 80) g = (i - 80) * 4;
                if (g > 255) g = 255;
                
                // Blue component (only at highest heat for white)
                if (i > 180) b = (i - 180) * 6;
                if (b > 255) b = 255;

                // Set alpha to full except for very low heat (transparency smoke effect)
                int a = (i < 10) ? i * 25 : 255; 

                palette[i] = Color.FromArgb(a, r, g, b);
            }
        }

        private void UpdateFrame()
        {
            if (heatMap == null || fireBitmap == null) return;

            UpdateFirePhysics();
            UpdateTextAnimation();
            RenderFireToBitmap();
            
            this.Invalidate(); // Memicu OnPaint
        }

        private void UpdateFirePhysics()
        {
            // 1. Seed the bottom line with random intense heat
            Random rnd = new Random();
            for (int x = 0; x < fireWidth; x++)
            {
                // Baris paling bawah (fireHeight - 1)
                int index = (fireHeight - 1) * fireWidth + x;
                heatMap[index] = rnd.Next(200, 256); // Panas acak 200-255
            }

            // 2. Propagate heat upwards
            for (int y = 0; y < fireHeight - 1; y++)
            {
                for (int x = 0; x < fireWidth; x++)
                {
                    // Ambil panas dari pixel di bawahnya
                    // Kita ambil sedikit acak kiri/kanan untuk efek angin
                    int drift = rnd.Next(0, 3); // 0, 1, atau 2
                    int srcIndex = (y + 1) * fireWidth + ((x - drift + 1 + fireWidth) % fireWidth);
                    
                    int heat = heatMap[srcIndex];
                    
                    // Dinginkan api saat naik ke atas
                    int decay = rnd.Next(0, 3); // Pengurangan panas acak
                    heat -= decay;
                    
                    if (heat < 0) heat = 0;

                    int destIndex = y * fireWidth + x;
                    heatMap[destIndex] = heat;
                }
            }
        }

        private unsafe void RenderFireToBitmap()
        {
            BitmapData bData = fireBitmap!.LockBits(new Rectangle(0, 0, fireWidth, fireHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            int* scan0 = (int*)bData.Scan0.ToPointer();
            int totalPixels = fireWidth * fireHeight;

            for (int i = 0; i < totalPixels; i++)
            {
                int heatIndex = heatMap[i];
                if (heatIndex > 255) heatIndex = 255;
                if (heatIndex < 0) heatIndex = 0;
                
                scan0[i] = palette[heatIndex].ToArgb();
            }

            fireBitmap.UnlockBits(bData);
        }

        private void UpdateTextAnimation()
        {
            switch (textState)
            {
                case 0: // Fade In
                    textAlpha += FADE_SPEED;
                    if (textAlpha >= 255)
                    {
                        textAlpha = 255;
                        textState = 1; // Hold
                        holdTimer = HOLD_DURATION;
                    }
                    break;
                case 1: // Hold
                    holdTimer--;
                    if (holdTimer <= 0)
                    {
                        textState = 2; // Fade Out
                    }
                    break;
                case 2: // Fade Out
                    textAlpha -= FADE_SPEED;
                    if (textAlpha <= 0)
                    {
                        textAlpha = 0;
                        textState = 3; // Switch Quote
                    }
                    break;
                case 3: // Switch Quote
                    currentQuoteIndex = (currentQuoteIndex + 1) % quotes.Length;
                    textState = 0; // Back to Fade In
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (fireBitmap == null) return;

            Graphics g = e.Graphics;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor; // Agar pixel api terlihat tajam (retro look)
            g.PixelOffsetMode = PixelOffsetMode.Half;

            // Gambar Api (Stretch ke seluruh layar)
            g.DrawImage(fireBitmap, new Rectangle(0, 0, this.Width, this.Height));

            // Gambar Text
            DrawMotivationalText(g);
        }

        private void DrawMotivationalText(Graphics g)
        {
            if (textAlpha <= 5) return;

            string text = quotes[currentQuoteIndex];
            
            // Gunakan font yang estetik
            using (Font font = new Font("Segoe UI", 36, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(textAlpha, 255, 255, 255)))
            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(textAlpha > 100 ? 100 : textAlpha, 0, 0, 0)))
            {
                SizeF textSize = g.MeasureString(text, font);
                float x = (this.Width - textSize.Width) / 2;
                float y = (this.Height - textSize.Height) / 2;

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                
                // Gambar Shadow agar terbaca jelas di atas api
                g.DrawString(text, font, shadowBrush, x + 2, y + 2);
                
                // Gambar Teks Utama
                g.DrawString(text, font, textBrush, x, y);
            }
        }
    }
}

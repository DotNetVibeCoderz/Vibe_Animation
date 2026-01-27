using System;

namespace P5Net.Examples
{
    /// <summary>
    /// Contoh Animasi Kaleidoscope - Pola simetris yang indah
    /// </summary>
    public class KaleidoscopeExample : P5Canvas
    {
        private int symmetry = 12; // Jumlah simetri
        private float angle;
        
        public KaleidoscopeExample()
        {
            Width = 800;
            Height = 800;
            this.ClientSize = new System.Drawing.Size(Width, Height);
            this.Text = "P5Net - Kaleidoscope Animation";
        }
        
        public override void Setup()
        {
            Background(0);
            angle = TWO_PI / symmetry;
        }
        
        public override void Draw()
        {
            // Background gelap dengan sedikit transparansi untuk efek trail
            Fill(0, 0, 0, 10);
            Rect(0, 0, Width, Height);
            
            // Pindah ke tengah
            PushMatrix();
            Translate(Width / 2, Height / 2);
            
            // Hitung posisi mouse relatif ke center
            float mx = MouseX - Width / 2;
            float my = MouseY - Height / 2;
            
            // Gambar pola kaleidoscope
            for (int i = 0; i < symmetry; i++)
            {
                PushMatrix();
                Rotate(angle * i);
                
                // Pattern 1: Lingkaran bergerak
                float x1 = mx * 0.5f;
                float y1 = my * 0.5f;
                float size1 = 20 + (float)Math.Sin(FrameCount * 0.05f) * 10;
                
                // Warna rainbow berdasarkan posisi
                int hue = (int)((FrameCount + i * 30) % 360);
                var color = ColorFromHSV(hue, 100, 100);
                
                Fill(color.R, color.G, color.B, 180);
                NoStroke();
                Circle(x1, y1, size1);
                
                // Pattern 2: Garis dari center
                Stroke(color.R, color.G, color.B, 100);
                StrokeWeight(2);
                Line(0, 0, x1, y1);
                
                // Pattern 3: Lingkaran kecil di sepanjang radius
                for (int j = 0; j < 5; j++)
                {
                    float t = j / 5f;
                    float px = Lerp(0, x1, t);
                    float py = Lerp(0, y1, t);
                    float pSize = 5 + (float)Math.Sin(FrameCount * 0.1f + j) * 3;
                    
                    NoStroke();
                    Fill(color.R, color.G, color.B, 150);
                    Circle(px, py, pSize);
                }
                
                // Mirror pattern (cermin horizontal)
                PushMatrix();
                Scale(1, -1);
                
                Fill(color.R, color.G, color.B, 120);
                Circle(x1, y1, size1 * 0.7f);
                
                PopMatrix();
                
                PopMatrix();
            }
            
            // Lingkaran tengah
            Fill(255, 255, 255, 200);
            NoStroke();
            Circle(0, 0, 30 + (float)Math.Sin(FrameCount * 0.1f) * 10);
            
            PopMatrix();
            
            // Info text
            Fill(255);
            g.DrawString("Kaleidoscope Animation - Move your mouse!", 
                new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold), 
                new System.Drawing.SolidBrush(System.Drawing.Color.White), 
                10, 10);
            g.DrawString($"Symmetry: {symmetry} | Frame: {FrameCount}", 
                new System.Drawing.Font("Arial", 10), 
                new System.Drawing.SolidBrush(System.Drawing.Color.White), 
                10, 35);
        }
        
        // Helper untuk convert HSV ke RGB
        private System.Drawing.Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value / 100;
            saturation = saturation / 100;

            int v = Convert.ToInt32(value * 255);
            int p = Convert.ToInt32(v * (1 - saturation));
            int q = Convert.ToInt32(v * (1 - f * saturation));
            int t = Convert.ToInt32(v * (1 - (1 - f) * saturation));

            if (hi == 0)
                return System.Drawing.Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return System.Drawing.Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return System.Drawing.Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return System.Drawing.Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return System.Drawing.Color.FromArgb(255, t, p, v);
            else
                return System.Drawing.Color.FromArgb(255, v, p, q);
        }
    }
}

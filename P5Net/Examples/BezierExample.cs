using System;
using System.Collections.Generic;

namespace P5Net.Examples
{
    /// <summary>
    /// Contoh Animasi Bezier - Kurva Bezier yang dinamis dan interaktif
    /// </summary>
    public class BezierExample : P5Canvas
    {
        private List<BezierCurve> curves;
        private float t = 0;
        
        private class BezierCurve
        {
            public float x1, y1, x2, y2, x3, y3, x4, y4;
            public System.Drawing.Color color;
            public float phase;
            
            public BezierCurve(float x1, float y1, float x2, float y2, 
                             float x3, float y3, float x4, float y4, 
                             System.Drawing.Color color, float phase)
            {
                this.x1 = x1; this.y1 = y1;
                this.x2 = x2; this.y2 = y2;
                this.x3 = x3; this.y3 = y3;
                this.x4 = x4; this.y4 = y4;
                this.color = color;
                this.phase = phase;
            }
        }
        
        public BezierExample()
        {
            Width = 900;
            Height = 600;
            this.ClientSize = new System.Drawing.Size(Width, Height);
            this.Text = "P5Net - Bezier Animation";
            curves = new List<BezierCurve>();
        }
        
        public override void Setup()
        {
            Background(20, 20, 30);
            
            // Buat beberapa kurva bezier dengan warna berbeda
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                float x1 = 100;
                float y1 = Height / 2;
                float x4 = Width - 100;
                float y4 = Height / 2;
                float x2 = Random(200, Width - 200);
                float y2 = Random(100, Height - 100);
                float x3 = Random(200, Width - 200);
                float y3 = Random(100, Height - 100);
                
                var color = ColorFromHSV(i * 60, 80, 100);
                curves.Add(new BezierCurve(x1, y1, x2, y2, x3, y3, x4, y4, color, i * 0.5f));
            }
        }
        
        public override void Draw()
        {
            // Background dengan transparansi untuk efek trail
            Fill(20, 20, 30, 30);
            NoStroke();
            Rect(0, 0, Width, Height);
            
            // Update time parameter
            t += 0.005f;
            if (t > 1) t = 0;
            
            // Gambar setiap kurva bezier
            for (int i = 0; i < curves.Count; i++)
            {
                var curve = curves[i];
                
                // Animasi control points
                float phase = curve.phase + FrameCount * 0.02f;
                float x2 = curve.x2 + (float)Math.Sin(phase) * 50;
                float y2 = curve.y2 + (float)Math.Cos(phase) * 50;
                float x3 = curve.x3 + (float)Math.Cos(phase * 1.3f) * 50;
                float y3 = curve.y3 + (float)Math.Sin(phase * 1.3f) * 50;
                
                // Gambar kurva bezier
                NoFill();
                Stroke(curve.color.R, curve.color.G, curve.color.B, 150);
                StrokeWeight(3);
                Bezier(curve.x1, curve.y1, x2, y2, x3, y3, curve.x4, curve.y4);
                
                // Gambar control points
                Fill(curve.color.R, curve.color.G, curve.color.B, 100);
                NoStroke();
                Circle(curve.x1, curve.y1, 10);
                Circle(curve.x4, curve.y4, 10);
                
                Fill(255, 255, 255, 100);
                Circle(x2, y2, 8);
                Circle(x3, y3, 8);
                
                // Gambar garis ke control points
                Stroke(255, 255, 255, 50);
                StrokeWeight(1);
                Line(curve.x1, curve.y1, x2, y2);
                Line(curve.x4, curve.y4, x3, y3);
                
                // Gambar titik yang bergerak di sepanjang kurva
                float currentT = (t + i * 0.2f) % 1;
                var point = BezierPoint(curve.x1, curve.y1, x2, y2, x3, y3, curve.x4, curve.y4, currentT);
                
                NoStroke();
                Fill(curve.color.R, curve.color.G, curve.color.B, 255);
                Circle(point.X, point.Y, 15);
                
                // Glow effect
                Fill(curve.color.R, curve.color.G, curve.color.B, 50);
                Circle(point.X, point.Y, 30);
                
                // Gambar partikel trail
                for (float tt = 0; tt < currentT; tt += 0.02f)
                {
                    var trailPoint = BezierPoint(curve.x1, curve.y1, x2, y2, x3, y3, curve.x4, curve.y4, tt);
                    float alpha = 20 * (tt / currentT);
                    Fill(curve.color.R, curve.color.G, curve.color.B, (int)alpha);
                    Circle(trailPoint.X, trailPoint.Y, 5);
                }
            }
            
            // Wave effect di background
            NoFill();
            for (int i = 0; i < 3; i++)
            {
                Stroke(100, 150, 255, 20);
                StrokeWeight(2);
                
                PushMatrix();
                Translate(0, Height / 2 + i * 50);
                
                for (float x = 0; x < Width; x += 10)
                {
                    float y = (float)Math.Sin(x * 0.02f + FrameCount * 0.05f + i) * 30;
                    Point(x, y);
                }
                PopMatrix();
            }
            
            // Info text
            Fill(255);
            var font14 = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            var font10 = new System.Drawing.Font("Arial", 10);
            var whiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            
            g.DrawString("Bezier Curve Animation", font14, whiteBrush, 10, 10);
            g.DrawString("Watch the points travel along the curves", font10, whiteBrush, 10, 35);
            g.DrawString($"Frame: {FrameCount}", font10, whiteBrush, 10, 55);
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

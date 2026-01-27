using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScreenSavers.Animations
{
    // --- Marquee / Text Scroller ---
    public class MarqueeAnimation : IScreenSaverAnimation
    {
        private float x;
        private string text = "Welcome to Gravicode Studios! Jacky is coding...";
        private Font font = new Font("Arial", 48, FontStyle.Bold);
        
        public void Initialize(int width, int height)
        {
            x = width;
        }

        public void Resize(int width, int height) { }

        public void Update(float deltaTime)
        {
            x -= 200 * deltaTime;
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Black);
            var size = g.MeasureString(text, font);
            
            if (x < -size.Width) x = width;

            using (Brush b = new SolidBrush(Color.Lime))
            {
                g.DrawString(text, font, b, x, height / 2 - size.Height / 2);
            }
        }
    }

    // --- Bezier Curves ---
    public class BezierAnimation : IScreenSaverAnimation
    {
        private PointF[] pts = new PointF[4];
        private PointF[] vels = new PointF[4];
        private Random rand = new Random();
        private Color color;

        public void Initialize(int width, int height)
        {
            color = Color.Cyan;
            for(int i=0; i<4; i++)
            {
                pts[i] = new PointF(rand.Next(width), rand.Next(height));
                vels[i] = new PointF(rand.Next(-100, 100), rand.Next(-100, 100));
            }
        }

        public void Resize(int width, int height) { }
        public void Update(float deltaTime) { }

        public void Draw(Graphics g, int width, int height)
        {
            // Trail effect by not clearing completely? 
            // Standard winforms double buffer clears. So we just draw one curve for simplicity or multiple.
            g.Clear(Color.Black);

            for (int i = 0; i < 4; i++)
            {
                pts[i].X += vels[i].X * 0.02f;
                pts[i].Y += vels[i].Y * 0.02f;

                if (pts[i].X < 0 || pts[i].X > width) vels[i].X = -vels[i].X;
                if (pts[i].Y < 0 || pts[i].Y > height) vels[i].Y = -vels[i].Y;
            }

            using (Pen p = new Pen(color, 5))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawBezier(p, pts[0], pts[1], pts[2], pts[3]);
            }
        }
    }

    // --- Wavy Flag (Simplified Sine Wave Grid) ---
    public class WavyFlagAnimation : IScreenSaverAnimation
    {
        private float offset = 0;

        public void Initialize(int width, int height) { }
        public void Resize(int width, int height) { }
        public void Update(float deltaTime)
        {
            offset += deltaTime * 5;
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Blue);
            int rows = 10;
            int cols = 15;
            float cellW = width / (float)cols;
            float cellH = height / (float)rows;

            using (Pen p = new Pen(Color.White, 2))
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        // Calculate wavy position
                        float waveX = (float)Math.Sin(x * 0.5 + offset) * 20;
                        float waveY = (float)Math.Cos(y * 0.5 + offset) * 20;

                        float drawX = x * cellW + waveX;
                        float drawY = y * cellH + waveY;

                        g.DrawRectangle(p, drawX, drawY, cellW * 0.8f, cellH * 0.8f);
                    }
                }
            }
        }
    }
}
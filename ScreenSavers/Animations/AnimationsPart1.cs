using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScreenSavers.Animations
{
    // --- Starfield Simulation ---
    public class StarfieldAnimation : IScreenSaverAnimation
    {
        private struct Star
        {
            public float X, Y, Z;
        }

        private List<Star> stars = new List<Star>();
        private Random rand = new Random();
        private float speed = 100.0f;

        public void Initialize(int width, int height)
        {
            stars.Clear();
            for (int i = 0; i < 400; i++)
            {
                stars.Add(GetRandomStar(width, height));
            }
        }

        private Star GetRandomStar(int w, int h)
        {
            return new Star
            {
                X = rand.Next(-w, w),
                Y = rand.Next(-h, h),
                Z = rand.Next(1, w)
            };
        }

        public void Resize(int width, int height) { }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                var s = stars[i];
                s.Z -= speed * deltaTime;
                if (s.Z <= 1)
                {
                    // Reset star to back
                    s.Z = 1000; 
                    s.X = rand.Next(-1000, 1000);
                    s.Y = rand.Next(-1000, 1000);
                }
                stars[i] = s;
            }
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Black);
            float cx = width / 2f;
            float cy = height / 2f;

            using (Brush brush = new SolidBrush(Color.White))
            {
                foreach (var s in stars)
                {
                    float k = 128.0f / s.Z;
                    float x = s.X * k + cx;
                    float y = s.Y * k + cy;

                    float size = (1 - s.Z / 1000.0f) * 5;
                    if (x >= 0 && x < width && y >= 0 && y < height && size > 0)
                    {
                        g.FillEllipse(brush, x, y, size, size);
                    }
                }
            }
        }
    }

    // --- Flying Windows (Simulated with Colored Rectangles) ---
    public class FlyingWindowsAnimation : IScreenSaverAnimation
    {
        private float x, y;
        private float dx, dy;
        private int rectSize = 80;
        private Color currentColor;
        private Random rand = new Random();

        public void Initialize(int width, int height)
        {
            x = rand.Next(0, width - rectSize);
            y = rand.Next(0, height - rectSize);
            dx = 150f;
            dy = 150f;
            PickNewColor();
        }

        private void PickNewColor()
        {
            currentColor = Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255));
        }

        public void Resize(int width, int height) { }

        public void Update(float deltaTime)
        {
            x += dx * deltaTime;
            y += dy * deltaTime;

            // Bounce logic
            // Note: Since we don't pass width/height to update, we assume drawing happens frequently enough
            // But wait, Update doesn't know width/height here. 
            // We should store bounds or pass them. Let's rely on Draw for bounds check or store in Resize.
        }

        // Modified architecture: Pass bounds to update? Or store locally. 
        // Let's simple check bounds in Draw or Update with stored bounds.
        private int _width, _height;

        public void Draw(Graphics g, int width, int height)
        {
            _width = width;
            _height = height;

            // Move logic check here to ensure bounds
            if (x <= 0 || x + rectSize >= width)
            {
                dx = -dx;
                PickNewColor();
                x = Math.Clamp(x, 0, width - rectSize);
            }
            if (y <= 0 || y + rectSize >= height)
            {
                dy = -dy;
                PickNewColor();
                y = Math.Clamp(y, 0, height - rectSize);
            }

            g.Clear(Color.Black);
            
            // Draw Window-like shape
            using (Pen p = new Pen(currentColor, 4))
            {
                g.DrawRectangle(p, x, y, rectSize, rectSize);
                g.DrawLine(p, x, y + rectSize/2, x + rectSize, y + rectSize/2);
                g.DrawLine(p, x + rectSize/2, y, x + rectSize/2, y + rectSize);
            }
        }
    }

    // --- Mystify ---
    public class MystifyAnimation : IScreenSaverAnimation
    {
        private List<PointF[]> history = new List<PointF[]>();
        private PointF[] points = new PointF[4];
        private PointF[] vels = new PointF[4];
        private Color[] colors = { Color.Red, Color.Blue, Color.Lime, Color.Yellow };
        private Random rand = new Random();

        public void Initialize(int width, int height)
        {
            for(int i=0; i<4; i++)
            {
                points[i] = new PointF(rand.Next(width), rand.Next(height));
                vels[i] = new PointF((float)(rand.NextDouble()*200 - 100), (float)(rand.NextDouble()*200 - 100));
            }
        }

        public void Resize(int width, int height) { }

        public void Update(float deltaTime)
        {
            // Update points logic is better handled inside Draw or with stored bounds
            // But strict separation is better. Let's just animate colors in Update.
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Black);

            // Update Physics
            for (int i = 0; i < 4; i++)
            {
                points[i].X += vels[i].X * 0.016f; // approx delta
                points[i].Y += vels[i].Y * 0.016f;

                if (points[i].X < 0 || points[i].X > width) vels[i].X = -vels[i].X;
                if (points[i].Y < 0 || points[i].Y > height) vels[i].Y = -vels[i].Y;
            }

            // Add to history
            PointF[] snapshot = (PointF[])points.Clone();
            history.Add(snapshot);
            if (history.Count > 20) history.RemoveAt(0);

            // Draw
            for (int h = 0; h < history.Count; h++)
            {
                using (Pen p = new Pen(Color.FromArgb(255 * h / 20, colors[h % 4]), 2))
                {
                    g.DrawPolygon(p, history[h]);
                }
            }
        }
    }
}
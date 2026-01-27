using System;
using System.Drawing;
using System.Collections.Generic;

namespace ScreenSavers.Animations
{
    // --- 3D Flower Box (Cube) ---
    public class FlowerBoxAnimation : IScreenSaverAnimation
    {
        private float angle = 0;

        public void Initialize(int width, int height) { }
        public void Resize(int width, int height) { }
        public void Update(float deltaTime)
        {
            angle += deltaTime;
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Black);
            float cx = width / 2;
            float cy = height / 2;
            float size = 100;

            // Simple 3D projection logic
            PointF[] vertices = new PointF[8];
            int[] signs = { -1, -1, -1,  1, -1, -1,  1, 1, -1,  -1, 1, -1,
                            -1, -1,  1,  1, -1,  1,  1, 1,  1,  -1, 1,  1 };
            
            for(int i=0; i<8; i++)
            {
                float x = signs[i*3] * size;
                float y = signs[i*3+1] * size;
                float z = signs[i*3+2] * size;

                // Rotate Y
                float x2 = x * (float)Math.Cos(angle) - z * (float)Math.Sin(angle);
                float z2 = x * (float)Math.Sin(angle) + z * (float)Math.Cos(angle);
                
                // Rotate X
                float y3 = y * (float)Math.Cos(angle*0.5f) - z2 * (float)Math.Sin(angle*0.5f);
                
                vertices[i] = new PointF(cx + x2, cy + y3);
            }

            // Draw Edges
            using (Pen p = new Pen(Color.Magenta, 3))
            {
                g.DrawLine(p, vertices[0], vertices[1]); g.DrawLine(p, vertices[1], vertices[2]);
                g.DrawLine(p, vertices[2], vertices[3]); g.DrawLine(p, vertices[3], vertices[0]);

                g.DrawLine(p, vertices[4], vertices[5]); g.DrawLine(p, vertices[5], vertices[6]);
                g.DrawLine(p, vertices[6], vertices[7]); g.DrawLine(p, vertices[7], vertices[4]);

                g.DrawLine(p, vertices[0], vertices[4]); g.DrawLine(p, vertices[1], vertices[5]);
                g.DrawLine(p, vertices[2], vertices[6]); g.DrawLine(p, vertices[3], vertices[7]);
            }
        }
    }

    // --- Flying Toasters (Simulated) ---
    public class FlyingToasterAnimation : IScreenSaverAnimation
    {
        private struct Toaster
        {
            public float x, y;
        }
        private List<Toaster> toasters = new List<Toaster>();
        private Random rand = new Random();

        public void Initialize(int width, int height)
        {
            for(int i=0; i<5; i++)
            {
                toasters.Add(new Toaster { x = rand.Next(width), y = rand.Next(height) });
            }
        }

        public void Resize(int width, int height) { }
        public void Update(float deltaTime)
        {
            for(int i=0; i<toasters.Count; i++)
            {
                var t = toasters[i];
                t.x -= 100 * deltaTime;
                t.y += 50 * deltaTime;
                if(t.x < -50) { t.x = 800; t.y = rand.Next(600); } // Reset roughly
                toasters[i] = t;
            }
        }

        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.Black);
            foreach(var t in toasters)
            {
                // Draw Toaster Body
                g.FillRectangle(Brushes.Silver, t.x, t.y, 40, 30);
                g.DrawRectangle(Pens.Gray, t.x, t.y, 40, 30);
                
                // Draw Wings
                g.FillPolygon(Brushes.White, new PointF[] { 
                    new PointF(t.x + 10, t.y), 
                    new PointF(t.x - 10, t.y - 20), 
                    new PointF(t.x + 20, t.y) 
                });
            }
        }
    }

    // --- Aquarium (Placeholder) ---
    public class AquariumAnimation : IScreenSaverAnimation
    {
        private List<PointF> fish = new List<PointF>();
        private Random rand = new Random();

        public void Initialize(int width, int height)
        {
            for (int i = 0; i < 10; i++) fish.Add(new PointF(rand.Next(width), rand.Next(height)));
        }
        public void Resize(int width, int height) { }
        public void Update(float deltaTime) 
        {
            for(int i=0; i<fish.Count; i++)
            {
                var f = fish[i];
                f.X -= 50 * deltaTime;
                if (f.X < -50) f.X = 800;
                fish[i] = f;
            }
        }
        public void Draw(Graphics g, int width, int height)
        {
            g.Clear(Color.DarkBlue);
            using(Brush b = new SolidBrush(Color.Orange))
            {
                foreach(var f in fish)
                {
                    g.FillEllipse(b, f.X, f.Y, 30, 15);
                    g.FillPolygon(Brushes.Orange, new PointF[] { new PointF(f.X+30, f.Y+7), new PointF(f.X+40, f.Y), new PointF(f.X+40, f.Y+15) });
                }
            }
        }
    }
    
    // --- Johnny Castaway (Text Placeholder) ---
    public class JohnnyCastawayAnimation : IScreenSaverAnimation
    {
        public void Initialize(int w, int h) {}
        public void Resize(int w, int h) {}
        public void Update(float dt) {}
        public void Draw(Graphics g, int w, int h)
        {
            g.Clear(Color.SkyBlue);
            g.FillRectangle(Brushes.SandyBrown, 0, h-100, w, 100); // Island
            g.DrawString("Johnny Castaway Simulation\n(Needs complex assets)", new Font("Arial", 20), Brushes.Black, 50, 50);
            
            // Simple stickman
            g.DrawLine(Pens.Black, w/2, h-100, w/2, h-150);
            g.DrawEllipse(Pens.Black, w/2-10, h-170, 20, 20);
        }
    }

    // --- 3D Pipes (Placeholder) ---
    public class PipesAnimation : IScreenSaverAnimation
    {
        public void Initialize(int w, int h) {}
        public void Resize(int w, int h) {}
        public void Update(float dt) {}
        public void Draw(Graphics g, int w, int h)
        {
            g.Clear(Color.Black);
            g.DrawString("3D Pipes Simulation", new Font("Arial", 20), Brushes.Lime, 50, 50);
            // Draw some static pipes
            g.FillRectangle(Brushes.Green, 100, 100, 20, 200);
            g.FillRectangle(Brushes.Green, 100, 300, 200, 20);
        }
    }
}
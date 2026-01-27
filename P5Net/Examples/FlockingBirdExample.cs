using System;
using System.Collections.Generic;

namespace P5Net.Examples
{
    /// <summary>
    /// Contoh Animasi Flocking Bird - Simulasi burung yang bergerombol (Boids algorithm)
    /// </summary>
    public class FlockingBirdExample : P5Canvas
    {
        private List<Boid> flock;
        private int flockSize = 50;
        
        public FlockingBirdExample()
        {
            Width = 1000;
            Height = 700;
            this.ClientSize = new System.Drawing.Size(Width, Height);
            this.Text = "P5Net - Flocking Bird Animation (Boids)";
            flock = new List<Boid>();
        }
        
        public override void Setup()
        {
            Background(20, 30, 50);
            
            // Buat flock burung
            Random rand = new Random();
            for (int i = 0; i < flockSize; i++)
            {
                float x = Random(Width);
                float y = Random(Height);
                flock.Add(new Boid(x, y, this));
            }
        }
        
        public override void Draw()
        {
            // Background dengan transparansi untuk trail effect
            Fill(20, 30, 50, 25);
            NoStroke();
            Rect(0, 0, Width, Height);
            
            // Update dan gambar setiap boid
            foreach (var boid in flock)
            {
                boid.Flock(flock);
                boid.Update();
                boid.Edges();
                boid.Show(this);
            }
            
            // Gambar predator (mouse)
            if (MouseX > 0 && MouseY > 0)
            {
                Fill(255, 50, 50, 150);
                NoStroke();
                Circle(MouseX, MouseY, 30);
                
                Fill(255, 50, 50, 50);
                Circle(MouseX, MouseY, 60);
                
                // Buat boid menghindari mouse
                Vector2D mousePos = new Vector2D(MouseX, MouseY);
                foreach (var boid in flock)
                {
                    float d = boid.position.Dist(mousePos);
                    if (d < 100)
                    {
                        Vector2D flee = Vector2D.Sub(boid.position, mousePos);
                        flee.Normalize();
                        flee.Mult(0.5f);
                        boid.acceleration.Add(flee);
                    }
                }
            }
            
            // Info text
            Fill(255);
            var font14 = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            var font10 = new System.Drawing.Font("Arial", 10);
            var whiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            
            g.DrawString("Flocking Bird Animation (Boids Algorithm)", font14, whiteBrush, 10, 10);
            g.DrawString("Move mouse to scare the birds!", font10, whiteBrush, 10, 35);
            g.DrawString($"Birds: {flockSize} | Frame: {FrameCount}", font10, whiteBrush, 10, 55);
        }
        
        /// <summary>
        /// Class Boid - Representasi satu burung dalam flock
        /// </summary>
        private class Boid
        {
            public Vector2D position;
            public Vector2D velocity;
            public Vector2D acceleration;
            
            private float maxForce = 0.2f;
            private float maxSpeed = 4f;
            private float perceptionRadius = 50f;
            private P5Canvas canvas;
            private System.Drawing.Color color;
            
            public Boid(float x, float y, P5Canvas canvas)
            {
                this.position = new Vector2D(x, y);
                this.velocity = Vector2D.Random2D();
                this.velocity.SetMag(new Random().Next(2, 4));
                this.acceleration = new Vector2D(0, 0);
                this.canvas = canvas;
                
                // Warna random untuk setiap boid
                Random rand = new Random(DateTime.Now.Millisecond + (int)x + (int)y);
                int hue = rand.Next(180, 240); // Biru ke cyan
                this.color = ColorFromHSV(hue, 70, 100);
            }
            
            public void Flock(List<Boid> boids)
            {
                Vector2D alignment = Align(boids);
                Vector2D cohesion = Cohere(boids);
                Vector2D separation = Separate(boids);
                
                // Weight dari setiap behavior
                alignment.Mult(1.0f);
                cohesion.Mult(1.0f);
                separation.Mult(1.5f);
                
                acceleration.Add(alignment);
                acceleration.Add(cohesion);
                acceleration.Add(separation);
            }
            
            public void Update()
            {
                position.Add(velocity);
                velocity.Add(acceleration);
                velocity.Limit(maxSpeed);
                acceleration.Mult(0);
            }
            
            public void Edges()
            {
                if (position.X > canvas.Width) position.X = 0;
                if (position.X < 0) position.X = canvas.Width;
                if (position.Y > canvas.Height) position.Y = 0;
                if (position.Y < 0) position.Y = canvas.Height;
            }
            
            // Alignment: steer towards average heading of neighbors
            private Vector2D Align(List<Boid> boids)
            {
                Vector2D steering = new Vector2D(0, 0);
                int total = 0;
                
                foreach (var other in boids)
                {
                    float d = position.Dist(other.position);
                    if (other != this && d < perceptionRadius)
                    {
                        steering.Add(other.velocity);
                        total++;
                    }
                }
                
                if (total > 0)
                {
                    steering.Div(total);
                    steering.SetMag(maxSpeed);
                    steering.Sub(velocity);
                    steering.Limit(maxForce);
                }
                
                return steering;
            }
            
            // Cohesion: steer towards average position of neighbors
            private Vector2D Cohere(List<Boid> boids)
            {
                Vector2D steering = new Vector2D(0, 0);
                int total = 0;
                
                foreach (var other in boids)
                {
                    float d = position.Dist(other.position);
                    if (other != this && d < perceptionRadius)
                    {
                        steering.Add(other.position);
                        total++;
                    }
                }
                
                if (total > 0)
                {
                    steering.Div(total);
                    steering.Sub(position);
                    steering.SetMag(maxSpeed);
                    steering.Sub(velocity);
                    steering.Limit(maxForce);
                }
                
                return steering;
            }
            
            // Separation: steer to avoid crowding neighbors
            private Vector2D Separate(List<Boid> boids)
            {
                Vector2D steering = new Vector2D(0, 0);
                int total = 0;
                
                foreach (var other in boids)
                {
                    float d = position.Dist(other.position);
                    if (other != this && d < perceptionRadius / 2)
                    {
                        Vector2D diff = Vector2D.Sub(position, other.position);
                        diff.Div(d * d); // Weight by distance
                        steering.Add(diff);
                        total++;
                    }
                }
                
                if (total > 0)
                {
                    steering.Div(total);
                    steering.SetMag(maxSpeed);
                    steering.Sub(velocity);
                    steering.Limit(maxForce);
                }
                
                return steering;
            }
            
            public void Show(P5Canvas canvas)
            {
                // Hitung angle dari velocity
                float angle = velocity.Heading();
                
                canvas.PushMatrix();
                canvas.Translate(position.X, position.Y);
                canvas.Rotate(angle);
                
                // Gambar body burung
                canvas.Fill(color.R, color.G, color.B, 200);
                canvas.NoStroke();
                canvas.Ellipse(0, 0, 16, 8);
                
                // Gambar kepala
                canvas.Fill(color.R, color.G, color.B, 255);
                canvas.Circle(8, 0, 8);
                
                // Gambar mata
                canvas.Fill(0, 0, 0);
                canvas.Circle(10, -1, 2);
                
                // Gambar sayap (animasi mengepak)
                float wingFlap = (float)Math.Sin(canvas.FrameCount * 0.5f + position.X) * 5;
                canvas.Stroke(color.R, color.G, color.B, 150);
                canvas.StrokeWeight(2);
                canvas.Line(0, 0, -8, -8 + wingFlap);
                canvas.Line(0, 0, -8, 8 - wingFlap);
                
                // Gambar ekor
                canvas.NoStroke();
                canvas.Fill(color.R, color.G, color.B, 150);
                canvas.Triangle(-8, 0, -12, -4, -12, 4);
                
                canvas.PopMatrix();
                
                // Gambar trail kecil
                canvas.NoStroke();
                canvas.Fill(color.R, color.G, color.B, 30);
                canvas.Circle(position.X - velocity.X, position.Y - velocity.Y, 4);
            }
            
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
}

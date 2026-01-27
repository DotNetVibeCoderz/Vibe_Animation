using System;

namespace P5Net.Examples
{
    /// <summary>
    /// Contoh Animasi Orbit Control - Planet yang mengorbit
    /// </summary>
    public class OrbitControlExample : P5Canvas
    {
        private float angle = 0;
        private float orbitRadius = 150;
        private float planetSize = 30;
        private float moonAngle = 0;
        
        public OrbitControlExample()
        {
            Width = 800;
            Height = 600;
            this.ClientSize = new System.Drawing.Size(Width, Height);
            this.Text = "P5Net - Orbit Control Animation";
        }
        
        public override void Setup()
        {
            Background(0);
        }
        
        public override void Draw()
        {
            // Background dengan transparansi untuk efek trail
            Background(0, 0, 0);
            
            // Pindah ke tengah canvas
            PushMatrix();
            Translate(Width / 2, Height / 2);
            
            // Gambar matahari di tengah
            Fill(255, 200, 0);
            NoStroke();
            Circle(0, 0, 80);
            
            // Gambar orbit path
            NoFill();
            Stroke(100, 100, 150, 50);
            StrokeWeight(2);
            Circle(0, 0, orbitRadius * 2);
            
            // Planet pertama (Biru)
            PushMatrix();
            Rotate(angle);
            Translate(orbitRadius, 0);
            
            // Gambar planet biru
            Fill(50, 150, 255);
            NoStroke();
            Circle(0, 0, planetSize);
            
            // Moon orbit di sekitar planet
            NoFill();
            Stroke(150, 150, 200, 30);
            StrokeWeight(1);
            Circle(0, 0, 60);
            
            // Gambar moon
            PushMatrix();
            Rotate(moonAngle);
            Translate(30, 0);
            Fill(200, 200, 200);
            Circle(0, 0, 10);
            PopMatrix();
            
            PopMatrix();
            
            // Planet kedua (Merah) - orbit lebih cepat
            PushMatrix();
            Rotate(angle * 1.5f);
            Translate(orbitRadius * 0.6f, 0);
            Fill(255, 100, 50);
            Circle(0, 0, planetSize * 0.7f);
            PopMatrix();
            
            // Planet ketiga (Hijau) - orbit lebih lambat
            PushMatrix();
            Rotate(angle * 0.5f);
            Translate(orbitRadius * 1.4f, 0);
            Fill(100, 255, 100);
            Circle(0, 0, planetSize * 1.2f);
            
            // Ring di planet hijau
            NoFill();
            Stroke(100, 255, 100, 150);
            StrokeWeight(3);
            Ellipse(0, 0, 50, 15);
            
            PopMatrix();
            
            PopMatrix();
            
            // Update angles
            angle += 0.01f;
            moonAngle += 0.05f;
            
            // Info text
            Fill(255);
            g.DrawString("Orbit Control Animation", 
                new System.Drawing.Font("Arial", 14), 
                currentFill as System.Drawing.SolidBrush, 
                10, 10);
            g.DrawString($"Frame: {FrameCount}", 
                new System.Drawing.Font("Arial", 10), 
                currentFill as System.Drawing.SolidBrush, 
                10, 35);
        }
        
        private System.Drawing.SolidBrush currentFill = new System.Drawing.SolidBrush(System.Drawing.Color.White);
    }
}

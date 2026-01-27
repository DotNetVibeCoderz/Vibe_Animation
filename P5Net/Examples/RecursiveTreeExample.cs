using System;

namespace P5Net.Examples
{
    /// <summary>
    /// Contoh Animasi Recursive Tree - Pohon fractal yang tumbuh
    /// </summary>
    public class RecursiveTreeExample : P5Canvas
    {
        private float angle = 0;
        private float angleOffset = 0;
        
        public RecursiveTreeExample()
        {
            Width = 800;
            Height = 600;
            this.ClientSize = new System.Drawing.Size(Width, Height);
            this.Text = "P5Net - Recursive Tree Animation";
        }
        
        public override void Setup()
        {
            Background(20, 20, 40);
        }
        
        public override void Draw()
        {
            // Background
            Background(20, 20, 40);
            
            // Angle berubah berdasarkan mouse atau animasi
            angle = Map(MouseX, 0, Width, 0, PI / 2);
            
            // Animasi jika mouse tidak bergerak
            if (MouseX == PMMouseX)
            {
                angleOffset += 0.01f;
                angle = PI / 6 + (float)Math.Sin(angleOffset) * PI / 8;
            }
            
            // Pindah ke bawah tengah canvas
            PushMatrix();
            Translate(Width / 2, Height);
            
            // Gambar tanah
            Fill(50, 100, 50);
            NoStroke();
            Rect(-Width / 2, 0, Width, 50);
            
            // Gambar pohon
            Stroke(139, 90, 43); // Warna coklat untuk batang
            StrokeWeight(2);
            DrawBranch(120); // Panjang batang utama
            
            PopMatrix();
            
            // Info text
            Fill(255);
            g.DrawString("Recursive Tree Animation", 
                new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold), 
                new System.Drawing.SolidBrush(System.Drawing.Color.White), 
                10, 10);
            g.DrawString("Move mouse left-right to change angle", 
                new System.Drawing.Font("Arial", 10), 
                new System.Drawing.SolidBrush(System.Drawing.Color.LightGray), 
                10, 35);
            g.DrawString($"Angle: {(angle * 180 / PI):F1}° | Frame: {FrameCount}", 
                new System.Drawing.Font("Arial", 10), 
                new System.Drawing.SolidBrush(System.Drawing.Color.LightGray), 
                10, 55);
        }
        
        /// <summary>
        /// Fungsi rekursif untuk menggambar cabang pohon
        /// </summary>
        private void DrawBranch(float len)
        {
            // Base case: berhenti jika cabang terlalu kecil
            if (len < 4)
            {
                // Gambar daun di ujung cabang
                NoStroke();
                
                // Warna daun berubah berdasarkan musim (animasi)
                float seasonCycle = (float)Math.Sin(FrameCount * 0.02f);
                int r = (int)Lerp(50, 255, (seasonCycle + 1) / 2);
                int g = (int)Lerp(150, 200, (seasonCycle + 1) / 2);
                int b = 50;
                
                Fill(r, g, b, 200);
                Circle(0, 0, 8);
                return;
            }
            
            // Ketebalan garis berkurang seiring dengan panjang cabang
            StrokeWeight(len / 15);
            
            // Warna batang dari coklat gelap (bawah) ke coklat terang (atas)
            int brown = (int)Map(len, 120, 4, 139, 180);
            Stroke(brown, (int)(brown * 0.65f), 43);
            
            // Gambar garis cabang ke atas
            Line(0, 0, 0, -len);
            
            // Pindah ke ujung cabang
            Translate(0, -len);
            
            // Cabang kanan
            PushMatrix();
            Rotate(angle);
            DrawBranch(len * 0.67f); // Cabang menjadi 67% dari panjang sebelumnya
            PopMatrix();
            
            // Cabang kiri
            PushMatrix();
            Rotate(-angle);
            DrawBranch(len * 0.67f);
            PopMatrix();
            
            // Cabang tambahan di tengah (jarang muncul)
            if (len > 60 && Random(1) > 0.7f)
            {
                PushMatrix();
                Rotate(angle * 0.3f);
                DrawBranch(len * 0.5f);
                PopMatrix();
            }
        }
    }
}

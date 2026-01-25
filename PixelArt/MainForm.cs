using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PixelArt
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer animTimer;
        private System.Windows.Forms.Timer sceneTimer;
        
        // Pixel Art Settings
        private const int CanvasWidth = 160;
        private const int CanvasHeight = 120;
        private float ScaleFactor = 4.0f; // Akan dihitung ulang saat resize
        private Bitmap canvas;
        private Graphics gCanvas;
        private Random rnd = new Random();

        // Scene Management
        private IScene currentScene;
        private List<IScene> scenes;
        private int sceneIndex = 0;

        // Quotes
        private List<string> quotes;
        private string currentQuote = "";
        private float quoteAlpha = 0;
        private bool quoteFadingIn = true;

        public MainForm()
        {
            InitializeComponent();
            
            // Setup Form
            this.Text = "Pixel Art Motivator - Jacky the Code Bender";
            this.Size = new Size(800, 600);
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Setup Rendering
            canvas = new Bitmap(CanvasWidth, CanvasHeight);
            gCanvas = Graphics.FromImage(canvas);
            gCanvas.InterpolationMode = InterpolationMode.NearestNeighbor;
            gCanvas.PixelOffsetMode = PixelOffsetMode.None;

            // Setup Data
            InitQuotes();
            InitScenes();

            // Setup Timers
            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 33; // ~30 FPS
            animTimer.Tick += AnimTimer_Tick;
            animTimer.Start();

            sceneTimer = new System.Windows.Forms.Timer();
            sceneTimer.Interval = 8000; // Ganti scene setiap 8 detik
            sceneTimer.Tick += SceneTimer_Tick;
            sceneTimer.Start();

            // Set initial scene
            currentScene = scenes[0];
            currentQuote = quotes[0];

            this.Resize += MainForm_Resize;
            CalculateScale();
        }

        private void InitQuotes()
        {
            quotes = new List<string>
            {
                "Cara terbaik untuk memprediksi masa depan\nadalah dengan menciptakannya.",
                "Janganlah melihat jam dinding.\nTeruslah berkarya sebagaimana jarum jam berputar.",
                "Satu-satunya cara untuk melakukan pekerjaan hebat\nadalah dengan mencintai apa yang Anda lakukan.",
                "Kegagalan adalah bumbu yang memberikan kesuksesan\aroma rasanya.",
                "Kode yang bersih adalah tanda\npikiran yang jernih.",
                "Jangan takut berjalan lambat,\ntakutlah jika hanya berdiri diam."
            };
        }

        private void InitScenes()
        {
            scenes = new List<IScene>
            {
                new StarfieldScene(CanvasWidth, CanvasHeight),
                new FireScene(CanvasWidth, CanvasHeight),
                new MatrixRainScene(CanvasWidth, CanvasHeight)
            };
        }

        private void CalculateScale()
        {
            float scaleX = (float)this.ClientSize.Width / CanvasWidth;
            float scaleY = (float)this.ClientSize.Height / CanvasHeight;
            ScaleFactor = Math.Min(scaleX, scaleY);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            CalculateScale();
            this.Invalidate();
        }

        private void SceneTimer_Tick(object sender, EventArgs e)
        {
            // Transition effect simple logic
            sceneIndex = (sceneIndex + 1) % scenes.Count;
            currentScene = scenes[sceneIndex];
            
            // Change quote
            currentQuote = quotes[rnd.Next(quotes.Count)];
            quoteAlpha = 0;
            quoteFadingIn = true;
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            UpdateState();
            this.Invalidate();
        }

        private void UpdateState()
        {
            // Update Scene Pixel Art
            currentScene.Update();

            // Update Quote Animation Fade In/Out logic if needed
            // For now simple fade in
            if (quoteFadingIn)
            {
                quoteAlpha += 0.05f;
                if (quoteAlpha >= 1.0f)
                {
                    quoteAlpha = 1.0f;
                    // quoteFadingIn = false; // Keep it visible
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 1. Draw Scene to low-res Canvas
            gCanvas.Clear(Color.Black);
            currentScene.Draw(gCanvas);

            // 2. Draw Canvas to Form (Scaled Up)
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half; // Sharp pixels

            float scaledW = CanvasWidth * ScaleFactor;
            float scaledH = CanvasHeight * ScaleFactor;
            float offsetX = (this.ClientSize.Width - scaledW) / 2;
            float offsetY = (this.ClientSize.Height - scaledH) / 2;

            e.Graphics.DrawImage(canvas, offsetX, offsetY, scaledW, scaledH);

            // 3. Draw Quote (High Res Overlay)
            // Draw semi-transparent background for text
            if (!string.IsNullOrEmpty(currentQuote))
            {
                using (Font f = new Font("Consolas", 16, FontStyle.Bold))
                using (Brush b = new SolidBrush(Color.FromArgb((int)(quoteAlpha * 255), 255, 255, 255)))
                using (Brush shadow = new SolidBrush(Color.FromArgb((int)(quoteAlpha * 200), 0, 0, 0)))
                {
                    SizeF textSize = e.Graphics.MeasureString(currentQuote, f);
                    float tx = (this.ClientSize.Width - textSize.Width) / 2;
                    float ty = (this.ClientSize.Height - textSize.Height) / 2;
                    
                    // Shadow
                    e.Graphics.DrawString(currentQuote, f, shadow, tx + 2, ty + 2);
                    // Text
                    e.Graphics.DrawString(currentQuote, f, b, tx, ty);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "Pixel Art";
            this.ResumeLayout(false);
        }
    }

    // --- Interfaces & Scene Classes ---

    public interface IScene
    {
        void Update();
        void Draw(Graphics g);
    }

    // 1. Starfield Effect
    public class StarfieldScene : IScene
    {
        private struct Star { public float X, Y, Z; }
        private List<Star> stars = new List<Star>();
        private int width, height;
        private Random rnd = new Random();

        public StarfieldScene(int w, int h)
        {
            width = w;
            height = h;
            for (int i = 0; i < 100; i++) SpawnStar();
        }

        private void SpawnStar()
        {
            stars.Add(new Star
            {
                X = rnd.Next(-width, width),
                Y = rnd.Next(-height, height),
                Z = rnd.Next(1, width)
            });
        }

        public void Update()
        {
            for (int i = 0; i < stars.Count; i++)
            {
                var s = stars[i];
                s.Z -= 2; // Speed
                if (s.Z <= 1)
                {
                    s.Z = width;
                    s.X = rnd.Next(-width, width);
                    s.Y = rnd.Next(-height, height);
                }
                stars[i] = s;
            }
        }

        public void Draw(Graphics g)
        {
            g.Clear(Color.Black);
            using (Brush b = new SolidBrush(Color.White))
            {
                foreach (var s in stars)
                {
                    float k = 128.0f / s.Z;
                    float px = s.X * k + width / 2;
                    float py = s.Y * k + height / 2;

                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        float size = (1 - s.Z / width) * 2;
                        g.FillRectangle(b, px, py, Math.Max(1, size), Math.Max(1, size));
                    }
                }
            }
        }
    }

    // 2. Doom-style Fire Effect
    public class FireScene : IScene
    {
        private int width, height;
        private int[] firePixels;
        private Random rnd = new Random();
        private Bitmap buffer;
        private Color[] palette;

        public FireScene(int w, int h)
        {
            width = w;
            height = h;
            firePixels = new int[width * height];
            buffer = new Bitmap(width, height);
            GeneratePalette();
        }

        private void GeneratePalette()
        {
            palette = new Color[37];
            // Simple heat ramp
            for(int i=0; i<37; i++)
            {
                // Simple gradient logic approximation
                // Black -> Red -> Orange -> Yellow -> White
                // This is a simplified palette generation
                int r = 0, g = 0, b = 0;
                if (i > 24) { r = 255; g = 255; b = (i - 24) * 25; }
                else if (i > 12) { r = 255; g = (i - 12) * 20; b = 0; }
                else if (i > 4) { r = (i - 4) * 30; g = 0; b = 0; }
                
                palette[i] = Color.FromArgb(Math.Min(255,r), Math.Min(255,g), Math.Min(255,b));
            }
        }

        public void Update()
        {
            // Generator line at bottom
            for (int x = 0; x < width; x++)
            {
                firePixels[(height - 1) * width + x] = rnd.Next(0, 37) > 4 ? 36 : 0; // 36 is hottest
            }

            // Propagate fire
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    int src = y * width + x;
                    int pixelVal = firePixels[src];
                    
                    if (pixelVal == 0)
                    {
                        firePixels[(y - 1) * width + x] = 0;
                    }
                    else
                    {
                        int randIdx = rnd.Next(0, 3);
                        int dst = src - width - randIdx + 1;
                        if (dst >= 0 && dst < firePixels.Length)
                        {
                            firePixels[dst] = Math.Max(0, pixelVal - (randIdx & 1));
                        }
                    }
                }
            }
        }

        public void Draw(Graphics g)
        {
            // Draw firePixels to bitmap manually (slow but works for small res)
            // Locking bits would be faster but this is simple enough for 160x100
            System.Drawing.Imaging.BitmapData bData = buffer.LockBits(
                new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)bData.Scan0;
                for (int i = 0; i < width * height; i++)
                {
                    int colorIdx = firePixels[i];
                    Color c = palette[Math.Min(36, Math.Max(0, colorIdx))];
                    ptr[i * 4] = c.B;
                    ptr[i * 4 + 1] = c.G;
                    ptr[i * 4 + 2] = c.R;
                    ptr[i * 4 + 3] = 255;
                }
            }

            buffer.UnlockBits(bData);
            g.DrawImage(buffer, 0, 0);
        }
    }

    // 3. Matrix Rain Effect
    public class MatrixRainScene : IScene
    {
        private int width, height;
        private int[] drops;
        private Random rnd = new Random();
        private const int FontSize = 8;
        private int cols;

        public MatrixRainScene(int w, int h)
        {
            width = w;
            height = h;
            cols = width / FontSize;
            drops = new int[cols];
            for (int i = 0; i < cols; i++) drops[i] = rnd.Next(-20, 0);
        }

        public void Update()
        {
            for (int i = 0; i < cols; i++)
            {
                drops[i]++;
                if (drops[i] * FontSize > height && rnd.NextDouble() > 0.95)
                {
                    drops[i] = 0;
                }
            }
        }

        public void Draw(Graphics g)
        {
            g.Clear(Color.Black);
            using (Font f = new Font("Arial", 6))
            using (Brush b = new SolidBrush(Color.Lime))
            using (Brush head = new SolidBrush(Color.White))
            {
                for (int i = 0; i < cols; i++)
                {
                    int y = drops[i];
                    int x = i * FontSize;
                    
                    // Draw trail
                    for (int k = 0; k < 5; k++)
                    {
                        if (y - k >= 0)
                        {
                            char c = (char)('A' + rnd.Next(0, 26)); 
                            // Random char flicker
                            if (k == 0) // Head
                                g.DrawString(c.ToString(), f, head, x, (y - k) * FontSize);
                            else // Trail
                                g.DrawString(c.ToString(), f, b, x, (y - k) * FontSize);
                        }
                    }
                }
            }
        }
    }
}

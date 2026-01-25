using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WaterApp
{
    public partial class Form1 : Form
    {
        // ==========================================
        // CONFIGURATION
        // ==========================================
        private const int Damping = 960; // Semakin tinggi, air semakin kental (waves die faster)
        private const int LightRefraction = 190; // Kekuatan efek refraksi cahaya
        private const int RippleFrequency = 50; // Milliseconds physics update (semakin kecil semakin cepat)

        // ==========================================
        // VARIABLES
        // ==========================================
        private short[,] _buffer1;
        private short[,] _buffer2;
        private int _bitmapWidth;
        private int _bitmapHeight;
        private Bitmap _renderBitmap;
        private Bitmap _backgroundBitmap = null!; // Diinisialisasi di CreatePoolBottom
        private System.Windows.Forms.Timer _timerPhysics;
        private System.Windows.Forms.Timer _timerRandomDrop;
        private System.Windows.Forms.Timer _timerText;
        private Random _rnd = new Random();

        // Quotes variables
        private List<string> _quotes = new List<string>
        {
            "Air tenang menghanyutkan.",
            "Jadilah seperti air, kawan. - Bruce Lee",
            "Ribuan lilin dapat dinyalakan dari satu lilin.",
            "Perjalanan seribu mil dimulai dengan satu langkah.",
            "Kesabaran adalah pahit, tapi buahnya manis.",
            "Code is poetry. - Jacky",
            "Jangan lupa ngopi hari ini.",
            "Error adalah cara komputer mengatakan 'Coba lagi'.",
            "Gravicode Studios: We code your dreams."
        };
        private int _currentQuoteIndex = 0;
        private float _textOpacity = 0f;
        private bool _textFadingIn = true;

        public Form1()
        {
            InitializeComponent();
            
            // Setup properti Form untuk full screen effect yang bagus
            this.Text = "WaterApp - Realistic Ripple Effect";
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Inisialisasi Size
            _bitmapWidth = this.Width;
            _bitmapHeight = this.Height;

            // Inisialisasi Buffers untuk gelombang air
            _buffer1 = new short[_bitmapWidth, _bitmapHeight];
            _buffer2 = new short[_bitmapWidth, _bitmapHeight];

            // Inisialisasi Bitmap
            _renderBitmap = new Bitmap(_bitmapWidth, _bitmapHeight, PixelFormat.Format32bppPArgb);

            // Buat procedural background (Dasar Kolam)
            CreatePoolBottom();

            // Setup Timers
            _timerPhysics = new System.Windows.Forms.Timer();
            _timerPhysics.Interval = 1000 / 60; // Target 60 FPS
            _timerPhysics.Tick += TimerPhysics_Tick;
            _timerPhysics.Start();

            _timerRandomDrop = new System.Windows.Forms.Timer();
            _timerRandomDrop.Interval = 500; // Tetesan air acak setiap 0.5 detik
            _timerRandomDrop.Tick += TimerRandomDrop_Tick;
            _timerRandomDrop.Start();

            _timerText = new System.Windows.Forms.Timer();
            _timerText.Interval = 50; // Update opacity teks
            _timerText.Tick += TimerText_Tick;
            _timerText.Start();
        }

        // ==========================================
        // INITIALIZATION
        // ==========================================
        private void CreatePoolBottom()
        {
            // Kita buat background menarik, gradasi biru dengan noise
            _backgroundBitmap = new Bitmap(_bitmapWidth, _bitmapHeight);
            using (Graphics g = Graphics.FromImage(_backgroundBitmap))
            {
                // Background gradient biru tua
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(0, _bitmapHeight),
                    Color.FromArgb(0, 50, 100), Color.FromArgb(0, 10, 30)))
                {
                    g.FillRectangle(brush, 0, 0, _bitmapWidth, _bitmapHeight);
                }

                // Tambah "batu" atau texture noise simple
                for (int i = 0; i < 500; i++)
                {
                    int x = _rnd.Next(_bitmapWidth);
                    int y = _rnd.Next(_bitmapHeight);
                    int size = _rnd.Next(2, 5);
                    int alpha = _rnd.Next(20, 100);
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, 100, 200, 255)))
                    {
                        g.FillEllipse(b, x, y, size, size);
                    }
                }
                
                // Tambah logo gravicode kecil di pojok sebagai watermark ;)
                g.DrawString("Gravicode Studios", new Font("Arial", 8), Brushes.Gray, 10, _bitmapHeight - 20);
            }
        }

        // ==========================================
        // LOGIC & PHYSICS
        // ==========================================
        
        // Membuat gangguan (tetesan air) pada koordinat x,y radius r dan ketinggian h
        private void CreateRipple(int x, int y, short height, int radius)
        {
            if (x >= radius && x < _bitmapWidth - radius && y >= radius && y < _bitmapHeight - radius)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    for (int i = -radius; i <= radius; i++)
                    {
                        if (i * i + j * j <= radius * radius)
                        {
                            _buffer1[x + i, y + j] = height;
                        }
                    }
                }
            }
        }

        private void ProcessWaterPhysics()
        {
            // Algoritma Ripple effect klasik
            // Swap buffers setiap frame
            for (int y = 1; y < _bitmapHeight - 1; y++)
            {
                for (int x = 1; x < _bitmapWidth - 1; x++)
                {
                    int val = (_buffer1[x - 1, y] +
                               _buffer1[x + 1, y] +
                               _buffer1[x, y - 1] +
                               _buffer1[x, y + 1]) >> 1; // Dibagi 2

                    val -= _buffer2[x, y];
                    val -= val >> 5; // Damping

                    _buffer2[x, y] = (short)val;
                }
            }

            // Swap reference buffer
            short[,] temp = _buffer1;
            _buffer1 = _buffer2;
            _buffer2 = temp;
        }

        private unsafe void RenderWaterEffect()
        {
            // Lock bitmap untuk akses memori langsung
            BitmapData bitmapData = _renderBitmap.LockBits(
                new Rectangle(0, 0, _bitmapWidth, _bitmapHeight),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            
            // Lock background
            BitmapData bgData = _backgroundBitmap.LockBits(
                new Rectangle(0, 0, _bitmapWidth, _bitmapHeight),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);

            int* pBuffer = (int*)bitmapData.Scan0.ToPointer();
            int* pBg = (int*)bgData.Scan0.ToPointer();
            
            int stride = bitmapData.Stride / 4; 

            // Rendering loop
            for (int y = 0; y < _bitmapHeight; y++)
            {
                for (int x = 0; x < _bitmapWidth; x++)
                {
                    int xOffset = 0;
                    int yOffset = 0;

                    if (x > 0 && x < _bitmapWidth - 1 && y > 0 && y < _bitmapHeight - 1)
                    {
                        xOffset = _buffer1[x - 1, y] - _buffer1[x + 1, y];
                        yOffset = _buffer1[x, y - 1] - _buffer1[x, y + 1];
                    }

                    int textureX = x + xOffset;
                    int textureY = y + yOffset;

                    if (textureX < 0) textureX = 0;
                    if (textureX >= _bitmapWidth) textureX = _bitmapWidth - 1;
                    if (textureY < 0) textureY = 0;
                    if (textureY >= _bitmapHeight) textureY = _bitmapHeight - 1;

                    int color = pBg[textureY * stride + textureX];
                    pBuffer[y * stride + x] = color;
                }
            }

            _renderBitmap.UnlockBits(bitmapData);
            _backgroundBitmap.UnlockBits(bgData);
        }

        // ==========================================
        // EVENTS & TIMERS
        // ==========================================

        private void TimerPhysics_Tick(object? sender, EventArgs e)
        {
            ProcessWaterPhysics();
            RenderWaterEffect();
            
            // Menggambar text di atas air
            using (Graphics g = Graphics.FromImage(_renderBitmap))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                
                string txt = _quotes[_currentQuoteIndex];
                Font font = new Font("Segoe UI Light", 24, FontStyle.Italic);
                
                SizeF size = g.MeasureString(txt, font);
                PointF pos = new PointF((_bitmapWidth - size.Width) / 2, (_bitmapHeight - size.Height) / 2);

                int alpha = (int)(_textOpacity * 255);
                if(alpha < 0) alpha = 0; 
                if(alpha > 255) alpha = 255;

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                {
                    using(SolidBrush shadow = new SolidBrush(Color.FromArgb(alpha/2, 0,0,0)))
                    {
                        g.DrawString(txt, font, shadow, pos.X + 2, pos.Y + 2);
                    }
                    g.DrawString(txt, font, brush, pos.X, pos.Y);
                }
            }

            this.Invalidate();
        }

        private void TimerRandomDrop_Tick(object? sender, EventArgs e)
        {
            int x = _rnd.Next(20, _bitmapWidth - 20);
            int y = _rnd.Next(20, _bitmapHeight - 20);
            
            short power = (short)_rnd.Next(500, 2000);
            int radius = _rnd.Next(1, 4);

            CreateRipple(x, y, power, radius);
        }

        private void TimerText_Tick(object? sender, EventArgs e)
        {
            float fadeSpeed = 0.02f;

            if (_textFadingIn)
            {
                _textOpacity += fadeSpeed;
                if (_textOpacity >= 1f)
                {
                    _textOpacity = 1f;
                    _textFadingIn = false;
                    
                    _timerText.Stop();
                     
                     // Pakai System.Windows.Forms.Timer secara eksplisit
                     System.Windows.Forms.Timer delayTimer = new System.Windows.Forms.Timer();
                     delayTimer.Interval = 3000; 
                     delayTimer.Tick += (s, args) => {
                         _timerText.Start(); 
                         delayTimer.Stop();
                         delayTimer.Dispose();
                     };
                     delayTimer.Start();
                }
            }
            else
            {
                _textOpacity -= fadeSpeed;
                if (_textOpacity <= 0f)
                {
                    _textOpacity = 0f;
                    _textFadingIn = true;
                    _currentQuoteIndex = (_currentQuoteIndex + 1) % _quotes.Count;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
             if (_renderBitmap != null)
             {
                 e.Graphics.DrawImage(_renderBitmap, 0, 0);
             }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            CreateRipple(e.X, e.Y, 3000, 6);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
             _timerPhysics.Stop();
             _timerRandomDrop.Stop();
             _timerText.Stop();
             if(_renderBitmap != null) _renderBitmap.Dispose();
             if(_backgroundBitmap != null) _backgroundBitmap.Dispose();
             base.OnFormClosing(e);
        }
    }
}

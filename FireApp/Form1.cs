using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FireApp
{
    public partial class Form1 : Form
    {
        // Resolusi simulasi api (lebih kecil dari window agar retro style & cepat)
        private const int FireWidth = 320;
        private const int FireHeight = 240;
        
        // Array untuk menyimpan intensitas api (0 - 36)
        private int[] firePixels;
        
        // Bitmap buffer untuk digambar ke layar
        private Bitmap fireBitmap;
        
        // Timer untuk animasi loop
        private System.Windows.Forms.Timer loopTimer;
        
        // Random generator
        private Random random = new Random();

        // Palette warna api (Doom style) - dari Hitam -> Merah -> Orange -> Kuning -> Putih
        private readonly Color[] firePalette = new Color[]
        {
            Color.FromArgb(7, 7, 7), Color.FromArgb(31, 7, 7), Color.FromArgb(47, 15, 7), Color.FromArgb(71, 15, 7), Color.FromArgb(87, 23, 7), Color.FromArgb(103, 31, 7), Color.FromArgb(119, 31, 7), Color.FromArgb(143, 39, 7),
            Color.FromArgb(159, 47, 7), Color.FromArgb(175, 63, 7), Color.FromArgb(191, 71, 7), Color.FromArgb(199, 71, 7), Color.FromArgb(223, 79, 7), Color.FromArgb(223, 87, 7), Color.FromArgb(223, 87, 7), Color.FromArgb(215, 95, 7),
            Color.FromArgb(215, 95, 7), Color.FromArgb(215, 103, 15), Color.FromArgb(207, 111, 15), Color.FromArgb(207, 119, 15), Color.FromArgb(207, 127, 15), Color.FromArgb(207, 135, 23), Color.FromArgb(199, 135, 23), Color.FromArgb(199, 143, 23),
            Color.FromArgb(199, 151, 31), Color.FromArgb(191, 159, 31), Color.FromArgb(191, 159, 31), Color.FromArgb(191, 167, 39), Color.FromArgb(191, 167, 39), Color.FromArgb(191, 175, 47), Color.FromArgb(183, 175, 47), Color.FromArgb(183, 183, 47),
            Color.FromArgb(183, 183, 55), Color.FromArgb(207, 207, 111), Color.FromArgb(223, 223, 159), Color.FromArgb(239, 239, 199), Color.FromArgb(255, 255, 255)
        };

        public Form1()
        {
            InitializeComponent();
            
            // Setup form
            this.Text = "FireApp - By Jacky The Code Bender";
            this.DoubleBuffered = true; // Mencegah flickering
            this.BackColor = Color.Black;
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Inisialisasi data api
            firePixels = new int[FireWidth * FireHeight];
            fireBitmap = new Bitmap(FireWidth, FireHeight, PixelFormat.Format32bppPArgb);

            // Buat sumber api di baris paling bawah
            CreateFireSource();

            // Setup Timer (sekitar 30 FPS)
            loopTimer = new System.Windows.Forms.Timer();
            loopTimer.Interval = 33; 
            loopTimer.Tick += LoopTimer_Tick;
            loopTimer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Mengisi baris paling bawah dengan intensitas panas maksimal (36)
        /// </summary>
        private void CreateFireSource()
        {
            for (int x = 0; x < FireWidth; x++)
            {
                int index = (FireHeight - 1) * FireWidth + x;
                firePixels[index] = 36; // Indeks warna keputihan (paling panas)
            }
        }

        private void LoopTimer_Tick(object sender, EventArgs e)
        {
            UpdateFirePhysics();
            RenderFire();
            this.Invalidate(); // Memicu OnPaint
        }

        /// <summary>
        /// Algoritma inti Doom Fire. Menyebarkan panas dari bawah ke atas.
        /// </summary>
        private void UpdateFirePhysics()
        {
            for (int x = 0; x < FireWidth; x++)
            {
                for (int y = 1; y < FireHeight; y++) // Mulai dari baris ke-1 (bukan 0)
                {
                    int srcIndex = (y * FireWidth) + x;
                    int pixelValue = firePixels[srcIndex];

                    if (pixelValue == 0) // Jika pixel hitam, lewati
                    {
                        // Set pixel di atasnya jadi hitam juga (dingin)
                        int destIndexTemp = ((y - 1) * FireWidth) + x;
                        firePixels[destIndexTemp] = 0;
                        continue;
                    }

                    // Menghitung indeks acak untuk simulasi angin
                    // Angka random 0..3
                    int rand = random.Next(0, 3); 
                    int dstIndex = srcIndex - FireWidth + 1 - rand; // Pixel di atas, geser kiri/kanan sedikit
                    
                    // Pastikan tidak index out of bound
                    if (dstIndex >= 0 && dstIndex < firePixels.Length)
                    {
                        // Pendinginan: Kurangi intensitas panas (acak)
                        int heatDecay = rand & 1; // 0 atau 1
                        int newHeat = pixelValue - heatDecay;
                        
                        // Clamp heat biar ga kurang dari 0
                        firePixels[dstIndex] = newHeat < 0 ? 0 : newHeat;
                    }
                }
            }
        }

        /// <summary>
        /// Konversi array intensitas api ke Bitmap visual
        /// </summary>
        private void RenderFire()
        {
            // Lock bits supaya kencang nulis pixel-nya
            BitmapData bmpData = fireBitmap.LockBits(
                new Rectangle(0, 0, FireWidth, FireHeight),
                ImageLockMode.WriteOnly,
                fireBitmap.PixelFormat);

            // Buffer byte untuk menampung data RGBA
            // Format32bppPArgb = 4 byte per pixel (Blue, Green, Red, Alpha)
            int byteCount = bmpData.Stride * FireHeight;
            byte[] pixels = new byte[byteCount];

            // pointer scanning
            int ptr = 0;

            for (int y = 0; y < FireHeight; y++)
            {
                for (int x = 0; x < FireWidth; x++)
                {
                    // Ambil heat index
                    int idx = y * FireWidth + x;
                    int heatIndex = firePixels[idx];
                    
                    // Pastikan aman index-nya
                    if (heatIndex < 0) heatIndex = 0;
                    if (heatIndex >= firePalette.Length) heatIndex = firePalette.Length - 1;

                    Color color = firePalette[heatIndex];

                    // Tulis ke buffer byte (Little Endian: B, G, R, A)
                    pixels[ptr] = color.B;
                    pixels[ptr + 1] = color.G;
                    pixels[ptr + 2] = color.R;
                    pixels[ptr + 3] = 255; // Alpha penuh

                    ptr += 4;
                }
            }

            // Salin buffer byte kembali ke bitmap
            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            fireBitmap.UnlockBits(bmpData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (fireBitmap != null)
            {
                // Gambar bitmap api ke seluruh form (stretch)
                // Menggunakan NearestNeighbor agar pixel-nya tajam (retro look) 
                // atau HighQualityBicubic kalau mau halus. Saya pilih NearestNeighbor biar berasa Doom-nya.
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                
                e.Graphics.DrawImage(fireBitmap, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            }
            
            // Tambahkan sedikit teks info
            string info = "FireApp by Jacky The Code Bender";
            e.Graphics.DrawString(info, SystemFonts.DefaultFont, Brushes.White, 10, 10);
        }
    }
}
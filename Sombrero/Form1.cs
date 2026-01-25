using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sombrero
{
    public partial class Form1 : Form
    {
        // Constants matching the Python script
        const int WIDTH = 960;
        const int HEIGHT = 720;
        const int FPS = 60;
        const float FOV = 700f;
        const float VIEWER_DISTANCE = 90f;

        const int GRID_RADIUS = 28;
        const int GRID_STEP = 1;
        const float BASE_TILT_X = 55f * (float)(Math.PI / 180.0);
        const float SPIN_RATE = 0.25f;

        const float RIPPLE_FREQ = 0.65f;
        const float RIPPLE_SPEED = 4f;
        const float RIPPLE_DECAY = 0.015f;
        const float HEIGHT_SCALE = 20.0f;
        const float AMPLITUDE_FALLOFF = 0.06f;

        readonly Color BG_COLOR = Color.FromArgb(10, 12, 18);

        System.Windows.Forms.Timer renderTimer;
        DateTime startTime;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Important for smooth animation
            this.ClientSize = new Size(WIDTH, HEIGHT);
            this.Text = "Spinning Ripple Surface";
            this.BackColor = BG_COLOR;

            startTime = DateTime.Now;
            renderTimer = new System.Windows.Forms.Timer();
            renderTimer.Interval = 1000 / FPS;
            renderTimer.Tick += (s, e) => this.Invalidate();
            renderTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float t = (float)(DateTime.Now - startTime).TotalSeconds;

            var points = GenerateGridPoints(t);
            DrawHiddenLineMesh(g, points);
        }

        private float RippleHeight(float x, float y, float t)
        {
            float r = (float)Math.Sqrt(x * x + y * y);
            float wave = (float)Math.Cos(r * RIPPLE_FREQ - t * RIPPLE_SPEED);
            float envelope = (float)Math.Exp(-r * RIPPLE_DECAY);
            float amplitude = HEIGHT_SCALE / (1.0f + AMPLITUDE_FALLOFF * r);
            return wave * envelope * amplitude;
        }

        // Struct to hold point info: px, py, depth, amp.
        struct PointData
        {
            public float Px, Py, Depth, Amp;
            public bool IsValid;
        }

        private PointData[,] GenerateGridPoints(float t)
        {
            // Range: -GRID_RADIUS to GRID_RADIUS
            int size = (GRID_RADIUS * 2 / GRID_STEP) + 1;
            PointData[,] points = new PointData[size, size];

            float cosx = (float)Math.Cos(BASE_TILT_X);
            float sinx = (float)Math.Sin(BASE_TILT_X);
            float spinAngle = t * SPIN_RATE;
            float cosz = (float)Math.Cos(spinAngle);
            float sinz = (float)Math.Sin(spinAngle);

            int i = 0;
            for (float x = -GRID_RADIUS; x <= GRID_RADIUS; x += GRID_STEP)
            {
                int j = 0;
                for (float y = -GRID_RADIUS; y <= GRID_RADIUS; y += GRID_STEP)
                {
                    float z = RippleHeight(x, y, t);
                    float x_spin = x * cosz - y * sinz;
                    float y_spin = x * sinz + y * cosz;
                    // z_spin = z

                    float y_tilt = y_spin * cosx - z * sinx;
                    float z_tilt = y_spin * sinx + z * cosx;

                    float depth = VIEWER_DISTANCE + z_tilt;

                    if (depth <= 0.1f)
                    {
                        points[i, j] = new PointData { IsValid = false };
                    }
                    else
                    {
                        float factor = FOV / depth;
                        float px = x_spin * factor + WIDTH / 2f;
                        float py = -y_tilt * factor + HEIGHT / 2f;
                        float amp = Math.Abs(z);
                        points[i, j] = new PointData { Px = px, Py = py, Depth = depth, Amp = amp, IsValid = true };
                    }
                    j++;
                }
                i++;
            }
            return points;
        }

        struct Quad
        {
            public float AvgDepth;
            public PointF[] Corners; // 4 corners
            public float[] CornerAmps; // 4 amps
        }

        private void DrawHiddenLineMesh(Graphics g, PointData[,] points)
        {
            int rows = points.GetLength(0);
            int cols = points.GetLength(1);
            List<Quad> quads = new List<Quad>();

            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    var p00 = points[i, j];
                    var p10 = points[i + 1, j];
                    var p11 = points[i + 1, j + 1];
                    var p01 = points[i, j + 1];

                    if (!p00.IsValid || !p10.IsValid || !p11.IsValid || !p01.IsValid)
                        continue;

                    float avgDepth = (p00.Depth + p10.Depth + p11.Depth + p01.Depth) / 4.0f;

                    quads.Add(new Quad
                    {
                        AvgDepth = avgDepth,
                        Corners = new PointF[] {
                            new PointF(p00.Px, p00.Py),
                            new PointF(p10.Px, p10.Py),
                            new PointF(p11.Px, p11.Py),
                            new PointF(p01.Px, p01.Py)
                        },
                        CornerAmps = new float[] { p00.Amp, p10.Amp, p11.Amp, p01.Amp }
                    });
                }
            }

            // Sort far to near
            quads.Sort((a, b) => b.AvgDepth.CompareTo(a.AvgDepth));

            using (Brush bgBrush = new SolidBrush(BG_COLOR))
            {
                foreach (var quad in quads)
                {
                    // Fill
                    g.FillPolygon(bgBrush, quad.Corners);

                    // Draw edges
                    DrawEdge(g, quad.Corners[0], quad.Corners[1], quad.CornerAmps[0], quad.CornerAmps[1]);
                    DrawEdge(g, quad.Corners[1], quad.Corners[2], quad.CornerAmps[1], quad.CornerAmps[2]);
                    DrawEdge(g, quad.Corners[2], quad.Corners[3], quad.CornerAmps[2], quad.CornerAmps[3]);
                    DrawEdge(g, quad.Corners[3], quad.Corners[0], quad.CornerAmps[3], quad.CornerAmps[0]);
                }
            }
        }

        private void DrawEdge(Graphics g, PointF p1, PointF p2, float amp1, float amp2)
        {
            float avgAmp = (amp1 + amp2) * 0.5f;
            float hue = Math.Min(1.0f, avgAmp / HEIGHT_SCALE);
            // Hue in python: 0..1. 
            // In C# we need a helper.
            Color color = ColorFromHSV(hue * 360f, 1.0f, 1.0f);
            
            using (Pen pen = new Pen(color))
            {
                g.DrawLine(pen, p1, p2);
            }
        }

        // Helper to convert HSV to Color
        // Hue: 0-360, Sat: 0-1, Val: 0-1
        private Color ColorFromHSV(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromArgb(255, v, t, p);
                case 1: return Color.FromArgb(255, q, v, p);
                case 2: return Color.FromArgb(255, p, v, t);
                case 3: return Color.FromArgb(255, p, q, v);
                case 4: return Color.FromArgb(255, t, p, v);
                default: return Color.FromArgb(255, v, p, q);
            }
        }
    }
}

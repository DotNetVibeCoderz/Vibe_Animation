using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace P5Net
{
    /// <summary>
    /// P5Canvas - Base class untuk membuat sketch seperti P5.js
    /// </summary>
    public abstract class P5Canvas : Form
    {
        private System.Windows.Forms.Timer timer;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        
        // Properties untuk canvas
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new int Width { get; protected set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new int Height { get; protected set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Graphics g { get; private set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int FrameCount { get; private set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int MouseX { get; private set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int MouseY { get; private set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int PMMouseX { get; private set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int PMMouseY { get; private set; }
        
        // Math helpers
        public float TWO_PI = (float)(Math.PI * 2);
        public float PI = (float)Math.PI;
        public float HALF_PI = (float)(Math.PI / 2);
        public float QUARTER_PI = (float)(Math.PI / 4);
        
        protected P5Canvas()
        {
            Width = 800;
            Height = 600;
            FrameCount = 0;
            
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.ClientSize = new Size(Width, Height);
            this.Text = "P5Net Canvas";
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Setup double buffering
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(Width + 1, Height + 1);
            buffer = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, Width, Height));
            g = buffer.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Setup timer untuk animasi
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += Timer_Tick;
            
            // Event handlers
            this.Load += P5Canvas_Load;
            this.Paint += P5Canvas_Paint;
            this.MouseMove += P5Canvas_MouseMove;
            this.Resize += P5Canvas_Resize;
        }
        
        private void P5Canvas_Load(object? sender, EventArgs e)
        {
            Setup();
            timer.Start();
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            PMMouseX = MouseX;
            PMMouseY = MouseY;
            FrameCount++;
            
            Draw();
            
            this.Invalidate();
        }
        
        private void P5Canvas_Paint(object? sender, PaintEventArgs e)
        {
            buffer.Render(e.Graphics);
        }
        
        private void P5Canvas_MouseMove(object? sender, MouseEventArgs e)
        {
            MouseX = e.X;
            MouseY = e.Y;
        }
        
        private void P5Canvas_Resize(object? sender, EventArgs e)
        {
            if (this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
            {
                Width = this.ClientSize.Width;
                Height = this.ClientSize.Height;
                
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                if (buffer != null)
                    buffer.Dispose();
                    
                buffer = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, Width, Height));
                g = buffer.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }
        }
        
        // Abstract methods yang harus diimplementasikan
        public abstract void Setup();
        public abstract void Draw();
        
        // Drawing Functions
        public void Background(int gray)
        {
            g.Clear(Color.FromArgb(gray, gray, gray));
        }
        
        public void Background(int r, int gb, int b)
        {
            this.g.Clear(Color.FromArgb(r, gb, b));
        }
        
        public void Fill(int gray)
        {
            currentFill = new SolidBrush(Color.FromArgb(gray, gray, gray));
            useFill = true;
        }
        
        public void Fill(int r, int gb, int b)
        {
            currentFill = new SolidBrush(Color.FromArgb(r, gb, b));
            useFill = true;
        }
        
        public void Fill(int r, int gb, int b, int a)
        {
            currentFill = new SolidBrush(Color.FromArgb(a, r, gb, b));
            useFill = true;
        }
        
        public void NoFill()
        {
            useFill = false;
        }
        
        public void Stroke(int gray)
        {
            currentStroke = new Pen(Color.FromArgb(gray, gray, gray), strokeWeight);
            useStroke = true;
        }
        
        public void Stroke(int r, int gb, int b)
        {
            currentStroke = new Pen(Color.FromArgb(r, gb, b), strokeWeight);
            useStroke = true;
        }
        
        public void Stroke(int r, int gb, int b, int a)
        {
            currentStroke = new Pen(Color.FromArgb(a, r, gb, b), strokeWeight);
            useStroke = true;
        }
        
        public void NoStroke()
        {
            useStroke = false;
        }
        
        public void StrokeWeight(float weight)
        {
            strokeWeight = weight;
            if (currentStroke != null)
                currentStroke.Width = weight;
        }
        
        private Brush currentFill = new SolidBrush(Color.White);
        private Pen currentStroke = new Pen(Color.Black, 1);
        private float strokeWeight = 1;
        private bool useFill = true;
        private bool useStroke = true;
        
        // Shape Drawing
        public void Ellipse(float x, float y, float w, float h)
        {
            if (useFill)
                g.FillEllipse(currentFill, x - w / 2, y - h / 2, w, h);
            if (useStroke)
                g.DrawEllipse(currentStroke, x - w / 2, y - h / 2, w, h);
        }
        
        public void Circle(float x, float y, float d)
        {
            Ellipse(x, y, d, d);
        }
        
        public void Rect(float x, float y, float w, float h)
        {
            if (useFill)
                g.FillRectangle(currentFill, x, y, w, h);
            if (useStroke)
                g.DrawRectangle(currentStroke, x, y, w, h);
        }
        
        public void Line(float x1, float y1, float x2, float y2)
        {
            if (useStroke)
                g.DrawLine(currentStroke, x1, y1, x2, y2);
        }
        
        public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            PointF[] points = new PointF[] {
                new PointF(x1, y1),
                new PointF(x2, y2),
                new PointF(x3, y3)
            };
            
            if (useFill)
                g.FillPolygon(currentFill, points);
            if (useStroke)
                g.DrawPolygon(currentStroke, points);
        }
        
        public void Point(float x, float y)
        {
            g.FillEllipse(currentFill, x, y, strokeWeight, strokeWeight);
        }
        
        // Transformation
        public void PushMatrix()
        {
            matrixStack.Push(g.Transform);
        }
        
        public void PopMatrix()
        {
            if (matrixStack.Count > 0)
                g.Transform = matrixStack.Pop();
        }
        
        private System.Collections.Generic.Stack<System.Drawing.Drawing2D.Matrix> matrixStack = 
            new System.Collections.Generic.Stack<System.Drawing.Drawing2D.Matrix>();
        
        public void Translate(float x, float y)
        {
            g.TranslateTransform(x, y);
        }
        
        public void Rotate(float angle)
        {
            g.RotateTransform(angle * 180f / PI);
        }
        
        public new void Scale(float s)
        {
            g.ScaleTransform(s, s);
        }
        
        public new void Scale(float x, float y)
        {
            g.ScaleTransform(x, y);
        }
        
        // Math Functions
        public float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        }
        
        public float Lerp(float start, float stop, float amt)
        {
            return start + (stop - start) * amt;
        }
        
        public float Constrain(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
        
        public float Dist(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        
        public float Random(float max)
        {
            return (float)(random.NextDouble() * max);
        }
        
        public float Random(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        
        private Random random = new Random();
        
        // Bezier
        public void Bezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            if (useStroke)
            {
                g.DrawBezier(currentStroke, x1, y1, x2, y2, x3, y3, x4, y4);
            }
        }
        
        public PointF BezierPoint(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float t)
        {
            float x = (float)(Math.Pow(1 - t, 3) * x1 + 3 * Math.Pow(1 - t, 2) * t * x2 + 
                             3 * (1 - t) * Math.Pow(t, 2) * x3 + Math.Pow(t, 3) * x4);
            float y = (float)(Math.Pow(1 - t, 3) * y1 + 3 * Math.Pow(1 - t, 2) * t * y2 + 
                             3 * (1 - t) * Math.Pow(t, 2) * y3 + Math.Pow(t, 3) * y4);
            return new PointF(x, y);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer?.Stop();
                timer?.Dispose();
                buffer?.Dispose();
                context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

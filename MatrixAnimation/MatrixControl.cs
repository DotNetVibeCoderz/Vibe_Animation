using MatrixAnimation.Rendering;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MatrixAnimation
{
    /// <summary>
    /// SkiaSharp-backed control that owns the MatrixRenderer and drives the
    /// animation loop. It exposes an ApplySettings method so the parent form
    /// can push new settings in.
    /// </summary>
    public class MatrixControl : Control
    {
        private readonly SKControl _skControl = new();
        private MatrixRenderer? _renderer;
        private System.Windows.Forms.Timer? _timer;

        public MatrixRenderer Renderer => _renderer ??= new MatrixRenderer(new Models.AnimationSettings());

        public MatrixControl()
        {
            DoubleBuffered = true;
            BackColor = Color.Black;

            _skControl.Dock = DockStyle.Fill;
            _skControl.PaintSurface += OnPaint;
            Controls.Add(_skControl);

            // ~60 FPS paint timer
            _timer = new System.Windows.Forms.Timer { Interval = 16 };
            _timer.Tick += (s, e) => _skControl.Invalidate();
            _timer.Start();
        }

        public void ApplySettings(Models.AnimationSettings settings)
        {
            Renderer.Settings = settings;
        }

        public void TriggerRipple(int x, int y)
        {
            // Adjust for control-relative coordinates
            Renderer.TriggerRipple(x, y);
        }

        private void OnPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);
            Renderer.Render(canvas, e.Info.Width, e.Info.Height);
        }
    }
}

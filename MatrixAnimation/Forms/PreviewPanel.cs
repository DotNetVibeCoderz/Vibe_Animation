using MatrixAnimation.Models;
using MatrixAnimation.Rendering;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MatrixAnimation.Forms
{
    /// <summary>
    /// Tiny SkiaSharp-backed control used inside the SettingsForm so the user
    /// can preview changes in real time.
    /// </summary>
    public class PreviewPanel : Control
    {
        private readonly SKControl _skControl = new();
        private MatrixRenderer? _renderer;
        private AnimationSettings _settings = new();
        private bool _dirty;

        public PreviewPanel()
        {
            DoubleBuffered = true;
            _skControl.Dock = DockStyle.Fill;
            _skControl.PaintSurface += OnPaint;
            Controls.Add(_skControl);

            // A timer driving ~30 fps for the preview
            var timer = new System.Windows.Forms.Timer { Interval = 33 };
            timer.Tick += (s, e) => { if (_dirty || true) _skControl.Invalidate(); };
            timer.Start();
        }

        public void AttachSettings(AnimationSettings s)
        {
            _settings = s;
            _renderer?.Settings = s;
            MarkDirty();
        }

        public void MarkDirty() => _dirty = true;

        private void OnPaint(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            if (_renderer == null)
            {
                _renderer = new MatrixRenderer(_settings);
            }
            // Renderer can keep references to latest settings, but here we want to
            // mutate only when settings change to avoid GC pressure.
            if (_dirty)
            {
                _renderer.Settings = _settings;
                _dirty = false;
            }
            _renderer.Render(canvas, e.Info.Width, e.Info.Height);
        }
    }
}

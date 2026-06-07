using MatrixAnimation.Models;
using SkiaSharp;

namespace MatrixAnimation.Rendering
{
    /// <summary>
    /// Core rendering engine. Owns the streams and draws a single frame to a SKCanvas.
    /// Pure SkiaSharp - no WinForms dependency so it can be unit-tested.
    /// </summary>
    public class MatrixRenderer
    {
        private readonly Random _rng = new();
        private List<MatrixStream> _streams = new();
        private AnimationSettings _settings;
        private SKPaint _charPaint = null!;
        private SKPaint _glowPaint = null!;
        private SKPaint _headPaint = null!;
        private SKPaint _fadePaint = null!;
        private SKPaint _ripplePaint = null!;
        private SKTypeface _typeface = null!;
        private readonly List<Ripple> _ripples = new();

        // Last frame timestamp used to compute dt.
        private DateTime _lastFrame = DateTime.UtcNow;

        public AnimationSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value.Clone();
                RebuildPaints();
                // Streams will be (re)built by EnsureStreams on next frame
                _streams.Clear();
            }
        }

        public MatrixRenderer(AnimationSettings settings)
        {
            _settings = settings.Clone();
            // Use a monospace typeface that ships with Skia - "Courier" / "Menlo" / "Consolas" works on Win
            _typeface = SKTypeface.FromFamilyName("Consolas")
                       ?? SKTypeface.FromFamilyName("Courier New")
                       ?? SKTypeface.FromFamilyName("Monospace")
                       ?? SKTypeface.Default;
            RebuildPaints();
        }

        /// <summary>
        /// (Re)creates SKPaint objects when the settings change.
        /// </summary>
        private void RebuildPaints()
        {
            var theme = ColorThemes.Get(_settings.Theme);
            float fontSize = _settings.FontSize;

            _charPaint = new SKPaint
            {
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = fontSize,
                Color = theme.TrailColor,
                Style = SKPaintStyle.Fill,
            };

            _glowPaint = new SKPaint
            {
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = fontSize,
                Color = theme.GlowColor.WithAlpha((byte)(_settings.GlowIntensity * 255)),
                Style = SKPaintStyle.Fill,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, fontSize * 0.35f),
            };

            _headPaint = new SKPaint
            {
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = fontSize,
                Color = theme.HeadColor,
                Style = SKPaintStyle.Fill,
            };

            // Translucent black rectangle used to "fade" the previous frame
            // -> creates the smooth motion-trail / afterglow look.
            byte fadeAlpha = _settings.MotionBlur ? (byte)35 : (byte)200;
            _fadePaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, fadeAlpha),
                Style = SKPaintStyle.Fill,
            };

            _ripplePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                Color = new SKColor(255, 255, 255, (byte)220),
            };
        }

        /// <summary>
        /// Ensures that the stream list covers the whole canvas with the current
        /// column spacing / font size.
        /// </summary>
        private void EnsureStreams(int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            int spacing = Math.Max(8, _settings.ColumnSpacing);
            int neededColumns = (width / spacing) + 2;

            if (_streams.Count == neededColumns) return;

            // Rebuild for simplicity - cheap operation, runs only on resize
            _streams.Clear();
            for (int c = 0; c < neededColumns; c++)
            {
                int trailLen = _settings.TrailLength;
                float speed = _settings.FallSpeed * (0.5f + (float)_rng.NextDouble() * 1.0f);
                float x = c * spacing;
                float y = -_rng.Next(0, height);
                _streams.Add(new MatrixStream(c, x, y, _rng, speed, trailLen, _settings.MutationRate));
            }
        }

        /// <summary>
        /// Draws one frame to the given canvas.
        /// </summary>
        public void Render(SKCanvas canvas, int width, int height)
        {
            // Compute dt
            var now = DateTime.UtcNow;
            float dt = (float)(now - _lastFrame).TotalSeconds;
            if (dt > 0.1f) dt = 0.1f; // clamp big jumps
            _lastFrame = now;

            EnsureStreams(width, height);
            if (_streams.Count == 0) return;

            // Update all streams
            foreach (var s in _streams) s.Update(dt, height);

            // Apply motion-blur fade
            if (_settings.MotionBlur)
            {
                canvas.DrawRect(0, 0, width, height, _fadePaint);
            }
            else
            {
                canvas.Clear(SKColors.Black);
            }

            // Draw each stream
            var theme = ColorThemes.Get(_settings.Theme);
            int fontSize = _settings.FontSize;

            foreach (var s in _streams)
            {
                for (int i = 0; i < s.TrailLength; i++)
                {
                    float charY = s.Y - i * fontSize;
                    if (charY < -fontSize || charY > height + fontSize) continue;

                    char glyph = s.GetChar(i);
                    string txt = glyph.ToString();
                    float x = s.X;

                    // Trail fade: closer to head -> brighter
                    float trailAlpha = 1f - (i / (float)s.TrailLength);
                    byte alpha = (byte)(trailAlpha * 255);

                    if (i == 0)
                    {
                        // Head: pure white with neon glow
                        canvas.DrawText(txt, x, charY, _glowPaint);
                        canvas.DrawText(txt, x, charY, _headPaint);
                    }
                    else
                    {
                        // Body: use trail color with decreasing alpha
                        _charPaint.Color = theme.TrailColor.WithAlpha(alpha);
                        canvas.DrawText(txt, x, charY, _charPaint);
                    }
                }
            }

            // Draw click ripples on top
            DrawRipples(canvas, dt);
        }

        /// <summary>
        /// Add a ripple at the given screen position. Used when user clicks in fullscreen.
        /// </summary>
        public void TriggerRipple(float x, float y)
        {
            if (!_settings.ClickRipple) return;
            _ripples.Add(new Ripple { X = x, Y = y, Age = 0 });
            if (_ripples.Count > 32) _ripples.RemoveAt(0);
        }

        private void DrawRipples(SKCanvas canvas, float dt)
        {
            if (_ripples.Count == 0) return;
            for (int i = _ripples.Count - 1; i >= 0; i--)
            {
                var r = _ripples[i];
                r.Age += dt;
                if (r.Age > 1.2f) { _ripples.RemoveAt(i); continue; }
                float t = r.Age / 1.2f;
                float radius = t * 220f;
                byte alpha = (byte)((1f - t) * 220);
                _ripplePaint.Color = new SKColor(255, 255, 255, alpha);
                canvas.DrawCircle(r.X, r.Y, radius, _ripplePaint);
            }
        }

        private class Ripple
        {
            public float X;
            public float Y;
            public float Age;
        }
    }
}

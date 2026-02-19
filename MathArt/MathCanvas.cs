using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using SkiaSharp;

namespace MathArt;

public class MathCanvas : Control
{
    private WriteableBitmap? _bitmap;
    private DispatcherTimer _timer;
    private Stopwatch _stopwatch;
    private int _animationType = 0; // The index of the selected animation

    public MathCanvas()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // ~60fps
        _timer.Tick += (sender, e) => InvalidateVisual();
        _timer.Start();

        _stopwatch = Stopwatch.StartNew();
    }

    public int AnimationType
    {
        get => _animationType;
        set { _animationType = value; _stopwatch.Restart(); }
    }

    public override void Render(DrawingContext context)
    {
        var width = (int)Bounds.Width;
        var height = (int)Bounds.Height;

        if (width <= 0 || height <= 0) return;

        if (_bitmap == null || _bitmap.PixelSize.Width != width || _bitmap.PixelSize.Height != height)
        {
            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        using (var buf = _bitmap.Lock())
        {
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var surface = SKSurface.Create(info, buf.Address, buf.RowBytes))
            {
                DrawFrame(surface.Canvas, width, height, _stopwatch.Elapsed.TotalSeconds);
            }
        }

        context.DrawImage(_bitmap, new Rect(0, 0, width, height));
    }

    private void DrawFrame(SKCanvas canvas, int width, int height, double time)
    {
        canvas.Clear(SKColors.Black);
        canvas.Translate(width / 2f, height / 2f); // Center

        switch (_animationType)
        {
            case 0:
                DrawLissajous(canvas, width, height, time);
                break;
            case 1:
                DrawWave(canvas, width, height, time);
                break;
            case 2:
                DrawPolarFlower(canvas, width, height, time);
                break;
            case 3:
                DrawSpiralGrid(canvas, width, height, time);
                break;
            case 4:
                DrawFibonacciSpiral(canvas, width, height, time);
                break;
            case 5:
                DrawConcentricCircles(canvas, width, height, time);
                break;
            case 6:
                DrawRandomWalk(canvas, width, height, time);
                break;
        }
    }

    // 0: Lissajous Curve
    private void DrawLissajous(SKCanvas canvas, int width, int height, double time)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Orange,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };

        var path = new SKPath();
        double a = 5;
        double b = 4;
        double delta = Math.PI / 2 + time * 1.5; // animate phase smoothly

        int points = 1000;
        float scale = Math.Min(width, height) / 2.5f;

        for (int i = 0; i <= points; i++)
        {
            double t = (Math.PI * 2 * i) / points;
            float x = (float)(Math.Sin(a * t + delta) * scale);
            float y = (float)(Math.Sin(b * t) * scale);
            
            if (i == 0) path.MoveTo(x, y);
            else path.LineTo(x, y);
        }
        canvas.DrawPath(path, paint);
    }
    
    // 1: Smooth Wave (Generative Wave)
    private void DrawWave(SKCanvas canvas, int width, int height, double time)
    {
        using var paint = new SKPaint { Color = SKColors.Crimson, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 3 };
        var path = new SKPath();
        
        float scaleX = width / (float)(Math.PI * 2);
        float scaleY = height / 4f;

        canvas.Translate(-width / 2f, 0); // start from left

        int points = 1000;
        for (int i = 0; i <= points; i++)
        {
            double x = (Math.PI * 2 * i) / points;
            double y = Math.Sin(x - time * 2) + 0.5 * Math.Cos(5 * x + time * 3);
            
            float px = (float)(x * scaleX);
            float py = (float)(-y * scaleY);
            
            if (i == 0) path.MoveTo(px, py);
            else path.LineTo(px, py);
        }
        canvas.DrawPath(path, paint);
    }

    // 2: Polar Flower
    private void DrawPolarFlower(SKCanvas canvas, int width, int height, double time)
    {
        float scale = Math.Min(width, height) / 2.5f;

        for (int k = 2; k <= 7; k++)
        {
            using var paint = new SKPaint
            {
                Color = SKColor.FromHsv((float)((k * 40 + time * 50) % 360), 80, 100).WithAlpha(150),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            var path = new SKPath();
            int points = 500;
            
            for (int i = 0; i <= points; i++)
            {
                double theta = (Math.PI * 2 * i) / points;
                double r = Math.Sin(k * theta + time);
                
                float x = (float)(r * Math.Cos(theta) * scale);
                float y = (float)(r * Math.Sin(theta) * scale);
                
                if (i == 0) path.MoveTo(x, y);
                else path.LineTo(x, y);
            }
            canvas.DrawPath(path, paint);
        }
    }

    // 3: Algorithmic Spiral Grid
    private void DrawSpiralGrid(SKCanvas canvas, int width, int height, double time)
    {
        int n = 300;
        float scale = Math.Min(width, height) / 2.5f;

        for (int i = 0; i < n; i++)
        {
            double theta = (10 * Math.PI * i) / n + time;
            double r = 0.1 + (0.9 * i) / n;
            
            float x = (float)(r * Math.Cos(theta) * scale);
            float y = (float)(r * Math.Sin(theta) * scale);

            using var paint = new SKPaint
            {
                Color = SKColor.FromHsv((float)((i * 360f / n + time * 50) % 360), 80, 100).WithAlpha(200),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.5f
            };

            canvas.DrawLine(0, 0, x, y, paint);
        }
    }

    // 4: Fibonacci Spiral
    private void DrawFibonacciSpiral(SKCanvas canvas, int width, int height, double time)
    {
        int n = 400;
        double goldenAngle = Math.PI * (3 - Math.Sqrt(5));
        float scale = Math.Min(width, height) / 45f;

        for (int i = 0; i < n; i++)
        {
            double r = Math.Sqrt(i) * scale;
            double theta = i * goldenAngle + time * 0.5;

            float x = (float)(r * Math.Cos(theta));
            float y = (float)(r * Math.Sin(theta));

            using var paint = new SKPaint
            {
                Color = SKColor.FromHsv((float)((theta * 180 / Math.PI + time * 40) % 360), 80, 100),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            float size = (float)Math.Sqrt(i) * 0.5f;
            canvas.DrawCircle(x, y, size, paint);
        }
    }

    // 5: Concentric Circles (Symmetry)
    private void DrawConcentricCircles(SKCanvas canvas, int width, int height, double time)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Teal,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f
        };

        float maxRadius = Math.Min(width, height) / 2f;
        int count = 50;

        for (int i = 0; i < count; i++)
        {
            float radius = (i + 1) * (maxRadius / count) + (float)(Math.Sin(time * 3 + i * 0.2) * 10);
            if (radius > 0)
                canvas.DrawCircle(0, 0, radius, paint);
        }
    }

    // 6: Random Walk (Drunken Artist) - simulated gracefully
    private void DrawRandomWalk(SKCanvas canvas, int width, int height, double time)
    {
        // Deterministic pseudo-random walk based on time to make it animatable
        int steps = 1000;
        float scale = Math.Min(width, height) / 50f;

        using var paint = new SKPaint
        {
            Color = SKColors.SkyBlue.WithAlpha(200),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };

        var path = new SKPath();
        float x = 0;
        float y = 0;

        Random rand = new Random(42); // fixed seed for structure

        for (int i = 0; i < steps; i++)
        {
            // add a wobble over time
            double wobble = Math.Sin(time * 2 + i * 0.1) * 0.5;
            x += (float)((rand.NextDouble() - 0.5 + wobble) * scale);
            y += (float)((rand.NextDouble() - 0.5 + wobble) * scale);

            if (i == 0) path.MoveTo(x, y);
            else path.LineTo(x, y);
        }

        canvas.DrawPath(path, paint);
    }
}

using SkiaSharp;

namespace MatrixAnimation.Models
{
    /// <summary>
    /// A single color theme: head color, trail color and glow color.
    /// </summary>
    public class ColorTheme
    {
        public string Name { get; set; } = "";
        public SKColor HeadColor { get; set; }
        public SKColor TrailColor { get; set; }
        public SKColor GlowColor { get; set; }
    }

    /// <summary>
    /// Predefined neon color themes.
    /// </summary>
    public static class ColorThemes
    {
        public static readonly List<ColorTheme> All = new()
        {
            new ColorTheme
            {
                Name = "Green",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(0, 255, 70),
                GlowColor = new SKColor(80, 255, 120)
            },
            new ColorTheme
            {
                Name = "Pink",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(255, 60, 200),
                GlowColor = new SKColor(255, 130, 220)
            },
            new ColorTheme
            {
                Name = "Blue",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(60, 140, 255),
                GlowColor = new SKColor(120, 180, 255)
            },
            new ColorTheme
            {
                Name = "Cyan",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(0, 230, 230),
                GlowColor = new SKColor(120, 255, 255)
            },
            new ColorTheme
            {
                Name = "Amber",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(255, 170, 30),
                GlowColor = new SKColor(255, 210, 100)
            },
            new ColorTheme
            {
                Name = "Purple",
                HeadColor = SKColors.White,
                TrailColor = new SKColor(170, 80, 255),
                GlowColor = new SKColor(210, 150, 255)
            },
        };

        /// <summary>
        /// Look up a theme by name; falls back to the first one (Green).
        /// </summary>
        public static ColorTheme Get(string? name) =>
            All.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? All[0];
    }
}

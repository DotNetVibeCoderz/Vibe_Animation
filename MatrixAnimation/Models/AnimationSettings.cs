using System.Text.Json;

namespace MatrixAnimation.Models
{
    /// <summary>
    /// Holds all user-tweakable settings for the matrix animation.
    /// Serializable to JSON so we can persist user preferences.
    /// </summary>
    public class AnimationSettings
    {
        // Speed at which columns fall (pixels per second)
        public float FallSpeed { get; set; } = 220f;

        // Spacing between columns in pixels
        public int ColumnSpacing { get; set; } = 18;

        // Size of each character cell in pixels
        public int FontSize { get; set; } = 16;

        // How many characters in each column's trail
        public int TrailLength { get; set; } = 24;

        // How often a character morphs into a new random glyph (0..1)
        public float MutationRate { get; set; } = 0.06f;

        // Strength of the neon glow halo (0..1)
        public float GlowIntensity { get; set; } = 0.75f;

        // The color theme name: Green, Pink, Blue, Cyan, Amber, Rainbow
        public string Theme { get; set; } = "Green";

        // Whether to draw a soft background fade for motion blur
        public bool MotionBlur { get; set; } = true;

        // Whether to play a "ripple" effect when user clicks in fullscreen
        public bool ClickRipple { get; set; } = true;

        /// <summary>
        /// Returns a clone so we can edit values without mutating the live config.
        /// </summary>
        public AnimationSettings Clone() => (AnimationSettings)MemberwiseClone();

        private static string SettingsFile =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MatrixAnimation",
                "settings.json");

        /// <summary>
        /// Loads the persisted settings from disk or returns a default instance.
        /// </summary>
        public static AnimationSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var loaded = JsonSerializer.Deserialize<AnimationSettings>(json);
                    if (loaded != null) return loaded;
                }
            }
            catch
            {
                // If the file is corrupt just fall back to defaults
            }
            return new AnimationSettings();
        }

        /// <summary>
        /// Persists the settings to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch
            {
                // Swallow IO errors so the app never crashes on settings save
            }
        }
    }
}

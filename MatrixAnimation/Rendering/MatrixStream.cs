using System.Text;
using SkiaSharp;

namespace MatrixAnimation.Rendering
{
    /// <summary>
    /// Represents a single vertical "stream" of falling characters - the building block of
    /// the Matrix rain effect. Each stream has its own speed, length, character buffer and
    /// a y-position for the head.
    /// </summary>
    public class MatrixStream
    {
        // All the characters that may appear in the stream. Mix of katakana, latin, digits and symbols.
        private static readonly char[] Glyphs = BuildGlyphs();

        private static char[] BuildGlyphs()
        {
            // Katakana half-width characters: U+FF66 .. U+FF9D
            var sb = new StringBuilder();
            for (int c = 0xFF66; c <= 0xFF9D; c++) sb.Append((char)c);
            // Digits
            foreach (var c in "0123456789") sb.Append(c);
            // Latin uppercase
            foreach (var c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ") sb.Append(c);
            // Symbols
            foreach (var c in "<>=*+-/\\|:;!?$#@&%") sb.Append(c);
            return sb.ToString().ToCharArray();
        }

        private readonly Random _rng;
        public int ColumnIndex { get; }
        public float X { get; set; }   // Pixel x position
        public float Y { get; set; }   // Pixel y position of the head
        public float Speed { get; set; }     // Pixels per second
        public int TrailLength { get; set; } // Number of characters in the trail
        public float MutationRate { get; set; }

        private char[] _chars;
        private float[] _charAlphas; // Alpha 0..1, used to fade the trail gradually

        public MatrixStream(int columnIndex, float x, float startY, Random rng,
                            float speed, int trailLength, float mutationRate)
        {
            ColumnIndex = columnIndex;
            X = x;
            Y = startY;
            _rng = rng;
            Speed = speed;
            TrailLength = trailLength;
            MutationRate = mutationRate;

            _chars = new char[trailLength];
            _charAlphas = new float[trailLength];
            for (int i = 0; i < trailLength; i++)
            {
                _chars[i] = RandomGlyph();
                _charAlphas[i] = 1f - (i / (float)trailLength);
            }
        }

        /// <summary>Pick a random glyph from our charset.</summary>
        public static char RandomGlyphStatic(Random rng) => Glyphs[rng.Next(Glyphs.Length)];
        private char RandomGlyph() => Glyphs[_rng.Next(Glyphs.Length)];

        /// <summary>
        /// Advance the stream. dt is elapsed seconds.
        /// </summary>
        public void Update(float dt, float canvasHeight)
        {
            // Move head down
            Y += Speed * dt;

            // Periodically mutate a random character in the trail to create
            // the classic "glitchy" Matrix effect.
            if (_rng.NextDouble() < MutationRate * dt * 60f)
            {
                int idx = _rng.Next(_chars.Length);
                _chars[idx] = RandomGlyph();
            }

            // If the head moved off-screen, respawn at the top with a small random delay.
            if (Y - TrailLength * Speed / TrailLength > canvasHeight)
            {
                Y = -_rng.Next(0, (int)canvasHeight);
                Speed = RandomSpeed();
                for (int i = 0; i < _chars.Length; i++) _chars[i] = RandomGlyph();
            }
        }

        public static float RandomSpeed() => 60f + (float)Random.Shared.NextDouble() * 240f;
        public static int RandomTrailLength() => 10 + Random.Shared.Next(30);

        /// <summary>
        /// Get the character at position i from the head, where i=0 is the head.
        /// </summary>
        public char GetChar(int i) => _chars[i];

        /// <summary>
        /// Get the alpha (0..1) of character at position i. 0 = head, fades to invisible at the tail.
        /// </summary>
        public float GetAlpha(int i) => _charAlphas[i];
    }
}

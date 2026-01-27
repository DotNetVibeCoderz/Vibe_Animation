using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSavers
{
    // Interface untuk semua animasi screensaver
    public interface IScreenSaverAnimation
    {
        void Initialize(int width, int height);
        void Update(float deltaTime); // deltaTime in seconds
        void Draw(Graphics g, int width, int height);
        void Resize(int width, int height);
    }

    // Panel khusus dengan Double Buffering aktif untuk mencegah flickering
    public class CanvasPanel : Panel
    {
        public CanvasPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.BackColor = Color.Black;
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ScreenSavers.Animations;

namespace ScreenSavers
{
    public partial class MainForm : Form
    {
        private IScreenSaverAnimation currentAnimation;
        private Dictionary<string, IScreenSaverAnimation> animations;
        private bool isFullScreen = false;
        private FormWindowState previousState;
        private FormBorderStyle previousStyle;

        public MainForm()
        {
            InitializeComponent();
            InitializeAnimations();
            
            // Populate ComboBox
            foreach(var key in animations.Keys)
            {
                comboSaver.Items.Add(key);
            }
            if(comboSaver.Items.Count > 0) comboSaver.SelectedIndex = 0;
        }

        private void InitializeAnimations()
        {
            animations = new Dictionary<string, IScreenSaverAnimation>
            {
                { "Starfield Simulation", new StarfieldAnimation() },
                { "Flying Windows", new FlyingWindowsAnimation() },
                { "Mystify", new MystifyAnimation() },
                { "Marquee", new MarqueeAnimation() },
                { "Bezier Curves", new BezierAnimation() },
                { "Wavy Flag", new WavyFlagAnimation() },
                { "3D Flower Box", new FlowerBoxAnimation() },
                { "Flying Toasters", new FlyingToasterAnimation() },
                { "Aquarium", new AquariumAnimation() },
                { "Johnny Castaway", new JohnnyCastawayAnimation() },
                { "3D Pipes", new PipesAnimation() }
            };
        }

        private void ComboSaver_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = comboSaver.SelectedItem.ToString();
            if (animations.ContainsKey(selected))
            {
                currentAnimation = animations[selected];
                currentAnimation.Initialize(canvasPanel.Width, canvasPanel.Height);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Update(0.016f); // Assume 60fps
                canvasPanel.Invalidate();
            }
        }

        private void CanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            if (currentAnimation != null)
            {
                currentAnimation.Draw(e.Graphics, canvasPanel.Width, canvasPanel.Height);
            }
        }

        private void ChkFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFullScreen.Checked)
            {
                GoFullScreen();
            }
            else
            {
                ExitFullScreen();
            }
        }

        private void GoFullScreen()
        {
            previousStyle = this.FormBorderStyle;
            previousState = this.WindowState;
            
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            controlPanel.Visible = false; // Hide controls in full screen (or maybe toggle with key)
            
            // Add key listener to exit
            this.KeyPreview = true;
            
            // Re-init animation for new size
            if(currentAnimation != null) currentAnimation.Initialize(this.Width, this.Height);
        }

        private void ExitFullScreen()
        {
            this.FormBorderStyle = previousStyle;
            this.WindowState = previousState;
            controlPanel.Visible = true;
            this.KeyPreview = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (chkFullScreen.Checked && (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space))
            {
                chkFullScreen.Checked = false; // This will trigger CheckedChanged
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (currentAnimation != null && canvasPanel != null)
            {
                currentAnimation.Resize(canvasPanel.Width, canvasPanel.Height);
            }
        }
    }
}
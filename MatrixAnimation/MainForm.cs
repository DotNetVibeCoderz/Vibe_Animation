using MatrixAnimation.Forms;
using MatrixAnimation.Models;

namespace MatrixAnimation
{
    /// <summary>
    /// The main application window. Hosts the MatrixControl, exposes a menu strip
    /// for file / view / settings / help actions, and supports a fullscreen toggle.
    /// </summary>
    public class MainForm : Form
    {
        private MatrixControl _matrix = null!;
        private MenuStrip _menu = null!;
        private AnimationSettings _settings;
        private bool _isFullscreen;
        private FormWindowState _savedWindowState;
        private FormBorderStyle _savedBorderStyle;
        private bool _savedVisibleMenu;

        public MainForm()
        {
            Text = "Matrix Animation";
            Icon = SystemIcons.Application;
            BackColor = Color.Black;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1100, 700);
            KeyPreview = true;

            // Load persisted settings (or use defaults)
            _settings = AnimationSettings.Load();

            BuildMenu();
            BuildMatrix();
            HookEvents();

            // Push initial settings to renderer
            _matrix.ApplySettings(_settings);
        }

        private void BuildMenu()
        {
            _menu = new MenuStrip
            {
                BackColor = Color.FromArgb(8, 12, 14),
                ForeColor = Color.FromArgb(0, 255, 120),
                RenderMode = ToolStripRenderMode.Professional,
                Padding = new Padding(4, 2, 0, 2),
            };
            _menu.Renderer = new MatrixMenuRenderer();

            // ---- File
            var fileMenu = new ToolStripMenuItem("&File");
            var miFullscreen = new ToolStripMenuItem("Toggle &Fullscreen", null, (_, _) => ToggleFullscreen())
            {
                // Use F11 (a valid shortcut) and F via KeyDown handler
                ShortcutKeys = Keys.F11,
            };
            var miExit = new ToolStripMenuItem("E&xit", null, (_, _) => Close())
            {
                ShortcutKeys = Keys.Alt | Keys.F4,
            };
            fileMenu.DropDownItems.Add(miFullscreen);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(miExit);

            // ---- View
            var viewMenu = new ToolStripMenuItem("&View");
            viewMenu.DropDownItems.Add(miFullscreen);

            // ---- Settings
            var settingsMenu = new ToolStripMenuItem("&Settings");
            var miOpenSettings = new ToolStripMenuItem("&Animation Settings...", null, (_, _) => OpenSettings())
            {
                ShortcutKeys = Keys.Control | Keys.O,
            };
            var miResetSettings = new ToolStripMenuItem("&Reset to Defaults", null, (_, _) => ResetSettings());
            settingsMenu.DropDownItems.Add(miOpenSettings);
            settingsMenu.DropDownItems.Add(miResetSettings);

            // ---- Theme submenu (quick switching)
            var themeMenu = new ToolStripMenuItem("&Theme");
            foreach (var t in ColorThemes.All)
            {
                var tt = t;
                var mi = new ToolStripMenuItem(tt.Name, null, (_, _) =>
                {
                    _settings.Theme = tt.Name;
                    _settings.Save();
                    _matrix.ApplySettings(_settings);
                });
                themeMenu.DropDownItems.Add(mi);
            }
            settingsMenu.DropDownItems.Add(themeMenu);

            // ---- Help
            var helpMenu = new ToolStripMenuItem("&Help");
            var miAbout = new ToolStripMenuItem("&About Matrix Animation", null, (_, _) => OpenAbout());
            var miControls = new ToolStripMenuItem("&Keyboard Shortcuts", null, (_, _) => ShowShortcuts());
            helpMenu.DropDownItems.Add(miControls);
            helpMenu.DropDownItems.Add(miAbout);

            _menu.Items.Add(fileMenu);
            _menu.Items.Add(viewMenu);
            _menu.Items.Add(settingsMenu);
            _menu.Items.Add(helpMenu);

            MainMenuStrip = _menu;
            Controls.Add(_menu);
        }

        private void BuildMatrix()
        {
            _matrix = new MatrixControl
            {
                Dock = DockStyle.Fill,
            };
            // Make sure the matrix is BELOW the menu
            Controls.SetChildIndex(_menu, 0);
            Controls.Add(_matrix);
        }

        private void HookEvents()
        {
            KeyDown += MainForm_KeyDown;
            MouseClick += MainForm_MouseClick;
        }

        private void MainForm_MouseClick(object? sender, MouseEventArgs e)
        {
            // Translate to control-relative coords for the renderer
            var p = _matrix.PointToClient(Cursor.Position);
            _matrix.TriggerRipple(p.X, p.Y);
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F:
                    ToggleFullscreen();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    if (_isFullscreen) ToggleFullscreen();
                    break;
                case Keys.F11:
                    ToggleFullscreen();
                    e.Handled = true;
                    break;
            }
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _savedWindowState = WindowState;
                _savedBorderStyle = FormBorderStyle;
                _savedVisibleMenu = _menu.Visible;

                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                _menu.Visible = false;
                _isFullscreen = true;
                Cursor = Cursors.Default;
            }
            else
            {
                FormBorderStyle = _savedBorderStyle;
                WindowState = _savedWindowState;
                _menu.Visible = _savedVisibleMenu;
                _isFullscreen = false;
                Cursor = Cursors.Default;
            }
        }

        private void OpenSettings()
        {
            using var dlg = new SettingsForm(_settings);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _settings = dlg.Result;
                _settings.Save();
                _matrix.ApplySettings(_settings);
            }
        }

        private void ResetSettings()
        {
            _settings = new AnimationSettings();
            _settings.Save();
            _matrix.ApplySettings(_settings);
        }

        private void OpenAbout()
        {
            using var about = new AboutForm();
            about.ShowDialog(this);
        }

        private void ShowShortcuts()
        {
            MessageBox.Show(this,
                "Keyboard Shortcuts:\r\n\r\n" +
                "  F or F11    - Toggle fullscreen\r\n" +
                "  Esc         - Exit fullscreen\r\n" +
                "  Ctrl+O      - Open settings\r\n" +
                "  Click       - Trigger ripple effect\r\n" +
                "  Alt+F4      - Quit\r\n",
                "Matrix Animation - Shortcuts",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Custom renderer that gives the menu a green-on-black neon look.
        /// </summary>
        private class MatrixMenuRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                var bg = e.Item.Selected || e.Item.Pressed
                    ? Color.FromArgb(0, 90, 40)
                    : Color.FromArgb(8, 12, 14);
                using var b = new SolidBrush(bg);
                e.Graphics.FillRectangle(b, rect);
            }

            protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                var bg = e.Item.Selected
                    ? Color.FromArgb(0, 70, 35)
                    : Color.FromArgb(8, 12, 14);
                using var b = new SolidBrush(bg);
                e.Graphics.FillRectangle(b, rect);
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using var b = new SolidBrush(Color.FromArgb(8, 12, 14));
                e.Graphics.FillRectangle(b, e.AffectedBounds);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var y = e.Item.Height / 2;
                using var pen = new Pen(Color.FromArgb(0, 120, 60));
                e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
            }
        }
    }
}

using MatrixAnimation.Models;

namespace MatrixAnimation.Forms
{
    /// <summary>
    /// Modal dialog that lets the user tweak the matrix animation settings.
    /// All controls are wired up by code (no designer) so we keep the project clean.
    /// </summary>
    public class SettingsForm : Form
    {
        // UI controls
        private TrackBar _trkSpeed = null!;
        private TrackBar _trkColumnSpacing = null!;
        private TrackBar _trkFontSize = null!;
        private TrackBar _trkTrailLength = null!;
        private TrackBar _trkMutation = null!;
        private TrackBar _trkGlow = null!;
        private CheckBox _chkMotionBlur = null!;
        private CheckBox _chkClickRipple = null!;
        private ListBox _lstTheme = null!;
        private Label _lblSpeedVal = null!;
        private Label _lblColumnSpacingVal = null!;
        private Label _lblFontSizeVal = null!;
        private Label _lblTrailLengthVal = null!;
        private Label _lblMutationVal = null!;
        private Label _lblGlowVal = null!;

        // Preview panel
        private PreviewPanel _preview = null!;

        // The single settings object that we mutate in place. UI events push
        // values into this; OK returns it, Cancel discards it.
        private readonly AnimationSettings _working;

        public AnimationSettings Result => _working;

        public SettingsForm(AnimationSettings current)
        {
            _working = current.Clone();

            Text = "Matrix Animation - Settings";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(640, 560);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(20, 24, 28);
            ForeColor = Color.FromArgb(220, 255, 230);
            Font = new Font("Segoe UI", 9F);

            BuildUi();
            LoadFromSettings();
            StartPreview();
        }

        private void BuildUi()
        {
            var title = new Label
            {
                Text = "ANIMATION SETTINGS",
                Font = new Font("Consolas", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 120),
                AutoSize = true,
                Location = new Point(20, 14),
            };
            Controls.Add(title);

            int leftLabelX = 20;
            int trackX = 180;
            int trackW = 280;
            int valX = 470;
            int rowH = 38;
            int y = 60;

            // Speed
            AddLabel("Fall speed", leftLabelX, y);
            _trkSpeed = AddTrack(50, 600, trackX, y, trackW);
            _lblSpeedVal = AddValueLabel(valX, y);
            _trkSpeed.Scroll += (s, e) => { _working.FallSpeed = _trkSpeed.Value; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Column spacing
            AddLabel("Column spacing", leftLabelX, y);
            _trkColumnSpacing = AddTrack(8, 32, trackX, y, trackW);
            _lblColumnSpacingVal = AddValueLabel(valX, y);
            _trkColumnSpacing.Scroll += (s, e) => { _working.ColumnSpacing = _trkColumnSpacing.Value; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Font size
            AddLabel("Font size", leftLabelX, y);
            _trkFontSize = AddTrack(10, 32, trackX, y, trackW);
            _lblFontSizeVal = AddValueLabel(valX, y);
            _trkFontSize.Scroll += (s, e) => { _working.FontSize = _trkFontSize.Value; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Trail length
            AddLabel("Trail length", leftLabelX, y);
            _trkTrailLength = AddTrack(6, 60, trackX, y, trackW);
            _lblTrailLengthVal = AddValueLabel(valX, y);
            _trkTrailLength.Scroll += (s, e) => { _working.TrailLength = _trkTrailLength.Value; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Mutation
            AddLabel("Mutation rate", leftLabelX, y);
            _trkMutation = AddTrack(0, 100, trackX, y, trackW);
            _lblMutationVal = AddValueLabel(valX, y);
            _trkMutation.Scroll += (s, e) => { _working.MutationRate = _trkMutation.Value / 1000f; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Glow
            AddLabel("Glow intensity", leftLabelX, y);
            _trkGlow = AddTrack(0, 100, trackX, y, trackW);
            _lblGlowVal = AddValueLabel(valX, y);
            _trkGlow.Scroll += (s, e) => { _working.GlowIntensity = _trkGlow.Value / 100f; UpdateLabels(); _preview.MarkDirty(); };
            y += rowH;

            // Checkboxes
            _chkMotionBlur = new CheckBox
            {
                Text = "Motion blur (smoother trails)",
                AutoSize = true,
                Location = new Point(leftLabelX, y),
                ForeColor = Color.FromArgb(220, 255, 230),
                BackColor = Color.Transparent,
            };
            _chkMotionBlur.CheckedChanged += (s, e) => { _working.MotionBlur = _chkMotionBlur.Checked; _preview.MarkDirty(); };
            Controls.Add(_chkMotionBlur);
            y += 28;

            _chkClickRipple = new CheckBox
            {
                Text = "Click ripple in fullscreen",
                AutoSize = true,
                Location = new Point(leftLabelX, y),
                ForeColor = Color.FromArgb(220, 255, 230),
                BackColor = Color.Transparent,
            };
            _chkClickRipple.CheckedChanged += (s, e) => { _working.ClickRipple = _chkClickRipple.Checked; };
            Controls.Add(_chkClickRipple);
            y += 32;

            // Theme selector
            AddLabel("Color theme", leftLabelX, y);
            _lstTheme = new ListBox
            {
                Location = new Point(trackX, y - 2),
                Size = new Size(trackW, 60),
                BackColor = Color.FromArgb(8, 12, 14),
                ForeColor = Color.FromArgb(0, 255, 120),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F),
                IntegralHeight = false,
            };
            foreach (var t in ColorThemes.All) _lstTheme.Items.Add(t.Name);
            _lstTheme.SelectedIndexChanged += (s, e) =>
            {
                if (_lstTheme.SelectedItem is string name) { _working.Theme = name; _preview.MarkDirty(); }
            };
            Controls.Add(_lstTheme);

            // Preview area
            _preview = new PreviewPanel
            {
                Location = new Point(20, y + 80),
                Size = new Size(600, 80),
                BackColor = Color.Black,
            };
            Controls.Add(_preview);

            // Buttons
            int btnY = ClientSize.Height - 42;
            var btnReset = MakeButton("Reset", 20, btnY, 90);
            btnReset.Click += (s, e) => ResetToDefaults();

            var btnCancel = MakeButton("Cancel", ClientSize.Width - 200, btnY, 90);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            var btnOk = MakeButton("OK", ClientSize.Width - 100, btnY, 90);
            btnOk.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void StartPreview()
        {
            // A small inline preview renderer that uses a working copy of the settings.
            _preview.AttachSettings(_working);
        }

        private Label AddLabel(string text, int x, int y)
        {
            var l = new Label
            {
                Text = text,
                Location = new Point(x, y + 6),
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 255, 220),
            };
            Controls.Add(l);
            return l;
        }

        private TrackBar AddTrack(int min, int max, int x, int y, int w)
        {
            var tb = new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = min,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                TickStyle = TickStyle.None,
                BackColor = Color.FromArgb(20, 24, 28),
            };
            Controls.Add(tb);
            return tb;
        }

        private Label AddValueLabel(int x, int y)
        {
            var l = new Label
            {
                Text = "0",
                Location = new Point(x, y + 6),
                Size = new Size(120, 18),
                ForeColor = Color.FromArgb(0, 255, 120),
                Font = new Font("Consolas", 9F, FontStyle.Bold),
            };
            Controls.Add(l);
            return l;
        }

        private Button MakeButton(string text, int x, int y, int w)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 40, 44),
                ForeColor = Color.FromArgb(220, 255, 230),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(0, 200, 100);
            Controls.Add(b);
            return b;
        }

        /// <summary>
        /// Resets all the working settings to their default values.
        /// </summary>
        private void ResetToDefaults()
        {
            var defaults = new AnimationSettings();
            _working.FallSpeed = defaults.FallSpeed;
            _working.ColumnSpacing = defaults.ColumnSpacing;
            _working.FontSize = defaults.FontSize;
            _working.TrailLength = defaults.TrailLength;
            _working.MutationRate = defaults.MutationRate;
            _working.GlowIntensity = defaults.GlowIntensity;
            _working.Theme = defaults.Theme;
            _working.MotionBlur = defaults.MotionBlur;
            _working.ClickRipple = defaults.ClickRipple;
            LoadFromSettings();
            _preview.MarkDirty();
        }

        private void LoadFromSettings()
        {
            _trkSpeed.Value = ClampTrack(_trkSpeed, (int)_working.FallSpeed);
            _trkColumnSpacing.Value = ClampTrack(_trkColumnSpacing, _working.ColumnSpacing);
            _trkFontSize.Value = ClampTrack(_trkFontSize, _working.FontSize);
            _trkTrailLength.Value = ClampTrack(_trkTrailLength, _working.TrailLength);
            _trkMutation.Value = ClampTrack(_trkMutation, (int)(_working.MutationRate * 1000f));
            _trkGlow.Value = ClampTrack(_trkGlow, (int)(_working.GlowIntensity * 100f));
            _chkMotionBlur.Checked = _working.MotionBlur;
            _chkClickRipple.Checked = _working.ClickRipple;
            for (int i = 0; i < _lstTheme.Items.Count; i++)
                if ((string)_lstTheme.Items[i] == _working.Theme) { _lstTheme.SelectedIndex = i; break; }
            UpdateLabels();
        }

        private int ClampTrack(TrackBar tb, int v) => Math.Max(tb.Minimum, Math.Min(tb.Maximum, v));

        private void UpdateLabels()
        {
            _lblSpeedVal.Text = _trkSpeed.Value.ToString();
            _lblColumnSpacingVal.Text = _trkColumnSpacing.Value.ToString() + " px";
            _lblFontSizeVal.Text = _trkFontSize.Value.ToString() + " px";
            _lblTrailLengthVal.Text = _trkTrailLength.Value.ToString();
            _lblMutationVal.Text = (_trkMutation.Value / 1000f).ToString("0.000");
            _lblGlowVal.Text = (_trkGlow.Value / 100f).ToString("0.00");
        }
    }
}

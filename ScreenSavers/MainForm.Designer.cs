namespace ScreenSavers
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.canvasPanel = new ScreenSavers.CanvasPanel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.comboSaver = new System.Windows.Forms.ComboBox();
            this.chkFullScreen = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblSelect = new System.Windows.Forms.Label();

            this.controlPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // animationTimer
            // 
            this.animationTimer.Enabled = true;
            this.animationTimer.Interval = 16; // ~60 FPS
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);

            // 
            // canvasPanel
            // 
            this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasPanel.Location = new System.Drawing.Point(0, 0);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Size = new System.Drawing.Size(800, 400);
            this.canvasPanel.TabIndex = 0;
            this.canvasPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPanel_Paint);

            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.controlPanel.Controls.Add(this.lblSelect);
            this.controlPanel.Controls.Add(this.btnExit);
            this.controlPanel.Controls.Add(this.chkFullScreen);
            this.controlPanel.Controls.Add(this.comboSaver);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlPanel.Location = new System.Drawing.Point(0, 400);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(800, 50);
            this.controlPanel.TabIndex = 1;

            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.ForeColor = System.Drawing.Color.White;
            this.lblSelect.Location = new System.Drawing.Point(12, 18);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(85, 15);
            this.lblSelect.TabIndex = 3;
            this.lblSelect.Text = "Select Saver:";

            // 
            // comboSaver
            // 
            this.comboSaver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSaver.FormattingEnabled = true;
            this.comboSaver.Location = new System.Drawing.Point(100, 15);
            this.comboSaver.Name = "comboSaver";
            this.comboSaver.Size = new System.Drawing.Size(200, 23);
            this.comboSaver.TabIndex = 0;
            this.comboSaver.SelectedIndexChanged += new System.EventHandler(this.ComboSaver_SelectedIndexChanged);

            // 
            // chkFullScreen
            // 
            this.chkFullScreen.AutoSize = true;
            this.chkFullScreen.ForeColor = System.Drawing.Color.White;
            this.chkFullScreen.Location = new System.Drawing.Point(320, 17);
            this.chkFullScreen.Name = "chkFullScreen";
            this.chkFullScreen.Size = new System.Drawing.Size(80, 19);
            this.chkFullScreen.TabIndex = 1;
            this.chkFullScreen.Text = "Full Screen";
            this.chkFullScreen.UseVisualStyleBackColor = true;
            this.chkFullScreen.CheckedChanged += new System.EventHandler(this.ChkFullScreen_CheckedChanged);

            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(713, 14);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.canvasPanel);
            this.Controls.Add(this.controlPanel);
            this.Name = "MainForm";
            this.Text = "ScreenSavers Collection";
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Timer animationTimer;
        private ScreenSavers.CanvasPanel canvasPanel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.ComboBox comboSaver;
        private System.Windows.Forms.CheckBox chkFullScreen;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblSelect;
    }
}
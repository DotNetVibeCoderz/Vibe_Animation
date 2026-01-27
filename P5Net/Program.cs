using System;
using System.Windows.Forms;
using P5Net.Examples;

namespace P5Net
{
    static class Program
    {
        /// <summary>
        /// P5Net - Library P5.js untuk C# Windows Forms
        /// Dibuat oleh Gravicode Studios
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Tampilkan menu untuk memilih animasi
            var menu = new MenuForm();
            Application.Run(menu);
        }
    }
    
    /// <summary>
    /// Form Menu untuk memilih contoh animasi
    /// </summary>
    public class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            this.Text = "P5Net - Animation Examples Menu";
            this.Size = new System.Drawing.Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(40, 40, 60);
            
            // Title Label
            var titleLabel = new Label
            {
                Text = "P5Net Animation Examples",
                Font = new System.Drawing.Font("Arial", 20, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(440, 40)
            };
            this.Controls.Add(titleLabel);
            
            // Subtitle
            var subtitleLabel = new Label
            {
                Text = "Pilih contoh animasi yang ingin ditampilkan:",
                Font = new System.Drawing.Font("Arial", 10),
                ForeColor = System.Drawing.Color.LightGray,
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Location = new System.Drawing.Point(20, 65),
                Size = new System.Drawing.Size(440, 25)
            };
            this.Controls.Add(subtitleLabel);
            
            // Button style helper
            void StyleButton(Button btn)
            {
                btn.Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold);
                btn.ForeColor = System.Drawing.Color.White;
                btn.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                btn.Size = new System.Drawing.Size(400, 45);
            }
            
            // Button 1: Orbit Control
            var btn1 = new Button
            {
                Text = "1. Orbit Control Animation",
                Location = new System.Drawing.Point(50, 110)
            };
            StyleButton(btn1);
            btn1.Click += (s, e) => ShowExample(new OrbitControlExample());
            this.Controls.Add(btn1);
            
            // Button 2: Kaleidoscope
            var btn2 = new Button
            {
                Text = "2. Kaleidoscope Animation",
                Location = new System.Drawing.Point(50, 165)
            };
            StyleButton(btn2);
            btn2.Click += (s, e) => ShowExample(new KaleidoscopeExample());
            this.Controls.Add(btn2);
            
            // Button 3: Recursive Tree
            var btn3 = new Button
            {
                Text = "3. Recursive Tree Animation",
                Location = new System.Drawing.Point(50, 220)
            };
            StyleButton(btn3);
            btn3.Click += (s, e) => ShowExample(new RecursiveTreeExample());
            this.Controls.Add(btn3);
            
            // Button 4: Bezier
            var btn4 = new Button
            {
                Text = "4. Bezier Curve Animation",
                Location = new System.Drawing.Point(50, 275)
            };
            StyleButton(btn4);
            btn4.Click += (s, e) => ShowExample(new BezierExample());
            this.Controls.Add(btn4);
            
            // Button 5: Flocking Bird
            var btn5 = new Button
            {
                Text = "5. Flocking Bird Animation (Boids)",
                Location = new System.Drawing.Point(50, 330)
            };
            StyleButton(btn5);
            btn5.Click += (s, e) => ShowExample(new FlockingBirdExample());
            this.Controls.Add(btn5);
            
            // Footer
            var footerLabel = new Label
            {
                Text = "Made with ❤️ by Gravicode Studios",
                Font = new System.Drawing.Font("Arial", 9),
                ForeColor = System.Drawing.Color.Gray,
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Location = new System.Drawing.Point(20, 385),
                Size = new System.Drawing.Size(440, 20)
            };
            this.Controls.Add(footerLabel);
        }
        
        private void ShowExample(Form example)
        {
            example.ShowDialog();
        }
    }
}

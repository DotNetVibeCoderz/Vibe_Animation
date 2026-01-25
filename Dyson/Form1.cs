using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace Dyson
{
    public partial class Form1 : Form
    {
        // Settings
        const int WIDTH = 1200;
        const int HEIGHT = 900;
        const float R_INNER = 4.5f;
        const float R_OUTER = 7.5f;
        const float SEG_HEIGHT = 2.5f;
        const int SUBDIVISIONS = 6;
        const int LIGHTS_PER_SEGMENT = 200;
        
        // Colors
        Color FACE_EDGE_FRONT = Color.FromArgb(0, 255, 0);
        Color FACE_EDGE_BACK = Color.FromArgb(0, 200, 255);
        Color FACE_FILL_FRONT = Color.FromArgb(80, 0, 255, 0); // Alpha 80
        Color FACE_FILL_BACK = Color.FromArgb(45, 0, 200, 255); // Alpha 45
        Color BACKGROUND = Color.Black;

        // State
        List<MeshSegment> segments = new List<MeshSegment>();
        List<Star> stars = new List<Star>();
        
        float viewPitch = (float)(38 * Math.PI / 180.0);
        float viewYaw = (float)(42 * Math.PI / 180.0);
        float baseAzim = (float)(45 * Math.PI / 180.0);
        float rotSpeed = (float)(2 * Math.PI / 600.0);
        float currentTheta = 0;
        int frame = 0;

        // Interaction
        bool isDragging = false;
        Point lastMousePos;

        // GDI+ objects
        System.Windows.Forms.Timer gameTimer;
        Font infoFont = new Font("Consolas", 12);
        Brush textBrush = Brushes.Cyan;

        public Form1()
        {
            // Setup Form
            this.Text = "Dyson Ring Visualization (C# Rendering)";
            this.ClientSize = new Size(WIDTH, HEIGHT);
            this.BackColor = BACKGROUND;
            this.DoubleBuffered = true; // Essential for smooth animation
            
            // Events
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.MouseMove += Form1_MouseMove;
            this.Paint += Form1_Paint;

            InitializeSimulation();

            // Timer for 60 FPS
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; 
            gameTimer.Tick += (s, e) => { frame++; Invalidate(); };
            gameTimer.Start();
        }

        private void InitializeSimulation()
        {
            // Generate Stars
            stars = DysonCompute.GenerateStars(WIDTH, HEIGHT, 600);

            // Generate Ring Segments
            int nSegments = 12;
            float arcSpan = (float)(Math.PI * 2);
            float angleSpan = (arcSpan / nSegments) * 0.90f;
            float arcStart = (float)(-85 * Math.PI / 180.0);

            Random rnd = new Random();

            for (int i = 0; i < nSegments; i++)
            {
                float step = arcSpan / nSegments;
                float centerAngle = arcStart + step * (i + 0.5f);
                
                var segment = DysonCompute.MakeCurvedSegment(centerAngle, angleSpan, R_INNER, R_OUTER, SEG_HEIGHT, SUBDIVISIONS);
                
                // Add Lights to faces
                if (LIGHTS_PER_SEGMENT > 0)
                {
                    for (int l = 0; l < LIGHTS_PER_SEGMENT; l++)
                    {
                        int fIdx = rnd.Next(segment.Faces.Count);
                        // Get face vertices
                        var face = segment.Faces[fIdx];
                        Vector3 a = segment.Vertices[face.Indices[0]];
                        Vector3 b = segment.Vertices[face.Indices[1]];
                        Vector3 c = segment.Vertices[face.Indices[2]];
                        Vector3 d = segment.Vertices[face.Indices[3]];

                        Vector3 p = DysonCompute.SamplePointOnQuad(a, b, c, d);
                        float size = (rnd.NextDouble() < 0.1) ? (float)(3.0 + rnd.NextDouble() * 5.0) : (float)(0.15 + rnd.NextDouble() * 2.35);
                        Color col = Color.FromArgb(rnd.Next(100, 200), rnd.Next(0, 30), rnd.Next(0, 30)); // Red/Orange tint

                        if (segment.FaceLights[fIdx] == null) segment.FaceLights[fIdx] = new List<LightPoint>();
                        segment.FaceLights[fIdx].Add(new LightPoint { LocalPosition = p, Size = size, Color = col });
                    }
                }
                segments.Add(segment);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Draw Background Stars
            Random rnd = new Random(frame); // Deterministic twinkle based on frame? or random
            foreach (var s in stars)
            {
                int alpha = s.BaseColor.A;
                if (rnd.NextDouble() < 0.02) alpha = rnd.Next(100, 255); // Twinkle
                
                using (Brush b = new SolidBrush(Color.FromArgb(alpha, s.BaseColor)))
                {
                    g.FillEllipse(b, s.X, s.Y, s.Size, s.Size);
                }
            }

            // 2. Prepare Matrices
            currentTheta = frame * rotSpeed + baseAzim;
            
            // Matrix order matches Python: RotY(yaw) @ RotX(pitch) @ RotZ(theta)
            Matrix4x4 matZ = Matrix4x4.CreateRotationZ(currentTheta);
            Matrix4x4 matX = Matrix4x4.CreateRotationX(viewPitch);
            Matrix4x4 matY = Matrix4x4.CreateRotationY(viewYaw);

            // Combined matrix
            Matrix4x4 combined = matZ * matX * matY;

            // 3. Central Glow (Simulated Sun)
            float centerX = WIDTH / 2;
            float centerY = HEIGHT / 2;
            int starRadius = 8;
            int glowRadius = 30;
            
            for (int r = glowRadius; r > 0; r -= 5)
            {
                using (Brush b = new SolidBrush(Color.FromArgb(15, 255, 255, 255)))
                {
                    g.FillEllipse(b, centerX - r, centerY - r, r * 2, r * 2);
                }
            }
            g.FillEllipse(Brushes.White, centerX - starRadius, centerY - starRadius, starRadius * 2, starRadius * 2);

            // 4. Transform and Project Geometry
            List<RenderItem> renderList = new List<RenderItem>();
            float scale = 52.5f;

            foreach (var seg in segments)
            {
                // Transform Vertices
                Vector3[] worldVerts = new Vector3[seg.Vertices.Length];
                PointF[] projPoints = new PointF[seg.Vertices.Length];
                
                for (int i = 0; i < seg.Vertices.Length; i++)
                {
                    // Transform
                    Vector3 v = Vector3.Transform(seg.Vertices[i], combined);
                    worldVerts[i] = v;
                    
                    // Project
                    var proj = DysonCompute.Project(v, WIDTH, HEIGHT, scale);
                    projPoints[i] = new PointF(proj.x, proj.y);
                }

                // Process Faces
                for (int i = 0; i < seg.Faces.Count; i++)
                {
                    var face = seg.Faces[i];
                    var itemPoints = new List<PointF>();
                    float avgZ = 0;

                    foreach (int idx in face.Indices)
                    {
                        itemPoints.Add(projPoints[idx]);
                        avgZ += worldVerts[idx].Z; 
                    }
                    avgZ /= face.Indices.Length;

                    // Lights Projection
                    var resultLights = new List<ProjectedLight>();
                    if (seg.FaceLights[i] != null)
                    {
                        foreach (var l in seg.FaceLights[i])
                        {
                            Vector3 lWorld = Vector3.Transform(l.LocalPosition, combined);
                            var lProj = DysonCompute.Project(lWorld, WIDTH, HEIGHT, scale);
                            resultLights.Add(new ProjectedLight { X = lProj.x, Y = lProj.y, Size = l.Size, Color = l.Color });
                        }
                    }

                    // Front Face
                    renderList.Add(new RenderItem
                    {
                        Z = avgZ,
                        Points = itemPoints.ToArray(),
                        EdgeFlags = face.EdgeFlags,
                        FillColor = FACE_FILL_FRONT,
                        EdgeColor = FACE_EDGE_FRONT,
                        Lights = resultLights,
                        IsBackFace = false
                    });

                    // Back Face (Simulating thickness/transparency depth)
                    renderList.Add(new RenderItem
                    {
                        Z = avgZ - 0.0001f, // slight offset to force sort order
                        Points = itemPoints.AsEnumerable().Reverse().ToArray(),
                        EdgeFlags = ReverseFlags(face.EdgeFlags),
                        FillColor = FACE_FILL_BACK,
                        EdgeColor = FACE_EDGE_BACK,
                        Lights = resultLights, 
                        IsBackFace = true
                    });
                }
            }

            // 5. Sort by Depth (Z)
            // In typical projection where Camera looks down Z, larger Z is closer?
            // Actually in System.Numerics/Cartesian, usually +Y is up, +X right, +Z out of screen (Right Hand).
            // So larger Z is closer. Painters algorithm should draw furthest (smallest Z) first.
            renderList.Sort((a, b) => a.Z.CompareTo(b.Z));

            foreach (var item in renderList)
            {
                if (item.Points.Length < 3) continue;

                // Fill
                using (Brush b = new SolidBrush(item.FillColor))
                {
                    g.FillPolygon(b, item.Points);
                }

                // Edges
                using (Pen p = new Pen(item.EdgeColor, 1))
                {
                    for (int k = 0; k < item.Points.Length; k++)
                    {
                        int flagIndex = k;
                        if (flagIndex < item.EdgeFlags.Length && item.EdgeFlags[flagIndex])
                        {
                            var p1 = item.Points[k];
                            var p2 = item.Points[(k + 1) % item.Points.Length];
                            g.DrawLine(p, p1, p2);
                        }
                    }
                }
            }

            // Draw Lights separate pass if needed or on top of faces?
            // If we draw them in the loop inside renderList, they get occluded correctly by other transparent faces (sort of).
            // But we want them to pop. Let's draw them associated with the FRONT face only to avoid doubling brightness,
            // but drawing them at the sorted position of the face.
            
            foreach (var item in renderList) 
            {
               if(item.IsBackFace == false) 
               {
                   foreach(var l in item.Lights)
                   {
                        float s = Math.Max(1, l.Size);
                        using(Brush lb = new SolidBrush(l.Color))
                        {
                            g.FillEllipse(lb, l.X - s/2, l.Y - s/2, s, s);
                        }
                   }
               }
            }
            
            // Info Text
            g.DrawString($"Rot X (Pitch): {viewPitch * 180 / Math.PI:F1}", infoFont, textBrush, 10, 10);
            g.DrawString($"Rot Y (Yaw):   {viewYaw * 180 / Math.PI:F1}", infoFont, textBrush, 10, 30);
            g.DrawString($"Rot Z (Spin):  {currentTheta * 180 / Math.PI:F1}", infoFont, textBrush, 10, 50);
        }

        private bool[] ReverseFlags(bool[] flags)
        {
            if (flags.Length <= 1) return flags.ToArray();
            var list = flags.Take(flags.Length - 1).Reverse().ToList();
            list.Add(flags.Last());
            return list.ToArray();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePos = e.Location;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) isDragging = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                float dx = e.X - lastMousePos.X;
                float dy = e.Y - lastMousePos.Y;
                
                viewYaw += dx * 0.005f;
                viewPitch += dy * 0.005f;

                lastMousePos = e.Location;
                Invalidate();
            }
        }
    }
}

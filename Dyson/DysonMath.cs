using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Dyson
{
    // Struktur untuk Bintang Background
    public class Star
    {
        public float X, Y;
        public float Size;
        public float Brightness;
        public Color BaseColor;
        public Color CurrentColor;

        public Star(float x, float y, float size, float brightness, Color color)
        {
            X = x;
            Y = y;
            Size = size;
            Brightness = brightness;
            BaseColor = color;
            CurrentColor = color;
        }
    }

    // Struktur untuk menyimpan data Mesh (Vertex, Edge, Faces)
    public class MeshSegment
    {
        public Vector3[] Vertices;
        public List<Face> Faces;
        public List<LightPoint>[] FaceLights; // Lampu per wajah

        public MeshSegment(Vector3[] vertices, List<Face> faces)
        {
            Vertices = vertices;
            Faces = faces;
            FaceLights = new List<LightPoint>[faces.Count];
        }
    }

    public struct Face
    {
        public int[] Indices;
        public bool[] EdgeFlags; // Mana edge yang harus digambar
        public Face(int[] indices, bool[] edgeFlags)
        {
            Indices = indices;
            EdgeFlags = edgeFlags;
        }
    }

    public struct LightPoint
    {
        public Vector3 LocalPosition;
        public float Size;
        public Color Color;
    }

    public class RenderItem
    {
        public float Z; // Depth for sorting
        public PointF[] Points;
        public Color FillColor;
        public Color EdgeColor;
        public bool[] EdgeFlags;
        public List<ProjectedLight> Lights;
        public bool IsBackFace;
    }

    public struct ProjectedLight
    {
        public float X, Y, Size;
        public Color Color;
    }

    public static class DysonCompute
    {
        // Konversi HSL/HSV ke Color
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, v, p);
            else if (hi == 2) return Color.FromArgb(255, p, v, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, v);
            else if (hi == 4) return Color.FromArgb(255, t, p, v);
            else return Color.FromArgb(255, v, p, q);
        }

        public static List<Star> GenerateStars(int width, int height, int count)
        {
            var stars = new List<Star>();
            var rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                double hue = rnd.NextDouble() * 360.0;
                Color c = ColorFromHSV(hue, 1.0, 1.0); // Simple saturation/value
                
                stars.Add(new Star(
                    rnd.Next(0, width),
                    rnd.Next(0, height),
                    (float)(0.5 + rnd.NextDouble() * 1.5),
                    rnd.Next(100, 255),
                    c
                ));
            }
            return stars;
        }

        // Logic pembuatan mesh segmen melengkung
        public static MeshSegment MakeCurvedSegment(float angleCenter, float angleSpan, float rInner, float rOuter, float height, int subdivs)
        {
            float halfSpan = angleSpan * 0.5f;
            float[] rs = { rInner, rOuter, rOuter, rInner };
            float[] zs = { -height / 2, -height / 2, height / 2, height / 2 };

            // Buat vertices secara manual step by step
            List<Vector3> verts = new List<Vector3>();
            List<Face> faces = new List<Face>();

            // Kita buat array theta steps
            int steps = subdivs + 1;
            float stepSize = angleSpan / subdivs;
            float startAngle = angleCenter - halfSpan;

            int nRings = steps;
            int pointsPerRing = 4;

            for (int i = 0; i < nRings; i++)
            {
                float theta = startAngle + (i * stepSize);
                float c = (float)Math.Cos(theta);
                float s = (float)Math.Sin(theta);

                for (int j = 0; j < pointsPerRing; j++)
                {
                    verts.Add(new Vector3(rs[j] * c, rs[j] * s, zs[j]));
                }

                if (i > 0)
                {
                    int baseIdx = i * pointsPerRing;
                    int prevBase = (i - 1) * pointsPerRing;

                    for (int j = 0; j < pointsPerRing; j++)
                    {
                        int nextJ = (j + 1) % pointsPerRing;
                        
                        // Definisikan face quad antara ring ini dan sebelumnya
                        int[] indices = { prevBase + j, baseIdx + j, baseIdx + nextJ, prevBase + nextJ };
                        
                        // Flag edge (Long, Ring i, Long, Ring i-1)
                        // Note: Logic python: True, i == nRings-1, True, i == 1
                        bool[] flags = { true, i == nRings - 1, true, i == 1 };
                        
                        faces.Add(new Face(indices, flags));
                    }
                }
            }

            // End Caps
            // Start cap (i=0) -> indices 0,1,2,3
            faces.Add(new Face(new int[] { 0, 1, 2, 3 }, new bool[] { true, true, true, true }));
            
            // End cap (i=last)
            int lastBase = (nRings - 1) * pointsPerRing;
            faces.Add(new Face(new int[] { lastBase + 3, lastBase + 2, lastBase + 1, lastBase }, new bool[] { true, true, true, true }));

            return new MeshSegment(verts.ToArray(), faces);
        }

        // Project point 3D ke 2D (Orthographic)
        // returns (x, y, success, depth)
        // success dipakai jika point di belakang kamera (tidak relevan di orthographic murni tapi berguna kl mau implement perspective)
        public static (float x, float y) Project(Vector3 v, float width, float height, float scale)
        {
            // Orthographic: X map ke X screen, Y (Z in 3D usually mapped to Y, but here Input Z is Up)
            // Python input: x, y, z.
            // Python projection: width*0.5 + x, height*0.5 - y.
            // Verts are transformed before this function.
            
            return (width * 0.5f + v.X * scale, height * 0.5f - v.Y * scale);
        }

         public static Vector3 SamplePointOnQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var rnd = new Random();
            float u = (float)rnd.NextDouble();
            float v = (float)rnd.NextDouble();
            // Bilinear interpolation
            return (1 - u) * (1 - v) * a + u * (1 - v) * b + u * v * c + (1 - u) * v * d;
        }
    }
}

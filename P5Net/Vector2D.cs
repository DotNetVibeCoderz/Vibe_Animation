using System;

namespace P5Net
{
    /// <summary>
    /// Vector2D - Class untuk vektor 2D seperti di P5.js
    /// </summary>
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public Vector2D(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }
        
        // Copy constructor
        public Vector2D Copy()
        {
            return new Vector2D(X, Y);
        }
        
        // Addition
        public Vector2D Add(Vector2D v)
        {
            X += v.X;
            Y += v.Y;
            return this;
        }
        
        public static Vector2D Add(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        
        // Subtraction
        public Vector2D Sub(Vector2D v)
        {
            X -= v.X;
            Y -= v.Y;
            return this;
        }
        
        public static Vector2D Sub(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }
        
        // Multiplication
        public Vector2D Mult(float n)
        {
            X *= n;
            Y *= n;
            return this;
        }
        
        // Division
        public Vector2D Div(float n)
        {
            if (n != 0)
            {
                X /= n;
                Y /= n;
            }
            return this;
        }
        
        // Magnitude
        public float Mag()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }
        
        // Normalize
        public Vector2D Normalize()
        {
            float m = Mag();
            if (m != 0)
            {
                Div(m);
            }
            return this;
        }
        
        // Set magnitude
        public Vector2D SetMag(float mag)
        {
            Normalize();
            Mult(mag);
            return this;
        }
        
        // Limit
        public Vector2D Limit(float max)
        {
            if (Mag() > max)
            {
                SetMag(max);
            }
            return this;
        }
        
        // Heading (angle)
        public float Heading()
        {
            return (float)Math.Atan2(Y, X);
        }
        
        // Distance
        public float Dist(Vector2D v)
        {
            float dx = X - v.X;
            float dy = Y - v.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        
        // Dot product
        public float Dot(Vector2D v)
        {
            return X * v.X + Y * v.Y;
        }
        
        // Static method untuk membuat vektor dari angle
        public static Vector2D FromAngle(float angle)
        {
            return new Vector2D((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        
        // Random 2D vector
        public static Vector2D Random2D(Random? rand = null)
        {
            if (rand == null) rand = new Random();
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            return FromAngle(angle);
        }
    }
}

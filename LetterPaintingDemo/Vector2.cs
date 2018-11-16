using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LetterPaintingDemo
{
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(SumComponentSqrs());
            }
        }

        public float MagSqr
        {
            get
            {
                return SumComponentSqrs();
            }
        }

        public float Dot(Vector2 other)
        {
            return Dot(this, other);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return new Vector2(-v1.X, -v1.Y);
        }

        public static Vector2 operator *(Vector2 v1, float s1)
        {
            return new Vector2(v1.X * s1, v1.Y * s1);
        }

        public static Vector2 operator /(Vector2 v1, float s1)
        {
            return new Vector2(v1.X / s1, v1.Y / s1);
        }

        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public void Normalize()
        {
            X /= Magnitude;
            Y /= Magnitude;
        }

        public static float SumComponents(Vector2 v1)
        {
            return v1.X + v1.Y;
        }

        public float SumComponents()
        {
            return SumComponents(this);
        }

        public static Vector2 SqrComponents(Vector2 v1)
        {
            return new Vector2(v1.X * v1.X, v1.Y * v1.Y);
        }

        public Vector2 SqrComponents()
        {
            return SqrComponents(this);
        }

        public static float SumComponentSqrs(Vector2 v1)
        {
            Vector2 v2 = SqrComponents(v1);
            return v2.SumComponents();
        }

        public float SumComponentSqrs()
        {
            return SumComponentSqrs(this);
        }
    }
}
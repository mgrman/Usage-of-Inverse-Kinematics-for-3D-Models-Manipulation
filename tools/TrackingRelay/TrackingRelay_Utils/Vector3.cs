using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_Utils
{
    public struct Vector3
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator *(Vector3 a, double b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator *(double b, Vector3 a)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public Vector3 RotateAroundY(double angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            return new Vector3(X * cos - Z * sin, Y, X * sin + Y * cos);
        }
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", X, Y, Z);
        }
        public static bool TryParse(string text, out Vector3 vec)
        {
            double pars;
            var items = text.Split(';');
            vec = new Vector3();

            if (items.Any(o => !double.TryParse(o, out pars)))
            {
                return false;
            }
            if (items.Length == 3)
            {
                vec.X = double.Parse(items[0]);
                vec.Y = double.Parse(items[1]);
                vec.Z = double.Parse(items[2]);
            }

            if (items.Length == 1)
            {
                vec.X = double.Parse(items[0]);
                vec.Y = double.Parse(items[0]);
                vec.Z = double.Parse(items[0]);
            }
            return true;
        }
    }

}

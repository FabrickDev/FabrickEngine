using System.Numerics;

namespace Fabrick.ENGINE.MATH
{
    public class FABRICK_MATH
    {
        public static float Clamp(float num, float min, float max)
        {
            if (min == max)
            {
                return min;
            }
            if (num < min)
            {
                return min;
            }
            if (num > max)
            {
                return max;
            }
            return num;
        }
         public static float Clamp(float num, float min)
        {
            if (num < min)
            {
                return min;
            }
            return num;
        }
        public static int Clamp(int num, int min, int max)
        {
            if (min == max)
            {
                return min;
            }
            if (num < min)
            {
                return min;
            }
            if (num > max)
            {
                return max;
            }
            return num;
        }
        public static Vector2 Clamp(Vector2 num, Vector2 min, Vector2 max)
        {
            return new Vector2(Clamp(num.X, min.X, max.X), Clamp(num.Y, min.Y, max.Y));
        }
        public static float Length(Vector2 V)
        {
            return MathF.Sqrt(V.X * V.X + V.Y * V.Y);
        }

        public static float Distance(Vector2 A, Vector2 B)
        {
            float dx = A.X - B.X;
            float dy = A.Y - B.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
        public static Vector2 Normalize(Vector2 V)
        {
            float len = Length(V);
            return new Vector2(V.X / len, V.Y / len);
        }
        public static float Dot(Vector2 A, Vector2 B)
        {
            return A.X * B.X + A.Y * B.Y;
        }
        public static float Cross(Vector2 A, Vector2 B)
        {
            return A.X * B.Y - A.Y * B.X;
        }
        public static Vector2 MidPoint(Vector2 A, Vector2 B)
        {
            return new Vector2((A.X + B.X) / 2, (A.Y + B.Y) / 2);
        }
        public static float AngleToRad(float Angle)
        {
            return Angle * MathF.PI / 180;
        }
        public static float RadToAngle(float Rad)
        {
            return Rad * 180 / MathF.PI;
        }
    }
}
using System.Drawing;
using System.Numerics;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.PHYSICS
{
    public class Line
    {
        private Vector2[] Points = new Vector2[2];
        public Line(Vector2 Point1, Vector2 Point2)
        {
            Points[0] = Point1;
            Points[1] = Point2;
        }
        public Vector2 GetPoint(int point)
        {
            if (point > Points.Length)
            {
                FABRICK_DEBUG.SimpleMessageError($"The number point must be under {Points.Length}");
                return new Vector2(0, 0);
            }
            return Points[point];
        }
        public bool SetPoint(int point, Vector2 Pos)
        {
            if(point > 2) 
            {
                FABRICK_DEBUG.SimpleMessageError($"The number point must be under {Points.Length}");
                return false;
            }
            Points[point] = Pos;
            return true;
        }
    }
    public class Box
    {
        private Vector2 Pos = new Vector2();
        private Vector2 Offset = new Vector2();
        private Vector2 Size = new Vector2();

        public Box(Vector2 Pos, Vector2 Offset, Vector2 Size)
        {
            this.Offset = Offset;
            this.Size = Size;
            this.Pos = Pos;
        }

        public Vector2 GetPos()
        {
            return Pos - Offset;
        }
        public Vector2 GetActualPos()
        {
            return Pos;
        }
        public Vector2 GetOffset()
        {
            return Offset;
        }
        public Vector2 GetSize()
        {
            return Size;
        }
        public void SetPos(Vector2 Pos)
        {
            this.Pos = Pos - this.Offset;
        }
        public void SetOffset(Vector2 Offset, bool RecalculatePos = true)
        {
            if (RecalculatePos)
            {
                Pos = Pos - Offset;
            }
            this.Offset = Offset;
        }
        public void SetSize(Vector2 Size, bool RecalculatePos = true)
        {
            if (RecalculatePos)
            {
                Pos = Pos + (this.Size - Size) / 2;
            }
            this.Size = Size;
        }
        public float GetArea()
        {
            return Size.X * Size.Y;
        }
    }

    public class Circle
    {
        private Vector2 Pos = new Vector2();
        private float Radius = 0;

        public Circle(Vector2 Pos, float Radius)
        {
            this.Pos = Pos;
            this.Radius = Radius;
        }

        public Vector2 GetPos()
        {
            return Pos;
        }
        public float GetRadius()
        {
            return Radius;
        }
        public void SetPos(Vector2 Pos)
        {
            this.Pos = Pos;
        }
        public void SetRadius(float Radius)
        {
            this.Radius = Radius;
        }
        public float GetDiameter()
        {
            return Radius * 2;
        }
        public double GetArea()
        {
            return Radius * Radius * Math.PI;
        }
        public float GetRadius_F()
        {
            return Radius * Radius * (float)Math.PI;
        }
    }
}
using System.Numerics;
using Fabrick.ENGINE.MATH;

namespace Fabrick.ENGINE.ENTITY
{
    public class FABRICK_TRANSFORM
    {
        public float PositionX = 0;
        public float PositionY = 0;
        public float SizeX, SizeY = 0;
        public float CenterPointX, CenterPointY = 0;
        public float Angle = 0;
        public float Sin = 0;
        public float Cos = 0;

        public readonly static FABRICK_TRANSFORM Zero = new FABRICK_TRANSFORM(0f, 0f, 0f, 0f, 0f);

        public FABRICK_TRANSFORM(Vector2 Position, Vector2 Size, float Angle)
        {
            this.PositionX = Position.X;
            this.PositionY = Position.Y;
            this.Sin = MathF.Sin(Angle);
            this.Cos = MathF.Cos(Angle);
            this.SizeX = Size.X;
            this.SizeY = Size.Y;
            this.Angle = Angle;
        }
        public FABRICK_TRANSFORM(Vector2 Position, Vector2 Size, float Angle, Vector2 CenterPoint)
        {
            this.PositionX = Position.X;
            this.PositionY = Position.Y;
            this.Sin = MathF.Sin(Angle);
            this.Cos = MathF.Cos(Angle);
            this.SizeX = Size.X;
            this.SizeY = Size.Y;
            this.Angle = Angle;
            this.CenterPointX = CenterPoint.X;
            this.CenterPointY = CenterPoint.Y;
        }
        public FABRICK_TRANSFORM(float PosX, float PosY, float SizeX, float SizeY, float Angle)
        {
            this.PositionX = PosX;
            this.PositionY = PosY;
            this.Sin = MathF.Sin(Angle);
            this.Cos = MathF.Cos(Angle);
            this.SizeX = SizeX;
            this.SizeY = SizeY;
            this.Angle = Angle;
        }
        public FABRICK_TRANSFORM(float PosX, float PosY, float SizeX, float SizeY, float Angle, float CenterPointX, float CenterPointY)
        {
            this.PositionX = PosX;
            this.PositionY = PosY;
            this.Sin = MathF.Sin(Angle);
            this.Cos = MathF.Cos(Angle);
            this.SizeX = SizeX;
            this.SizeY = SizeY;
            this.Angle = Angle;
            this.CenterPointX = CenterPointX;
            this.CenterPointY = CenterPointY;
        }
        public void SetPosition(Vector2 Pos)
        {
            this.PositionX = Pos.X;
            this.PositionY = Pos.Y;
        }
        public void SetPosition(float PosX, float PosY)
        {
            SetPosition(new Vector2(PosX, PosY));
        }
        public Vector2 GetPosition()
        {
            return new Vector2(PositionX, PositionY);
        }

        public void SetSize(Vector2 Size)
        {
            this.SizeX = Size.X;
            this.SizeY = Size.Y;
        }
        public void SetSize(float SizeX, float SizeY)
        {
            this.SizeX = SizeX;
            this.SizeY = SizeY;
        }
        public Vector2 GetSize()
        {
            return new Vector2(SizeX, SizeY);
        }

        public void SetAngle(float Angle)
        {
            this.Angle = Angle;
        }
        public float GetAngle()
        {
            return Angle;
        }
        public float GetSin()
        {
            return MathF.Sin(FABRICK_MATH.AngleToRad(Angle));
        }
        public float GetCos()
        {
            return MathF.Cos(FABRICK_MATH.AngleToRad(Angle));
        }
        public Vector2 GetCenterPoint()
        {
            return new Vector2(CenterPointX, CenterPointY);
        }
        public void SetCenterPoint(Vector2 Pos)
        {
            CenterPointX = Pos.X;
            CenterPointY = Pos.Y;
        }
    }
}
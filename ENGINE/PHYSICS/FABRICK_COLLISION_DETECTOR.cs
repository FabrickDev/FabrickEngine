using System.Numerics;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using SDL3;

namespace Fabrick.ENGINE.PHYSICS
{
    public static class FABRICK_COLLISIONS
    {
        #if false
        public static bool IntersectCirclePolygon(Vector2 CircleCenter,
                                                float CicelRadius,
                                                Vector2 PolygonCenter,
                                                Vector2[] Vertices,
                                                out Vector2 Normal,
                                                out float Depth)
        {
        }
        #endif
        public static bool IntersectCircle(Vector2 CenterA,
                                            float RadiusA,
                                            Vector2 CenterB,
                                            float RadiusB,
                                            out Vector2 Normal,
                                            out float Depth)
        {
            Normal = Vector2.Zero;
            Depth = 0f;

            float _Distance = FABRICK_MATH.Distance(CenterA, CenterB);
            float _Radius = RadiusA + RadiusB;

            if (_Distance >= _Radius)
            {
                return false;
            }

            Normal = FABRICK_MATH.Normalize(CenterA - CenterB);
            Depth = _Radius - _Distance;
            
            return true;
        }
        public static bool IntersectCircle(Circle A, Circle B, out Vector2 Normal, out float Depth)
        {
            Vector2 PosA = A.GetPos();
            Vector2 PosB = B.GetPos();
            
            float RadA = A.GetRadius();
            float RadB = B.GetRadius();

            return IntersectCircle(PosA, RadA, PosB, RadB, out Normal, out Depth);
        }

        public static bool IntersectCollisionPointBox(Vector2 Point, Box Box)
        {
            bool Collision = false;
            if ((Point.X >= Box.GetPos().X) &&
                (Point.X < Box.GetPos().X + Box.GetSize().X) &&
                (Point.Y >= Box.GetPos().Y) &&
                (Point.Y < Box.GetPos().Y + Box.GetSize().Y))
            {
                Collision = true;
            }
            return Collision;
        }
        public static bool MouseClickToBox_Pressed(Box box, SDL.MouseButtonFlags KeyCode, bool IsUi = true)
        {
            bool Return = false;
            Vector2 MousePos = Vector2.Zero;
            if (IsUi)
            {
                MousePos = FABRICK_INPUT.GetMousePos_UI();
            }
            else
            {
                MousePos = FABRICK_INPUT.GetMousePos();
            }
            if(IntersectCollisionPointBox(MousePos, box) && FABRICK_INPUT.IsMousePressed(KeyCode))
            {
                Return = true;
            }
            return Return;
        }
        public static bool MouseClickToBox_Down(Box box, SDL.MouseButtonFlags KeyCode, bool IsUi = true)
        {
            bool Return = false;
            Vector2 MousePos = Vector2.Zero;
            if (IsUi)
            {
                MousePos = FABRICK_INPUT.GetMousePos_UI();
            }
            else
            {
                MousePos = FABRICK_INPUT.GetMousePos();
            }
            if(IntersectCollisionPointBox(MousePos, box) && FABRICK_INPUT.IsMouseDown(KeyCode))
            {
                Return = true;
            }
            return Return;
        }
    }
}
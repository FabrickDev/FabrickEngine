using System.Numerics;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;

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
            if ((Point.X >= Box.GetActualPos().X) &&
                (Point.X < Box.GetActualPos().X + Box.GetSize().X) &&
                (Point.Y >= Box.GetActualPos().Y) &&
                (Point.Y < Box.GetActualPos().Y + Box.GetSize().Y))
            {
                Collision = true;
            }
            return Collision;
        }
    }
}
/*
References:
https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
*/

using System.Numerics;

namespace Fabrick.ENGINE.PHYSICS
{
    public class FABRICK_PHYSIC
    {
        public static bool RayCast(Vector2 Origin, Vector2 Direction, Vector2 WallStart, Vector2 WallEnd)
        {
            bool IsInterseced = false;
            float x1 = WallStart.X;
            float y1 = WallStart.Y;
            float x2 = WallEnd.X;
            float y2 = WallEnd.Y;

            float x3 = Origin.X;
            float y3 = Origin.Y;
            float x4 = x3 + Direction.X;
            float y4 = y3 + Direction.Y;

            // Calculate Deterimant / Parallel Lines Checker
            float denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denom == 0)
            {
                return IsInterseced;
            }

            // Calculate Intersection Point
            float t = ((x1 - x3) * (y3 - y4) - (y1  - y3) * (x3 - x4)) / denom;
            float u = ((x1 - x2) * (y1 - y3) - (y1  - y2) * (x1 - x3)) / denom;

            if (t >= 0 && t <= 1 && u >= 0)
            {
                IsInterseced = true;
            }

            return IsInterseced;
        }

        public static bool RayCast_Line2Line(Line Line1, Line Line2)
        {
            bool IsInterseced = false;
            float x1 = Line2.GetPoint(0).X;
            float y1 = Line2.GetPoint(0).Y;
            float x2 = Line2.GetPoint(1).X;
            float y2 = Line2.GetPoint(1).Y;

            float x3 = Line1.GetPoint(0).X;
            float y3 = Line1.GetPoint(0).Y;
            float x4 = Line1.GetPoint(1).X;
            float y4 = Line1.GetPoint(1).Y;

            // Calculate Deterimant / Parallel Lines Checker
            float denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denom == 0)
            {
                return IsInterseced;
            }

            // Calculate Intersection Point
            float t = ((x1 - x3) * (y3 - y4) - (y1  - y3) * (x3 - x4)) / denom;
            float u = -1 * ((x1 - x2) * (y1 - y3) - (y1  - y2) * (x1 - x3)) / denom;

            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                IsInterseced = true;
            }

            return IsInterseced;
        }
    }
}
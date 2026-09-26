using System.Numerics;

/*
References:
https://github.com/twobitcoder101/FlatPhysics/blob/main/FlatAABB.cs
*/
namespace Fabrick.ENGINE.PHYSICS
{
    public readonly struct AABB
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public AABB(Vector2 Min, Vector2 Max)
        {
            this.Min = Min;
            this.Max = Max;
        }

        public AABB(float MinX, float MinY, float MaxX, float MaxY)
        {
            Min = new Vector2(MinX, MinY);
            Max = new Vector2(MaxX, MaxY);
        }
    }
}
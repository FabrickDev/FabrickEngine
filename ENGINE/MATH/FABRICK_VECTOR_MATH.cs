using System.Numerics;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.MATH
{
    public class VECTOR_MATH
    {
        public static Vector2 Transform(Vector2 v, FABRICK_TRANSFORM transform)
        {
            return new Vector2(
                transform.Cos * v.X - transform.Sin * v.Y + transform.PositionX,
                transform.Sin * v.X + transform.Cos * v.Y + transform.PositionY
            );
        }
    }
}
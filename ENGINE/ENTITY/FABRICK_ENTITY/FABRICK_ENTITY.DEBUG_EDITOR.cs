using System.Numerics;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.ENTITY
{
    public partial class FABRICK_ENTITY
    {
        // Debug Mode
        private bool IsEngineDebugMode = false;
        private Box DebugBoxCollision = new(Vector2.Zero, new Vector2(-50, -50), new Vector2(100, 100));
        private FABRICK_COLOR DebugBoxColor = FABRICK_COLOR.Orange();

        // Keep This Function On Render, So it Can Be Update All Time
        private void DEBUG_EDITOR_Update()
        {
            if (IsEngineDebugMode)
            {
                DebugBoxCollision.SetPos(Transform.GetPosition());
                FABRICK_VISUALIZE_SHAPE.Visualize(DebugBoxCollision, DebugBoxColor, true, true);
            }
        }
    }
}
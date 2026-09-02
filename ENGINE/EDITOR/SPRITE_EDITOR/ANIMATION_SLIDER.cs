/*
Color usage:
Dragablearea/Unfocus: Color1
Main Area: Color2
Text: Color3
Focushed: Color4
*/

using System.Numerics;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public class ANIMATION_SLIDER
    {
        private Vector2 WorkspacePos = new Vector2(0, 1080 - 150);
        private Vector2 WorkspaceSize = new Vector2(1920, 150);
        public void Initialize()
        {
            
        }
        public void Update()
        {
            
        }
        public void Render()
        {
            RenderBG();
        }
        private void RenderBG()
        {
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(WorkspacePos, WorkspaceSize, EDITOR_MANAGER.Color2);
        }
    }
}
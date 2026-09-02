using System.Numerics;
using Fabrick.ENGINE.EDITOR.ITEMS;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DEV_SPRITE_ANIMATION_EDITOR
    {
        private struct SpriteSheetEditorBlock
        {
            public Vector2 StartPos;
            public Vector2 EndPos;
        }

        ANIMATION_SLIDER ANIMATION_SLIDER = new ANIMATION_SLIDER();
        public void Initialize()
        {
            
        }
        public void Update()
        {
            
        }
        public void Render()
        {
            ANIMATION_SLIDER.Render();
        }
    }
}
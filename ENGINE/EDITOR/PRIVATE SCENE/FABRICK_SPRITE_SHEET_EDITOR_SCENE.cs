using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR.SCENE
{
    public class FABRICK_SPRITE_SHEET_EDITOR_SCENE : SCENE_DATA
    {
        private FABRICK_DEV_SPRITE_SHEET_EDITOR ThisScene = new();

        public FABRICK_SPRITE_SHEET_EDITOR_SCENE()
        {
            SceneName = "@DEV.SPRITE_SHEET_EDITOR";
        }
        public override void Initialize()
        {
            FABRICK_ENTITY_MANAGER.RemoveCompleteAllEntityList();
            ThisScene.Initialize();
        }
        public override void Update()
        {
            ThisScene.Update();
        }
        public override void Render()
        {
            ThisScene.Render();
        }
        public override void Leave()
        {
            
        }
    }
}
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
            InitializeAction = Initialize_Func;
            RunningAction = Update_Func;
            RenderAction = Render_Func;
            LeaveAction = Leave_Func;
        }
        public void Initialize_Func()
        {
            FABRICK_ENTITY_MANAGER.RemoveCompleteAllEntityList();
            ThisScene.Initialize();
        }
        public void Update_Func()
        {
            ThisScene.Update();
        }
        public void Render_Func()
        {
            ThisScene.Render();
        }
        public void Leave_Func()
        {
            
        }
    }
}
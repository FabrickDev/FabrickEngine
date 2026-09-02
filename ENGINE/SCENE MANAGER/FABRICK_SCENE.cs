
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class FABRICK_SCENE : SCENE_DATA
    {
        public FABRICK_SCENE(string SceneName)
        {
            this.SceneName = SceneName;
            InitializeAction = Initialize_Func;
            RunningAction = Update_Func;
            RenderAction = Render_Func;
            LeaveAction = Leave_Func;
        }
        public void Initialize_Func()
        {
            if (FABRICK_ENTITY_RWDATA.ReadEntityData(FABRICK_SCENE_MANAGER.GetCurrentScene()) && Engine is not null)
            {
                FABRICK_ENTITY_MANAGER.InitializeAllEntity();
            }
        }
        public void Update_Func()
        {
            if(Engine is not null)
            {
                FABRICK_ENTITY_MANAGER.UpdateAllEntity(Engine);
            }
        }
        public void Leave_Func()
        {
            FABRICK_DEBUG.Log("[FABRICK_SCENE] Leaving scene: " + this.SceneName);
            FABRICK_ENTITY_MANAGER.RemoveAllEntityList();
            FABRICK_SCENE_MANAGER.InitializeScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
            FABRICK_SCENE_MANAGER.RunScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
            FABRICK_SCENE_MANAGER.UpdateScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
        }
        public void Render_Func()
        {
            FABRICK_ENTITY_MANAGER.RenderAllEntity();
        }
        public override SCENE_DATA Clone()
        {
            return new FABRICK_SCENE(this.SceneName);
        }
    }
}

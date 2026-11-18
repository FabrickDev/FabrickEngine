using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class FABRICK_SCENE : SCENE_DATA
    {
        public FABRICK_SCENE(string SceneName)
        {
            this.SceneName = SceneName;
        }
        public override void Initialize()
        {
            if (FABRICK_ENTITY_RWDATA.ReadEntityData(SceneName))
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENE : {SceneName}] Initializing All Entity");
                FABRICK_ENTITY_MANAGER.InitializeAllEntity();
            }
            FABRICK_DEBUG.Log($"[FABRICK_SCENE : {SceneName}] Current Entity Order: {FABRICK_ENTITY_MANAGER.EntityOrder}");
            FABRICK_DEBUG.Log($"[FABRICK_SCENE : {SceneName}] Current Unremovable Entity Order: {FABRICK_ENTITY_MANAGER.UnremovableEntityOrder}");
        }
        public override void Update()
        {
            if(Engine is not null)
            {
                FABRICK_ENTITY_MANAGER.UpdateAllEntity(Engine);
            }
        }
        public override void Leave()
        {
            FABRICK_DEBUG.Log("[FABRICK_SCENE] Leaving scene: " + this.SceneName);
            //FABRICK_SCENE_MANAGER.InitializeScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
            //FABRICK_SCENE_MANAGER.RunScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
            //FABRICK_SCENE_MANAGER.UpdateScene(FABRICK_SCENE_MANAGER.GetCurrentScene());
        }
        public override void Render()
        {
            FABRICK_ENTITY_MANAGER.RenderAllEntity();
        }
        public override SCENE_DATA Clone()
        {
            return new FABRICK_SCENE(this.SceneName);
        }
    }
}

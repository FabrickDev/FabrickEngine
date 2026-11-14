using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class SCENE_DATA
    {
        public Action? RunningAction {get; set;}
        public Action? RenderAction {get; set;}
        public Action? LeaveAction {get; set;}
        public Action? InitializeAction {get; set;}

        public string SceneName = "";
        public bool IsMain = false;
        public bool IsOverlay = false;

        public FABRICK_ENGINE? Engine;
        public bool IsRunning {get; set;}

        public virtual SCENE_DATA Clone()
        {
            return new();
        }

        public virtual void Initialize()
        {
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Initialize Scene: {SceneName}...");
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Total Entity: {FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count}");
            InitializeAction?.Invoke();
        }

        public virtual void Update()
        {   
            RunningAction?.Invoke();
        }
        public virtual void Render()
        {
            RenderAction?.Invoke();
        }
        public virtual void Leave()
        {
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Leaving Scene: {SceneName}...");
            LeaveAction?.Invoke();
        }
        public void UpdateEngine(FABRICK_ENGINE Engine)
        {
            this.Engine = Engine;
        }
    }
}
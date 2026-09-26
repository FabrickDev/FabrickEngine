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
        public bool UpdateCurrentScene = true;

        // VN Purpose
        public bool ContainTextDialogue = false;

        public FABRICK_ENGINE? Engine;
        public bool IsRunning {get; set;}
        public bool OnLeave {get; set;}
        public int InitializeState {get; set;}

        public virtual SCENE_DATA Clone()
        {
            return new();
        }

        public virtual void Initialize()
        {
            if(UpdateCurrentScene)FABRICK_SCENE_MANAGER.CurrentSceneName = this.SceneName;
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Initialize Scene: {SceneName}...");
            InitializeAction?.Invoke();
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Total Entity: {FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count}");
        }

        public virtual void Update()
        {
            // IDK but it works
            if (InitializeState < 1)
            {
                Initialize();
                InitializeState++;
            }
            if (UpdateCurrentScene)
            {
                FABRICK_SCENE_MANAGER.CurrentSceneName = this.SceneName;
            }
            
            RunningAction?.Invoke();
        }
        public virtual void Render()
        {
            RenderAction?.Invoke();
        }
        public virtual void Leave()
        {
            InitializeState = 0;
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Leaving Scene: {SceneName}...");
            LeaveAction?.Invoke();
        }
        public void RestartScene()
        {
            IsRunning = false;
            OnLeave = false;
            InitializeState = 0;
        }
        public void UpdateEngine(FABRICK_ENGINE Engine)
        {
            this.Engine = Engine;
        }
    }
}
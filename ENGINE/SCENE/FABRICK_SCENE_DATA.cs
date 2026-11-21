using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class SCENE_DATA
    {
        public Action? UpdateAction {get; set;}
        public Action? RenderAction {get; set;}
        public Action? LeaveAction {get; set;}
        public Action? InitializeAction {get; set;}

        internal Action? VN_InitializeAction {get; set;}
        internal Action? VN_UpdateAction {get; set;}
        internal Action? VN_RenderAction {get; set;}
        internal Action? VN_LeaveAction {get; set;}

        public string SceneName = "";
        public bool IsMain = false;
        public bool IsOverlay = false;
        public bool IsNew = false;

        public FABRICK_ENGINE? Engine;
        public bool IsRunning {get; set;}
        public bool Initialized = false;
        public bool Leaved = true;

        public virtual SCENE_DATA Clone()
        {
            return new();
        }

        internal void SceneInitialize()
        {
            if(Initialized) return;
            FABRICK_DEBUG.Log($"[FABRICK_SCENE_DATA : {SceneName}] Initialize Scene...");
            Leaved = false;
            Initialized = true;

            InitializeAction = Initialize;
            UpdateAction = Update;
            RenderAction = Render;
            LeaveAction = Leave;

            VN_InitializeAction = VN_Initialize;
            VN_UpdateAction = VN_Update;
            VN_RenderAction = VN_Render;
            VN_LeaveAction = VN_Leave;
            InitializeAction?.Invoke();
            VN_InitializeAction?.Invoke();
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Total Entity: {FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count}");
        }

        internal void SceneUpdate(FABRICK_ENGINE Engine)
        {   
            this.Engine = Engine;
            IsRunning = true;
            UpdateAction?.Invoke();
            VN_UpdateAction?.Invoke();
        }
        internal void SceneRender()
        {
            //FABRICK_DEBUG.Log($"[SCENE_DATA : {SceneName} Rendering...]");
            RenderAction?.Invoke();
            VN_RenderAction?.Invoke();
        }
        internal void SceneLeave()
        {
            if(Leaved) return;
            FABRICK_DEBUG.Log($"[FABRICK_SCENE_DATA : {SceneName}] Leaving Scene...");
            Leaved = true;
            Initialized = false;
            IsRunning = false;
            LeaveAction?.Invoke();
            VN_LeaveAction?.Invoke();
        }

        public virtual void Initialize(){}
        public virtual void Update(){}
        public virtual void Render(){}
        public virtual void Leave(){}

        internal virtual void VN_Initialize(){}
        internal virtual void VN_Update(){}
        internal virtual void VN_Render(){}
        internal virtual void VN_Leave(){}
    }
}
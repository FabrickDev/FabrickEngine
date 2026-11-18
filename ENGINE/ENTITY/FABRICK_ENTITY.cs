using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.PHYSICS;

namespace Fabrick.ENGINE.ENTITY
{
    public partial class FABRICK_ENTITY
    {
        public string Name = "";
        public string Category = "";
        public int Id = 0;
        public int EntityOrder = 0;
        public bool Unremovable = false;

        public FABRICK_TRANSFORM Transform = new FABRICK_TRANSFORM(0, 0, 0, 0, 0);
        public bool Visibility = true;

        public float DeltaTime = 0f;
        public Action? InitializeAction;
        public Action? UpdateAction;
        public Action? RenderAction;


        public virtual FABRICK_ENTITY Clone()
        {
            return new FABRICK_ENTITY { Name = this.Name,
                                        Category = this.Category,
                                        Id = this.Id,
                                        Transform = this.Transform,
                                        Visibility = this.Visibility,
                                        InitializeAction = this.InitializeAction,
                                        UpdateAction = this.UpdateAction,
                                        RenderAction = this.RenderAction };
        }

        public virtual void EntityInitialize()
        {
            FABRICK_DEBUG.Log($"[FABRICK_ENTITY : {this.Name}] Initialize...");
            InitializeAction = Initialize;
            UpdateAction = Update;
            RenderAction = Render;
            InitializeAction?.Invoke();
        }
        public virtual void EntityUpdate(FABRICK_ENGINE Engine)
        {
            DeltaTime = Engine.DeltaTime;
            IsEngineDebugMode = Engine.IsEditorMode;
            UpdateAction?.Invoke();
        }
        public virtual void EntityRender()
        {
            DEBUG_EDITOR_Update();
            RenderAction?.Invoke();
        }
        public virtual void Initialize(){}
        public virtual void Update(){}
        public virtual void Render(){}
    }
}
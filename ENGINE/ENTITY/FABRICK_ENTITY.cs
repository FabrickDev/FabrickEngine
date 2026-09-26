using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.ENTITY
{
    public class FABRICK_ENTITY
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

        public List<string> TextureRequest = new();

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
            InitializeAction?.Invoke();
        }
        public virtual void EntityUpdate(FABRICK_ENGINE Engine)
        {
            DeltaTime = Engine.DeltaTime;
            UpdateAction?.Invoke();
        }
        public virtual void EntityRender()
        {
            RenderAction?.Invoke();
        }
    }
}
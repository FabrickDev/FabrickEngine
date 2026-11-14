using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.RENDERER;
using SDL3;

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

        private List<string> SpriteTextureList = new();
        private bool UnloadSpriteTextureWhenEntityRemoved = true;

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
        public void SPRITE_AssignTextureSpriteSheet(string SpriteSheetName, string FileName, string Category)
        {
            string FinalCategory = Category + "EntitySprite" + this.Name;
            SPRITE_SHEET_MANAGER.AssignTextureSpriteSheet(SpriteSheetName, FinalCategory, "", FileName);
            if (SpriteTextureList.Contains(SPRITE_SHEET_MANAGER.GetSpriteTextureName(SpriteSheetName, FinalCategory)))
            {
                return;
            }
            SpriteTextureList.Add(SPRITE_SHEET_MANAGER.GetSpriteTextureName(SpriteSheetName, FinalCategory));
        }
        public void SPRITE_CreateSprite(string SheetName, string SpriteSheetName, string Category, SPRITE_COOR_DATA CoorData)
        {
            string FinalCategory = Category + "EntitySprite" + this.Name;
            SPRITE_SHEET_MANAGER.CreateSpriteSheet(SheetName, FinalCategory, SpriteSheetName, CoorData.Pos, CoorData.Size);
        }
        public void SPRITE_CreateSpriteAnimation(string AnimationName, string SpriteSheetName, string Category, List<SPRITE_COOR_DATA> CoorData)
        {
            string FinalCategory = Category + "EntitySprite" + this.Name;
            int Iteration = 0;
            Vector2 CenterPoint = Vector2.Zero;
            foreach (SPRITE_COOR_DATA ThisCoordata in CoorData)
            {
                CenterPoint = ThisCoordata.Size / 2;
                SPRITE_CreateSprite(AnimationName + SpriteSheetName + Convert.ToString(Iteration), SpriteSheetName, FinalCategory, ThisCoordata);
                Iteration++;
            }
            SPRITE_SHEET_MANAGER.CreateSpriteAnimation(AnimationName, "EntitySprite" + this.Name, SPRITE_SHEET_MANAGER.AddSpriteSheetSequence(AnimationName + SpriteSheetName, 0, Iteration - 1), CenterPoint);
        }
        public void SPRITE_RenderSprite(string SheetName, string Category, FABRICK_TRANSFORM SpriteTransform, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
        {
            string FinalCategory = Category + "EntitySprite" + this.Name;
            SPRITE_SHEET_MANAGER.RenderSprite(SheetName, FinalCategory, SpriteTransform, Transparency, isUi, Flip, Parallax);
        }
        public void SPRITE_RenderSpriteAnimation(string AnimationName, string Category, FABRICK_TRANSFORM Tranform, float Transparency = 0, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1, int FPS = 24, bool Loop = true)
        {
            string FinalCategory = Category + "EntitySprite" + this.Name;
            string KeyWorker = AnimationName + this.Name;
            SPRITE_SHEET_MANAGER.RenderSpriteAnimation(SPRITE_SHEET_MANAGER.AutoPlaySpriteAnimation(
                AnimationName,
                FinalCategory,
                KeyWorker,
                Loop,
                FPS
            ), Transform, Transparency, isUi, Flip, Parallax);
        }
        public void SPRITE_DestroyAllTexture()
        {
            if(!UnloadSpriteTextureWhenEntityRemoved) return;
            FABRICK_DEBUG.Log($"[FABRICK_ENTITY: {this.Name}] Unloading All Assigned Texture Sprite Sheet");
            FABRICK_TEXTURE_MANAGER.UnloadTextureByList(SpriteTextureList);
        }
    }
}
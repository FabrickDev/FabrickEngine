using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.RENDERER;
using SDL3;

namespace Fabrick.ENGINE.ENTITY
{
    public partial class FABRICK_ENTITY
    {
        private List<string> SpriteTextureList = new();
        private bool UnloadSpriteTextureWhenEntityRemoved = true;
        
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
using System.Numerics;
using System.Runtime.CompilerServices;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR : FABRICK_ENTITY
    {
        public FABRICK_TRANSFORM PortraitTransform = new(Vector2.Zero, Vector2.Zero, 0f);
        private bool IsMainChar = false;
        public Dictionary<string, string> VariantPortrait = new();
        public Dictionary<string, string> VariantChar = new();
        private string Portrait_CurrentVariantTexture = "";
        private string Char_CurrentVariantTexture = "";
        private bool isFlipped = false;
        private string FrameTextureName = "";
        private float VisibilityFlo = 1f;
        private float TransparencyPortrait, TransparencyFrame = 0f;
        private float PortraitVisibility = 1f;
        public string DisplayName = "";
        private bool IsPortraitEnabled = true;
        private bool IsCharEnabled = true;
        private bool IsCharFlipped = false;

        private Vector2 FRAMEPos, FRAMESize = new();
        public FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR(string Name, bool IsMainChar, int Id)
        {
            this.Category = "VisualNovelPortrait";
            this.Name = Name;
            this.Id = Id;
            this.IsMainChar = IsMainChar;
        }
        public override void Initialize()
        {
            
        }
        public override void Update()
        {
            TransparencyFrame = 1 - VisibilityFlo;
            TransparencyPortrait = 1 - PortraitVisibility * VisibilityFlo;
            //FABRICK_DEBUG.Log($"[FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR : {Name}] My Current Pos: {Transform.GetPosition()}");
        }
        public override void Render()
        {
            // Render Portrait
            //FABRICK_DEBUG.Log("Rendering Frame");
            if (IsCharEnabled && Char_CurrentVariantTexture.Length > 0)
            {
                if (IsCharFlipped)
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(Char_CurrentVariantTexture, this.Transform.GetPosition(), this.Transform.GetSize().X, TransparencyPortrait, false, 1, SDL3.SDL.FlipMode.Horizontal);
                }
                else
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(Char_CurrentVariantTexture, this.Transform.GetPosition(), this.Transform.GetSize().X, TransparencyPortrait, false, 1);
                }
            }
            if(!IsPortraitEnabled) return;
            if (Portrait_CurrentVariantTexture.Length > 0)
            {
                if (isFlipped)
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(Portrait_CurrentVariantTexture, this.PortraitTransform.GetPosition(), this.PortraitTransform.GetSize(), TransparencyPortrait, true, 1, SDL3.SDL.FlipMode.Horizontal);
                }
                else
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(Portrait_CurrentVariantTexture, this.PortraitTransform.GetPosition(), this.PortraitTransform.GetSize(), TransparencyPortrait, true);
                }
            }
            // Render Frame
            if (FrameTextureName.Length > 0)
            {
                FABRICK_DRAW_TEXTURE.RenderTexture(FrameTextureName, this.PortraitTransform.GetPosition() + FRAMEPos, this.PortraitTransform.GetSize() + FRAMESize, TransparencyFrame, true);
            }
        }
        public void AssignSpriteSheetPortrait()
        {
            
        }
        public void AssignSpriteSheetFullChar()
        {
            
        }
        public void EnablePortrait(bool State)
        {
            IsPortraitEnabled = State;
        }
        public void EnableChar(bool State)
        {
            IsCharEnabled = State;
        }
        public void CreateVariantPortrait(string CharName, string VariantName, string SpriteSheetName, string Category)
        {
            if (VariantPortrait.ContainsKey(CharName + "." + VariantName))
            {
                FABRICK_DEBUG.Log($"Cannot Create Variant: {CharName + "." + VariantName}, Cus already exist!!");
                return;
            }

            VariantPortrait[CharName + "." + VariantName] = SPRITE_GetTextureName(SpriteSheetName, Category);
        }
        public void CreateVariantChar(string CharName, string VariantName, string SpriteSheetName, string Category)
        {
            if (VariantChar.ContainsKey(CharName + "." + VariantName))
            {
                FABRICK_DEBUG.Log($"Cannot Create Variant: {CharName + "." + VariantName}, Cus already exist!!");
                return;
            }

            VariantChar[CharName + "." + VariantName] = SPRITE_GetTextureName(SpriteSheetName, Category);
        }
        public void CreateVariantPortraitNChar(string CharName, string VariantName, string SpriteSheetName, string CategoryPortrait, string CategoryChar)
        {
            CreateVariantChar(CharName, VariantName, SpriteSheetName, CategoryChar);
            CreateVariantPortrait(CharName, VariantName, SpriteSheetName, CategoryPortrait);
        }
        public void SetVariant(string Char, string VariantName)
        {
            if (IsPortraitEnabled && VariantPortrait.ContainsKey(Char + "." + VariantName))
            {
                Portrait_CurrentVariantTexture = VariantPortrait[Char + "." + VariantName];
            }
            if (IsPortraitEnabled && VariantChar.ContainsKey(Char + "." + VariantName))
            {
                Char_CurrentVariantTexture = VariantChar[Char + "." + VariantName];
            }
        }
        public void SetFrame(string SpriteSheetName, string Category)
        {
            if (!SPRITE_IsTextureAvailable(SpriteSheetName, Category))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"Cannot Set Frame, Cus sprite: {SpriteSheetName}, Category: {Category} doesn't exist!!");
                return;
            }
            FrameTextureName = SPRITE_GetTextureName(SpriteSheetName, Category);;
        }
        public void SetFrameOffsetPos(float PosX, float PosY)
        {
            FRAMEPos = new Vector2(PosX, PosY);
        }
        public void SetFrameOffsetSize(float SizeX, float SizeY)
        {
            FRAMESize = new Vector2(SizeX, SizeY);
        }
        public void SetFlipped(bool Flipped)
        {
            isFlipped = Flipped;
            IsCharFlipped = Flipped;
        }
        public void SetPortraitFlipped(bool Flipped)
        {
            isFlipped = Flipped;
        }
        public void SetCharFlipped(bool Flipped)
        {
            IsCharFlipped = Flipped;
        }
        public void SetVisibility(float Visibility)
        {
            this.VisibilityFlo = Visibility;
        }
        public void SetPortraitVisibility(float Visibility)
        {
            this.PortraitVisibility = Visibility;
        }
        public bool IsMain()
        {
            return IsMainChar;
        }
    }
}
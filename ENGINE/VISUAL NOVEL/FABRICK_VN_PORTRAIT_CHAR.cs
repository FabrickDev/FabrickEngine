using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR : FABRICK_ENTITY
    {
        private bool IsMainChar = false;
        public Dictionary<string, string> Variant = new();
        private string CurrentVariantTexture = "";
        private bool isFlipped = false;
        private string FrameTextureName = "";
        private float VisibilityFlo = 1f;
        private float TransparencyPortrait, TransparencyFrame = 0f;
        private float PortraitVisibility = 1f;
        public string DisplayName = "";

        private Vector2 FRAMEPos, FRAMESize = new();
        public FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR(string Name, bool IsMainChar, int Id)
        {
            this.Category = "VN Portrait";
            this.Name = Name;
            this.Id = Id;
            this.IsMainChar = IsMainChar;

            InitializeAction = Initialize;
            UpdateAction = Update;
            RenderAction = Render;
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
            if (!CurrentVariantTexture.IsWhiteSpace())
            {
                if (isFlipped)
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(CurrentVariantTexture, this.Transform.GetPosition(), this.Transform.GetSize(), TransparencyPortrait, true, 1, SDL3.SDL.FlipMode.Horizontal);
                }
                else
                {
                    FABRICK_DRAW_TEXTURE.RenderTexture(CurrentVariantTexture, this.Transform.GetPosition(), this.Transform.GetSize(), TransparencyPortrait, true);
                }
            }
            // Render Frame
            if (!FrameTextureName.IsWhiteSpace())
            {
                FABRICK_DRAW_TEXTURE.RenderTexture(FrameTextureName, this.Transform.GetPosition() + FRAMEPos, this.Transform.GetSize() + FRAMESize, TransparencyFrame, true);
            }
        }
        public void CreateVariant(string Char, string VariantName, string TextureName)
        {
            if (Variant.ContainsKey(Char + "." + VariantName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"Cannot Create Variant: {Char + "." + VariantName}, Cus already exist!!");
                return;
            }
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(TextureName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"Cannot Create Variant: {Char + "." + VariantName}, Cus texture: {TextureName} doesn't exist!!");
                return;
            }
            Variant[Char + "." + VariantName] = TextureName;
        }
        public void SetVariant(string Char, string VariantName)
        {
            if (Variant.ContainsKey(Char + "." + VariantName))
            {
                CurrentVariantTexture = Variant[Char + "." + VariantName];
            }
        }
        public void SetFrame(string TextureName)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(TextureName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"Cannot Set Frame, Cus texture: {TextureName} doesn't exist!!");
                return;
            }
            FrameTextureName = TextureName;
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
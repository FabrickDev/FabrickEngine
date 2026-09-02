using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public struct SPRITE_SHEET
    {
        public string Category;
        public SDL.FRect SRC;
        public string TextureName;
        public SPRITE_SHEET()
        {
            Category = "";
            SRC = new();
            TextureName = "";
        }
    }
    public struct SPRITE_ANIMATION
    {
        public Vector2 CenterPoint;
        public Dictionary<int, string> SpriteSheet = new Dictionary<int, string>();
        public SPRITE_ANIMATION()
        {
            CenterPoint = Vector2.Zero;
            SpriteSheet = new Dictionary<int, string>();
        }
    }
    public struct PLAY_SPRITE_ANIMATION
    {
        public string SpriteAnimation;
        public Vector2 CenterPoint;
        public PLAY_SPRITE_ANIMATION()
        {
            SpriteAnimation = "";
            CenterPoint = Vector2.Zero;
        }
    }
    public class SPRITE_SHEET_MANAGER
    {
        private static string SPRITE_SHEET_CATEGORY = "@FABRICK_SPRITE_SHEET.";
        private static Dictionary<string, SPRITE_SHEET> SpriteSheetList = new Dictionary<string, SPRITE_SHEET>();
        private static Dictionary<string, SPRITE_ANIMATION> SpriteAnimationList = new Dictionary<string, SPRITE_ANIMATION>();

        public static void AssignTextureSpriteSheet(string Name, string Category, string Directory, string Filename, bool EnginePath = true)
        {
            FABRICK_TEXTURE_MANAGER.AssignTexture(SPRITE_SHEET_CATEGORY + Category + "." + Name, Directory, Filename, EnginePath);
        }
        public static string GetSpriteTextureName(string Name, string Category)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(SPRITE_SHEET_CATEGORY + Name))
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot find Sprite named: {Name}");
                return "";
            }
            return SPRITE_SHEET_CATEGORY + Category + "." + Name;
        } 

        // Create Source Position and Size in SpriteSheetList
        public static void CreateSpriteSheet(string SpriteSheetName, string Category, string TextureName, Vector2 SourcePos, Vector2 SourceSize)
        {
            string TexSprite = SPRITE_SHEET_CATEGORY + Category + "." + TextureName;
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(TexSprite))
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot Create Sprite Sheet, Cus {SpriteSheetName} didn't exist!");
                return;
            }
            SPRITE_SHEET S = new();
            S.SRC.X = SourcePos.X;
            S.SRC.Y = SourcePos.Y;
            S.SRC.W = SourceSize.X;
            S.SRC.H = SourceSize.Y;
            S.TextureName = TexSprite;

            SpriteSheetList.Add(Category + "." + SpriteSheetName, S);
        }

        // Transform must contain: {Position, Angle, Size, CenterPoint}
        public static void RenderSprite(string SpriteSheetName, string Category, FABRICK_TRANSFORM Transform, float Parallax = 1)
        {
            if (!SpriteSheetList.ContainsKey(Category + "." + SpriteSheetName))
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot Render Sprite Sheet, Cus {Category + "." + SpriteSheetName} didn't exist!");
                return;
            }
            SPRITE_SHEET S = SpriteSheetList[Category + "." + SpriteSheetName];
            Vector2 SRC_POS = new Vector2(S.SRC.X, S.SRC.Y);
            Vector2 SRC_SIZE = new Vector2(S.SRC.W, S.SRC.H);
            //F RenderTexture(string name, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Angle, Vector2 CenterPoint)
            FABRICK_TEXTURE_MANAGER.RenderTexture(S.TextureName, Transform.GetPosition(), Transform.GetSize(), SRC_POS, SRC_SIZE, Transform.GetAngle(), Transform.GetCenterPoint(), Parallax);
        }
        public static void RenderSprite(string FinalSpriteSheetName, FABRICK_TRANSFORM Transform, float Parallax)
        {
            if (!SpriteSheetList.ContainsKey(FinalSpriteSheetName))
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot Render Sprite Sheet, Cus {FinalSpriteSheetName} didn't exist!");
                return;
            }
            SPRITE_SHEET S = SpriteSheetList[FinalSpriteSheetName];
            Vector2 SRC_POS = new Vector2(S.SRC.X, S.SRC.Y);
            Vector2 SRC_SIZE = new Vector2(S.SRC.W, S.SRC.H);
            //F RenderTexture(string name, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Angle, Vector2 CenterPoint)
            FABRICK_TEXTURE_MANAGER.RenderTexture(S.TextureName, Transform.GetPosition(), Transform.GetSize(), SRC_POS, SRC_SIZE, Transform.GetAngle(), Transform.GetCenterPoint(), Parallax);
        }
        public static Dictionary<int, string> AddSpriteSheetSequence(string Name, int Min, int Max)
        {
            Dictionary<int, string> N = new Dictionary<int, string>();
            for (int i = 0; i <= Max; i++)
            {
                string C = Name + i.ToString();
                N.Add(i, C);
            }
            return N;
        }
        public static void CreateSpriteAnimation(string Name, string Category, Dictionary<int, string> SpriteSheet, Vector2 CenterPoint)
        {
            FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Creating new Sprite Animation: {Category + "." + Name}");
            if (SpriteAnimationList.ContainsKey(Category + "." + Name))
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot create new Sprite Animation, cus ts already exist: {Category + "." + Name}");
                return;
            }
            SPRITE_ANIMATION SP = new SPRITE_ANIMATION();
            SP.CenterPoint = CenterPoint;

            for (int i = 0; i < SpriteSheet.Count; i++)
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Add Sprite Sheet: {SpriteSheet[i]}");
                SP.SpriteSheet.Add(i, Category + "." + SpriteSheet[i]);
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Added Sprite Sheet: {SP.SpriteSheet[i]}");
            }

            SpriteAnimationList.Add(Category + "." + Name, SP);
            FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Created new Sprite Animation: {Category + "." + Name}");
        }
        public static PLAY_SPRITE_ANIMATION PlaySpriteAnimation(string Name, string Category, int Frame, out int MaxFrame)
        {
            PLAY_SPRITE_ANIMATION PSA = new PLAY_SPRITE_ANIMATION();
            PSA.SpriteAnimation = "";
            PSA.CenterPoint = Vector2.Zero;

            string ThisAnimation = Category + "." + Name;
            bool Continue = false;

            foreach (string ThisKey in SpriteAnimationList.Keys)
            {
                if (ThisKey == ThisAnimation)
                {
                    Continue = true;
                }
            }
            if (!Continue)
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot play ts sprite animation: {ThisAnimation}, cuz its doesn't exist!!");
                MaxFrame = 0;
                return PSA;
            }

            MaxFrame = SpriteAnimationList[ThisAnimation].SpriteSheet.Count;

            // SPRITE_SHEET_CATEGORY + Category + "." + TextureName
            PSA.SpriteAnimation = SpriteAnimationList[ThisAnimation].SpriteSheet[Frame];
            PSA.CenterPoint = SpriteAnimationList[ThisAnimation].CenterPoint;

            return PSA;
        }
        public static void RenderSpriteAnimation(PLAY_SPRITE_ANIMATION SpriteAnimation, FABRICK_TRANSFORM Transform, float Parallax = 1)
        {
            Transform.SetCenterPoint(SpriteAnimation.CenterPoint);
            RenderSprite(SpriteAnimation.SpriteAnimation, Transform, Parallax);
        }
    }
}
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
    public class PLAY_SPRITE_ANIMATION
    {
        public string SpriteAnimation;
        public Vector2 CenterPoint;
        public PLAY_SPRITE_ANIMATION()
        {
            SpriteAnimation = "";
            CenterPoint = Vector2.Zero;
        }
    }
    public class PLAY_SPRITE_ANIMATION_WORKER
    {
        public int PlayFrame;
        public float AnimTime;
        public bool InUse;
        public PLAY_SPRITE_ANIMATION_WORKER()
        {
            PlayFrame = 0;
            AnimTime = 0f;
            InUse = true;
        }
    }
    public class SPRITE_SHEET_MANAGER
    {
        internal static float DeltaTime = 0;
        private static string SPRITE_SHEET_CATEGORY = "@FABRICK_SPRITE_SHEET.";
        private static Dictionary<string, SPRITE_SHEET> SpriteSheetList = new Dictionary<string, SPRITE_SHEET>();
        private static Dictionary<string, SPRITE_ANIMATION> SpriteAnimationList = new Dictionary<string, SPRITE_ANIMATION>();
        private static Dictionary<string, PLAY_SPRITE_ANIMATION_WORKER> AutoPlaySpriteAnimationWorker = new();

        public static void AssignTextureSpriteSheet(string Name, string Category, string Directory, string Filename, bool EnginePath = true, string FileFormat = "png")
        {
            FABRICK_TEXTURE_MANAGER.AssignTexture(SPRITE_SHEET_CATEGORY + Category + "." + Name, Directory, Filename, EnginePath, FileFormat);
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
        public static void RenderSprite(string SpriteSheetName, string Category, FABRICK_TRANSFORM Transform, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
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
            FABRICK_TEXTURE_MANAGER.RenderTexture(S.TextureName, Transform.GetPosition(), Transform.GetSize(), SRC_POS, SRC_SIZE, Transform.GetAngle(), Transform.GetCenterPoint(), Transparency, isUi, Flip, Parallax);
        }
        public static void RenderSprite(string FinalSpriteSheetName, FABRICK_TRANSFORM Transform, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
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
            FABRICK_TEXTURE_MANAGER.RenderTexture(S.TextureName, Transform.GetActualPosition(), Transform.GetSize(), SRC_POS, SRC_SIZE, Transform.GetAngle(), Transform.GetCenterPoint(), Transparency, isUi, Flip, Parallax);
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
            string ThisAnimation = Category + "." + Name;
            bool Continue = false;

            if (SpriteAnimationList.ContainsKey(ThisAnimation))
            {
                Continue = true;
            }

            if (!Continue)
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Cannot play ts sprite animation: {ThisAnimation}, cuz its doesn't exist!!");
                MaxFrame = 0;
                return new();
            }

            return PlaySpriteAnimation_NoChecking(Name, Category, Frame, out MaxFrame);
        }
        internal static PLAY_SPRITE_ANIMATION PlaySpriteAnimation_NoChecking(string Name, string Category, int Frame, out int MaxFrame)
        {
            string ThisAnimation = Category + "." + Name;

            PLAY_SPRITE_ANIMATION PSA = new PLAY_SPRITE_ANIMATION();
            PSA.SpriteAnimation = "";
            PSA.CenterPoint = Vector2.Zero;

            MaxFrame = SpriteAnimationList[ThisAnimation].SpriteSheet.Count;

            // SPRITE_SHEET_CATEGORY + Category + "." + TextureName
            PSA.SpriteAnimation = SpriteAnimationList[ThisAnimation].SpriteSheet[Frame];
            PSA.CenterPoint = SpriteAnimationList[ThisAnimation].CenterPoint;

            return PSA;
        }

        /*
        FABRICK_ANIMATION_FRAME_STRUCT ThisPlayListStruct = AnimationPlayList[IndexOfUnusedPlayList];

            ThisPlayListStruct.AnimTime += (float)ThisEngine.DeltaTime;
            float TimeFloat = ThisPlayListStruct.AnimTime * TargetFPS;
            ThisPlayListStruct.CurrentFrame = (int)TimeFloat % TotalFrames;

            AnimationPlayList[IndexOfUnusedPlayList] = ThisPlayListStruct;

            return ThisPlayListStruct.CurrentFrame;
        */

        public static PLAY_SPRITE_ANIMATION? AutoPlaySpriteAnimation(string Name, string Category, string KeyWorker, bool Loop = true, int FPS = 24)
        {
            string ThisAnim = Category + "." + Name;
            if (!SpriteAnimationList.ContainsKey(ThisAnim))
            {
                return null;
            }
            // Create Worker
            if (!AutoPlaySpriteAnimationWorker.ContainsKey(KeyWorker))
            {
                AutoPlaySpriteAnimationWorker.Add(KeyWorker, new());
                AutoPlaySpriteAnimationWorker[KeyWorker].InUse = true;
                return null;
            }
            else if(SpriteAnimationList.ContainsKey(Category + "." + Name))
            {
                AutoPlaySpriteAnimationWorker[KeyWorker].InUse = true;
                AutoPlaySpriteAnimationWorker[KeyWorker].AnimTime += DeltaTime;
                float TimeFloat = AutoPlaySpriteAnimationWorker[KeyWorker].AnimTime * FPS;
                AutoPlaySpriteAnimationWorker[KeyWorker].PlayFrame = (int)TimeFloat % SpriteAnimationList[ThisAnim].SpriteSheet.Count;
            }

            // Final Moment
            //FABRICK_DEBUG.Log($"Test Current Frame: {AutoPlaySpriteAnimationWorker[KeyWorker].PlayFrame}");
            return PlaySpriteAnimation_NoChecking(Name, Category, AutoPlaySpriteAnimationWorker[KeyWorker].PlayFrame, out _);
        }
        public static void RenderSpriteAnimation(PLAY_SPRITE_ANIMATION? SpriteAnimation, FABRICK_TRANSFORM Transform, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
        {
            if (SpriteAnimation is null)
            {
                return;
            }
            Transform.SetCenterPoint(SpriteAnimation.CenterPoint.X, SpriteAnimation.CenterPoint.Y);
            RenderSprite(SpriteAnimation.SpriteAnimation, Transform, Transparency, isUi, Flip, Parallax);
        }

        internal static void AutoPlayResetInUseWorker()
        {
            foreach (string ThisKey in AutoPlaySpriteAnimationWorker.Keys)
            {
                AutoPlaySpriteAnimationWorker[ThisKey].InUse = false;
            }
        }
        internal static void AutoPlayCleanWorker()
        {
            List<string> toDelete = new();
            foreach (string ThisKey in AutoPlaySpriteAnimationWorker.Keys)
            {
                if (AutoPlaySpriteAnimationWorker[ThisKey].InUse == false)
                {
                    toDelete.Add(ThisKey);
                }
            }
            foreach (string ThisKey in toDelete)
            {
                FABRICK_DEBUG.Log($"[SPRITE_SHEET_MANAGER] Delete Worker: {ThisKey}");
                AutoPlaySpriteAnimationWorker.Remove(ThisKey);
            }
        }
    }
}
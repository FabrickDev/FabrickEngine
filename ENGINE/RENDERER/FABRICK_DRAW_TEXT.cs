using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public enum Algin
    {
        Left = 0,
        Center = 1,
        Right = 2
    }
    public struct FontData
    {
        public string File;
        public nint Font;
    }
    public class FABRICK_DRAW_TEXT
    {
        private static FABRICK_ENGINE? ThisEngine;
        private static string MainEngineDevAssetsPath = "D:\\FABRICK ENGINE\\FABRICK_ENGINE\\FABRICK_ENGINE\\ENGINE\\DEV_ASSETS\\";
        private static uint FontQuality = 64;
        private static uint FontRenderQuality = 8;
        private static string DefaultFontFileName = "Segoe UI Bold.ttf";
        private static string DefaultFontName = "DefaultFont";
        private static Dictionary<string, FontData> FontList = new();
        
        public static void UpdateEngine(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
        }
        public static void Initialize()
        {
            // Assign Default Font First
            FontData _data;
            _data.File = Path.Combine(MainEngineDevAssetsPath, "FONT", DefaultFontFileName);
            _data.Font = TTF.OpenFont(_data.File, FontQuality);
            FontList[DefaultFontName] = _data;

            return;
        }
        public static void AssignFont(string Name, string FileName, bool UseEngineDataPath = true)
        {
            if (FontList.ContainsKey(Name))
            {
                FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT] Cannot Add Font Named: {Name} That Already Exist!!");
                return;
            }
            if (!File.Exists(FileName))
            {
                FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT] {FileName} Doesn't Exist!!");
                return;
            }
            if (UseEngineDataPath && ThisEngine is not null)
            {
                FileName = Path.Combine(ThisEngine.MainEngineDataPath, "Font", FileName);
            }

            FontData _data;
            _data.File = FileName;
            _data.Font = TTF.OpenFont(_data.File, FontQuality);
            FontList[Name] = _data;
            FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT] Added Font {Name}");
        }
        public static void PrintFontList()
        {
            FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT] All font list:");
            uint Number = 1;
            foreach (string ThisFont in FontList.Keys)
            {
                FABRICK_DEBUG.Log($"{Number}. {ThisFont}");
                Number++;
            }
            FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT] ---------");
        }
        public static void DestroyAllFont()
        {
            foreach (FontData ThisFont in FontList.Values)
            {
                TTF.CloseFont(ThisFont.Font);
            }
            FontList.Clear();
        }
        public static string GetDefaultFontName()
        {
            return DefaultFontName;
        }
        public static void SetFontQuality(uint Quality)
        {
            if (ThisEngine is not null)
            {
                FontQuality = Quality;
                FontRenderQuality = Quality / 8;
                foreach (string ThisFont in FontList.Keys)
                {
                    TTF.CloseFont(FontList[ThisFont].Font);
                    
                    FontData _data;
                    _data.File = FontList[ThisFont].File;
                    _data.Font = TTF.OpenFont(FontList[ThisFont].File, FontQuality);
                    FontList[ThisFont] = _data;
                }
                PrintFontList();
            }
        }
        public static void DrawTextEachLetter(string Text, int Step, string FontName, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 BoundSize, int MaxSentenceEachLine = 0, int ExtraPaddingEachLine = 0, bool IsUI = true)
        {
            BoundSize = Vector2.Zero;
            if (Step <= 0) return;

            string ModifiedText = "";

            Step = FABRICK_MATH.Clamp(Step, 0, Text.Length);

            for (int i = 0; i < Step; i++)
            {
                ModifiedText += Text[i];
            }

            DrawTextAdvanced(ModifiedText, FontName, Pos, Size, Color, Align, out BoundSize, MaxSentenceEachLine, ExtraPaddingEachLine, true, IsUI);
        }
        public static void DrawTextAdvanced(string Text, string FontName, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 SizeBound, int MaxSentenceEachLine = 0, int ExtraPaddingEachLine = 0, bool Render = true, bool IsUI = false)
        {
            if(ThisEngine is null)
            {
                SizeBound = Vector2.Zero;
                return;
            }
            if (!FontList.ContainsKey(FontName))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_DRAW_TEXT] Cannot Draw Text Cuz Theres no This Font: {FontName}");
                SizeBound = Vector2.Zero;
                return;
            }

            // Multiline Handle
            Text = Text.Replace("\r\n", "\n");
            string tempText = "";
            List<string> TextEachLine = new();
            List<Vector2> TextLineTextureSize = new();
            int Iterator = 1;
            bool NextLine = false;
            for (int j = 0; j < Text.Length; j++)
            {
                if (Iterator % (MaxSentenceEachLine) == 0 && MaxSentenceEachLine > 0)
                {
                    NextLine = true;
                }

                if (Text[j] != '\n') //IDK Why this is not working bruh
                {
                    tempText += Text[j];
                }

                if((NextLine && Text[j] == ' ') || Text[j] == '\n')
                {
                    string _temp = "";
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        if (i != 0 || tempText[i] != ' ')
                        {
                            // Do what?
                            _temp += tempText[i];
                        }
                    }
                    TextEachLine.Add(_temp);
                    tempText = "";
                    Iterator = 0;
                    NextLine = false;
                }
                Iterator++;
            }
            TextEachLine.Add(tempText);

            for (int i = 0; i < TextEachLine.Count; i++)
            {
                nint textRender = TTF.RenderTextSolid(FontList[FontName].Font, TextEachLine[i], 0, Color.Get_SDL_Color());
                nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
                SDL.DestroySurface(textRender);
                SDL.GetTextureSize(textTexture, out float W, out float H);
        
                SDL.FRect DRect = new SDL.FRect();
                if (Align == Algin.Center)
                {
                    DRect.W = W * Size / FontRenderQuality;
                    DRect.H = H * Size / FontRenderQuality;
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Left)
                {
                    DRect.W = W * Size / FontRenderQuality;
                    DRect.H = H * Size / FontRenderQuality;
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Right)
                {
                    DRect.W = W * Size / FontRenderQuality;
                    DRect.H = H * Size / FontRenderQuality;
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                }
                if (i != 0)
                {
                    DRect.Y += (DRect.H * i) + ExtraPaddingEachLine;
                    TextLineTextureSize.Add(new Vector2(DRect.W, DRect.H + ExtraPaddingEachLine));
                }
                else
                {
                    TextLineTextureSize.Add(new Vector2(DRect.W, DRect.H));
                }
                if (Render)
                {
                    if (IsUI)
                    {
                        SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, DRect);
                    }
                    else
                    {
                        FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
                        SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetPosition(), T.GetSize());
                        SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, R);
                    }
                }
                SDL.DestroyTexture(textTexture);
            }

            SizeBound = Vector2.Zero;
            for (int i = 0; i < TextLineTextureSize.Count; i++)
            {
                SizeBound = new Vector2(Math.Max(SizeBound.X, TextLineTextureSize[i].X), SizeBound.Y + TextLineTextureSize[i].Y);
            }
        }
        public static void DrawDevText(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align)
        {
            if(ThisEngine is null)
            {
                return;
            }
            nint textRender = TTF.RenderTextSolid(FontList[DefaultFontName].Font, Text, 0, Color.Get_SDL_Color());
            nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
            
            SDL.GetTextureSize(textTexture, out float W, out float H);
            
            SDL.FRect DRect = new SDL.FRect();
            if (Align == Algin.Center)
            {
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
                DRect.X = Pos.X - (DRect.W / 2);
                DRect.Y = Pos.Y;
            }

            if (Align == Algin.Left)
            {
                DRect.X = Pos.X;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
            }

            if (Align == Algin.Right)
            {
                DRect.X = Pos.X - DRect.W;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
            }
            

            SDL.DestroySurface(textRender);
            //SDL.SetRenderDrawColor(Renderer, Color.R, Color.G, Color.B, Color.A);

            FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
            SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetPosition(), T.GetSize());
            SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, R);

            SDL.DestroyTexture(textTexture);
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align)
        {
            DrawDevText_UI(Text, Pos, Size, Color, Align, out _, true);
        }
        public static Vector2 GetSizeDrawDevText_UI(string Text, float Size, Algin Align)
        {
            Vector2 _Size;
            DrawDevText_UI(Text, Vector2.Zero, Size, new FABRICK_COLOR(0, 0, 0, 0), Align, out _Size, false);
            return _Size;
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 SizeBound, bool Render = true)
        {
            if(ThisEngine is null)
            {
                SizeBound = Vector2.Zero;
                return;
            }
            nint textRender = TTF.RenderTextSolid(FontList[DefaultFontName].Font, Text, 0, Color.Get_SDL_Color());
            nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
            
            SDL.GetTextureSize(textTexture, out float W, out float H);
            
            SDL.FRect DRect = new SDL.FRect();
            if (Align == Algin.Center)
            {
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
                DRect.X = Pos.X - (DRect.W / 2);
                DRect.Y = Pos.Y;
            }

            if (Align == Algin.Left)
            {
                DRect.X = Pos.X;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
            }

            if (Align == Algin.Right)
            {
                DRect.X = Pos.X - DRect.W;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / FontRenderQuality;
                DRect.H = H * Size / FontRenderQuality;
            }

            SDL.DestroySurface(textRender);
            SizeBound = new Vector2(DRect.W, DRect.H);
            if (Render)
            {
                SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, DRect);
            }
            SDL.DestroyTexture(textTexture);
        }
        public static void DrawDevFPS(Vector2 Pos, float FontSize)
        {
            if(ThisEngine is null) return;
            FABRICK_COLOR BG_Color = new FABRICK_COLOR(15, 15, 15, 128);
            Vector2 Offsetter = new Vector2(6, 6);
            Vector2 GetTextSize = GetSizeDrawDevText_UI($"FPS: {ThisEngine.FrameRateInt}", FontSize, Algin.Left);
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(Pos, GetTextSize + Offsetter + Offsetter, BG_Color);
            DrawDevText_UI($"FPS: {ThisEngine.FrameRateInt}", Pos + Offsetter, FontSize, FABRICK_COLOR.White(), Algin.Left);
        }
    }
}
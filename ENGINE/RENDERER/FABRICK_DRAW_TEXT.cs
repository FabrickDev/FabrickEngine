using System.Numerics;
using System.Text;
using Fabrick.ENGINE.CORE;
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
    public class FABRICK_DRAW_TEXT
    {
        private static FABRICK_ENGINE? ThisEngine;
        private static nint DefaultFont = TTF.OpenFont("D:\\PROJECT\\BATARA\\FABRICK_ENGINE\\FABRICK_ENGINE\\ENGINE\\DEV_ASSETS\\FONTS\\Segoe UI Bold.ttf", 64);
        private static Dictionary<string, nint> FontList = new Dictionary<string, nint>();
        
        public static void UpdateEngine(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
        }
        public static void Initialize()
        {
           return;
        }
        public static void AssignFont(string Name, string Path)
        {
            
        }
        public static void DestroyAllFont()
        {
            TTF.CloseFont(DefaultFont);
        }
        public static void DrawDevText(string Text, Vector2 Pos, float Size, Color Color, Algin Align)
        {
            if(ThisEngine is null)
            {
                return;
            }
            nint textRender = TTF.RenderTextSolid(DefaultFont, Text, 0, Color.Get_SDL_Color());
            nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
            
            SDL.GetTextureSize(textTexture, out float W, out float H);
            
            SDL.FRect DRect = new SDL.FRect();
            if (Align == Algin.Center)
            {
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
                DRect.X = Pos.X - (DRect.W / 2);
                DRect.Y = Pos.Y;
            }

            if (Align == Algin.Left)
            {
                DRect.X = Pos.X;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
            }

            if (Align == Algin.Right)
            {
                DRect.X = Pos.X - DRect.W;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
            }
            
            FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));

            SDL.DestroySurface(textRender);
            //SDL.SetRenderDrawColor(Renderer, Color.R, Color.G, Color.B, Color.A);

            SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetPosition(), T.GetSize());

            SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, R);
            SDL.DestroyTexture(textTexture);
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, Color Color, Algin Align)
        {
            DrawDevText_UI(Text, Pos, Size, Color, Align, out _, true);
        }
        public static Vector2 GetSizeDrawDevText_UI(string Text, float Size, Algin Align)
        {
            Vector2 _Size;
            DrawDevText_UI(Text, Vector2.Zero, Size, new Color(0, 0, 0, 0), Align, out _Size, false);
            return _Size;
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, Color Color, Algin Align, out Vector2 SizeBound, bool Render = true)
        {
            if(ThisEngine is null)
            {
                SizeBound = Vector2.Zero;
                return;
            }
            nint textRender = TTF.RenderTextSolid(DefaultFont, Text, 0, Color.Get_SDL_Color());
            nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
            
            SDL.GetTextureSize(textTexture, out float W, out float H);
            
            SDL.FRect DRect = new SDL.FRect();
            if (Align == Algin.Center)
            {
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
                DRect.X = Pos.X - (DRect.W / 2);
                DRect.Y = Pos.Y;
            }

            if (Align == Algin.Left)
            {
                DRect.X = Pos.X;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
            }

            if (Align == Algin.Right)
            {
                DRect.X = Pos.X - DRect.W;
                DRect.Y = Pos.Y;
                DRect.W = W * Size / 8;
                DRect.H = H * Size / 8;
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
            Color BG_Color = new Color(15, 15, 15, 128);
            Vector2 Offsetter = new Vector2(6, 6);
            Vector2 GetTextSize = GetSizeDrawDevText_UI($"FPS: {ThisEngine.FrameRateInt}", FontSize, Algin.Left);
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(Pos, GetTextSize + Offsetter + Offsetter, BG_Color);
            DrawDevText_UI($"FPS: {ThisEngine.FrameRateInt}", Pos + Offsetter, FontSize, Color.White(), Algin.Left);
        }
    }
}
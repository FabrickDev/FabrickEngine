using System.Numerics;
using System.Security.AccessControl;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public class FABRICK_DRAW_SHAPE
    {
        private static FABRICK_ENGINE? ThisEngine;
        public static void Update(FABRICK_ENGINE Engine)
        {
            ThisEngine = Engine;
        }
        public static void DrawLine(Vector2 Point1, Vector2 Point2, Color color)
        {
            
        }
        public static SDL.FRect MakeRect(int X, int Y, int W, int H)
        {
            SDL.FRect _r;
            _r.X = X;
            _r.Y = Y;
            _r.W = W;
            _r.H = H;
            return _r;
        }
        public static SDL.FRect MakeRect(Box Box)
        {
            SDL.FRect r = new SDL.FRect();
            r.X = Box.GetPos().X;
            r.Y = Box.GetPos().Y;
            r.W = Box.GetSize().X;
            r.H = Box.GetSize().Y;
            return r;
        }
        public static SDL.FRect MakeRect(Vector2 Pos, Vector2 Size)
        {
            return MakeRect((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
        }
        public static void DrawFillRect(Vector2 Pos, Vector2 Size, Color Color, float Parallax = 1)
        {
            FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(Pos, Size);

            SDL.FRect R = MakeRect(T.GetPosition(), T.GetSize());
            float X1 = T.GetPosition().X;
            float Y1 = T.GetPosition().Y;
            float X2 = T.GetSize().X;
            float Y2 = T.GetSize().Y;
            float SIN = T.GetSin();
            float COS = T.GetCos();

            SDL.Vertex[] Vert = new SDL.Vertex[4];
            // Top Left
            Vert[0].Position.X = X1;
            Vert[0].Position.Y = Y1;
            // Top Right
            Vert[1].Position.X = X1 + X2 * COS;
            Vert[1].Position.Y = Y1 + X2 * SIN;
            // Bottom Right
            Vert[2].Position.X = X1 + X2 * COS - Y2 * SIN;
            Vert[2].Position.Y = Y1 + X2 * SIN + Y2 * COS;
            // Bottom Left
            Vert[3].Position.X = X1 - Y2 * SIN;
            Vert[3].Position.Y = Y1 + Y2 * COS;

            for (int i = 0; i < 4; i++)
            {
                Vert[i].Color = Color.Get_SDL_FColor();
            }

            int[] Indices = {0, 1, 2, 0, 2, 3};
            
            if(ThisEngine is not null)
            {
                SDL.RenderGeometry(ThisEngine.GetRenderer(), IntPtr.Zero, Vert, 4, Indices, 6);
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_DRAW_SHAPE] Error Draw Fill Rect, Engine is null!!");
            }
        }
        public static void DrawRect(Vector2 Pos, Vector2 Size, Color Color, float Parallax = 1)
        {
            FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(Pos, Size);

            SDL.FRect R = MakeRect(T.GetPosition(), T.GetSize());
            float X1 = T.GetPosition().X;
            float Y1 = T.GetPosition().Y;
            float X2 = T.GetSize().X;
            float Y2 = T.GetSize().Y;
            float SIN = T.GetSin();
            float COS = T.GetCos();

            SDL.Vertex[] Vert = new SDL.Vertex[4];
            // Top Left
            Vert[0].Position.X = X1;
            Vert[0].Position.Y = Y1;
            // Top Right
            Vert[1].Position.X = X1 + X2 * COS;
            Vert[1].Position.Y = Y1 + X2 * SIN;
            // Bottom Right
            Vert[2].Position.X = X1 + X2 * COS - Y2 * SIN;
            Vert[2].Position.Y = Y1 + X2 * SIN + Y2 * COS;
            // Bottom Left
            Vert[3].Position.X = X1 - Y2 * SIN;
            Vert[3].Position.Y = Y1 + Y2 * COS;

            if (ThisEngine is not null)
            {
                SDL.SetRenderDrawColor(ThisEngine.GetRenderer(), Color.R, Color.G, Color.B, Color.A);
                SDL.RenderLine(ThisEngine.GetRenderer(), Vert[0].Position.X, Vert[0].Position.Y, Vert[1].Position.X, Vert[1].Position.Y);
                SDL.RenderLine(ThisEngine.GetRenderer(), Vert[1].Position.X, Vert[1].Position.Y, Vert[2].Position.X, Vert[2].Position.Y);
                SDL.RenderLine(ThisEngine.GetRenderer(), Vert[2].Position.X, Vert[2].Position.Y, Vert[3].Position.X, Vert[3].Position.Y);
                SDL.RenderLine(ThisEngine.GetRenderer(), Vert[3].Position.X, Vert[3].Position.Y, Vert[0].Position.X, Vert[0].Position.Y);
            }
        }
        public static void DrawCircle(Vector2 Pos, float Radius, Color Color)
        {
            if (ThisEngine is not null)
            {
                FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(Pos, new Vector2(Radius));
                Pos = T.GetPosition();
                Radius = T.GetSize().X;

                nint renderer = ThisEngine.GetRenderer();
                float PosX = Pos.X;
                float PosY = Pos.Y;
                float Diameter = Radius * 2;
                float x = Radius - 1;
                float y = 0;
                float tx = 1;
                float ty = 1;
                float error = tx - Diameter;

                SDL.SetRenderDrawColor(renderer, Color.R, Color.G, Color.B, Color.A);
                while (x >= y)
                {
                    SDL.RenderPoint(renderer, PosX + x, PosY - y);
                    SDL.RenderPoint(renderer, PosX + x, PosY + y);
                    SDL.RenderPoint(renderer, PosX - x, PosY - y);
                    SDL.RenderPoint(renderer, PosX - x, PosY + y);
                    SDL.RenderPoint(renderer, PosX + y, PosY - x);
                    SDL.RenderPoint(renderer, PosX + y, PosY + x);
                    SDL.RenderPoint(renderer, PosX - y, PosY - x);
                    SDL.RenderPoint(renderer, PosX - y, PosY + x);

                    if (error <= 0)
                    {
                        y++;
                        error += ty;
                        ty += 2;
                    }
                    if (error > 0)
                    {
                        x--;
                        tx += 2;
                        error += tx - Diameter;
                    }
                }
            }
        }
        public static void DrawFillTriangle(Vector2 Pos, float Radius, float Angle, Color Color, bool WorldToScreen = true)
        {
            if (ThisEngine is not null)
            {
                if (WorldToScreen)
                {
                    FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(Pos, new Vector2(Radius), Angle);
                    Pos = T.GetPosition();
                    Radius = T.GetSize().X;
                    Angle = T.GetAngle();
                }

                SDL.Vertex[] Vert = new SDL.Vertex[3];
                Vert[0].Position.X = Pos.X + Radius * MathF.Cos(FABRICK_MATH.AngleToRad(0 + Angle));
                Vert[0].Position.Y = Pos.Y + Radius * MathF.Sin(FABRICK_MATH.AngleToRad(0 + Angle));
                Vert[1].Position.X = Pos.X + Radius * MathF.Cos(FABRICK_MATH.AngleToRad(120 + Angle));
                Vert[1].Position.Y = Pos.Y + Radius * MathF.Sin(FABRICK_MATH.AngleToRad(120 + Angle));
                Vert[2].Position.X = Pos.X + Radius * MathF.Cos(FABRICK_MATH.AngleToRad(240 + Angle));
                Vert[2].Position.Y = Pos.Y + Radius * MathF.Sin(FABRICK_MATH.AngleToRad(240 + Angle));
                for (int i = 0; i < 3; i++)
                {
                    Vert[i].Color = Color.Get_SDL_FColor();
                }
                SDL.RenderGeometry(ThisEngine.GetRenderer(), IntPtr.Zero, Vert, 3, IntPtr.Zero, 0);
            }
        }
        public static void DrawFillTriangle_UI(Vector2 Pos, float Radius, float Angle, Color Color)
        {
            DrawFillTriangle(Pos, Radius, Angle, Color, false);
        }
        
        public static void DrawFillRect_UI(Vector2 Pos, Vector2 Size, Color Color)
        {
           DrawRect_UI(Pos, Size, Color, true);
        }
        public static void DrawRect_UI(Vector2 Pos, Vector2 Size, Color Color, bool Filled = false)
        {
            float X1 = Pos.X;
            float Y1 = Pos.Y;
            float X2 = Size.X;
            float Y2 = Size.Y;
            float SIN = 0;
            float COS = 1;

            SDL.Vertex[] Vert = new SDL.Vertex[4];
            // Top Left
            Vert[0].Position.X = X1;
            Vert[0].Position.Y = Y1;
            // Top Right
            Vert[1].Position.X = X1 + X2 * COS;
            Vert[1].Position.Y = Y1 + X2 * SIN;
            // Bottom Right
            Vert[2].Position.X = X1 + X2 * COS - Y2 * SIN;
            Vert[2].Position.Y = Y1 + X2 * SIN + Y2 * COS;
            // Bottom Left
            Vert[3].Position.X = X1 - Y2 * SIN;
            Vert[3].Position.Y = Y1 + Y2 * COS;

            for (int i = 0; i < 4; i++)
            {
                Vert[i].Color = Color.Get_SDL_FColor();
            }

            int[] Indices = {0, 1, 2, 0, 2, 3};
            
            if(ThisEngine is not null)
            {
                if (Filled)
                {
                    SDL.RenderGeometry(ThisEngine.GetRenderer(), IntPtr.Zero, Vert, 4, Indices, 6);
                }
                else
                {
                    SDL.SetRenderDrawColor(ThisEngine.GetRenderer(), Color.R, Color.G, Color.B, Color.A);
                    SDL.RenderLine(ThisEngine.GetRenderer(), Vert[0].Position.X, Vert[0].Position.Y, Vert[1].Position.X, Vert[1].Position.Y);
                    SDL.RenderLine(ThisEngine.GetRenderer(), Vert[1].Position.X, Vert[1].Position.Y, Vert[2].Position.X, Vert[2].Position.Y);
                    SDL.RenderLine(ThisEngine.GetRenderer(), Vert[2].Position.X, Vert[2].Position.Y, Vert[3].Position.X, Vert[3].Position.Y);
                    SDL.RenderLine(ThisEngine.GetRenderer(), Vert[3].Position.X, Vert[3].Position.Y, Vert[0].Position.X, Vert[0].Position.Y);
                }
                
            }
        }
        public static void DrawCircle_UI(Vector2 Pos, float Radius, Color Color)
        {
            if (ThisEngine is not null)
            {   
                nint renderer = ThisEngine.GetRenderer();
                float PosX = Pos.X;
                float PosY = Pos.Y;
                float Diameter = Radius * 2;
                float x = Radius - 1;
                float y = 0;
                float tx = 1;
                float ty = 1;
                float error = tx - Diameter;

                SDL.SetRenderDrawColor(renderer, Color.R, Color.G, Color.B, Color.A);
                while (x >= y)
                {
                    SDL.RenderPoint(renderer, PosX + x, PosY - y);
                    SDL.RenderPoint(renderer, PosX + x, PosY + y);
                    SDL.RenderPoint(renderer, PosX - x, PosY - y);
                    SDL.RenderPoint(renderer, PosX - x, PosY + y);
                    SDL.RenderPoint(renderer, PosX + y, PosY - x);
                    SDL.RenderPoint(renderer, PosX + y, PosY + x);
                    SDL.RenderPoint(renderer, PosX - y, PosY - x);
                    SDL.RenderPoint(renderer, PosX - y, PosY + x);

                    if (error <= 0)
                    {
                        y++;
                        error += ty;
                        ty += 2;
                    }
                    if (error > 0)
                    {
                        x--;
                        tx += 2;
                        error += tx - Diameter;
                    }
                }
            }
        }
    }
}
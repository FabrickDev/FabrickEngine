using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.MATH;
using SDL3;
using System.Numerics;

/*
References:
https://stackoverflow.com/questions/38334081/how-to-draw-circles-arcs-and-vector-graphics-in-sdl
*/

namespace Fabrick.ENGINE.RENDERER
{
    public class FABRICK_VISUALIZE_SHAPE
    {
        public static nint renderer;
        public static void Visualize(Line Line, Color Color)
        {
            SDL.SetRenderDrawColor(renderer, Color.R, Color.G, Color.B, Color.A);
            SDL.RenderLine(renderer, Line.GetPoint(0).X, Line.GetPoint(0).Y, Line.GetPoint(1).X, Line.GetPoint(1).Y);
        }
        public static void Visualize(Circle Circle, Color Color)
        {
            FABRICK_DRAW_SHAPE.DrawCircle(Circle.GetPos(), Circle.GetRadius(), Color);;
        }
        public static void Visualize(Box Box, Color Color, bool Fill = true)
        {
            Visualize(Box, Color, Fill, false);
        }
        public static void Visualize(Box Box, Color Color, bool Fill = true, bool OffsetPoint = false)
        {
            Vector2 Pos = new Vector2(Box.GetActualPos().X, Box.GetActualPos().Y);
            Vector2 Size = new Vector2(Box.GetSize().X, Box.GetSize().Y);
            if (Fill)
            {
                FABRICK_DRAW_SHAPE.DrawFillRect(Pos, Size, Color);
            }
            else
            {
                FABRICK_DRAW_SHAPE.DrawRect(Pos, Size, Color);
            }
            if (OffsetPoint)
            {
                Vector2 PosP = new Vector2(Box.GetActualPos().X + Box.GetOffset().X - 5, Box.GetActualPos().Y + Box.GetOffset().Y - 5);
                Vector2 SizeP = new Vector2(10, 10);
                FABRICK_DRAW_SHAPE.DrawFillRect(PosP, SizeP, Color.Black());
            }
            return;
        }

        public static void VisualizeNormal(Vector2 Pos, Vector2 Normal, float Length, Color Color)
        {
            float Degree = MathF.Atan2(Normal.X, Normal.Y);

            Vector2 PosA = new Vector2(-Length / 2, 0);
            Vector2 PosB = new Vector2(Length / 2, 0);

            float _PosAX = PosA.X * MathF.Cos(Degree) + PosA.Y * MathF.Sin(Degree);
            float _PosAY = -PosA.X * MathF.Sin(Degree) + PosA.Y * MathF.Cos(Degree);

            float _PosBX = PosB.X * MathF.Cos(Degree) + PosB.Y * MathF.Sin(Degree);
            float _PosBY = -PosB.X * MathF.Sin(Degree) + PosB.Y * MathF.Cos(Degree);

            SDL.SetRenderDrawColor(renderer, Color.R, Color.G, Color.B, Color.A);
            SDL.RenderLine(renderer, _PosAX + Pos.X, _PosAY + Pos.Y, _PosBX + Pos.X, _PosBY + Pos.Y);
        }
        public static void VisualizeNormal(Vector2 Pos, Vector2 Normal, float Length, Color Color1, Color Color2)
        {
            VisualizeNormal(Pos, Normal, Length, Color1);
            
            Vector2 PosA = Pos + Normal * Length;

            SDL.SetRenderDrawColor(renderer, Color2.R, Color2.G, Color2.B, Color2.A);
            SDL.RenderLine(renderer, Pos.X, Pos.Y, PosA.X, PosA.Y);

        }
    }
    
}
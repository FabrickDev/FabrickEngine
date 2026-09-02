using SDL3;

namespace Fabrick.ENGINE.MATH
{
    public class Color
    {
        public byte R, G, B, A;
        public Color(byte R, byte G, byte B, byte A)
        {
            this.A = A;
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public SDL.Color Get_SDL_Color()
        {
            SDL.Color color;
            color.R = R;
            color.G = G;
            color.B = B;
            color.A = A;
            return color;
        }
        public SDL.FColor Get_SDL_FColor()
        {
            SDL.FColor color = new();
            color.R = R / 255f;
            color.G = G / 255f;
            color.B = B / 255f;
            color.A = A / 255f;
            return color;
        }
        public static Color Red()
        {
            return new Color(255, 0, 0, 255);
        }
        public static Color Green()
        {
            return new Color(0, 255, 0, 255);
        }
        public static Color Orange()
        {
            return new Color(255, 156, 0, 255);
        }
        public static Color Blue()
        {
            return new Color(0, 0, 255, 255);
        }
        public static Color Black()
        {
            return new Color(0, 0, 0, 255);
        }
        public static Color White()
        {
            return new Color(255, 255, 255, 255);
        }
    }
}
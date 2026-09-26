using SDL3;

namespace Fabrick.ENGINE.MATH
{
    public class FABRICK_COLOR
    {
        public byte R, G, B, A;
        public FABRICK_COLOR(byte R, byte G, byte B, byte A)
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
        public static FABRICK_COLOR Red()
        {
            return new FABRICK_COLOR(255, 0, 0, 255);
        }
        public static FABRICK_COLOR Green()
        {
            return new FABRICK_COLOR(0, 255, 0, 255);
        }
        public static FABRICK_COLOR Orange()
        {
            return new FABRICK_COLOR(255, 156, 0, 255);
        }
        public static FABRICK_COLOR Blue()
        {
            return new FABRICK_COLOR(0, 0, 255, 255);
        }
        public static FABRICK_COLOR Black()
        {
            return new FABRICK_COLOR(0, 0, 0, 255);
        }
        public static FABRICK_COLOR White()
        {
            return new FABRICK_COLOR(255, 255, 255, 255);
        }
    }
}
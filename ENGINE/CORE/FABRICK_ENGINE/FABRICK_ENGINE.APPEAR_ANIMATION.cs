using System.Numerics;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;
using SDL3;

namespace Fabrick.ENGINE.CORE
{
    public partial class FABRICK_ENGINE
    {
        // Appear Fade Animation
        private bool UseOutro = false;
        private string OutroText = "";
        private FABRICK_COLOR AnimBGColor = new(0, 0, 0, 0);
        private float WindowTransparencyAnimation = -1f;
        private float OutroAnimation = 0f;
        private Vector2 TextSize = Vector2.Zero;

        // Animation Appear Setting
        public void UseOutroText(string Text, FABRICK_COLOR BackgroundColor)
        {
            if(Text.Length == 0) return;
            if(Text.Length > 0) UseOutro = true;
            OutroText = Text;
            AnimBGColor = BackgroundColor;
        }
        private void AppearHandle()
        {
            if(WindowTransparencyAnimation >= 1f || EngineWantToExit)
            {
                return;
            }
            if (WindowTransparencyAnimation > 1f)
            {
                WindowTransparencyAnimation = 1f;
            }
            WindowTransparencyAnimation += DeltaTime;
            if(WindowTransparencyAnimation < 0) return;
            SDL.SetWindowOpacity(window, WindowTransparencyAnimation);
        }
        private void DisappearHandle()
        {
            if (EngineWantToExit)
            {
                if (WindowTransparencyAnimation < 0f)
                {
                    EngineRunning = false;
                }
                if (UseOutro)
                {
                    OutroAnimation += DeltaTime;
                    int Anim = Convert.ToInt16(OutroAnimation * 255f);
                    AnimBGColor = new(AnimBGColor.R, AnimBGColor.G, AnimBGColor.B, Convert.ToByte(FABRICK_MATH.Clamp(Anim, 0, 255)));
                    FABRICK_COLOR _TextColor = new(DefaultLogicalBackgroundColor.R, DefaultLogicalBackgroundColor.G, DefaultLogicalBackgroundColor.B, Convert.ToByte(FABRICK_MATH.Clamp(Anim, 0, 255)));
                    FABRICK_DRAW_SHAPE.DrawFillRect_UI(Vector2.Zero, LogicalResolution, AnimBGColor);
                    FABRICK_DRAW_TEXT.DrawTextAdvanced(OutroText, FABRICK_DRAW_TEXT.DefaultFontName, (LogicalResolution / 2) - new Vector2(0, TextSize.Y / 2), 12f, _TextColor, Algin.Center, out TextSize, 0, 0, true, true);
                }
                if(UseOutro && OutroAnimation < 1.25f) return;
                WindowTransparencyAnimation -= DeltaTime;
                SDL.SetWindowOpacity(window, WindowTransparencyAnimation);
            }
        }
    }
}
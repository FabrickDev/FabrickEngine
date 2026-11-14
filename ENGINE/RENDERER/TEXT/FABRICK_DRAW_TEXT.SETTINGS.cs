using Fabrick.ENGINE.DEBUG;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public partial class FABRICK_DRAW_TEXT
    {
        public static void SetFontQuality(string FontName, uint Quality)
        {
            if (ThisEngine is not null)
            {
                if (!FontList.ContainsKey(FontName))
                {
                    FABRICK_DEBUG.SimpleMessageError("[FABRICK_DRAW_TEXT] Error Cannot Set Quality font: " + FontName);
                    return;
                }
                TTF.CloseFont(FontList[FontName].Font);
                FontData _data = new()
                {
                    Quality = Quality,
                    Renderquality = Quality / 8
                };
                _data.File = FontList[FontName].File;
                _data.Font = TTF.OpenFont(FontList[FontName].File, _data.Quality);
                FontList[FontName] = _data;
            }
        }
        public static void SetFontQuality(uint Quality)
        {
            if (ThisEngine is not null)
            {
                foreach (string ThisFont in FontList.Keys)
                {
                    TTF.CloseFont(FontList[ThisFont].Font);
                    FontData _data = new()
                    {
                        Quality = Quality,
                        Renderquality = Quality / 8
                    };
                    _data.File = FontList[ThisFont].File;
                    _data.Font = TTF.OpenFont(FontList[ThisFont].File, _data.Quality);
                    FontList[ThisFont] = _data;
                }

                PrintFontList();
            }
        }
    }
}
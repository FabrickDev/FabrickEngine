using System.ComponentModel;
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
    public class FontData
    {
        public uint Quality = 64;
        public uint Renderquality = 8;
        public string File = "";
        public nint Font;
    }
    public class TextTextureCache
    {
        public nint Texture;
        public float W;
        public float H;
        public Vector2 SizeBound;
    }
    public class CacheTextSetting
    {
        public string Text = "";
        public string FontName = "";
        public float Size;
        public FABRICK_COLOR Color = new(0, 0, 0, 0);
    }
    public partial class FABRICK_DRAW_TEXT
    {
        private static FABRICK_ENGINE? ThisEngine;
        private static string MainEngineDevAssetsPath = "D:\\FABRICK ENGINE\\FABRICK_ENGINE\\FABRICK_ENGINE\\ENGINE\\DEV_ASSETS\\";
        private static string DefaultFontFileName = "Segoe UI Bold.ttf";
        public static string DefaultFontName = "DefaultFont";
        private static Dictionary<string, FontData> FontList = new();
        private static Dictionary<string, TextTextureCache> TextTextureCacheList = new();
        
        public static void UpdateEngine(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
        }
        public static void Initialize()
        {
            // Assign Default Font First
            FontData _data = new();
            _data.File = Path.Combine(MainEngineDevAssetsPath, "FONT", DefaultFontFileName);
            _data.Font = TTF.OpenFont(_data.File, _data.Quality);
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
                FileName = Path.Combine(ThisEngine.AssetsDataPath, "Font", FileName);
            }

            FontData _data = new();
            _data.File = FileName;
            _data.Font = TTF.OpenFont(_data.File, _data.Quality);
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
        public static string GetDefaultFontName()
        {
            return DefaultFontName;
        }
    }
}
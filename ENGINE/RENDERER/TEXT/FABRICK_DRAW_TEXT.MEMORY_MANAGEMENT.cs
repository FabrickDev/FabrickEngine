using Fabrick.ENGINE.DEBUG;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public partial class FABRICK_DRAW_TEXT
    {
        private static int CurrentRenderSizeX, CurrentRenderSizeY = 0;
        internal static void OnChangeRenderResolutionHandle()
        {
            if (ThisEngine is null) return;
            SDL.GetCurrentRenderOutputSize(ThisEngine.GetRenderer(), out int W, out int H);
            if (W == CurrentRenderSizeX && H == CurrentRenderSizeY)
            {
                return;
            }
            FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] W: {W}, H: {H}");
            DestroyAllCache();
            CurrentRenderSizeX = W;
            CurrentRenderSizeY = H;
        }
        public static void DestroyAllFont()
        {
            foreach (FontData ThisFont in FontList.Values)
            {
                TTF.CloseFont(ThisFont.Font);
            }
            FontList.Clear();
            DestroyAllCache();
        }
        internal static void DestroyAllCache()
        {
            foreach (TextTextureCache ThisTexture in TextTextureCacheList.Values)
            {
                FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Destroy Cache: {ThisTexture.Texture}");
                SDL.DestroyTexture(ThisTexture.Texture);
            }
            TextTextureCacheList.Clear();
        }
        internal static string CacheConvertFormat(CacheTextSetting CacheSetting)
        {
            return $"{CacheSetting.Text}{CacheSetting.FontName}{CacheSetting.Size}{CacheSetting.Color.R}{CacheSetting.Color.G}{CacheSetting.Color.B}";
        }
        internal static void RemoveCache(CacheTextSetting CacheSetting)
        {
            if (!TextTextureCacheList.ContainsKey(CacheConvertFormat(CacheSetting)))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_DRAW_TEXT] Cannot Remove Cache Text: {CacheSetting.Text}, Font: {CacheSetting.FontName}, Size: {CacheSetting} Cuz Didn't Exist");
                return;
            }
            SDL.DestroyTexture(TextTextureCacheList[CacheConvertFormat(CacheSetting)].Texture);
            TextTextureCacheList.Remove(CacheConvertFormat(CacheSetting));
        }
        internal static void RemoveCacheByCategory(string Category)
        {
            FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Removing Cache by Category: {Category}");
            foreach (string ThisKey in TextTextureCacheList.Keys.Where(x => x.StartsWith(Category)).ToList())
            {
                FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Remove Cache: {ThisKey}");
                SDL.DestroyTexture(TextTextureCacheList[ThisKey].Texture);
                TextTextureCacheList.Remove(ThisKey);
            }
        }
    }
}
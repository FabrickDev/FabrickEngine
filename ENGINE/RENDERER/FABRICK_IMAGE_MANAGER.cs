using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using SDL3;
namespace Fabrick.ENGINE.RENDERER
{
    public struct TextureData
    {
        public nint Texture;
        public SDL.FRect SRC, DST;
    }
    public class FABRICK_TEXTURE_MANAGER : FABRICK_DRAW_TEXTURE
    {
        // Save Textures Here
        public static Dictionary<string, TextureData> TextureDataList = new Dictionary<string, TextureData>();
        public static Dictionary<string, TextureData> DevTextureDataList = new Dictionary<string, TextureData>();

        public static void AssignTexture(string name, string Directory, string filename, bool EnginePath = true)
        {
            if (TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.Log($"[TEXTURE_MANAGER] Error cannot add texture name: {name} cus it already exist!!");
                return;
            }
            string PT = "";
            if (EnginePath)
            {
                PT = Path.Combine(FABRICK_ENGINE.MainEngineDataPath, Directory, filename);
            }
            else
            {
                PT = Path.Combine(Directory, filename);
            }
            if (!File.Exists(PT))
            {
                FABRICK_DEBUG.Log($"[TEXTURE_MANAGER] Error couldn't find file: {PT}");
                return;
            }
            nint Surf = SDL.LoadSurface(PT);
            nint Tex = SDL.CreateTextureFromSurface(renderer, Surf);
            SDL.DestroySurface(Surf);

            TextureData _data = new TextureData();
            _data.Texture = Tex;

            if(!SDL.GetTextureSize(_data.Texture, out float _W, out float _H))
            {
                FABRICK_DEBUG.SimpleMessageError($"[TEXTURE_MANAGER] Error Get Texture Size: {SDL.GetError()}");
            }

            SDL.FRect _SRC, _DST;
            _SRC.X = 0;
            _SRC.Y = 0;
            _SRC.W = _W;
            _SRC.H = _H;

            _DST.X = 0;
            _DST.Y = 0;
            _DST.W = _W;
            _DST.H = _H;

            _data.SRC = _SRC;
            _data.DST = _DST;

            FABRICK_TEXTURE_MANAGER.TextureDataList[name] = _data;
        }
        public static void UnloadAllTexture()
        {
            if (FABRICK_TEXTURE_MANAGER.TextureDataList.Count == 0)
            {
                return;
            }
            SDL.Log("[TEXTURE_MANAGER] Unloading All Texture...");

            Dictionary<string, TextureData>.ValueCollection AllTexture = TextureDataList.Values;
            List<TextureData> TextureList = new List<TextureData>(AllTexture);
            foreach (TextureData ThisTexture in TextureList)
            {
                SDL.DestroyTexture(ThisTexture.Texture);
            }
        }
        public static void DevUnloadAllTexture()
        {
            if (FABRICK_TEXTURE_MANAGER.DevTextureDataList.Count == 0)
            {
                return;
            }
            SDL.Log("[TEXTURE_MANAGER] Unloading All Dev Texture...");

            Dictionary<string, TextureData>.ValueCollection AllTexture = FABRICK_TEXTURE_MANAGER.DevTextureDataList.Values;
            List<TextureData> TextureList = new List<TextureData>(AllTexture);
            foreach (TextureData ThisTexture in TextureList)
            {
                SDL.DestroyTexture(ThisTexture.Texture);
            }
        }
    }
}
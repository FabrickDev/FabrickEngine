/*
From AI:
Purpose: Load image or any file from memory (Cuz this engine load from FabrickPacker)

nint WROPS = SDL.IOFromFile("Path", "rb");
// Offset By 1024 Byte
SDL.SeekIO(WROPS, 1024, 0);
nint Surface = SDL.LoadPNGIO(WROPS, false);
*/

using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.IOFILES;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public struct TextureData
    {
        public nint Texture;
        public SDL.FRect SRC, DST;
        public string FileName;
        public string Directory;
    }
    public struct TextureAssetsData
    {
        public string Category;
        public string FileName;
    }
    
    public class FABRICK_TEXTURE_MANAGER : FABRICK_DRAW_TEXTURE
    {
        // Save Textures Here
        internal static string TextureDefaultDirectory = "Image";
        public static List<TextureAssetsData> TextureAssetsList = new();
        public static Dictionary<string, TextureData> TextureDataList = new();
        public static Dictionary<string, TextureData> DevTextureDataList = new();

        internal static void Initialize()
        {
            List<string> _temp = FABRICK_IOFILES_MANAGER.GetFiles("Image");
            foreach (string ThisFile in _temp)
            {
                TextureAssetsData _texass = new();
                int index = ThisFile.IndexOf("\\");
                _texass.Category = index >= 0 ? ThisFile.Substring(0, index) : "Default";
                _texass.FileName = ThisFile.Replace(_texass.Category, "").Replace("\\", "");
                TextureAssetsList.Add(_texass);
            }
            FABRICK_DEBUG.Log("[FABRICK_TEXTURE_MANAGER] Textures List:");
            for (int i = 0; i < TextureAssetsList.Count(); i++)
            {
                FABRICK_DEBUG.Log($"{i + 1}. {TextureAssetsList[i].Category} | {TextureAssetsList[i].FileName}");
            }
        }
        public static void AssignTexture(string Name, string Category, string Format = "png")
        {
            AssignTexture(Category + "." +Name, Category, Name, true, Format);
        }
        public static void AssignTexture(string name, string Directory, string filename, bool EnginePath = true, string FileFormat = "png")
        {
            if (TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.Log($"[FABRICK_TEXTURE_MANAGER] Error cannot add texture name: {name} cus it already exist!!");
                return;
            }
            string PT = "";
            filename = filename + "." + FileFormat;
            if (EnginePath)
            {
                PT = Path.Combine(EngineDataPath, TextureDefaultDirectory, Directory, filename);
            }
            else
            {
                PT = Path.Combine(Directory, filename);
            }
            if (!File.Exists(PT))
            {
                FABRICK_DEBUG.Log($"[FABRICK_TEXTURE_MANAGER] Error couldn't find file: {PT}");
                return;
            }

            nint Surf = SDL.LoadSurface(PT);
            nint Tex = SDL.CreateTextureFromSurface(renderer, Surf);
            SDL.DestroySurface(Surf);

            TextureData _data = new TextureData();
            _data.Texture = Tex;
            _data.FileName = filename;
            _data.Directory = Directory;

            if(!SDL.GetTextureSize(_data.Texture, out float _W, out float _H))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_TEXTURE_MANAGER] Error Get Texture Size: {SDL.GetError()}");
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
        public static void UnloadTexture(string TextureName)
        {
            if (TextureDataList.ContainsKey(TextureName))
            {
                FABRICK_DEBUG.Log($"Unloading: {TextureName}");
                SDL.DestroyTexture(TextureDataList[TextureName].Texture);
                TextureDataList.Remove(TextureName);
            }
            else
            {
                FABRICK_DEBUG.Log($"[FABRICK_TEXTURE_MANAGER] Texture Named: {TextureName} Doesn't Exist!!");
                return;
            }
        }
        public static void UnloadTextureByList(List<string> TextureNames)
        {
            if (TextureNames.Count != 0)
            {
                FABRICK_DEBUG.Log("[FABRICK_TEXTURE_MANAGER] Unloading All Texture...");
            }
            else
            {
                return;
            }
            uint Number = 1;
            FABRICK_DEBUG.Log("[FABRICK_TEXTURE_MANAGER] Unload Textures by List: ");
            foreach (string ThisTextureName in TextureNames)
            {
                UnloadTexture(ThisTextureName);
                FABRICK_DEBUG.Log($"{Number}. {ThisTextureName}");
                Number++;
            }
            FABRICK_DEBUG.Log("[FABRICK_TEXTURE_MANAGER] Unload Textures By List Finished!!");
        }
        internal static void UnloadAllTexture()
        {
            if (FABRICK_TEXTURE_MANAGER.TextureDataList.Count == 0)
            {
                return;
            }
            SDL.Log("[FABRICK_TEXTURE_MANAGER] Unloading All Texture...");

            Dictionary<string, TextureData>.ValueCollection AllTexture = TextureDataList.Values;
            List<TextureData> TextureList = new List<TextureData>(AllTexture);
            foreach (TextureData ThisTexture in TextureList)
            {
                FABRICK_DEBUG.Log($"Unloading: {ThisTexture.Directory} | {ThisTexture.FileName}");
                SDL.DestroyTexture(ThisTexture.Texture);
            }
            TextureDataList.Clear();
        }
        internal static void DevUnloadAllTexture()
        {
            if (FABRICK_TEXTURE_MANAGER.DevTextureDataList.Count == 0)
            {
                return;
            }
            SDL.Log("[FABRICK_TEXTURE_MANAGER] Unloading All Dev Texture...");

            Dictionary<string, TextureData>.ValueCollection AllTexture = FABRICK_TEXTURE_MANAGER.DevTextureDataList.Values;
            List<TextureData> TextureList = new List<TextureData>(AllTexture);
            foreach (TextureData ThisTexture in TextureList)
            {
                SDL.DestroyTexture(ThisTexture.Texture);
            }
        }
    }
}
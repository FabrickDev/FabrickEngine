using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public class FABRICK_DRAW_TEXTURE
    {
        internal static nint renderer;
        internal static string EngineDataPath = "";

        public static void SetSource(string name, Vector2 Pos, Vector2 Size)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return;
            }

            TextureData _TData = new TextureData();
            _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];

            _TData.SRC.X = Pos.X;
            _TData.SRC.Y = Pos.Y;
            _TData.SRC.W = Size.X;
            _TData.SRC.H = Size.Y;

            FABRICK_TEXTURE_MANAGER.TextureDataList[name] = _TData;
        }
        public static SDL.FRect GetSource(string name)
        {
            SDL.FRect _FR = new SDL.FRect();

            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return _FR;
            }

            _FR = FABRICK_TEXTURE_MANAGER.TextureDataList[name].SRC;

            return _FR;
        }
        public static void ResetSource(string name)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return;
            }

            if(!SDL.GetTextureSize(FABRICK_TEXTURE_MANAGER.TextureDataList[name].Texture, out float _W, out float _H))
            {
                FABRICK_DEBUG.SimpleMessageError($"Error Get Texture Size: {SDL.GetError()}");
            }

            TextureData _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];
            SDL.FRect _FR = new SDL.FRect();

            _FR.W = _W;
            _FR.H = _H;

            _TData.SRC = _FR;
            FABRICK_TEXTURE_MANAGER.TextureDataList[name] = _TData;

            return;
        }
        public static void SetDestination(string name, Vector2 Pos, Vector2 Size)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return;
            }

            TextureData _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];

            _TData.DST.X = Pos.X;
            _TData.DST.Y = Pos.Y;
            _TData.DST.W = Size.X;
            _TData.DST.H = Size.Y;

            FABRICK_TEXTURE_MANAGER.TextureDataList[name] = _TData;
        }
        public static SDL.FRect GetDestination(string name)
        {
            SDL.FRect _FR = new SDL.FRect();

            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return _FR;
            }

            _FR = FABRICK_TEXTURE_MANAGER.TextureDataList[name].DST;

            return _FR;
        }
        public static void ResetDestination(string name)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                return;
            }

            if(!SDL.GetTextureSize(FABRICK_TEXTURE_MANAGER.TextureDataList[name].Texture, out float _W, out float _H))
            {
                FABRICK_DEBUG.SimpleMessageError($"Error Get Texture Size: {SDL.GetError()}");
            }

            TextureData _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];
            SDL.FRect _FR = new SDL.FRect();

            _FR.W = _W;
            _FR.H = _H;

            _TData.DST = _FR;
            FABRICK_TEXTURE_MANAGER.TextureDataList[name] = _TData;

            return;
        }

        // Default Render
        public static bool RenderTexture(string name,float Transparency = 0f,  bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.SimpleMessageWarning("Texture " + name + " Doesn't Exist!!");
                return false;
            }

            SDL.FRect _Dst = FABRICK_TEXTURE_MANAGER.TextureDataList[name].DST;
            
            return RenderTexture(name, new Vector2(_Dst.X, _Dst.Y),Transparency, isUi, 1, Flip);
        }
        public static bool RenderTexture(string name, Vector2 Pos, float Transparency = 0f, bool isUi = false, float Parallax = 1, SDL.FlipMode Flip = SDL.FlipMode.None)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.SimpleMessageWarning("Texture " + name + " Doesn't Exist!!");
                return false;
            }

            SDL.FRect _Dst = FABRICK_TEXTURE_MANAGER.TextureDataList[name].DST;
            
            return RenderTexture(name, Pos, new Vector2(_Dst.W, _Dst.H), Transparency, isUi, Parallax, Flip);
        }

        public static bool RenderTexture(string name, Vector2 DPos, Vector2 DSize, float Transparency = 0f, bool isUi = false, float Parallax = 1, SDL.FlipMode Flip = SDL.FlipMode.None)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.SimpleMessageWarning("Texture " + name + " Doesn't Exist!!");
                return false;
            }

            TextureData _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];
            
            return RenderTexture(name, DPos, DSize, new Vector2(_TData.SRC.X, _TData.SRC.X), new Vector2(_TData.SRC.W, _TData.SRC.H), Transparency, isUi, Flip, Parallax);
        }

        public static bool RenderTexture(string name, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
        {
            return RenderTexture(name, DPos, DSize, SPos, SSize, 0, Transparency, isUi, Flip, Parallax);
        }
        public static bool RenderTexture(string name, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Angle, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
        {
            Vector2 CenterPoint = new Vector2(DSize.X/2, DSize.Y/2);
            return RenderTexture(name,DPos, DSize, SPos, SSize, Angle, CenterPoint, Transparency, isUi, Flip, Parallax);
        }
        public static bool RenderTexture(string name, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Angle, Vector2 CenterPoint, float Transparency = 0f, bool isUi = false, SDL.FlipMode Flip = SDL.FlipMode.None, float Parallax = 1)
        {
            if (!FABRICK_TEXTURE_MANAGER.TextureDataList.ContainsKey(name))
            {
                FABRICK_DEBUG.SimpleMessageWarning("Texture " + name + " Doesn't Exist!!");
                return false;
            }

            TextureData _TData = FABRICK_TEXTURE_MANAGER.TextureDataList[name];
            
            SDL.FRect _Dst, _Src = new SDL.FRect();
            _Dst.X = DPos.X;
            _Dst.Y = DPos.Y;
            _Dst.W = DSize.X;
            _Dst.H = DSize.Y;

            _Src.X = SPos.X;
            _Src.Y = SPos.Y;
            _Src.W = SSize.X;
            _Src.H = SSize.Y;
            
            return RenderTexture(_TData.Texture, DPos, DSize, SPos, SSize, Angle, CenterPoint, Flip, Transparency, isUi, Parallax);
        }
        private static bool RenderTexture(nint Tex, Vector2 DPos, Vector2 DSize, Vector2 SPos, Vector2 SSize, float Angle, Vector2 CenterPoint, SDL.FlipMode Flip, float Transparency = 0f, bool isUi = false, float Parallax = 1)
        {
            SDL.FRect _Dst, _Src = new SDL.FRect();
            _Dst.X = DPos.X;
            _Dst.Y = DPos.Y;
            _Dst.W = DSize.X;
            _Dst.H = DSize.Y;

            _Src.X = SPos.X;
            _Src.Y = SPos.Y;
            _Src.W = SSize.X;
            _Src.H = SSize.Y;

            return RenderTexture(Tex, _Dst, _Src, Angle, CenterPoint, Flip, Transparency, isUi, Parallax);
        }
        private static bool RenderTexture(nint Tex, SDL.FRect Dst, SDL.FRect Src, float Angle, Vector2 CenterPoint, SDL.FlipMode Flip, float Transparency = 0f, bool isUi = false, float Parallax = 1)
        {
            if (!isUi)
            {
                Vector2 OriPos = new Vector2(Dst.X, Dst.Y);

                FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(OriPos, new Vector2(Dst.W, Dst.H), Angle, Parallax); // -128, -128

                Dst.X = T.GetActualPosition().X;
                Dst.Y = T.GetActualPosition().Y;

                Vector2 Center = new Vector2(Dst.X, Dst.Y);
                float SIN = T.GetSin();
                float COS = T.GetCos();

                Dst.X = Center.X - CenterPoint.X * COS * FABRICK_CAMERA_MANAGER.GetCamZoom() + CenterPoint.Y * SIN * FABRICK_CAMERA_MANAGER.GetCamZoom();
                Dst.Y = Center.Y - CenterPoint.X * SIN * FABRICK_CAMERA_MANAGER.GetCamZoom() - CenterPoint.Y * COS * FABRICK_CAMERA_MANAGER.GetCamZoom();

                Dst.W = T.GetSize().X;
                Dst.H = T.GetSize().Y;
                Angle = T.GetAngle();
            }
            else
            {
                Dst.X = Dst.X - CenterPoint.X;
                Dst.Y = Dst.Y - CenterPoint.Y;
            }
            
            SDL.FPoint FP = new();
            FP.X = 0;
            FP.Y = 0;

            SDL.SetTextureAlphaModFloat(Tex, 1f - Transparency);

            if(!SDL.RenderTextureRotated(renderer, Tex, Src, Dst, Angle, FP, Flip))
            {
                FABRICK_DEBUG.SimpleMessageError($"[DRAW_TEXTURE_MANAGER] Draw Texture: was error cause: {SDL.GetError()}");
                return false;
            }
            //FABRICK_DRAW_SHAPE.DrawFillRect(new Vector2(FP.X, FP.Y), new Vector2(15, 15), FABRICK_COLOR.Blue());
            return true;
        }
    }
}
using System.Numerics;
using System.Text;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using SDL3;

namespace Fabrick.ENGINE.RENDERER
{
    public partial class FABRICK_DRAW_TEXT
    {
        internal static string DevTextTagCache = "Dev@";
        public static void DrawTextEachLetter(string Text, int Step, string FontName, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 BoundSize, int MaxSentenceEachLine = 0, int ExtraPaddingEachLine = 0, bool IsUI = true, bool Optimize = true, string CustomCacheCategory = "")
        {
            BoundSize = Vector2.Zero;
            if (Step <= 0) return;

            //string ModifiedText = "";

            //Step = FABRICK_MATH.Clamp(Step, 0, Text.Length);


            // for (int i = 0; i < Step; i++)
            // {
            //     ModifiedText += Text[i];
            // }

            DrawTextAdvanced(Text.Substring(0, Step), FontName, Pos, Size, Color, Align, out BoundSize, MaxSentenceEachLine, ExtraPaddingEachLine, true, IsUI, Optimize, CustomCacheCategory);
        }
        public static void DrawTextAdvanced(string Text, string FontName, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 SizeBound, int MaxSentenceEachLine = 0, int ExtraPaddingEachLine = 0, bool Render = true, bool IsUI = false, bool Optimize = true, string CustomCacheCategory = "")
        {
            SizeBound = Vector2.Zero;
            if(ThisEngine is null || Text.Length == 0)
            {
                return;
            }
            if (!FontList.ContainsKey(FontName))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_DRAW_TEXT] Cannot Draw Text Cuz Theres no This Font: {FontName}");
                return;
            }
            // Multiline Handle
            //Text = Text.Replace("\r\n", "\n");
            string tempText = "";
            List<string> TextEachLine = new();
            List<Vector2> TextLineTextureSize = new();
            int Iterator = 1;
            bool NextLine = false;
            for (int j = 0; j < Text.Length; j++)
            {
                if (MaxSentenceEachLine > 0)
                {
                    if (Iterator % (MaxSentenceEachLine) == 0)
                    {
                        NextLine = true;
                    }
                }

                if (Text[j] != '\n') //IDK Why this is not working bruh
                {
                    tempText += Text[j];
                }

                if ((NextLine && Text[j] == ' ') || Text[j] == '\n')
                {
                    string _temp = "";
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        if (i != 0 || tempText[i] != ' ')
                        {
                            // Do what?
                            _temp += tempText[i];
                        }
                    }
                    TextEachLine.Add(_temp);
                    tempText = "";
                    Iterator = 0;
                    NextLine = false;
                }
                Iterator++;
            }
            TextEachLine.Add(tempText);

            CacheTextSetting _Setting = new()
            {
                FontName = FontName,
                Size = Size,
                Color = Color
            };

            bool CacheFound = false;

            SDL.FRect DRect = new();

            for (int i = 0; i < TextEachLine.Count; i++)
            {
                CacheFound = false;
                if (CustomCacheCategory.Length > 0)
                {
                    _Setting.Text = CustomCacheCategory + "@" + TextEachLine[i];
                }
                else
                {
                    _Setting.Text =  TextEachLine[i];
                }


                if (TextTextureCacheList.ContainsKey(CacheConvertFormat(_Setting)) && Optimize)
                {
                    CacheFound = true;
                    DRect = new(){
                        W = TextTextureCacheList[CacheConvertFormat(_Setting)].W,
                        H = TextTextureCacheList[CacheConvertFormat(_Setting)].H
                    };
                    if (Align == Algin.Center)
                    {
                        DRect.X = Pos.X - (DRect.W / 2);
                        DRect.Y = Pos.Y;
                    }
                    if (Align == Algin.Left)
                    {
                        DRect.X = Pos.X;
                        DRect.Y = Pos.Y;
                    }
                    if (Align == Algin.Right)
                    {
                        DRect.X = Pos.X - DRect.W;
                        DRect.Y = Pos.Y;
                    }
                    if (i > 0)
                    {
                        DRect.Y += (DRect.H * i) + ExtraPaddingEachLine;
                    }
                    if (Render)
                    {
                        //SDL.SetRenderDrawColor(ThisEngine.GetRenderer(), Color.R, Color.G, Color.B, Color.A);
                        SDL.SetTextureAlphaMod(TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, Color.A);
                        if (IsUI)
                        {
                            SDL.RenderTexture(ThisEngine.GetRenderer(), TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, IntPtr.Zero, DRect);
                        }
                        else
                        {
                            FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
                            SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetActualPosition(), T.GetSize());
                            if ((R.X > FABRICK_CAMERA_MANAGER.GetCamSize().X || 
                                R.Y > FABRICK_CAMERA_MANAGER.GetCamSize().Y ||
                                R.X + R.W < 0 ||
                                R.Y + R.H < 0) && Optimize)
                            {
                                //SDL.DestroyTexture(textTexture);
                            }
                            else
                            {
                                //FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Cache Found! {CacheConvertFormat(_Setting)}");
                                SDL.RenderTexture(ThisEngine.GetRenderer(), TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, IntPtr.Zero, R);
                            }
                        }
                    }
                    SizeBound = new Vector2(Math.Max(SizeBound.X, TextTextureCacheList[CacheConvertFormat(_Setting)].W), SizeBound.Y + TextTextureCacheList[CacheConvertFormat(_Setting)].H);
                }
            }
            if (CacheFound)
            {
                if (Optimize)
                {
                    //FABRICK_DEBUG.Log(_Setting.Text + "_NotFound");
                }
                return;
            }
            // if (!CreateCachePhase)
            // {
                
            //     return;
            // }
            // Cache Data
            for (int i = 0; i < TextEachLine.Count; i++)
            {
                TextTextureCache _Cache = new();
                nint textRender = TTF.RenderTextSolid(FontList[FontName].Font, TextEachLine[i], 0, Color.Get_SDL_Color());
                nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
                SDL.DestroySurface(textRender);
                SDL.GetTextureSize(textTexture, out float W, out float H);

                if (CustomCacheCategory.Length > 0)
                {
                    _Setting.Text = CustomCacheCategory + "@" + TextEachLine[i];
                }
                else
                {
                    _Setting.Text = TextEachLine[i];
                }
                //FABRICK_DEBUG.Log("[FABRICK_DRAW_TEXT.DRAW_TEXT] Current Render: " + TextEachLine[i]);
                FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.DRAW_TEXT] Create Cache: {CacheConvertFormat(_Setting)}"); 

                DRect = new SDL.FRect
                {
                    W = W * Size / FontList[FontName].Renderquality,
                    H = H * Size / FontList[FontName].Renderquality
                };
                _Cache.W = DRect.W;
                _Cache.H = DRect.H;

                if (Align == Algin.Center)
                {
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Left)
                {
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Right)
                {
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                }
                if (i != 0)
                {
                    DRect.Y += (DRect.H * i) + ExtraPaddingEachLine;
                    TextLineTextureSize.Add(new Vector2(DRect.W, DRect.H + ExtraPaddingEachLine));
                }
                else
                {
                    TextLineTextureSize.Add(new Vector2(DRect.W, DRect.H));
                }
                _Cache.W = DRect.W;
                _Cache.H = DRect.H;

                if (Render)
                {
                    SDL.SetTextureAlphaMod(textTexture, Color.A);
                    if (IsUI)
                    {
                        SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, DRect);
                    }
                    else
                    {
                        FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
                        SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetActualPosition(), T.GetSize());

                        SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, R);
                    }
                }
                _Cache.Texture = textTexture;
                
                SizeBound = new Vector2(Math.Max(SizeBound.X, SizeBound.X + TextLineTextureSize[i].X), SizeBound.Y + TextLineTextureSize[i].Y);

                _Cache.SizeBound = SizeBound;

                //SDL.DestroyTexture(textTexture);
                if(!Optimize) 
                {
                    SDL.DestroyTexture(textTexture);
                }
                else
                {
                    TextTextureCacheList[CacheConvertFormat(_Setting)] = _Cache;
                    //FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.DRAW_TEXT] Create Cache: {CacheConvertFormat(_Setting)}"); 
                }
            }
        }
        public static void DrawDevText(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, bool Optimize = true)
        {
            if(ThisEngine is null)
            {
                return;
            }
            CacheTextSetting _Setting = new()
            {
                Text = DevTextTagCache + Text,
                FontName = DefaultFontName,
                Size = Size,
                Color = Color
            };
            // Check Cache First
            if (TextTextureCacheList.ContainsKey(CacheConvertFormat(_Setting)) && Optimize)
            {
                //FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Cache Found! {CacheConvertFormat(_Setting)}");
                SDL.FRect DRect = new SDL.FRect()
                {
                    W = TextTextureCacheList[CacheConvertFormat(_Setting)].W,
                    H = TextTextureCacheList[CacheConvertFormat(_Setting)].H
                };
                if (Align == Algin.Center)
                {
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Left)
                {
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Right)
                {
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                }

                SDL.SetTextureAlphaMod(TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, Color.A);
                FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
                SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetActualPosition(), T.GetSize());
                SDL.RenderTexture(ThisEngine.GetRenderer(), TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, IntPtr.Zero, R);
                return;
            }
            else
            {
                SDL.FRect DRect = new SDL.FRect();
                nint textRender = TTF.RenderTextSolid(FontList[DefaultFontName].Font, Text, 0, Color.Get_SDL_Color());
                nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);

                SDL.GetTextureSize(textTexture, out float W, out float H);

                if (Align == Algin.Center)
                {
                    DRect.W = W * Size / FontList[DefaultFontName].Renderquality;
                    DRect.H = H * Size / FontList[DefaultFontName].Renderquality;
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }

                if (Align == Algin.Left)
                {
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                    DRect.W = W * Size / FontList[DefaultFontName].Renderquality;
                    DRect.H = H * Size / FontList[DefaultFontName].Renderquality;
                }

                if (Align == Algin.Right)
                {
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                    DRect.W = W * Size / FontList[DefaultFontName].Renderquality;
                    DRect.H = H * Size / FontList[DefaultFontName].Renderquality;
                }


                SDL.DestroySurface(textRender);
                //SDL.SetRenderDrawColor(Renderer, Color.R, Color.G, Color.B, Color.A);

                FABRICK_TRANSFORM T = FABRICK_CAMERA_MANAGER.WorldToScreen(new Vector2(DRect.X, DRect.Y), new Vector2(DRect.W, DRect.H));
                SDL.FRect R = FABRICK_DRAW_SHAPE.MakeRect(T.GetActualPosition(), T.GetSize());
                if ((R.X > FABRICK_CAMERA_MANAGER.GetCamSize().X || 
                    R.Y > FABRICK_CAMERA_MANAGER.GetCamSize().Y ||
                    R.X + R.W < 0 ||
                    R.Y + R.H < 0) && Optimize)
                {
                    SDL.DestroyTexture(textTexture);
                    return;
                }
                SDL.SetTextureAlphaMod(textTexture, Color.A);
                SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, R);
                if(!Optimize) 
                {
                    SDL.DestroyTexture(textTexture);
                    return;
                }
                TextTextureCacheList[CacheConvertFormat(_Setting)] = new()
                {
                    Texture = textTexture,
                    H = R.H,
                    W = R.W
                };
            }
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, bool Optimize = true)
        {
            DrawDevText_UI(Text, Pos, Size, Color, Align, out _, true, Optimize);
        }
        public static Vector2 GetSizeDrawDevText_UI(string Text, float Size, Algin Align)
        {
            Vector2 _Size;
            DrawDevText_UI(Text, Vector2.Zero, Size, new FABRICK_COLOR(0, 0, 0, 0), Align, out _Size, false, false);
            return _Size;
        }
        public static void DrawDevText_UI(string Text, Vector2 Pos, float Size, FABRICK_COLOR Color, Algin Align, out Vector2 SizeBound, bool Render = true, bool Optimize = true)
        {
            if(ThisEngine is null)
            {
                SizeBound = Vector2.Zero;
                return;
            }
            SDL.FRect DRect = new SDL.FRect();

            CacheTextSetting _Setting = new()
            {
                Text = DevTextTagCache + Text,
                FontName = DefaultFontName,
                Size = Size,
                Color = Color
            };

            // Check Cache First
            if (TextTextureCacheList.ContainsKey(CacheConvertFormat(_Setting)) && Optimize)
            {
                //FABRICK_DEBUG.Log($"[FABRICK_DRAW_TEXT.MEMORY_MANAGEMENT] Cache Found! {CacheConvertFormat(_Setting)}");
                DRect = new(){
                    W = TextTextureCacheList[CacheConvertFormat(_Setting)].W,
                    H = TextTextureCacheList[CacheConvertFormat(_Setting)].H
                };

                if (Align == Algin.Center)
                {
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Left)
                {
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                }
                if (Align == Algin.Right)
                {
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                }
                SizeBound = new Vector2(DRect.W, DRect.H);

                if (Render)
                {
                    SDL.SetTextureAlphaMod(TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, Color.A);
                    SDL.RenderTexture(ThisEngine.GetRenderer(), TextTextureCacheList[CacheConvertFormat(_Setting)].Texture, IntPtr.Zero, DRect);
                }
                
                return;
            }
            else
            {
                nint textRender = TTF.RenderTextSolid(FontList[DefaultFontName].Font, Text, 0, Color.Get_SDL_Color());
                nint textTexture = SDL.CreateTextureFromSurface(ThisEngine.GetRenderer(), textRender);
                SDL.DestroySurface(textRender);

                SDL.GetTextureSize(textTexture, out float W, out float H);
                DRect.W = W * Size / FontList[DefaultFontName].Renderquality;
                DRect.H = H * Size / FontList[DefaultFontName].Renderquality;
                if (Align == Algin.Center)
                {
                    DRect.X = Pos.X - (DRect.W / 2);
                    DRect.Y = Pos.Y;
                }

                if (Align == Algin.Left)
                {
                    DRect.X = Pos.X;
                    DRect.Y = Pos.Y;
                }

                if (Align == Algin.Right)
                {
                    DRect.X = Pos.X - DRect.W;
                    DRect.Y = Pos.Y;
                }

                if (Render)
                {
                    SDL.SetTextureAlphaModFloat(textTexture, Color.A);
                    SDL.RenderTexture(ThisEngine.GetRenderer(), textTexture, IntPtr.Zero, DRect);
                }
                
                SizeBound = new Vector2(DRect.W, DRect.H);
                if(Optimize == false) 
                {
                    SDL.DestroyTexture(textTexture);
                    return;
                }
                else
                {
                    TextTextureCacheList[CacheConvertFormat(_Setting)] = new()
                    {
                        Texture = textTexture,
                        H = DRect.H,
                        W = DRect.W
                    };
                }
            }
        }
        public static void DrawDevFPS(Vector2 Pos, float FontSize, int FPS)
        {
            FABRICK_COLOR BG_Color = new FABRICK_COLOR(15, 15, 15, 128);
            Vector2 Offsetter = new Vector2(6, 6);
            Vector2 GetTextSize = GetSizeDrawDevText_UI($"FPS: {FPS}", FontSize, Algin.Left);
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(Pos, GetTextSize + Offsetter + Offsetter, BG_Color);
            DrawDevText_UI($"FPS: {FPS}", Pos + Offsetter, FontSize, FABRICK_COLOR.White(), Algin.Left, false);
        }
    }
}
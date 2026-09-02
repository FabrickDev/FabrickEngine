using System.Numerics;
using SDL3;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.EDITOR;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.SCENE_MANAGER;
using Fabrick.ENGINE.ANIMATION;
using Fabrick.ENGINE.MATH;

/*
    References:
    https://github.com/constref/sdl3-gamedev/blob/a436a04436658c4ecef62aad8dc362cb9e16769d/sdl3-demo/sdl3-demo.cpp
    https://www.youtube.com/watch?v=Wu2g-N5Z78Y&list=PL30rnYpLa1_o660wOb_VMFIiZwbGUD0aD
    https://wiki.libsdl.org/SDL3/SDL_RendererLogicalPresentation
*/

namespace Fabrick.ENGINE.CORE
{
    public class FABRICK_ENGINE
    {
        // Public
    
        // Private
        private bool IsDevMode = false;
        private bool IsEditorMode = false;
        private bool IsEditorModeOnceUpdate = true;

        private nint window;
        private nint renderer;

        // Color "3F1E11"
        private Color DefaultBackgroundColor = new Color(0x3F, 0x1E, 0x11, 0xFF);
        // Color FFF3D1
        private Color DefaultLogicalBackgroundColor = new Color(0xFF, 0xF3, 0xD1, 0xFF);

        public static string MainEngineDataPath = "D:\\PROJECT\\BATARA\\FABRICK_ENGINE\\FABRICK_ENGINE";

        private Dictionary<int, Action> FunctionUpdateList = new Dictionary<int, Action>();
        private UInt16 FunctionUpdateList_I = 0;
        private bool FunctionUpdateList_Error = false;

        private Dictionary<int, Action> FunctionInitializeList = new Dictionary<int, Action>();
        private UInt16 FunctionInitializeList_I = 0;
        private bool FunctionInitializeList_Error = false;
        private bool FunctionInitializeList_Finished = false;

        private Dictionary<int, Action> RenderList = new Dictionary<int, Action>();
        private UInt16 RenderList_I = 0;
        private bool RenderList_Error = false;

        private bool EngineRunning = true;
        private bool RenderingVSync = false;
        private bool RenderingFullScreen = false;
        private Vector2 LogicalResolution = new Vector2(1920, 1080);
        private int ActualResolutionX, ActualResolutionY;

        // Run Event
        private SDL.Event RunEvent;

        // Time Var
        private UInt64 prevTime = 0;
        private UInt64 nowTime = 0;
        public float DeltaTime;
        private UInt64 StartPerformanceCounter = SDL.GetPerformanceCounter();
        private UInt64 EndPerformanceCounter = 0;
        public float FrameRate = 0f;
        public int FrameRateInt = 0;

        // Dev Mode
        EDITOR_MANAGER EDITOR_MANAGER = new EDITOR_MANAGER();

        private static int CallCount;
        public FABRICK_ENGINE()
        {
            CallCount++;
            FABRICK_DEBUG.Log($"Engine call count: {CallCount}");
        }
        public void DevMode(bool state)
        {
            IsDevMode = state;
        }
        // Initialize Render Engine
        public bool Initialize(string WindowName, int Width, int Height, bool FullScreen, int VSync, string EngineDataPath)
        {
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] |=============== FABRICK ENGINE ===============|");
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Fabrick Engine Initialize Phase!!");

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Video...");
            if (!SDL.Init(SDL.InitFlags.Video))
            {
                FABRICK_DEBUG.SimpleMessageError($"SDL could not initialize: {SDL.GetError()}");
                SDL.LogError(SDL.LogCategory.System, $"SDL could not initialize: {SDL.GetError()}");
                return false;
            }

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize TTF...");
            if (!TTF.Init())
            {
                FABRICK_DEBUG.SimpleMessageError($"SDL_TTF could not initialize!");
            }

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Window and Renderer...");
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Available Driver: ");

            // bool VulkanAvailable = false;
            // int VulkanNum = 0;
            for (int i = 0; i < SDL.GetNumRenderDrivers(); i++)
            {
                FABRICK_DEBUG.Log($"{i}. {SDL.GetRenderDriver(i)}.");
                // if (SDL.GetRenderDriver(i) == "vulkan")
                // {
                //     VulkanAvailable = true;
                //     VulkanNum = i;
                // }
            }

            // if (VulkanAvailable)
            // {
            //     FABRICK_DEBUG.Log("[FABRICK_ENGINE] Vulkan Available, set to Vulkan Driver rn.");
            // }
            // window = SDL.CreateWindow(WindowName, Width, Height, 0);
            // renderer = SDL.CreateRenderer(window, "gpu");
            // FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Renderer API: {SDL.GetRendererName(renderer)}");

            if (!SDL.CreateWindowAndRenderer(WindowName, Width, Height, 0, out window, out renderer))
            {
                FABRICK_DEBUG.SimpleMessageError($"Error creating window and renderer: {SDL.GetError()}");
                SDL.LogError(SDL.LogCategory.Application, $"Error creating window and renderer: {SDL.GetError()}");
                return false;
            }
            else
            {
                FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Renderer API: {SDL.GetRendererName(renderer)}");
            }

            if (VSync == 1)
            {
                FABRICK_DEBUG.Log("[FABRICK_ENGINE] VSync On!!");
                RenderingVSync = true;
            }
            else
            {
                FABRICK_DEBUG.Log("[FABRICK_ENGINE] VSync Off!!");
                RenderingVSync = false;
            }

            // Random Bullshit
            SDL.SetRenderVSync(renderer, VSync);
            SDL.SetRenderDrawBlendMode(renderer, SDL.BlendMode.Blend);
    
            /* SDL LOGOCAL REFERENCE 🥀 Biar enggak lupa*/
            // https://wiki.libsdl.org/SDL3/SDL_RendererLogicalPresentation
            //typedef enum SDL_RendererLogicalPresentation
            //{
            //    SDL_LOGICAL_PRESENTATION_DISABLED,  /**< There is no logical size in effect */
            //    SDL_LOGICAL_PRESENTATION_STRETCH,   /**< The rendered content is stretched to the output resolution */
            //    SDL_LOGICAL_PRESENTATION_LETTERBOX, /**< The rendered content is fit to the largest dimension and the other dimension is letterboxed with the clear color */
            //    SDL_LOGICAL_PRESENTATION_OVERSCAN,  /**< The rendered content is fit to the smallest dimension and the other dimension extends beyond the output bounds */
            //    SDL_LOGICAL_PRESENTATION_INTEGER_SCALE   /**< The rendered content is scaled up by integer multiples to fit the output resolution */
            //} SDL_RendererLogicalPresentation;

            LogicalResolution.X = Width;
            LogicalResolution.Y = Height;
            FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Set logical/development resolution: X: {Width}, Y: {Height}.");
            SDL.SetRenderLogicalPresentation(renderer, (int)LogicalResolution.X, (int)LogicalResolution.Y, SDL.RendererLogicalPresentation.Letterbox);
    
            /*
            FABRICK_DEBUG.Log("Initialize Mixer...");
            if (!Mixer.Init())
            {
                FABRICK_DEBUG.SimpleMessageError($"Error creating audio device: {SDL.GetError()}", window);
                SDL.LogError(SDL.LogCategory.Application, $"Error creating audio device: {SDL.GetError()}");
                return false;
            }
            */
    
            SDL.SetWindowFullscreen(window, FullScreen);

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Set Engine Data Path: " + EngineDataPath);
            MainEngineDataPath = EngineDataPath;
            FABRICK_ENTITY_RWDATA.SetEngineDataPath(MainEngineDataPath);
            FABRICK_SCENE_RWDATA.SetEngineDataPath(MainEngineDataPath);

            // Main Componen
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Main Components...");
            MainComponentInitialize();

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialization Finish!!");
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] |==============================================|");
            FABRICK_DEBUG.Log("");
            EngineRunning = true;

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] |=============== FABRICK ENGINE ===============|");
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Fabrick Engine Update/Running Phase!!");
            FABRICK_DEBUG.Log("");
            return true;
        }

        // Fabrick Engine Update
        public void Update()
        {

            // Check If Ready to Run!!
            if (EngineRunning == false)
            {
                return;
            }

            // Loop
            while (EngineRunning)
            {
                //MainComponentInitializeNUpdate();
                while (SDL.PollEvent(out RunEvent))
                {
                    if ((SDL.EventType)RunEvent.Type == SDL.EventType.Quit)
                    {
                        EngineRunning = false;
                    }
                    FABRICK_INPUT.MouseScrollEventHandle(RunEvent);
                }

                // Deltatime & FPS Cacl
                StartPerformanceCounter = EndPerformanceCounter;
                prevTime = nowTime;

                nowTime = SDL.GetTicks();
                EndPerformanceCounter = SDL.GetPerformanceCounter();
                DeltaTime = (nowTime - prevTime) / 1000.0f;

                float Elapsed = (float)(EndPerformanceCounter - StartPerformanceCounter) / SDL.GetPerformanceFrequency();
                FrameRate = 1.0f / Elapsed;
                FrameRateInt = (int)FrameRate;
                

                // ========================== Initialization Phase ========================== //
                // Check if Function Initialize Available
                if (FunctionInitializeList_Finished == false)
                {
                    if (!FunctionInitializeList_Error && (FunctionInitializeList == null || !FunctionInitializeList.Any()))
                    {
                        FunctionInitializeList_Error = true;
                        if (IsDevMode)
                        {
                            FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] Function Initialize List is null or empty!");
                            FunctionInitializeList_Finished = true;
                        }
                    }
                    FABRICK_ENTITY_MANAGER.InitializeAllEntity();
                    FABRICK_ENTITY_MANAGER.UpdateAllEntity(this);
                    if(FunctionInitializeList_Error == false)
                    {
                        // Run Functions Initialize
                        for (UInt16 i = 0; i < FunctionInitializeList_I; i++)
                        {
                            if(FunctionInitializeList != null) FunctionInitializeList[i]();
                        }
                        FunctionInitializeList_Finished = true;
                    }
                    FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Function Complete!");
                }

                // Main Component Update
                MainComponentUpdate();

                // ========================== Update Phase ========================== //
                // Check if Function Update Available
                if (!FunctionUpdateList_Error && (FunctionUpdateList == null || !FunctionUpdateList.Any()))
                {
                    FunctionUpdateList_Error = true;
                    if (IsDevMode)
                    {
                        FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] Function Update List is null or empty!");
                        FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] This Loop run nothing vro🥀");
                    }
                }
                
                if(FunctionUpdateList_Error == false && !IsEditorMode)
                {
                    // Run Functions Update
                    for (UInt16 i = 0; i < FunctionUpdateList_I; i++)
                    {
                        if(FunctionUpdateList != null) FunctionUpdateList[i]();
                    }
                }

                // ========================== Render Phase ========================== //
                // Check if Render Order Available
                if (!RenderList_Error && (RenderList == null || !RenderList.Any()))
                {
                    RenderList_Error = true;
                    if (IsDevMode)
                    {
                        FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] Nothing to Render?!");
                    }
                }
                
                if(RenderList_Error == false)
                {
                    // Run Functions Update
                    for (UInt16 i = 0; i < RenderList_I; i++)
                    {
                        if(RenderList != null) RenderList[i]();
                    }
                }

                // DevEditor Listener
                if (IsDevMode)
                {
                    if (FABRICK_INPUT.IsKeyPressed(SDL.Scancode.F1))
                    {
                        IsEditorMode = !IsEditorMode;
                        if (IsEditorMode)
                        {
                            EDITOR_MANAGER.LastEngineCamera = FABRICK_CAMERA_MANAGER.GetCurrentCamera();
                            if (EDITOR_MANAGER.LastEditorCamera != "@DEV.CAMNULL")
                            {
                                FABRICK_CAMERA_MANAGER.SetCurrentCamera(EDITOR_MANAGER.LastEditorCamera);
                            }
                            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Editor Mode: On");
                        }
                        else
                        {
                            EDITOR_MANAGER.LastEditorCamera = FABRICK_CAMERA_MANAGER.GetCurrentCamera();
                            FABRICK_CAMERA_MANAGER.SetCurrentCamera(EDITOR_MANAGER.LastEngineCamera);
                            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Editor Mode: Off");
                        }
                    }
                }
                // Developer Component Update
                if (IsEditorMode)
                {
                    DeveloperEditorComponentUpdate();
                }

                // Loop After Update
                FABRICK_ANIMATION_FRAME_PLAY.Update();

                // Render Loop
                FABRICK_DRAW_TEXT.DrawDevFPS(new Vector2(2, 2), 3);
                SDL.RenderPresent(renderer);

                SDL.SetRenderDrawColor(renderer, DefaultBackgroundColor.R, DefaultBackgroundColor.G, DefaultBackgroundColor.B, DefaultBackgroundColor.A);
                SDL.RenderClear(renderer);
                FABRICK_DRAW_SHAPE.DrawFillRect_UI(Vector2.Zero, LogicalResolution, DefaultLogicalBackgroundColor);

                // Deltatime End
                //FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Deltatime: {DeltaTime}");
                //SDL.Delay(1);
            }
            Quit();
        }
        public void Quit()
        {
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Stopping Engine..");
            EngineRunning = false;
            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            FABRICK_TEXTURE_MANAGER.UnloadAllTexture();
            FABRICK_TEXTURE_MANAGER.DevUnloadAllTexture();
            FABRICK_DRAW_TEXT.DestroyAllFont();

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Engine Stopped!!");
            SDL.Quit();
        }
        private void MainComponentInitialize()
        {
            FABRICK_INPUT.InitializeKeyState();
            FABRICK_DEBUG.window = window;
            FABRICK_VISUALIZE_SHAPE.renderer = renderer;
            FABRICK_DRAW_TEXTURE.renderer = renderer;
            FABRICK_CAMERA_MANAGER.Initialize(this);

            // Dev Purpose
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Dev Editor Manager...");
            EDITOR_MANAGER.Initialize();
        }
        private void MainComponentUpdate()
        {
            // INPUT ENGINE UPDATE
            FABRICK_INPUT.EngineUpdate(this);
            // Get Actual Resolution
            if (!SDL.GetRenderViewport(renderer, out SDL.Rect _Rec))
            {
                FABRICK_DEBUG.SimpleMessageError(SDL.GetError());
            }
            SDL.GetWindowSize(window, out ActualResolutionX, out ActualResolutionY);
            // DrawShape Update
            FABRICK_DRAW_SHAPE.Update(this);
            // DrawText Update
            FABRICK_DRAW_TEXT.UpdateEngine(this);
            // Camera Update
            FABRICK_CAMERA_MANAGER.Update(this);
            // Scenemanager Update State Handle
            FABRICK_SCENE_MANAGER.LeaveSceneHandle();
            FABRICK_SCENE_MANAGER.UpdateRunningState();
            FABRICK_SCENE_MANAGER.PauseAllScene(IsEditorMode);
            FABRICK_SCENE_MANAGER.UpdateAllScene(this);
            FABRICK_SCENE_MANAGER.RenderAllScene();
        }
        private void DeveloperEditorComponentUpdate()
        {
            EDITOR_MANAGER.Update((float)DeltaTime);
        }

        // Save Functions
        public void AssignFunctionInitialize(Action func)
        {
            FunctionInitializeList[FunctionInitializeList_I] = func;
            FunctionInitializeList_I++;
        }
        public void AssignFunctionUpdate(Action func)
        {
            FunctionUpdateList[FunctionUpdateList_I] = func;
            FunctionUpdateList_I++;
        }
        public void AssignRenderOrder(Action func)
        {
            RenderList[RenderList_I] = func;
            RenderList_I++;
        }

        // Get Window and Renderer
        public nint GetWindow()
        {
            return window;
        }
        public nint GetRenderer()
        {
            return renderer;
        }
        
        // Window Flag Setting
        public void SetResizeableWindow(bool state)
        {
            SDL.SetWindowResizable(window, state);
        }

        // Get/Set Rendering Feature
        public void SetFullScreen(bool state)
        {
            RenderingFullScreen = state;
            SDL.SetWindowFullscreen(window, state);
        }
        public bool IsFullScreen()
        {
            return RenderingFullScreen;
        }
        public void SetVSync(bool state)
        {
            int state_i = 0;
            if (state)
            {
                state_i = 1;
            }
            else
            {
                state_i = 0;
            }
            RenderingVSync = state;
            SDL.SetRenderVSync(renderer, state_i);
        }
        public bool IsVSync()
        {
            return RenderingVSync;
        }
        public Vector2 GetLogicalResolution()
        {
            return LogicalResolution;
        }
        public Vector2 GetLogicalPresentationResolution()
        {
            SDL.FRect _x;
            SDL.GetRenderLogicalPresentationRect(renderer, out _x);
            return new Vector2(_x.W, _x.H);
        }
        public Vector2 GetActualResolution()
        {
            return new Vector2(ActualResolutionX, ActualResolutionY);
        }
    }
}
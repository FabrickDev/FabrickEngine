using System.Runtime.InteropServices;
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
using Fabrick.ENGINE.IOFILES;
using Fabrick.ENGINE.VISUAL_NOVEL;

/*
    References:
    https://github.com/constref/sdl3-gamedev/blob/a436a04436658c4ecef62aad8dc362cb9e16769d/sdl3-demo/sdl3-demo.cpp
    https://www.youtube.com/watch?v=Wu2g-N5Z78Y&list=PL30rnYpLa1_o660wOb_VMFIiZwbGUD0aD
    https://wiki.libsdl.org/SDL3/SDL_RendererLogicalPresentation
*/

namespace Fabrick.ENGINE.CORE
{
    public partial class FABRICK_ENGINE
    {
        // Public
        public enum Platform
        {
            Windows = 0,
            Android = 1,
            Linux = 2,
            MacOS = 3
        }
        private Platform CurrentPlatform = Platform.Windows;

        private nint window;
        private nint renderer;
        private nint device;

        // Color "3F1E11"
        private FABRICK_COLOR DefaultBackgroundColor = new FABRICK_COLOR(0x3F, 0x1E, 0x11, 0xFF);
        // Color FFF3D1
        private FABRICK_COLOR DefaultLogicalBackgroundColor = new FABRICK_COLOR(0xFF, 0xF3, 0xD1, 0xFF);

        public string AssetsDataPath = @"D:\PROJECT\BATARA\FABRICK_ENGINE\FABRICK_ENGINE\ASSETS";
        public string SaveDataPath = @"D:\PROJECT\BATARA\FABRICK_ENGINE\FABRICK_ENGINE\SAVES";

        private List<Action> FunctionUpdateList = new();
        private UInt16 FunctionUpdateList_I = 0;
        private bool FunctionUpdateList_Error = false;

        private List<Action> FunctionInitializeList = new();
        private UInt16 FunctionInitializeList_I = 0;
        private bool FunctionInitializeList_Error = false;
        private bool FunctionInitializeList_Finished = false;

        private List<Action> RenderList = new();
        private UInt16 RenderList_I = 0;
        private bool RenderList_Error = false;

        private bool EngineWantToExit = false;
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
        private float DeltaCount = 0;
        public float DeltaTime;
        private UInt64 StartPerformanceCounter = SDL.GetPerformanceCounter();
        private UInt64 EndPerformanceCounter = 0;
        public float FrameRate = 0f;
        public int FrameRateInt = 0;
        private int FrameRateUpdateSecond = 0;

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
        public bool Initialize(string WindowName, int Width, int Height, bool FullScreen, int VSync, string? AssetsDataPath, string? SaveDataPath)
        {
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] |=============== FABRICK ENGINE ===============|");
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Fabrick Engine Initialize Phase!!");

            FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Detecting Current Platform: {SDL.GetPlatform()}");
            switch (SDL.GetPlatform())
            {
                case "Windows":
                    CurrentPlatform = Platform.Windows;
                break;
                case "Android":
                    CurrentPlatform = Platform.Android;
                break;
                case "Linux":
                    CurrentPlatform = Platform.Linux;
                break;
                case "macOS":
                    CurrentPlatform = Platform.MacOS;
                break;
            }

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Set Assets Data Path: " + AssetsDataPath);
            if(AssetsDataPath != null && AssetsDataPath.Length > 0) this.AssetsDataPath = AssetsDataPath;
            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Set Save Data Path: " + SaveDataPath);
            if(SaveDataPath != null && SaveDataPath.Length > 0) this.SaveDataPath = SaveDataPath;
            if (!Directory.Exists(this.AssetsDataPath))
            {
                Directory.CreateDirectory(this.AssetsDataPath);
            }
            if (!Directory.Exists(this.SaveDataPath))
            {
                Directory.CreateDirectory(this.SaveDataPath);
            }
            FABRICK_GLOBAL_VARIABLE_MANAGER.SaveDataPath = this.SaveDataPath;
            FABRICK_IOFILES_MANAGER.ThisAssetsPath = this.AssetsDataPath;
            FABRICK_DRAW_TEXTURE.ThisAssetsPath = this.AssetsDataPath;
            FABRICK_SHADER_MANAGER.AssetDataPath = this.AssetsDataPath;
            FABRICK_ENTITY_RWDATA.SetAssetsPath(this.AssetsDataPath);
            FABRICK_SCENE_RWDATA.SetAssetsPath(this.AssetsDataPath);
            FABRICK_SCENARIO_RWDATA.SetAssetsPath(this.AssetsDataPath);

            FABRICK_TEXTURE_MANAGER.Initialize();
            FABRICK_GLOBAL_VARIABLE_MANAGER.Initialize();

            FABRICK_DEBUG.Log("[FABRICK_ENGINE] Initialize Video...");
            // if (!SDL.SetHint("SDL_RENDER_GPU_LOW_POWER", "0"))
            // {
            //     FABRICK_DEBUG.SimpleMessageWarning($"SDL could not set hint: {SDL.GetError()}");
            //     SDL.LogError(SDL.LogCategory.System, $"SDL could not set hint: {SDL.GetError()}");
            // }

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
            FABRICK_DRAW_TEXT.Initialize();

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
            
            window = SDL.CreateWindow(WindowName, Width, Height, SDL.WindowFlags.Transparent);
            device = SDL.CreateGPUDevice(SDL.GPUShaderFormat.SPIRV, true, "vulkan");
            renderer = SDL.CreateRenderer(window, null);
            SDL.SetWindowOpacity(window, 0f);
            FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Using Driver: {SDL.GetCurrentVideoDriver()}");
            FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Renderer API: {SDL.GetRendererName(renderer)}");


            // if (!SDL.CreateWindowAndRenderer(WindowName, Width, Height, 0, out window, out renderer))
            // {
            //    FABRICK_DEBUG.SimpleMessageError($"Error creating window and renderer: {SDL.GetError()}");
            //    SDL.LogError(SDL.LogCategory.Application, $"Error creating window and renderer: {SDL.GetError()}");
            //    return false;
            // }
            // else
            // {
            //    FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Renderer API: {SDL.GetRendererName(renderer)}");
            // }

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

            // Always Set Window to Half Size of Screen
            SDL.DisplayMode? DisplayMode = SDL.GetCurrentDisplayMode(SDL.GetPrimaryDisplay()).Value;
            if (DisplayMode.HasValue && !FullScreen)
            {
                SDL.SetWindowSize(window, DisplayMode.Value.W / 2, DisplayMode.Value.H / 2);
                SDL.SetWindowPosition(window, DisplayMode.Value.W / 4, DisplayMode.Value.H / 4);
            }

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

            // Shader Thing
            //nint Frag = FABRICK_SHADER_MANAGER.LoadShader(device, "Grayscale.frag.spv");
            
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
                        EngineWantToExit = true;
                        // The EngineRunning Handled in FABRICK_ENGINE.APPEAR_ANIMATION.cs!!
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
                    //FABRICK_ENTITY_MANAGER.UpdateAllEntity(this);
                    if(FunctionInitializeList_Error == false)
                    {
                        // Run Functions Initialize
                        //for (UInt16 i = 0; i < FunctionInitializeList_I; i++)
                        //{
                        //    if(FunctionInitializeList != null) FunctionInitializeList[i]?.Invoke();
                        //}
                        if (FunctionInitializeList != null)
                        {
                            foreach (Action ThisAction in FunctionInitializeList)
                            {
                                ThisAction.Invoke();
                            }
                        }
                        FunctionInitializeList_Finished = true;
                    }
                    FABRICK_VISUAL_NOVEL_MANAGER.Initialize();
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
                    if(FunctionUpdateList != null)
                    {
                        for (int i = 0; i < FunctionUpdateList.Count; i++)
                        {
                            if (FunctionUpdateList != null) FunctionUpdateList[i]();
                        }
                    }
                }
                
                if(RenderList_Error == false)
                {
                    // Run Functions Update
                    if (RenderList != null)
                    {
                        for (int i = 0; i < RenderList.Count; i++)
                        {
                            if (RenderList != null) RenderList[i]();
                        }
                    }
                }

                // DevEditor Listener
                DevEditorListener();

                // Loop After Update
                FABRICK_ANIMATION_FRAME_PLAY.Update();

                // Cleanup and Reset Worker AutoPlay Frame Animation
                SPRITE_SHEET_MANAGER.AutoPlayCleanWorker();
                SPRITE_SHEET_MANAGER.AutoPlayResetInUseWorker();

                // Render Loop
                DrawFPS();
                AppearHandle();
                DisappearHandle();
                SDL.RenderPresent(renderer);

                SDL.SetRenderDrawColor(renderer, DefaultBackgroundColor.R, DefaultBackgroundColor.G, DefaultBackgroundColor.B, DefaultBackgroundColor.A);
                SDL.RenderClear(renderer);

                FABRICK_DRAW_SHAPE.DrawFillRect_UI(Vector2.Zero, LogicalResolution, DefaultLogicalBackgroundColor);

                // Fullscreen Listener
                FullscreenShortcut();
                FABRICK_INPUT.AfterCycleHandle();

                // Deltatime End
                //FABRICK_DEBUG.Log($"[FABRICK_ENGINE] Deltatime: {DeltaTime}");
                //SDL.Delay(1);
                // Animation Purpose
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
            FABRICK_SCENARIO_MANAGER.InitializeScenarioHandle();

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
            SPRITE_SHEET_MANAGER.DeltaTime = DeltaTime;
            // Scenario Manager Update
            // DrawText Update
            FABRICK_DRAW_TEXT.UpdateEngine(this);
            // Camera Update
            FABRICK_CAMERA_MANAGER.Update(this);
            // Scenemanager Update State Handle
            FABRICK_SCENARIO_MANAGER.ThisEngine = this;
            FABRICK_SCENARIO_MANAGER.RunScenarioHandle();
            //FABRICK_SCENE_MANAGER.LeaveSceneHandle();
            FABRICK_SCENE_MANAGER.PauseAllScene(IsEditorMode);
            FABRICK_SCENE_MANAGER.UpdateAllScene(this);
            FABRICK_SCENE_MANAGER.RenderAllScene();
        }
        
        private void FullscreenShortcut()
        {
            if (FABRICK_INPUT.IsKeyPressed(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList["Engine.FullscreenKey"].variableValue_KEYBOARD))
            {
                SetFullScreen(!IsFullScreen());
            }
        }

        // Save Functions
        public void AssignFunctionInitialize(Action func)
        {
            FunctionInitializeList.Add(func);
        }
        public void AssignFunctionUpdate(Action func)
        {
            FunctionUpdateList.Add(func);
        }
        public void AssignRenderOrder(Action func)
        {
            RenderList.Add(func);
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
        public void DrawFPS()
        {
            float MaxDelta = DeltaTime * FrameRate;
            DeltaCount += DeltaTime;
            if (DeltaCount > MaxDelta)
            {
                FrameRateUpdateSecond = FrameRateInt;
                DeltaCount = 0;
            }
            FABRICK_DRAW_TEXT.DrawDevFPS(new Vector2(2, 2), 3, FrameRateUpdateSecond);
        }
        public Platform GetCurrentPlatform()
        {
            return CurrentPlatform;
        }
    }
}
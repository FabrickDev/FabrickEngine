using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.EDITOR.SCENE;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class EDITOR_MANAGER
    {
        public static string DevKey = "@DEV.";
        public static string LastEngineCamera = "";
        public static string LastEditorCamera = "@DEV.CAMNULL";
        internal static string LastSceneUsed = "";

        private static bool RunOnce = false;
        // Focus Manager
        public static string FocusWindow = "";
        public static bool ChangeableFocusWindow = true;
        public static Vector2 CamPosParent;

        private static Vector2 MinWindow = new Vector2(0f, 25f); 
        private static Vector2 MaxWindow = new Vector2(1920f, 1080f); 

        // Main List Tab Menu
        TAB_LIST_MENU TAB_LIST_MENU = new();
        List<TAB_LIST_ITEM> TAB_LIST_ITEM = new();

        // Operation
        public enum EditorOperation
        {
            None = -1,
            SceneEditor = 0,
            SceneEntityEditor = 1,
            SpriteSheetEditor = 2,
            SpriteAnimationEditor = 3,
            BackgroundEditor = 4,
            CollisionEditor = 5,
            DialogueEditor = 6
        }

        // Color Palette
        public static FABRICK_COLOR Color1 = new FABRICK_COLOR(50, 50, 50, 255);
        public static FABRICK_COLOR Color2 = new FABRICK_COLOR(15, 15, 15, 128);
        public static FABRICK_COLOR Color3 = new FABRICK_COLOR(255, 255, 255, 255);
        public static FABRICK_COLOR Color4 = new FABRICK_COLOR(75, 75, 75, 255);

        private static EditorOperation CurrentOperation;

        public void Initialize()
        {
            CreateListTabItem();
            TAB_LIST_MENU.Initialize(Vector2.Zero);
            DevSceneAssign();
            
            SCENE_EDITOR_MANAGER.Initialize();
            ENTITY_SCENE_EDITOR_MANAGER.Initialize();

            CurrentOperation = EditorOperation.None;
        }
        public void Update(float DeltaTime)
        {
            Hotkeys();
            TAB_LIST_MENU.Update(TAB_LIST_ITEM);
            UpdateMinWindow();

            if (!RunOnce)
            {
                DevCameraAssign();
                RunOnce = true;
            }
            DevCameraControl(DeltaTime);
            switch (CurrentOperation)
            {
                case EditorOperation.SceneEditor:
                    SCENE_EDITOR_MANAGER.Update();
                break;
                case EditorOperation.SceneEntityEditor:
                    ENTITY_SCENE_EDITOR_MANAGER.Update();
                break;
            }
            SetChangeAbleFocusWindowBackOnClick();
        }

        public EditorOperation GetCurrentOperation()
        {
            return CurrentOperation;
        }

        // Tab List Menu
        private void UpdateMinWindow()
        {
            MinWindow = new Vector2(MinWindow.X, TAB_LIST_MENU.GetMaxSize().Y);
        }
        private void CreateListTabItem()
        {
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Scene Editor", ()=>ActionTabSceneEditor()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Scene Entity Editor", ()=>ActionTabSceneEntityEditor()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Sprite Sheet Editor", ()=>ActionTabNone()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Sprite Animation Editor", ()=>ActionTabNone()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Background Editor", ()=>ActionTabNone()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Collision Editor", ()=>ActionTabNone()));
            TAB_LIST_ITEM.Add(new ITEMS.TAB_LIST_ITEM("Dialogue Editor", ()=>ActionTabDialogueEditor()));
        }
        private void ActionTabSceneEditor()
        {
            FABRICK_DEBUG.Log("[EDITOR_MANAGER] Set Operation to Scene Editor");
            FABRICK_CAMERA_MANAGER.SetCurrentCamera("@DEV.SCENE_EDITOR");
            CurrentOperation = EditorOperation.SceneEditor;
        }
        private void ActionTabSceneEntityEditor()
        {
            FABRICK_DEBUG.Log("[EDITOR_MANAGER] Set Operation to Scene Entity Editor");
            FABRICK_CAMERA_MANAGER.SetCurrentCamera("@DEV.SCENE_EDITOR");
            CurrentOperation = EditorOperation.SceneEntityEditor;
        }
        private void ActionTabDialogueEditor()
        {
            FABRICK_DEBUG.Log("[EDITOR_MANAGER] Set Operation to Scene Entity Editor");
            FABRICK_SCENE_MANAGER.SetCurrentScene("@DEV.DIALOGUE_EDITOR");
            CurrentOperation = EditorOperation.DialogueEditor;
        }
        private void ActionTabNone()
        {
            FABRICK_DEBUG.SimpleMessageInfo("No Function Yet 🥀");
        }

        private void Hotkeys()
        {
            // Scene Editor Menu
            if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F2))
            {
                FABRICK_CAMERA_MANAGER.SetCurrentCamera("DEV.SCENE_EDITOR");
                CurrentOperation = EditorOperation.SceneEntityEditor;
            }else if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F3))
            {
                CurrentOperation = EditorOperation.SpriteSheetEditor;
            }else if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F4))
            {
                CurrentOperation = EditorOperation.SpriteAnimationEditor;
            }else if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F5))
            {
                FABRICK_CAMERA_MANAGER.SetCurrentCamera("DEV.SCENE_EDITOR");
                CurrentOperation = EditorOperation.BackgroundEditor;
            }else if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F6))
            {
                FABRICK_CAMERA_MANAGER.SetCurrentCamera("DEV.SCENE_EDITOR");
                CurrentOperation = EditorOperation.CollisionEditor;
            }
        }
        private void DevCameraAssign()
        {
            FABRICK_DEBUG.Log("[EDITOR_MANAGER] Adding New Devs Camera...");
            FABRICK_CAMERA SceneEditor_Cam = new FABRICK_CAMERA(Vector2.Zero, MaxWindow, FABRICK_CAMERA_ALIGN.MidCenter);

            FABRICK_CAMERA_MANAGER.AssignCamera("@DEV.SCENE_EDITOR", SceneEditor_Cam);
        }

        private Vector2 CamGrabOffset = Vector2.Zero;
        private Vector2 CamPos = Vector2.Zero;
        private float CamZoom = 0;
        private void DevCameraControl(float DeltaTime)
        {
            if(CurrentOperation == EditorOperation.None) return;
            if(!FABRICK_CAMERA_MANAGER.GetCurrentCamera().Contains("@DEV.")) return;

            // Reset Camera
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LAlt) && FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.Space))
            {
                FABRICK_CAMERA_MANAGER.SetCamPos(Vector2.Zero);
                FABRICK_CAMERA_MANAGER.SetCameraZoom(1f);
                FABRICK_CAMERA_MANAGER.SetCameraAngle(0f);
            }

            bool ZoomLock = false;

            // Drag Handle
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LAlt) && FABRICK_INPUT.IsMouseDown(SDL3.SDL.MouseButtonFlags.Left))
            {
                ZoomLock = true;
                FABRICK_CAMERA_MANAGER.SetCamPos(CamPos + (CamGrabOffset - FABRICK_INPUT.GetMousePos_UI()) / CamZoom);
            }
            else
            {
                CamPos = FABRICK_CAMERA_MANAGER.GetActualCamPos();
                CamGrabOffset = FABRICK_INPUT.GetMousePos_UI();
                CamZoom = FABRICK_CAMERA_MANAGER.GetCamZoom();
            }

            // SideScroll Handle
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl) && FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LAlt)) // Make sure
            {
                ZoomLock = true;
                CamPos.X += FABRICK_INPUT.GetMouseScroll().X * DeltaTime;
                CamPos.Y -= FABRICK_INPUT.GetMouseScroll().Y * DeltaTime;
                FABRICK_CAMERA_MANAGER.SetCamPos(CamPos);
            }

            // Zoom Handle
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LAlt) && !ZoomLock)
            {
                float _CurrentZoom = FABRICK_CAMERA_MANAGER.GetCamZoom();
                _CurrentZoom += FABRICK_INPUT.GetMouseScroll().Y * DeltaTime / 3f;
                _CurrentZoom = FABRICK_MATH.Clamp(_CurrentZoom, 0f);
                FABRICK_CAMERA_MANAGER.SetCameraZoom(_CurrentZoom);
            }
        }
        private void DevSceneAssign()
        {
            FABRICK_SCENE_MANAGER.AssignScene("@DEV.SPRITE_ANIM_EDITOR", new FABRICK_SPRITE_ANIMATION_EDITOR_SCENE());
            FABRICK_SCENE_MANAGER.AssignScene("@DEV.SPRITE_SHEET_EDITOR", new FABRICK_SPRITE_SHEET_EDITOR_SCENE());
            FABRICK_SCENE_MANAGER.AssignScene("@DEV.DIALOGUE_EDITOR", new FABRICK_DIALOGUE_EDITOR_SCENE());
        }

        public static void SetFocusWindow(string WindowName)
        {
            if (ChangeableFocusWindow)
            {
                FocusWindow = WindowName;
            }
        }

        public static Vector2 GetWindowMinLimit()
        {
            return MinWindow;
        }
        public static Vector2 GetWindowMaxLimit()
        {
            return MaxWindow;
        }

        private void SetChangeAbleFocusWindowBackOnClick()
        {
            if (FABRICK_INPUT.IsMouseUp(SDL3.SDL.MouseButtonFlags.Left) && FABRICK_INPUT.IsMouseUp(SDL3.SDL.MouseButtonFlags.Right))
            {
                ChangeableFocusWindow = true;
            }
        }
    }
}
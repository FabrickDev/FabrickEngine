using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class SCENE_EDITOR_MANAGER
    {
        // Parts
        private static FABRICK_DEV_ENTITY_EDITOR OBJECTS_EDITOR_MENU = new();
        private static FABRICK_DEV_SCENE_EDITOR SCENE_EDITOR_MENU = new();

        private static int EditorType = 0;
        private static int MaxEditorType = 2;
        public static void Initialize()
        {
            OBJECTS_EDITOR_MENU.Initialize();
            SCENE_EDITOR_MENU.Initialize();
        }
        public static void Update()
        {
            Hotkeys();
            UpdateableCurrentEditorType();
            CurrentEditorTypeHandle();
            OBJECTS_EDITOR_MENU.Update();
            SCENE_EDITOR_MENU.Update();  
        }
        private static void Hotkeys()
        {
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl) && FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.Right))
            {
                EditorType++;
            }else if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl) && FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.Left))
            {
                EditorType--;
            }
            if (EditorType == -1)
            {
                EditorType = MaxEditorType - 1;
            }
            if (EditorType == MaxEditorType)
            {
                EditorType = 0;
            }
        }

        private static void UpdateableCurrentEditorType()
        {
            if (FABRICK_SCENE_MANAGER.GetCurrentScene().Contains("@DEV."))
            {
                OBJECTS_EDITOR_MENU.IsRun = false;
                SCENE_EDITOR_MENU.IsRun = false;
            }
            else
            {
                OBJECTS_EDITOR_MENU.IsRun = true;
                SCENE_EDITOR_MENU.IsRun = true;
            }
        }
        private static void CurrentEditorTypeHandle()
        {
            switch (EditorType)
            {
                case 0:
                    OBJECTS_EDITOR_MENU.IsRun = false;
                    SCENE_EDITOR_MENU.IsRun = true;
                break;
                case 1:
                    OBJECTS_EDITOR_MENU.IsRun = true;
                    SCENE_EDITOR_MENU.IsRun = false;
                break;
            }
        }
    }
}
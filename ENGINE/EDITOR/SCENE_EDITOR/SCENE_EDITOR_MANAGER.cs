using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class SCENE_EDITOR_MANAGER
    {
        // Parts
        private static FABRICK_DEV_SCENE_EDITOR SCENE_EDITOR_MENU = new();
        public static void Initialize()
        {
            SCENE_EDITOR_MENU.Initialize();
        }
        public static void Update()
        {
            UpdateableCurrentEditorType();
            SCENE_EDITOR_MENU.Update();
        }

        private static void UpdateableCurrentEditorType()
        {
            SCENE_EDITOR_MENU.IsRun = !FABRICK_SCENE_MANAGER.GetCurrentScene().Contains(EDITOR_MANAGER.DevKey);
        }
    }
}
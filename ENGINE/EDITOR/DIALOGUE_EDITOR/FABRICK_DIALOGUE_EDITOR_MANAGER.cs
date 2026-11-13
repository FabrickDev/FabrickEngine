using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DIALOGUE_EDITOR_MANAGER
    {
        // Parts
        internal static FABRICK_DIALOGUE_EDITOR DIALOGUE_EDITOR = new();
        internal static String FocusModule = "";
        public static void Initialize()
        {
            DIALOGUE_EDITOR.Initialize();
        }
        public static void Update()
        {
            UpdateableCurrentEditorType();
            DIALOGUE_EDITOR.Update();
        }

        private static void UpdateableCurrentEditorType()
        {
            DIALOGUE_EDITOR.IsRun = FABRICK_SCENE_MANAGER.GetCurrentScene().Contains(EDITOR_MANAGER.DevKey);
        }
    }
}
using System.Numerics;
using Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DIALOGUE_EDITOR_MANAGER
    {
        // Parts
        internal static FABRICK_DIALOGUE_EDITOR DIALOGUE_EDITOR = new();
        internal static String FocusModule = "";

        internal static string CurrentScenarioEditing = "";
        internal static List<FABRICK_DIALOGUE_EDITOR_MODULE> DialogueModuleList = new();
        internal static string ModuleWantToConnect = "";
        internal static float PinLineWidth = 3f;

        internal static Vector2 MinWorkingArea = new Vector2(0, 75);
        internal static Box ListWindowBox = new(Vector2.Zero, Vector2.Zero, Vector2.Zero);

        public static void Initialize()
        {
            DIALOGUE_EDITOR.Initialize();
        }
        public static void Update()
        {
            UpdateableCurrentEditorType();
            DIALOGUE_EDITOR.Update();
            ListWindowBox = DIALOGUE_EDITOR.LIST_WINDOW.GetWindowBox();
        }

        private static void UpdateableCurrentEditorType()
        {
            DIALOGUE_EDITOR.IsRun = FABRICK_SCENE_MANAGER.GetCurrentScene().Contains(EDITOR_MANAGER.DevKey);
        }
    }
}
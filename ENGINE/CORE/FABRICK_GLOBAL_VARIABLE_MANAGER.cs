using SDL3;

namespace Fabrick.ENGINE.CORE
{
    public enum GLOBAL_VARIABLE_TYPE
    {
        String,
        Number,
        Bool,
        Keyboard
    }
    public class FABRICK_GLOBAL_VARIABLE_DATA
    {
        public GLOBAL_VARIABLE_TYPE Type = GLOBAL_VARIABLE_TYPE.String;
        public string VariableValue_STR = "";
        public float VariableValue_NUM = 0f;
        public bool VariableValue_BOOL = false;
        public SDL.Scancode variableValue_KEYBOARD = SDL.Scancode.Unknown;
    }
    
    public class FABRICK_GLOBAL_VARIABLE_MANAGER
    {
        public static Dictionary<string, FABRICK_GLOBAL_VARIABLE_DATA> GlobalVariableDataList = new();

        // Engine Puspose
        internal static void Initialize()
        {
            GlobalVariableDataList["Engine.Fullscreen"] = (new FABRICK_GLOBAL_VARIABLE_DATA()
            {
                Type = GLOBAL_VARIABLE_TYPE.Bool,
                VariableValue_BOOL = false
            });
            GlobalVariableDataList["Engine.FullscreenKey"] = (new FABRICK_GLOBAL_VARIABLE_DATA()
            {
                Type = GLOBAL_VARIABLE_TYPE.Keyboard,
                variableValue_KEYBOARD = SDL.Scancode.F11
            });
            GlobalVariableDataList["Engine.VisualNovelKeyNext"] = (new FABRICK_GLOBAL_VARIABLE_DATA()
            {
                Type = GLOBAL_VARIABLE_TYPE.Keyboard,
                variableValue_KEYBOARD = SDL.Scancode.Space
            });
        }
    }
}
using System.Text.Json;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.VISUAL_NOVEL;
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
        public double VariableValue_NUM = 0f;
        public bool VariableValue_BOOL = false;
        public SDL.Scancode variableValue_KEYBOARD = SDL.Scancode.Unknown;
    }
    
    public class FABRICK_GLOBAL_VARIABLE_MANAGER
    {
        internal static string SaveDataPath = "";
        private static string SaveDataFileName = "GlobalVariables.json";
        public static Dictionary<string, FABRICK_GLOBAL_VARIABLE_DATA> GlobalVariableDataList = new();

        // Engine Puspose
        internal static void Initialize()
        {
            GlobalVariableDataList["Engine.Fullscreen"] = new()
            {
                Type = GLOBAL_VARIABLE_TYPE.Bool,
                VariableValue_BOOL = false
            };
            GlobalVariableDataList["Engine.FullscreenKey"] = new()
            {
                Type = GLOBAL_VARIABLE_TYPE.Keyboard,
                variableValue_KEYBOARD = SDL.Scancode.F11
            };
            GlobalVariableDataList["Engine.VisualNovelKeyNext"] = new()
            {
                Type = GLOBAL_VARIABLE_TYPE.Keyboard,
                variableValue_KEYBOARD = SDL.Scancode.Space
            };

            // Testing Purpose
            GlobalVariableDataList[ FABRICK_SCENARIO_MANAGER.GlobalVariableTag + ".TestingVar"] = new()
            {
                Type = GLOBAL_VARIABLE_TYPE.String,
                VariableValue_STR = "Hello"
            };
        }
        public static void SaveAllGlobalVariable()
        {
            FABRICK_DEBUG.Log($"[FABRICK_GLOBAL_VARIABLE_MANAGER] Writing Global Variables Data: {SaveDataPath} \\ {SaveDataFileName}");
            if (!Directory.Exists(SaveDataPath))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_GLOBAL_VARIABLE_MANAGER] Save Data Path Not Found: {SaveDataPath}");
                return;
            }
            List<FABRICK_GLOBAL_VARIABLE_DATA> _DataList = [.. GlobalVariableDataList.Values];
            string JSONString = JsonSerializer.Serialize(_DataList);
            File.WriteAllText(Path.Combine(SaveDataPath, SaveDataFileName), JSONString);
            FABRICK_DEBUG.Log($"[FABRICK_GLOBAL_VARIABLE_MANAGER] Writing Global Variables Data Done!!");
        }
        public static void LoadAllGlobalVariable()
        {
            
        }
    }
}
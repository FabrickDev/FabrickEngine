using System.CodeDom;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR;
using SDL3;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_SCENARIO_RWDATA
    {
        private static string AssetsPath = "";
        private static string FileExtension = ".json";

        internal static void SetAssetsPath(string path)
        {
            AssetsPath = Path.Combine(path, "Scenario");
        }
        internal static async Task Editor_Save(string ScenarioName, string ScenarioCategory, List<FABRICK_DIALOGUE_EDITOR_MODULE> ModuleList)
        {
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Write Scenario Data for: {ScenarioName}, Category: {ScenarioCategory}");
            // Check Directory
            string path = Path.Combine(AssetsPath, ScenarioCategory, ScenarioName + FileExtension);
            // if (!Directory.Exists(path))
            // {
            //     FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Creating new directory cuz ts didnt exist: {path}...");
            //     Directory.CreateDirectory(path);
            // }
            FABRICK_DEBUG.Log(path);
            if (File.Exists(path))
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Warning File Already Exist!: {path}...");
                if(!FABRICK_DEBUG.DialogueBoolInput("[FABRICK_SCENARIO_RWDATA] Warning", "File Already Exist, Overwrite?"))
                {
                    return;
                }
            }
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Writing...");
            // Get Data
            List<SCENARIO_MODULE_DATA> ModuleDataList = new();
            foreach (FABRICK_DIALOGUE_EDITOR_MODULE ThisModule in ModuleList)
            {
                ModuleDataList.Add(ThisModule.ModuleData);
            }
            var resolver = new DefaultJsonTypeInfoResolver();

            resolver.Modifiers.Add(typeInfo =>
            {
                foreach (var property in typeInfo.Properties)
                {
                    property.ShouldSerialize = (obj, value) =>
                    {
                        if (value == null)
                            return false;

                        if (value is string str)
                            return !string.IsNullOrWhiteSpace(str);

                        if (value is string str1)
                            return str1 != "0";

                        if (value is int i)
                            return i != 0;

                        if (value is float f)
                            return f != 0;

                        if (value is double d)
                            return d != 0;
                        
                        if (value is bool b)
                            return b != false;

                        if (value is SCENARIO_MODULE_TYPE SMT)
                            return SMT != 0;

                        if (value is GLOBAL_VARIABLE_TYPE GVT)
                            return GVT != 0;

                        if (value is SCENARIO_MODULE_CONDITION SMC)
                            return SMC != 0;

                        if (value is SDL.Scancode Scancode)
                            return Scancode != 0;

                        if (value is List<string> collection)
                            return collection.Count > 0;
                        
                        if (value is Dictionary<string, List<string>> collection1)
                            return collection1.Count > 0;

                        return true;
                    };
                }
            });
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = resolver,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            string ScenarioDataString = JsonSerializer.Serialize(ModuleDataList, options);
            FABRICK_DEBUG.Log("Content: " + ScenarioDataString);
            File.WriteAllText(path, ScenarioDataString);
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Write Scenario Data for: {ScenarioName}, Category: {ScenarioCategory} is finished!!");
        }
        internal static void Editor_Open(string ScenarioName, string ScenarioCategory, out List<FABRICK_DIALOGUE_EDITOR_MODULE> ModuleList)
        {
            string path = Path.Combine(AssetsPath, ScenarioCategory, ScenarioName + FileExtension);
            if (!File.Exists(path))
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Error File Doesn't Exist!: {path}...");
                FABRICK_DEBUG.SimpleMessageError($"File Doesn't Exist: {path}");
                ModuleList = new();
                return;
            }
            string ModuleDataString = File.ReadAllText(path);
            List<FABRICK_DIALOGUE_EDITOR_MODULE> _ModuleList = new();
            List<SCENARIO_MODULE_DATA> I = JsonSerializer.Deserialize<List<SCENARIO_MODULE_DATA>>(ModuleDataString) ?? new();
            foreach (SCENARIO_MODULE_DATA ThisModuleData in I)
            {
                FABRICK_DIALOGUE_EDITOR_MODULE _k = new(ThisModuleData.Type)
                {
                    ModuleData = ThisModuleData
                };
                _k.Pos.X = ThisModuleData.PosX;
                _k.Pos.Y = ThisModuleData.PosY;
                _k.ModuleId = ThisModuleData.ModuleId;
                _ModuleList.Add(_k);
            }

            // Handle Input Output Channel
            ModuleList = _ModuleList;
            return;
        }
        internal static List<SCENARIO_MODULE_DATA> LoadScenario(string ScenarioName, string ScenarioCategory)
        {
            string path = Path.Combine(AssetsPath, ScenarioCategory);
            if (!Directory.Exists(path))
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Directory didn't Exist!!: {path}");
                return new();
            }
            path = Path.Combine(path, ScenarioName + FileExtension);
            if (!File.Exists(path))
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] File didn't Exist!!: {path}");
                return new();
            }
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Scenario File reading: {path}");
            string ModuleDataString = File.ReadAllText(path);
            List<SCENARIO_MODULE_DATA> ModuleData = new();
            List<SCENARIO_MODULE_DATA> I = JsonSerializer.Deserialize<List<SCENARIO_MODULE_DATA>>(ModuleDataString) ?? new();
            foreach (SCENARIO_MODULE_DATA ThisModuleData in I)
            {
                foreach (string ThisKey in ThisModuleData.OutputToModule.Keys)
                {
                    ThisModuleData.OutputToModule[ThisKey] = ThisModuleData.OutputToModule[ThisKey].OrderBy(x => x.EndsWith("_Pauseable") ? 1 : 0).ToList();
                }
                ModuleData.Add(ThisModuleData);
            }

            // Start always on top
            ModuleData = ModuleData
                .OrderByDescending(x => x.ModuleId.Contains("_Start"))
                .ToList();
            // How to priority Output to Module is not Dialogue or Coice?
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_RWDATA] Scenario reading finished!!");
            return ModuleData;
        }
    }
}
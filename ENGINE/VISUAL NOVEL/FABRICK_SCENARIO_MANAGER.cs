using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.SCENE_MANAGER;
using SDL3;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public partial class FABRICK_SCENARIO_MANAGER
    {
        internal static FABRICK_ENGINE? ThisEngine;
        internal static string GlobalVariableTag = "ScenarioVariable";
        public static Dictionary<string, List<SCENARIO_MODULE_DATA>> ScenarioList = new();
        public static string CurrentScenario = "";
        internal static string VisualNovelSceneName = "";
        public static bool VisualNovelStarting = false;
        public static bool VisualNovelRunning = false;
        public static bool VisualNovelEnding = false;
        public static bool VisualNovelEnded = false;
        public static bool VisualNovelTextNext = true;

        // Coice Variable
        public static string CoiceSelected = "";
        public static bool CoiceSelectedNext = true;

        // Form Input Module
        public static string FormValue = "";
        public static bool FormValueNext = true;

        private static List<string> NextModuleToExecute = new();
        internal static void InitializeScenarioHandle(FABRICK_ENGINE Engine)
        {
            ThisEngine = Engine;
            // Testing Purpose
            FABRICK_CAMERA VN_CAMERA = new(Vector2.Zero, ThisEngine.GetLogicalResolution());
            FABRICK_CAMERA_MANAGER.AssignCamera("VISUAL NOVEL CAM", VN_CAMERA);
            AddScenario("Testing Scenario", "Test");
        }
        
        internal static void RunScenarioHandle()
        {
            // How to pause scenario?

            // Testing Scenario Run
            if (FABRICK_INPUT.IsKeyDown(SDL.Scancode.F2))
            {
                RunScenario("Testing Scenario", "Test");
            }
            if (FABRICK_INPUT.IsKeyPressed(SDL.Scancode.F3))
            {
                ReplaceScenarioModuleData("Testing Scenario", "Test", FABRICK_SCENARIO_RWDATA.LoadScenario("Testing Scenario", "Test"));
                RunScenario("Testing Scenario", "Test");
            }
            if (VisualNovelEnded)
            {
                VisualNovelCast = "";
                VisualNovelText = "";
                FABRICK_SCENE_MANAGER.StopOverlaySceneRun(VisualNovelSceneName);
                VisualNovelEnded = false;
            }
            if (string.IsNullOrEmpty(CurrentScenario))
            {
                return;
            }
            if (!ScenarioList.ContainsKey(CurrentScenario))
            {
                return;
            }
            if (VisualNovelRunning)
            {
                VisualNovelStarting = false;
            }
            if (!VisualNovelTextNext || VisualNovelStarting) // Maybe Can be Paused By: VisualNovelDialogue, Coice, and Input
            {
                return;
            }

            // Initialize Start
            if (!VisualNovelRunning)
            {
                SCENARIO_MODULE_DATA StartData = ScenarioList[CurrentScenario].FirstOrDefault(x => x.Type == SCENARIO_MODULE_TYPE.Start);
                SetNextListScenario(StartData.OutputToModule.Values.SelectMany(x => x).ToList());
                FABRICK_SCENE_MANAGER.StopOverlaySceneRun(VisualNovelSceneName);
                FABRICK_SCENE_MANAGER.AddOverlaySceneRunList(VisualNovelSceneName);
                VisualNovelStarting = true;
                VisualNovelEnding = false;
                CoiceList.Clear();
                SelectedCoice = "";
                FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Starting Scenario: " + CurrentScenario);
                return;
            }

            // Based NextModuleToExecute
            foreach (string ThisExecute in NextModuleToExecute)
            {
                SCENARIO_MODULE_DATA ThisData = ScenarioList[CurrentScenario].FirstOrDefault(x => x.ModuleId == ThisExecute);

                if (ThisData.Type == SCENARIO_MODULE_TYPE.ChangeCast)
                {
                    VisualNovelCurrentCast = ThisData.DialogueCast;
                    VisualNovelCurrentCastVariant = ThisData.DialogueCastVariant;
                    VisualNovelCast = VisualNovelCurrentCast;
                    //FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Change Cast: " + VisualNovelCurrentCast);
                    continue;
                }
                if (ThisData.Type == SCENARIO_MODULE_TYPE.ModifyCastVariant)
                {
                    ModifyCastVariant(ThisData.DialogueCast, ThisData.DialogueCastVariant);
                    continue;
                }
                if (ThisData.Type == SCENARIO_MODULE_TYPE.Coice)
                {
                    //CoiceList.Clear();
                    CoiceList = ThisData.CoiceList;
                    //FABRICK_DEBUG.Log("Waiting Coice...");
                    if(SelectedCoice.Length == 0) return;
                    for (int i = 0; i < CoiceList.Count; i++)
                    {
                        if (CoiceList[i] == SelectedCoice)
                        {
                            SetNextListScenario(ThisData.OutputToModule[Convert.ToString(i)]);
                        }
                    }
                    CoiceList.Clear();
                    SelectedCoice = "";
                    return;
                }
                if (ThisData.Type == SCENARIO_MODULE_TYPE.Dialogue)
                {
                    // Execute Dialogue Text
                    if (VisualNovelTextNext)
                    {
                        //VisualNovelCast = ThisScenarioModuleData.DialogueCast;
                        VisualNovelText = ThisData.DialogueText;
                        SetFocusCast(VisualNovelCurrentCast, VisualNovelCurrentCastVariant);
                        VisualNovelTextNext = false;
                        SetNextListScenario(ThisData.OutputToModule.Values.SelectMany(x => x).ToList());
                        FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Dialogue: " + VisualNovelText);
                        return;
                    }
                }
                if (ThisData.Type == SCENARIO_MODULE_TYPE.Compare)
                {
                    string STR_SecondVal = "";
                    double DBL_SecondVal = 0;
                    bool BOL_SecondVal = false;
                    SDL.Scancode KEY_SecondVal = SDL.Scancode.Unknown;

                    // Getting Second Data Variable
                    if (ThisData.UseConditionGlobalVariable)
                    {
                        switch (ThisData.GlobalVariableType)
                        {
                            case GLOBAL_VARIABLE_TYPE.String:
                                STR_SecondVal = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.ConditionGlobalVariableName].VariableValue_STR;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Number:
                                DBL_SecondVal = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.ConditionGlobalVariableName].VariableValue_NUM;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Bool:
                                BOL_SecondVal = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.ConditionGlobalVariableName].VariableValue_BOOL;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Keyboard:
                                KEY_SecondVal = FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.ConditionGlobalVariableName].variableValue_KEYBOARD;
                            break;
                        }
                    }
                    else
                    {
                        switch (ThisData.GlobalVariableType)
                        {
                            case GLOBAL_VARIABLE_TYPE.String:
                                STR_SecondVal = Convert.ToString(ThisData.ConditionModuleVariableValue);
                            break;
                            case GLOBAL_VARIABLE_TYPE.Number:
                                DBL_SecondVal = Convert.ToDouble(ThisData.ConditionModuleVariableValue);
                            break;
                            case GLOBAL_VARIABLE_TYPE.Bool:
                                BOL_SecondVal = Convert.ToBoolean(ThisData.ConditionModuleVariableValue);
                            break;
                            case GLOBAL_VARIABLE_TYPE.Keyboard:
                                SDL.Scancode[] _Key = Enum.GetValues<SDL.Scancode>();
                                KEY_SecondVal = _Key[Convert.ToInt16(ThisData.ConditionModuleVariableValue)];
                            break;
                        }
                    }

                    bool Satisfied = false;
                    switch (ThisData.GlobalVariableType)
                    {
                        case GLOBAL_VARIABLE_TYPE.String:
                            if(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.FirstGlobalVariableName].VariableValue_STR == STR_SecondVal) Satisfied = true;
                        break;
                        case GLOBAL_VARIABLE_TYPE.Number:
                            if(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.FirstGlobalVariableName].VariableValue_NUM == DBL_SecondVal) Satisfied = true;
                        break;
                        case GLOBAL_VARIABLE_TYPE.Bool:
                            if(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.FirstGlobalVariableName].VariableValue_BOOL == BOL_SecondVal) Satisfied = true;
                        break;
                        case GLOBAL_VARIABLE_TYPE.Keyboard:
                            if(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[ThisData.FirstGlobalVariableName].variableValue_KEYBOARD == KEY_SecondVal) Satisfied = true;
                        break;
                    }

                    if (Satisfied)
                    {
                        FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Module: {ThisData.ModuleId} as Comparator use Normal Route");
                        SetNextListScenario(ThisData.OutputToModule["Normal"]);
                    }
                    else
                    {
                        FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Module: {ThisData.ModuleId} as Comparator use Except Route");
                        SetNextListScenario(ThisData.OutputToModule["Except"]);
                    }
                    return;
                }
                if (ThisData.Type == SCENARIO_MODULE_TYPE.End)
                {
                    VisualNovelRunning = false;
                    VisualNovelStarting = false;
                    VisualNovelEnding = true;
                    FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] End Of Scenario: {CurrentScenario}");
                    //FABRICK_DEBUG.SimpleMessageInfo($"[FABRICK_SCENARIO_MANAGER] End of Scenario: {CurrentScenario}");
                    //ScenarioList[CurrentScenario][IndexScenarioData - 1].IsFinishedModule = true;
                    VisualNovelCast = "";
                    VisualNovelText = "";
                    CurrentScenario = "";
                }
            }
        }
        public static void SetFormValue(string Value)
        {
            FormValue = Value;
        }
    }
}
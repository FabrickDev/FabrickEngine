using System.Globalization;
using System.Runtime.CompilerServices;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_SCENARIO_MANAGER
    {
        public static Dictionary<string, List<SCENARIO_MODULE_DATA>> ScenarioList = new();
        public static Dictionary<string, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR> PortraitsCastList = new();
        public static string CurrentScenario = "";
        private static string VisualNovelSceneName = "";
        public static string VisualNovelCurrentCast = ""; // Control who speaking
        public static string VisualNovelCurrentCastVariant = "Normal";
        public static string VisualNovelCast = "Narrator"; // This actually just for display
        public static string VisualNovelText = "I'm Appearing...";
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
        internal static void InitializeScenarioHandle()
        {
            // Testing Purpose
            AddScenario("Testing Scenario", "Test");
        }
        // Portraits Cast
        public static void AssignPortraitCast(string Name, string DisplayName, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR PostraitsCastTemplate)
        {
            if (PortraitsCastList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Assign Postrais Cast: {Name} cus its already exist!!");
                return;
            }
            PortraitsCastList[Name] = PostraitsCastTemplate;
            PortraitsCastList[Name].DisplayName = DisplayName;
        }
        public static void SetFocusCast(string Cast, string Variant)
        {
            if (!PortraitsCastList.ContainsKey(Cast))
            {
                foreach (string ThisKey in PortraitsCastList.Keys)
                {
                    PortraitsCastList[ThisKey].SetPortraitVisibility(0.5f);
                }
                //FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Set Focus Cast: {Name} cus its doesn't exist!!");
                return;
            }
            foreach (string ThisKey in PortraitsCastList.Keys)
            {
                PortraitsCastList[ThisKey].SetPortraitVisibility(0.5f);
            }
            PortraitsCastList[Cast].SetPortraitVisibility(1f);
            PortraitsCastList[Cast].SetVariant(Cast, Variant);
        }
        public static void SetVisualNovelScene(string VNScene)
        {
            string _tmp = FABRICK_VISUAL_NOVEL_MANAGER.VisualNovelSceneName + VNScene;
            if (!FABRICK_SCENE_MANAGER.OverlaySceneList.ContainsKey(_tmp))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Set Visual Novel Scene: {VNScene} cus its already exist!!");
                return;
            }
            VisualNovelSceneName = _tmp;
        }
        public static void AddScenario(string ScenarioName, string ScenarioCategory)
        {
            if (!ScenarioList.ContainsKey(ScenarioName + "@" + ScenarioCategory))
            {
                ScenarioList[ScenarioName + "@" + ScenarioCategory] = new();
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Adding Scenario: {ScenarioName}, Category: {ScenarioCategory} cus its already exist!!");
            }
        }
        public static void ReplaceScenarioModuleData(string ScenarioName, string ScenarioCategory, List<SCENARIO_MODULE_DATA> ScenarioData)
        {
            string ThisScenario = ScenarioName + "@" + ScenarioCategory;
            if (ScenarioList.ContainsKey(ThisScenario))
            {
                ScenarioList[ThisScenario] = ScenarioData;
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot ReplaceScenarioModuleData cus scenario: {ScenarioName}, Category: {ScenarioCategory} doesn't exist!!");
                return;
            }
        }
        public static void RunScenario(string ScenarioName, string ScenarioCategory)
        {
            string ThisScenario = ScenarioName + "@" + ScenarioCategory;
            if (!ScenarioList.ContainsKey(ThisScenario))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Run Scenario: {ScenarioName}, Category: {ScenarioCategory} doesn't Exist!!");
                return;
            }
            if (CurrentScenario != ThisScenario)
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Run Scenario: {ScenarioName}, Category: {ScenarioCategory}");
                VisualNovelTextNext = true;
                VisualNovelText = "";
                NextModuleToExecute.Clear();
                CurrentScenario = ThisScenario;
                for (int i = 0; i < ScenarioList[ThisScenario].Count; i++)
                {
                    ScenarioList[CurrentScenario][i].IsFinishedModule = false;
                }
            }
        }
        internal static void RunScenarioHandle()
        {
            // How to pause scenario?

            // Testing Scenario Run
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl))
            {
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
                SetNextListScenario(StartData.OutputToModule);
                FABRICK_SCENE_MANAGER.AddOverlaySceneRunList(VisualNovelSceneName);
                VisualNovelStarting = true;
                VisualNovelEnding = false;
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
                    FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Change Cast: " + VisualNovelCurrentCast);
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
                        SetNextListScenario(ThisData.OutputToModule);
                        FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Dialogue: " + VisualNovelText);
                        return;
                    }
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
            // End Scenario Handle
            

            return;
            int IndexScenarioData = 0;
            foreach (SCENARIO_MODULE_DATA ThisScenarioModuleData in ScenarioList[CurrentScenario])
            {
                IndexScenarioData++;

                if (ThisScenarioModuleData.IsFinishedModule)
                {
                    continue;
                }
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Running Module: {ThisScenarioModuleData.ModuleId}");

                // Getting Start Module
                if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Start && !VisualNovelRunning)
                {
                    // What To Do?
                    //FABRICK_SCENE_MANAGER.ClearOverlaySceneRunList();
                    SetNextListScenario(ThisScenarioModuleData.OutputToModule);
                    FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Starting Scene VN: {VisualNovelSceneName}");
                    FABRICK_SCENE_MANAGER.AddOverlaySceneRunList(VisualNovelSceneName);
                    VisualNovelStarting = true;
                    VisualNovelEnding = false;
                    return;
                }
                
                // Other Module Execute
                foreach (string ThisScenarioKey in NextModuleToExecute)
                {
                    if (ThisScenarioModuleData.ModuleId != ThisScenarioKey)
                    {
                        continue;
                    }
                    ///////////////////?? Check Each Module
                    if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Dialogue)
                    {
                        // Execute Dialogue Text
                        if (VisualNovelTextNext)
                        {
                            //VisualNovelCast = ThisScenarioModuleData.DialogueCast;
                            VisualNovelText = ThisScenarioModuleData.DialogueText;
                            SetFocusCast(VisualNovelCurrentCast, VisualNovelCurrentCastVariant);

                            VisualNovelTextNext = false;
                            ScenarioList[CurrentScenario][IndexScenarioData - 1].IsFinishedModule = true;
                            SetNextListScenario(ThisScenarioModuleData.OutputToModule);
                            return;
                        }
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Compare)
                    {
                        switch (ThisScenarioModuleData.GlobalVariableType)
                        {
                            case CORE.GLOBAL_VARIABLE_TYPE.String:
                                if (ThisScenarioModuleData.Condition == SCENARIO_MODULE_CONDITION.Same)
                                {
                                    if (ThisScenarioModuleData.UseConditionGlobalVariable)
                                    {
                                        if (ThisScenarioModuleData.FirstGlobalVariableName == ThisScenarioModuleData.ConditionGlobalVariableName)
                                        {
                                            AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                        }
                                    }
                                    else
                                    {
                                        if (ThisScenarioModuleData.FirstGlobalVariableName == ThisScenarioModuleData.ConditionModuleVariableValue)
                                        {
                                            AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                        }
                                    }
                                }
                                else if (ThisScenarioModuleData.Condition == SCENARIO_MODULE_CONDITION.Not)
                                {
                                    if (ThisScenarioModuleData.UseConditionGlobalVariable)
                                    {
                                        if (ThisScenarioModuleData.FirstGlobalVariableName != ThisScenarioModuleData.ConditionGlobalVariableName)
                                        {
                                            AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                        }
                                    }
                                    else
                                    {
                                        if (ThisScenarioModuleData.FirstGlobalVariableName != ThisScenarioModuleData.ConditionModuleVariableValue)
                                        {
                                            AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                        }
                                    }
                                }
                                else
                                {
                                    FABRICK_DEBUG.SimpleMessageWarning("[FABRICK_SCENARIO_MANAGER] Cannot Compare String else Same Comparator");
                                }
                            break;
                            case CORE.GLOBAL_VARIABLE_TYPE.Number:
                                float Num1 = float.Parse(ThisScenarioModuleData.FirstGlobalVariableName, CultureInfo.InvariantCulture.NumberFormat);
                                float Num2 = 0f;
                                if (ThisScenarioModuleData.UseConditionGlobalVariable)
                                {
                                    Num2 = float.Parse(ThisScenarioModuleData.ConditionGlobalVariableName, CultureInfo.InvariantCulture.NumberFormat);
                                }
                                else
                                {
                                    Num2 = float.Parse(ThisScenarioModuleData.ConditionModuleVariableValue, CultureInfo.InvariantCulture.NumberFormat);
                                }
                                //Comparator
                                switch (ThisScenarioModuleData.Condition)
                                {
                                    case SCENARIO_MODULE_CONDITION.Same:
                                        if(Num1 == Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                    case SCENARIO_MODULE_CONDITION.Not:
                                        if(Num1 != Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                    case SCENARIO_MODULE_CONDITION.GreaterThan:
                                        if(Num1 > Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                    case SCENARIO_MODULE_CONDITION.GreaterThanEqual:
                                        if(Num1 >= Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                    case SCENARIO_MODULE_CONDITION.LessThan:
                                        if(Num1 < Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                    case SCENARIO_MODULE_CONDITION.LessThanEqual:
                                        if(Num1 <= Num2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                    break;
                                }
                            break;
                            case CORE.GLOBAL_VARIABLE_TYPE.Bool:
                                bool Boolean1 = bool.Parse(ThisScenarioModuleData.FirstGlobalVariableName);
                                bool Boolean2 = false;
                                if (ThisScenarioModuleData.UseConditionGlobalVariable)
                                {
                                    Boolean2 = bool.Parse(ThisScenarioModuleData.ConditionGlobalVariableName);
                                }
                                else
                                {
                                    Boolean2 = bool.Parse(ThisScenarioModuleData.ConditionModuleVariableValue);
                                }
                                if (ThisScenarioModuleData.Condition == SCENARIO_MODULE_CONDITION.Same)
                                {
                                    if(Boolean1 == Boolean2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                }
                                else if (ThisScenarioModuleData.Condition == SCENARIO_MODULE_CONDITION.Not)
                                {
                                    if(Boolean1 != Boolean2) AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                                }
                            break;
                        }
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.SetGlobalVariable)
                    {
                        FABRICK_GLOBAL_VARIABLE_DATA ThisIndex = new();
                        string _thisVariableKey = ThisScenarioModuleData.GlobalVariable;
                        if (!FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList.ContainsKey(_thisVariableKey))
                        {
                            FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] This Global Variable: {_thisVariableKey}, doesn't exist!!");
                            continue;
                        }
                        if (FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[_thisVariableKey].Type != ThisScenarioModuleData.SetGlobalVariableType)
                        {
                            FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Variable: {_thisVariableKey}, Cannot Set Global Variable Cuz the Type Is Not Same!!");
                            continue;
                        }
                        GLOBAL_VARIABLE_TYPE _thisDataType = ThisScenarioModuleData.SetGlobalVariableType;

                        switch (_thisDataType)
                        {
                            case GLOBAL_VARIABLE_TYPE.String:
                                FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[_thisVariableKey].VariableValue_STR = ThisScenarioModuleData.SetGlobalVariableValue_STR;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Number:
                                FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[_thisVariableKey].VariableValue_NUM = ThisScenarioModuleData.SetGlobalVariableValue_NUM;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Bool:
                                FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[_thisVariableKey].VariableValue_BOOL = ThisScenarioModuleData.SetGlobalVariableValue_BOOL;
                            break;
                            case GLOBAL_VARIABLE_TYPE.Keyboard:
                                FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList[_thisVariableKey].variableValue_KEYBOARD = ThisScenarioModuleData.SetGlobalVariableValue_KEYBOARD;
                            break;
                        }
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.ChangeScenario)
                    {
                        RunScenario(ThisScenarioModuleData.ScenarioName, ThisScenarioModuleData.ScenarioCategory);
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Coice)
                    {
                        foreach (string ThisCoice in ThisScenarioModuleData.CoiceList)
                        {
                            if (CoiceSelected == ThisCoice)
                            {
                                AddNextModuleList(ThisScenarioModuleData.CoiceOutputToModule[ThisCoice]);
                            }
                        }
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Form)
                    {
                        AddNextModuleList(ThisScenarioModuleData.OutputToModule);
                    }
                    else if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.ChangeCast)
                    {
                        VisualNovelCurrentCast = ThisScenarioModuleData.DialogueCast;
                        VisualNovelCurrentCastVariant = ThisScenarioModuleData.DialogueCastVariant;
                        FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] ChangeCast: {VisualNovelCurrentCast}, Variant: {VisualNovelCurrentCastVariant}");
                    }
                }

                // Getting End Module
                if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.End)
                {
                    VisualNovelRunning = false;
                    VisualNovelStarting = false;
                    VisualNovelEnding = true;
                    FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] End Of Scenario: {CurrentScenario}");
                    //FABRICK_DEBUG.SimpleMessageInfo($"[FABRICK_SCENARIO_MANAGER] End of Scenario: {CurrentScenario}");
                    ScenarioList[CurrentScenario][IndexScenarioData - 1].IsFinishedModule = true;
                    VisualNovelCast = "";
                    VisualNovelText = "";
                    CurrentScenario = "";
                }

                if (ScenarioList.ContainsKey(CurrentScenario))
                {
                    SetNextListScenario(ThisScenarioModuleData.OutputToModule);
                    ScenarioList[CurrentScenario][IndexScenarioData - 1].IsFinishedModule = true;
                }
            }
        }
        public static void SetCoiceValue(string Value)
        {
            CoiceSelected = Value;
        }
        public static void ResetFormValue()
        {
            FormValue = "";
        }
        public static void SetFormValue(string Value)
        {
            FormValue = Value;
        }
        private static void AddNextModuleList(List<string> ModuleList)
        {
            foreach (string ThisModuleId in ModuleList)
            {
                NextModuleToExecute.Add(ThisModuleId);
            }
        }
        private static void SetNextListScenario(List<string> Scenarios)
        {
            FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Next to Execute:");
            foreach (string thisMod in Scenarios)
            {
                FABRICK_DEBUG.Log(thisMod);
            }
            NextModuleToExecute.Clear();
            NextModuleToExecute = Scenarios.ToList();
        }
    }
}
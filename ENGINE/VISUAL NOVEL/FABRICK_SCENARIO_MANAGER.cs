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
        public static Dictionary<string, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR> PortraitsCastList= new();
        public static string CurrentScenario = "";
        private static string VisualNovelSceneName = "";
        public static string VisualNovelCast = "Narrator";
        public static string VisualNovelText = "I'm Appearing...";
        public static bool VisualNovelStarting = false;
        public static bool VisualNovelPrepared = false;
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
        private static bool PauseModuleExecuting = false;
        internal static void InitializeScenarioHandle()
        {
            // Testing Purpose
            AddScenario("Testing Scenario");
            List<SCENARIO_MODULE_DATA> ScenarioTesting = new();

            // Start module
            SCENARIO_MODULE_DATA Data1 = new();
            Data1.Type = SCENARIO_MODULE_TYPE.Start;
            Data1.ModuleId = "A1";
            Data1.OutputToModule.Add("A2");
            
            // Dialogue Module
            SCENARIO_MODULE_DATA Data2 = new();
            Data2.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data2.ModuleId = "A2";
            Data2.DialogueCast = "Feb";
            Data2.DialogueCastVariant = "Normal";
            Data2.DialogueText = "Scenario: 'Testing Scenario' Start.\nThis Dialogue Text From Module A2";
            Data2.OutputToModule.Add("A3");

            // Dialogue Module
            SCENARIO_MODULE_DATA Data3 = new();
            Data3.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data3.ModuleId = "A3";
            Data3.DialogueCast = "Hargo";
            Data3.DialogueCastVariant = "Normal";
            Data3.DialogueText = "This Dialogue Text From Module A3.\nWell... You Know, I Just Ask Hendrik Last Week for A Dinner.\nWe Ended Drunk Together.";
            Data3.OutputToModule.Add("A4");

            // Dialogue Module
            SCENARIO_MODULE_DATA Data4 = new();
            Data4.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data4.ModuleId = "A4";
            Data4.DialogueCast = "Hendrik";
            Data4.DialogueCastVariant = "Normal";
            Data4.DialogueText = "This Dialogue Text From Module A4 \nLorem ipsum dolor sit amet, consectetur adipiscing elit, \nsed do eiusmod tempor incididunt ut labore et dolore \nmagna aliqua. Ut enim ad minim veniam, \nquis nostrud exercitation ullamco laboris nisi ut aliquip ex ea";
            Data4.OutputToModule.Add("A5");
            
            // Dialogue Module
            SCENARIO_MODULE_DATA Data5 = new();
            Data5.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data5.ModuleId = "A5";
            Data5.DialogueCast = "Hendrik";
            Data5.DialogueCastVariant = "Surprised";
            Data5.DialogueText = "What just I say???";
            Data5.OutputToModule.Add("A6");
            
            // Dialogue Module
            SCENARIO_MODULE_DATA Data6 = new();
            Data6.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data6.ModuleId = "A6";
            Data6.DialogueCast = "Hendrik";
            Data6.DialogueCastVariant = "Surprised";
            Data6.DialogueText = "Who Is Jason Anyway?\nI See Them A Lot In The Folder 'Game/Assets/Scene'";
            Data6.OutputToModule.Add("A7");

            // Dialogue Module
            SCENARIO_MODULE_DATA Data7 = new();
            Data7.Type = SCENARIO_MODULE_TYPE.Dialogue;
            Data7.ModuleId = "A7";
            Data7.DialogueCast = "Narrator";
            Data7.DialogueText = "Aight Guys, This Scenario: 'Testing Scenario', will end here!!";
            Data7.OutputToModule.Add("A8");

            // End Module
            SCENARIO_MODULE_DATA Data8 = new();
            Data8.Type = SCENARIO_MODULE_TYPE.End;
            Data8.ModuleId = "A8";

            ScenarioTesting.Add(Data1);
            ScenarioTesting.Add(Data2);
            ScenarioTesting.Add(Data3);
            ScenarioTesting.Add(Data4);
            ScenarioTesting.Add(Data5);
            ScenarioTesting.Add(Data6);
            ScenarioTesting.Add(Data7);
            ScenarioTesting.Add(Data8);
            ReplaceScenarioModuleData("Testing Scenario", ScenarioTesting);
        }
        // Portraits Cast
        public static void AssignPortraitCast(string Name, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR PostraitsCastTemplate)
        {
            if (PortraitsCastList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Assign Postrais Cast: {Name} cus its already exist!!");
                return;
            }
            PortraitsCastList[Name] = PostraitsCastTemplate;
        }
        public static void SetFocusCast(string Cast, string Variant)
        {
            if (!PortraitsCastList.ContainsKey(Cast))
            {
                foreach (string ThisKey in PortraitsCastList.Keys)
                {
                    PortraitsCastList[ThisKey].SetPortraitVisibility(1f);
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
        public static void AddScenario(string ScenarioName)
        {
            if (!ScenarioList.ContainsKey(ScenarioName))
            {
                ScenarioList[ScenarioName] = new();
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Adding Scenario: {ScenarioName} cus its already exist!!");
            }
        }
        public static void ReplaceScenarioModuleData(string ScenarioName, List<SCENARIO_MODULE_DATA> ScenarioData)
        {
            if (ScenarioList.ContainsKey(ScenarioName))
            {
                ScenarioList[ScenarioName] = ScenarioData;
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot ReplaceScenarioModuleData cus scenario: {ScenarioName} doesn't exist!!");
                return;
            }
        }
        public static void RunScenario(string ScenarioName)
        {
            if (!ScenarioList.ContainsKey(ScenarioName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Run Scenario: {ScenarioName} doesn't Exist!!");
                return;
            }
            if (CurrentScenario != ScenarioName)
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Run Scenario: {ScenarioName}");
                VisualNovelTextNext = true;
                VisualNovelText = "";
                NextModuleToExecute.Clear();
                CurrentScenario = ScenarioName;
                for (int i = 0; i < ScenarioList[CurrentScenario].Count; i++)
                {
                    ScenarioList[CurrentScenario][i].IsFinishedModule = false;
                }
            }
        }
        internal static void RunScenarioHandle()
        {
            // How to pause scenario?
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl))
            {
                RunScenario("Testing Scenario");
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
            if (VisualNovelPrepared)
            {
                VisualNovelStarting = false;
            }
            if (!VisualNovelTextNext || VisualNovelStarting) // Maybe Can be Paused By: VisualNovelDialogue, Coice, and Input
            {
                return;
            }


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
                if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.Start && !VisualNovelPrepared)
                {
                    // What To Do?
                    //FABRICK_SCENE_MANAGER.ClearOverlaySceneRunList();
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
                            VisualNovelCast = ThisScenarioModuleData.DialogueCast;
                            VisualNovelText = ThisScenarioModuleData.DialogueText;
                            SetFocusCast(ThisScenarioModuleData.DialogueCast, ThisScenarioModuleData.DialogueCastVariant);

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
                        RunScenario(ThisScenarioModuleData.ScenarioName);
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
                }

                // Getting End Module
                if (ThisScenarioModuleData.Type == SCENARIO_MODULE_TYPE.End)
                {
                    VisualNovelPrepared = false;
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
            NextModuleToExecute.Clear();
            NextModuleToExecute = Scenarios.ToList();
        }
    }
}
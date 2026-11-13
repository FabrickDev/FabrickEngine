using System.Numerics;
using System.Text.Json.Serialization;
using Fabrick.ENGINE.CORE;
using SDL3;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public enum SCENARIO_MODULE_CONDITION
    {
        Same,
        Not,
        GreaterThan,
        GreaterThanEqual,
        LessThan,
        LessThanEqual
    }
    public enum SCENARIO_MODULE_TYPE
    {
        Start,
        End,
        Dialogue,
        Compare,
        SetGlobalVariable,
        ChangeScenario,
        Coice,
        Form,
        ChangeCast,
        ChangeCastVariant
    }
    public class SCENARIO_MODULE_DATA
    {
        public SCENARIO_MODULE_TYPE Type {get; set;} = SCENARIO_MODULE_TYPE.Start;
        public string ModuleId {get; set;} = "";
        public float PosX {get; set;} = 0f;
        public float PosY {get; set;} = 0f;
        public List<string> OutputToModule {get; set;} = new();
        public bool IsStart {get; set;} = false;
        public bool IsFinishedModule {get; set;} = false;

        // Dialogue Text
        public string DialogueCast {get; set;} = "";
        public string DialogueCastVariant {get; set;} = "";
        public string DialogueText {get; set;} = "";

        // Compare
        public string FirstGlobalVariableName {get; set;} = "";
        public GLOBAL_VARIABLE_TYPE GlobalVariableType {get; set;} = GLOBAL_VARIABLE_TYPE.String;
        public string ConditionGlobalVariableName {get; set;} = "";
        public string ConditionModuleVariableValue {get; set;} = "";
        public bool UseConditionGlobalVariable {get; set;} = false;
        public SCENARIO_MODULE_CONDITION Condition {get; set;} = SCENARIO_MODULE_CONDITION.Same;

        // SetGlobalVariable
        public string GlobalVariable {get; set;} = "";
        public GLOBAL_VARIABLE_TYPE SetGlobalVariableType {get; set;} = GLOBAL_VARIABLE_TYPE.String;
        public string SetGlobalVariableValue_STR {get; set;} = "";
        public float SetGlobalVariableValue_NUM {get; set;} = 0f;
        public bool SetGlobalVariableValue_BOOL {get; set;} = false;
        public SDL.Scancode SetGlobalVariableValue_KEYBOARD {get; set;} = SDL.Scancode.Unknown;

        // Change Scenario
        public string ScenarioName {get; set;} = "";
        public string ScenarioCategory {get; set;} = "";

        // Coice
        public List<string> CoiceList {get; set;} = new();
        public Dictionary<string, List<string>> CoiceOutputToModule {get; set;} = new();

        // Form
        public string FormValue {get; set;} = "";
    }
}
using System.Numerics;
using System.Runtime.CompilerServices;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.VISUAL_NOVEL;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal class INPUT_OUTPUT_CAHNNEL
    {
        public string ModuleId = "Purry PayGorn";
        public bool IsInput = true;
        public List<string> ConnectedFromToId = new();
        public bool Execute = false;
    }

    internal class FABRICK_DIALOGUE_EDITOR_MODULE_ITEM
    {
        private float SizeY = 30;
        private float FontSize = 4f;
        private bool OnceUpdate = true;
        private Vector2 MoverSize = new Vector2(250, 50);

        public string Scenario = "";
        public string ModuleId = "";
        public Vector2 Pos = Vector2.Zero;
        public List<INPUT_OUTPUT_CAHNNEL> InputOutputChannel = new();
        public List<string> CoiceList = new();
        public FABRICK_COLOR ItemColor_1 = FABRICK_COLOR.Black();
        public SCENARIO_MODULE_TYPE ThisType = SCENARIO_MODULE_TYPE.Start;

        public bool DialogueIsNext = false;
        public bool IsConditionSatisfied = false;

        public void Update(SCENARIO_MODULE_TYPE Type)
        {
            // Once Update Handle or Always Update?
            if (OnceUpdate)
            {
                InputOutputChannel.Clear();
                INPUT_OUTPUT_CAHNNEL _temp0 = new();
                INPUT_OUTPUT_CAHNNEL _temp1 = new();
                _temp0.ModuleId = ModuleId;
                _temp1.ModuleId = ModuleId;
                _temp0.IsInput = true;
                _temp1.IsInput = false;
                switch (ThisType)
                {
                    case SCENARIO_MODULE_TYPE.Start:
                        // Draw Label (Only Output)
                        InputOutputChannel.Add(_temp0);
                    break;
                    case SCENARIO_MODULE_TYPE.End:
                        // Draw Label (Only Input)
                        InputOutputChannel.Add(_temp1);
                    break;
                    case SCENARIO_MODULE_TYPE.Dialogue:
                        // Draw Form (Input and Output)
                        InputOutputChannel.Add(_temp0);
                        InputOutputChannel.Add(_temp1);
                    break;
                    case SCENARIO_MODULE_TYPE.Compare:
                        // Draw Form (Input)
                        // Draw Six Comparator (Output)
                        InputOutputChannel.Add(_temp0);
                        InputOutputChannel.Add(_temp1);
                    break;
                    case SCENARIO_MODULE_TYPE.SetGlobalVariable:
                        // Form (Input)
                        InputOutputChannel.Add(_temp0);
                    break;
                    case SCENARIO_MODULE_TYPE.ChangeScenario:
                        // Form (Input)
                        InputOutputChannel.Add(_temp0);
                    break;
                    case SCENARIO_MODULE_TYPE.Coice:
                        // Label "Input" (Input)
                        InputOutputChannel.Add(_temp0);
                        // Stack Multiple Form of Coice (Each Button has Output)
                        for (int i = 0; i < CoiceList.Count; i++)
                        {
                            INPUT_OUTPUT_CAHNNEL _temp2 = new();
                            _temp2.IsInput = false;
                            InputOutputChannel.Add(_temp2);
                        }
                        // Button to Add Coice
                    break;
                    case SCENARIO_MODULE_TYPE.Form:
                        // Form (Input & Output) Output are String
                    break;
                }
                OnceUpdate = false;
            }

            // Loop
            switch (ThisType)
            {
                case SCENARIO_MODULE_TYPE.Start:
                    // Draw Label (Only Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Start", Pos, FontSize, ItemColor_1, Algin.Center);
                    Execute();
                break;
                case SCENARIO_MODULE_TYPE.End:
                    // Draw Label (Only Input)
                    FABRICK_DRAW_TEXT.DrawDevText("End", Pos, FontSize, ItemColor_1, Algin.Center);
                break;
                case SCENARIO_MODULE_TYPE.Dialogue:
                    // Draw Form (Input and Output)
                    if (DialogueIsNext)
                    {
                        Execute();
                        DialogueIsNext = false;
                    }
                break;
                case SCENARIO_MODULE_TYPE.Compare:
                    // Draw Form (Input)
                    // Draw Six Comparator (Output)
                    if (IsConditionSatisfied)
                    {
                        Execute();
                        IsConditionSatisfied = false;
                    }
                break;
                case SCENARIO_MODULE_TYPE.SetGlobalVariable:
                    // Form (Input)
                    SetGlobalVariable("ThisVariable", "ThisValue");
                break;
                case SCENARIO_MODULE_TYPE.ChangeScenario:
                    // Form (Input)
                    ChangeScenario("Scenarioooooo");
                break;
                case SCENARIO_MODULE_TYPE.Coice:
                    // Label "Input" (Input)
                    // Stack Multiple Button of Coice (Each Button has Output)
                break;
                case SCENARIO_MODULE_TYPE.Form:
                    // Form (Input & Output) Output are String
                    string Form = "Well Later";
                    Execute(Form);
                break;
            }
        }
        public void AddInput(string ModuleId)
        {
            
        }
        public void SetDialogueIsFinish()
        {
            DialogueIsNext = true;
        }
        private void Execute()
        {
            foreach (INPUT_OUTPUT_CAHNNEL ThisChannel in InputOutputChannel)
            {
                if (ThisChannel.IsInput != false)
                {
                    // Get Module Id then run that module, but how?
                }
            }
        }
        private void Execute(string str)
        {
            
        }
        private void ChangeScenario(string ScenarioName)
        {
            
        }
        private void SetGlobalVariable(string Variable, string Value)
        {
            
        }
    }
}
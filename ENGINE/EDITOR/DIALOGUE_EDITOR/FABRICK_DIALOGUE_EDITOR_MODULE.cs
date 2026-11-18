using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.VISUAL_NOVEL;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal partial class FABRICK_DIALOGUE_EDITOR_MODULE
    {
        internal SCENARIO_MODULE_DATA ModuleData = new();
        private float FontSize = 4f;
        private float PinRadius = 20f;
        private bool OnceUpdate = true;
        private Vector2 MoverSize = new Vector2(200, 50);
        private Vector2 SizeModule = new Vector2(200, 100);
        private bool MoverAreaState = false;
        private Vector2 MoverAreaOffset = Vector2.Zero;
        public bool IsFocushed = true;
        public string ModuleId = "";
        public Vector2 Pos, PosDrag;
        public List<Vector2> InputPos = new(); 
        public List<Vector2> OutputPos = new();
        //public List<INPUT_OUTPUT_CAHNNEL> InputOutputChannel = new();
        public SCENARIO_MODULE_TYPE ThisType = SCENARIO_MODULE_TYPE.Start;
        public bool IsConditionSatisfied = false;
        private int IOYOffset = 0;
        private int IOYOffsetEachLine = 30;

        public FABRICK_DIALOGUE_EDITOR_MODULE(SCENARIO_MODULE_TYPE ModuleType)
        {
            ThisType = ModuleType;
            if(ModuleId.Length > 0) return; 

            // Ref From: https://www.geeksforgeeks.org/generate-a-random-string-of-given-size-in-c-sharp/
            // Creating object of random class
            Random rand = new Random();

            // Choosing the size of string
            // Using Next() string
            int stringlen = rand.Next(4, 10);
            int randValue;
            string str = "";
            char letter;
            for (int i = 0; i < stringlen; i++)
            {
                // Generating a random number.
                randValue = rand.Next(0, 26);

                // Generating random character by converting
                // the random number into character.
                letter = Convert.ToChar(randValue + 65);

                // Appending the letter to string.
                str = str + letter;
            }
            ModuleId = str;
            if (ThisType == SCENARIO_MODULE_TYPE.Dialogue || ThisType == SCENARIO_MODULE_TYPE.Coice || ThisType == SCENARIO_MODULE_TYPE.Form)
            {
                ModuleId += "_Pauseable";
            }
            if (ThisType == SCENARIO_MODULE_TYPE.Start)
            {
                ModuleId += "_Start";
            }
        }

        public void Update()
        {
            // Once Update Handle or Always Update?
            if (OnceUpdate)
            {
                PosDrag = Pos;
                ModuleData.Type = ThisType;
                ModuleData.ModuleId = ModuleId;
                switch (ThisType)
                {
                    case SCENARIO_MODULE_TYPE.Start:
                        {
                            // Draw Label (Only Output)
                        }
                    break;
                    case SCENARIO_MODULE_TYPE.End:
                        // Draw Label (Only Input)
                        {
                            
                        }
                    break;
                    case SCENARIO_MODULE_TYPE.Dialogue:
                        {
                            MoverSize = new Vector2(450, 50);
                            SizeModule = new Vector2(450, 250);
                            IOYOffset = 150;
                            // Draw Form (Input and Output)
                        }
                    break;
                    case SCENARIO_MODULE_TYPE.Compare:
                        {
                            // Draw Form (Input)
                            // Draw Six Comparator (Output)
                            MoverSize = new Vector2(300, 50);
                            SizeModule = new Vector2(300, 250);
                            IOYOffset = 100;
                        }
                    break;
                    case SCENARIO_MODULE_TYPE.SetGlobalVariable:
                        // Form (Input)
                        //InputOutputChannel.Add(_temp0);
                    break;
                    case SCENARIO_MODULE_TYPE.ChangeScenario:
                        // Form (Input)
                        //InputOutputChannel.Add(_temp0);
                    break;
                    case SCENARIO_MODULE_TYPE.Coice:
                        // Stack Multiple Form of Coice (Each Button has Output)
                        // Button to Add Coice
                        MoverSize = new Vector2(300, 50);
                        SizeModule = new Vector2(300, 160);
                        IOYOffset = 10;
                        if (ModuleData.CoiceList.Count > 0)
                        {
                            Vector2 _Size = new Vector2(SizeModule.X - 20, 30);
                            for (int i = 0; i < ModuleData.CoiceList.Count; i++)
                            {
                                IOYOffset += (int)_Size.Y;
                                SizeModule.Y += (int)_Size.Y;
                            }
                        }
                    break;
                    case SCENARIO_MODULE_TYPE.Form:
                        // Form (Input & Output) Output are String
                    break;
                    case SCENARIO_MODULE_TYPE.ChangeCast:
                        MoverSize = new Vector2(300, 50);
                        SizeModule = new Vector2(300, 160);
                        IOYOffset = 75;
                    break;
                    case SCENARIO_MODULE_TYPE.ModifyCastVariant:
                        MoverSize = new Vector2(300, 50);
                        SizeModule = new Vector2(300, 160);
                        IOYOffset = 75;
                    break;
                }
                OnceUpdate = false;
            }

            // Loop
            // Draw Module
            Pos = PosDrag;
            FocusHandler();
            if (IsFocushed)
            {
                // Set Camera Focus Pos
                if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.F))
                {
                    FABRICK_CAMERA_MANAGER.SetCamPos(Pos);
                }
                DragHandle();
                ModuleData.PosX = Pos.X;
                ModuleData.PosY = Pos.Y;
            }

            // Draw Moveable Window Area
            FABRICK_DRAW_SHAPE.DrawFillRect(Pos - new Vector2(SizeModule.X / 2, 0), MoverSize, EDITOR_MANAGER.Color1);
            // Draw Module Area
            FABRICK_DRAW_SHAPE.DrawFillRect(Pos + new Vector2(0, MoverSize.Y) - new Vector2(SizeModule.X / 2, 0), SizeModule, EDITOR_MANAGER.Color2);

            // Draw Input Output Channel
            switch (ThisType)
            {
                case SCENARIO_MODULE_TYPE.Start:
                    // Draw Label (Only Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Start", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    UpdateOutputPin(["Normal"], out _);
                break;
                case SCENARIO_MODULE_TYPE.End:
                    // Draw Label (Only Input)
                    FABRICK_DRAW_TEXT.DrawDevText("End", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    UpdateInputPin(out _);
                break;
                case SCENARIO_MODULE_TYPE.Dialogue:
                    // Draw Form (Input and Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Dialogue", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    // Form Text
                    Dialogue_DialogueButton();
                    UpdateInputPin(out _);
                    UpdateOutputPin(["Normal"], out _);
                break;
                case SCENARIO_MODULE_TYPE.Compare:
                    // Draw Form (Input)
                    // Draw Six Comparator (Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Comparator", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    Comparator();
                    UpdateInputPin(out _);
                    UpdateOutputPin(["Normal", "Except"], out _);
                break;
                case SCENARIO_MODULE_TYPE.SetGlobalVariable:
                    // Form (Input)
                break;
                case SCENARIO_MODULE_TYPE.ChangeScenario:
                    // Form (Input)
                break;
                case SCENARIO_MODULE_TYPE.Coice:
                    // Label "Input" (Input)
                    // Stack Multiple Button of Coice (Each Button has Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Coice", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    Coice(out float _CPosY);
                    UpdateInputPin(out _);
                    List<string> _tmp = new();
                    for (int i = 0; i < ModuleData.CoiceList.Count; i++)
                    {
                        _tmp.Add(Convert.ToString(i));
                    }
                    UpdateOutputPin(_tmp.ToArray(), out _);
                break;
                case SCENARIO_MODULE_TYPE.Form:
                    // Form (Input & Output) Output are String
                break;
                case SCENARIO_MODULE_TYPE.ChangeCast:
                    FABRICK_DRAW_TEXT.DrawDevText("Change Main Cast", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    ChangeCast();
                    UpdateInputPin(out _);
                break;
                case SCENARIO_MODULE_TYPE.ModifyCastVariant:
                    FABRICK_DRAW_TEXT.DrawDevText("Modify Cast", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    ModifyCast();
                    UpdateInputPin(out _);
                break;
            }
        }
        
        private void FocusHandler()
        {
            if (FABRICK_INPUT.IsKeyDown(SDL.Scancode.LAlt) || 
                FABRICK_INPUT.GetMousePos_UI().Y < FABRICK_DIALOGUE_EDITOR_MANAGER.MinWorkingArea.Y ||
                FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), FABRICK_DIALOGUE_EDITOR_MANAGER.ListWindowBox))
            {
                IsFocushed = false;
                return;
            }
            Box FocusArea = new Box(Pos - new Vector2(SizeModule.X / 2, 0), Vector2.Zero, MoverSize + new Vector2(0, SizeModule.Y));
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), FocusArea) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left) && !FABRICK_SCENARIO_MANAGER.VisualNovelRunning)
            {
                FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule = ModuleId;
            }
            if (FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule == ModuleId)
            {
                IsFocushed = true;
            }
            else
            {
                IsFocushed = false;
            }
        }
        
        private void DragHandle()
        {
            //FABRICK_DEBUG.Log($"[MOUSE]: {FABRICK_INPUT.GetMousePos()} | [POS]: {Pos} | [SIZE]: {MoverSize}");
            Box MoveableArea = new Box(Pos - new Vector2(SizeModule.X / 2, 0), Vector2.Zero, MoverSize);
            if (FABRICK_INPUT.IsKeyUp(SDL.Scancode.LAlt) && FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), MoveableArea) && FABRICK_INPUT.IsMouseDown(SDL.MouseButtonFlags.Left) && !MoverAreaState)
            {
                MoverAreaState = true;
                MoverAreaOffset = PosDrag - FABRICK_INPUT.GetMousePos();
            }
            if (FABRICK_INPUT.IsMouseReleased(SDL.MouseButtonFlags.Left))
            {
                MoverAreaState = false;
            }
            if (MoverAreaState)
            {
                PosDrag = FABRICK_INPUT.GetMousePos() + MoverAreaOffset;
            }
        }
    }
}
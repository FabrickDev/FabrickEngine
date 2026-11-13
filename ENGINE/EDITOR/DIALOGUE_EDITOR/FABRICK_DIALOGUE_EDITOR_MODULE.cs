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
    internal class FABRICK_DIALOGUE_EDITOR_MODULE_ITEM
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
        public Vector2 Pos, PosDrag, InputPos, OutputPos;
        //public List<INPUT_OUTPUT_CAHNNEL> InputOutputChannel = new();
        public List<string> CoiceList = new();
        public SCENARIO_MODULE_TYPE ThisType = SCENARIO_MODULE_TYPE.Start;
        public bool IsConditionSatisfied = false;
        private int IOYOffset = 0;
        private int IOYOffsetEachLine = 30;

        private bool ChangeIsMainCast = false;

        public FABRICK_DIALOGUE_EDITOR_MODULE_ITEM(SCENARIO_MODULE_TYPE ModuleType)
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
                    break;
                    case SCENARIO_MODULE_TYPE.Form:
                        // Form (Input & Output) Output are String
                    break;
                    case SCENARIO_MODULE_TYPE.ChangeCast:
                        MoverSize = new Vector2(200, 50);
                        SizeModule = new Vector2(200, 160);
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
                    UpdateOutputPin();
                break;
                case SCENARIO_MODULE_TYPE.End:
                    // Draw Label (Only Input)
                    FABRICK_DRAW_TEXT.DrawDevText("End", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    UpdateInputPin();
                break;
                case SCENARIO_MODULE_TYPE.Dialogue:
                    // Draw Form (Input and Output)
                    FABRICK_DRAW_TEXT.DrawDevText("Dialogue", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    // Form Text
                    Dialogue_DialogueButton();
                    UpdateInputPin();
                    UpdateOutputPin();
                break;
                case SCENARIO_MODULE_TYPE.Compare:
                    // Draw Form (Input)
                    // Draw Six Comparator (Output)
                    if (IsConditionSatisfied)
                    {
                        IsConditionSatisfied = false;
                    }
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
                break;
                case SCENARIO_MODULE_TYPE.Form:
                    // Form (Input & Output) Output are String
                break;
                case SCENARIO_MODULE_TYPE.ChangeCast:
                    FABRICK_DRAW_TEXT.DrawDevText("Change Cast", Pos, FontSize, EDITOR_MANAGER.Color3, Algin.Center);
                    ChangeCast();
                    UpdateInputPin();
                break;
                case SCENARIO_MODULE_TYPE.ChangeCastVariant:
                break;
            }
        }
        
        private void FocusHandler()
        {
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
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), MoveableArea) && FABRICK_INPUT.IsMouseDown(SDL.MouseButtonFlags.Left) && !MoverAreaState)
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
        
        private void UpdateInputPin()
        {
            FABRICK_DRAW_TEXT.DrawDevText("Input",  new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2((PinRadius * 1.75f) - (MoverSize.X / 2),  MoverSize.Y + (PinRadius * 1.25f)), 2f, EDITOR_MANAGER.Color3, Algin.Left);
            Box _temp = new Box(                    new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2((PinRadius / 2.00f) - (MoverSize.X / 2),  MoverSize.Y + (PinRadius * 1.25f)), Vector2.Zero, new Vector2(PinRadius));
            InputPos = _temp.GetPos() + new Vector2(PinRadius / 2, PinRadius / 2);
            FABRICK_VISUALIZE_SHAPE.Visualize(_temp, EDITOR_MANAGER.Color3, true);
            // Receive Input
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left) && FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect.Length > 0)
            {
                if (FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect == ModuleId)
                {
                    // Abort
                    FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect = "";
                }
                else
                {
                    for (int i = 0; i < FABRICK_DIALOGUE_EDITOR.DialogueModuleList.Count; i++)
                    {
                        if (FABRICK_DIALOGUE_EDITOR.DialogueModuleList[i].ModuleId == FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect)
                        {
                            FABRICK_DEBUG.Log($"[MODULE: {FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect} Was Connected to {ModuleId}]");
                            FABRICK_DIALOGUE_EDITOR.DialogueModuleList[i].ModuleData.OutputToModule.Add(ModuleId);
                            FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect = "";
                        }
                    }
                }
            }
        }
        
        private void UpdateOutputPin()
        {
            FABRICK_DRAW_TEXT.DrawDevText   ("Output",  new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2(MoverSize.X - (PinRadius * 1.25f) - (MoverSize.X / 2), MoverSize.Y + (PinRadius * 1.25f)) + new Vector2(-PinRadius * 3, 0), 2f, EDITOR_MANAGER.Color3, Algin.Right);
            Box _temp = new Box             (           new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2(MoverSize.X - (PinRadius * 1.25f) - (MoverSize.X / 2), MoverSize.Y + (PinRadius * 1.25f)),                                        Vector2.Zero, new Vector2(PinRadius));
            OutputPos = _temp.GetPos() + new Vector2(PinRadius / 2, PinRadius / 2);
            FABRICK_VISUALIZE_SHAPE.Visualize(_temp, EDITOR_MANAGER.Color3, true);
            
            // Send Channel
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                FABRICK_DEBUG.Log($"[MODULE: {ModuleId} Want to Connect]");
                FABRICK_DIALOGUE_EDITOR.ModuleWantToConnect = ModuleId;
            }
            
            // Remove Channel on That Module
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Right))
            {
                ModuleData.OutputToModule.Clear();
            }
        }

        private void ChangeCast()
        {
            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            Vector2 _Size = new Vector2(SizeModule.X - 20, 30);
            Vector2 _Pos2 = _Pos + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            Vector2 _Pos3 = _Pos2 + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            
            List<string> MainOrOpp = new ()
            {
                "Main",
                "Opponent"
            };
            List<string> LString = new();
            List<string> VariantString = new();
            // Draw BG
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos2, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos3, _Size, EDITOR_MANAGER.Color3);
            
            string ThisCast = "Select Cast...";
            string ThisCastVariant = "Select Cast Variant...";
            if (ModuleData.DialogueCast.Length > 0)
            {
                ThisCast = ModuleData.DialogueCast;
            }
            if (ModuleData.DialogueCastVariant.Length > 0)
            {
                ThisCastVariant = ModuleData.DialogueCastVariant;
            }
            // Draw Text
            if (!ChangeIsMainCast)
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Main Cast", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _);
            }
            else
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Opponent Cast", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _);
            }
            
            // Name Char
            foreach (string ThisChar in FABRICK_SCENARIO_MANAGER.PortraitsCastList.Keys)
            {
                if (FABRICK_SCENARIO_MANAGER.PortraitsCastList[ThisChar].IsMain() != ChangeIsMainCast)
                {
                    LString.Add(ThisChar);
                }
            }

            // Variant Char
            if (ModuleData.DialogueCast.Length > 0)
            {
                foreach (string ThisVariant in FABRICK_SCENARIO_MANAGER.PortraitsCastList[ModuleData.DialogueCast].Variant.Keys)
                {
                    string[] vari = ThisVariant.Split(".");
                    if (vari[0] != ModuleData.DialogueCast)
                    {
                        continue;
                    }

                    VariantString.Add(vari[1]);
                }
            }
            
            FABRICK_DRAW_TEXT.DrawTextAdvanced(ThisCast, "DefaultFont", _Pos2, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _);
            FABRICK_DRAW_TEXT.DrawTextAdvanced(ThisCastVariant, "DefaultFont", _Pos3, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _);
            // Input Response
            if(!IsFocushed) return;
            Box box1 = new(_Pos, Vector2.Zero, _Size);
            Box box2 = new(_Pos2, Vector2.Zero, _Size);
            Box box3 = new(_Pos3, Vector2.Zero, _Size);
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box1) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                ChangeIsMainCast = Convert.ToBoolean(FABRICK_DEBUG.DialogueList("Select Cast", "Cast: ", ref MainOrOpp));
                ModuleData.DialogueCast = "";
                ModuleData.DialogueCastVariant = "";
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box2) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                int val = FABRICK_DEBUG.DialogueList("Select Cast", "Cast: ", ref LString);
                if (val != -1)
                {
                    ModuleData.DialogueCast = LString[val];
                    ModuleData.DialogueCastVariant = "";
                    FABRICK_DEBUG.Log($"[{ModuleId}]Selected Cast: " + ModuleData.DialogueCast);
                }
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box3) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                int val = FABRICK_DEBUG.DialogueList("Select Cast Variant", "Variant: ", ref VariantString);
                if (val != -1)
                {
                    ModuleData.DialogueCastVariant = VariantString[val];
                    FABRICK_DEBUG.Log($"[{ModuleId}]Selected Cast Variant: " + ModuleData.DialogueCastVariant);
                }
            }

        }

        private void Dialogue_DialogueButton()
        {
            // Draw BG
            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            Vector2 _Size = new Vector2(SizeModule.X - 20, 110);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos, _Size, EDITOR_MANAGER.Color3);

            // Draw Text
            if (ModuleData.DialogueText.Length > 0)
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced(ModuleData.DialogueText, "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 40);
            }
            else
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Enter Dialogue Here...", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 40);
            }
            // Response
            if(!IsFocushed) return;
            Box box = new(_Pos, Vector2.Zero, _Size);
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                ModuleData.DialogueText = FABRICK_DEBUG.DialogueInputText("Dialogue", "Input text here...", ModuleData.DialogueText, true);
            }
        }
    }
}
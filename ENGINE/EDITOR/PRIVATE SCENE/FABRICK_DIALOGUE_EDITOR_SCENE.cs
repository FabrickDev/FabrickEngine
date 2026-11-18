using System.CodeDom;
using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.SCENE_MANAGER;
using Fabrick.ENGINE.VISUAL_NOVEL;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.SCENE
{
    public class FABRICK_DIALOGUE_EDITOR_SCENE : SCENE_DATA
    {
        private TAB_LIST_MENU FDEC_MENU = new();
        private TAB_LIST_MENU OPERATION_MENU = new();
        private List<TAB_LIST_ITEM> FDEC_ITEMS = new();
        private List<TAB_LIST_ITEM> OPERATION_ITEMS = new();

        private bool IsAddModule = false;
        private SCENARIO_MODULE_TYPE IsAddModule_Type = SCENARIO_MODULE_TYPE.Start;

        public FABRICK_DIALOGUE_EDITOR_SCENE()
        {
            SceneName = "@DEV.DIALOGUE_EDITOR";
        }
        public override void Initialize()
        {
            OPERATION_ITEMS.Clear();
            FDEC_ITEMS.Clear();
            FABRICK_ENTITY_MANAGER.RemoveCompleteAllEntityList();
            OPERATION_MENU.Initialize(new Vector2(0, 25));
            FDEC_MENU.Initialize(new Vector2(0, 50));
            // FABRICK DIALOGUE EDITOR OPERATOR
            OPERATION_ITEMS.Add(new TAB_LIST_ITEM("Tast", ()=>Action_TestEditor()));
            OPERATION_ITEMS.Add(new TAB_LIST_ITEM("Save", ()=>Action_SaveEditor()));
            // FABRICK DIALOGUE EDITOR MODULES
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Start"            , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Start)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("End"              , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.End)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Dialogue"         , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Dialogue)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Coice"            , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Coice)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Form"             , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Form)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Change Main Cast" , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.ChangeCast)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Modify Cast"      , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.ModifyCastVariant)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Comparator"       , ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Compare)));
            //ThisScene.Initialize();
        }
        public override void Update()
        {
            // Do not put here
            //ThisScene.Update();
        }
        public override void Render()
        {
            //FABRICK_DEBUG.Log("DIALOGUE EDITOR RENDERING!!!");
            FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.ForEach((ThisModule) => ThisModule.Update());
            if (FABRICK_SCENARIO_MANAGER.VisualNovelRunning)
            {
                FABRICK_DRAW_SHAPE.DrawFillRect_UI(Vector2.Zero, new Vector2(1920, 1080), new MATH.FABRICK_COLOR(0, 0, 0, 125));
            }
            DrawLinePinConnector();
            FDEC_MENU.Update(FDEC_ITEMS);
            OPERATION_MENU.Update(OPERATION_ITEMS);

            if (OnWorkingArea())
            {
                AddModuleHandle();
                RemoveModuleHandle();
            }
            //ThisScene.Render();
        }
        public override void Leave()
        {
            
        }
        private bool OnWorkingArea()
        {
            bool State = true;
            if (FABRICK_INPUT.IsKeyDown(SDL.Scancode.LAlt) || 
                FABRICK_INPUT.GetMousePos_UI().Y < FABRICK_DIALOGUE_EDITOR_MANAGER.MinWorkingArea.Y ||
                FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), FABRICK_DIALOGUE_EDITOR_MANAGER.ListWindowBox))
            {
                State = false;
            }
            return State;
        }
        private void AddModuleHandle()
        {
            if (IsAddModule && FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.Escape))
            {
                IsAddModule = false;
                return;
            }
            if (IsAddModule && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left))
            {
                IsAddModule = false;
                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.Add(new FABRICK_DIALOGUE_EDITOR_MODULE(IsAddModule_Type));
                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList[^1].Pos = FABRICK_INPUT.GetMousePos();
                FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR_SCENE] Add Module: " + IsAddModule_Type);
            }
        }
        private void RemoveModuleHandle()
        {
            if (FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule.Length == 0)
            {
                return;
            }
            if (FABRICK_INPUT.IsKeyPressed(SDL.Scancode.Delete) && FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.Any(x => x.ModuleId == FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule))
            {
                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.RemoveAll(x => x.ModuleId == FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule);
                foreach (FABRICK_DIALOGUE_EDITOR_MODULE ThisModule in FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList)
                {
                    foreach (string ThisKey in ThisModule.ModuleData.OutputToModule.Keys)
                    {
                        if(ThisModule.ModuleData.OutputToModule[ThisKey].Contains(FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule)) ThisModule.ModuleData.OutputToModule[ThisKey].Remove(FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule);
                    }
                }
                FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR_SCENE] Removed ModuleID: " + FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule);
                FABRICK_DIALOGUE_EDITOR_MANAGER.FocusModule = "";
            }
        }
        private void Action_AddModule(SCENARIO_MODULE_TYPE ModuleType)
        {
            IsAddModule = true;
            IsAddModule_Type = ModuleType;
        }
        private void Action_SaveEditor()
        {
            string[] ThisScenario = FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing.Split("@");
            _ = FABRICK_SCENARIO_RWDATA.Editor_Save(ThisScenario[0], ThisScenario[1], FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList);
        }
        private void Action_TestEditor()
        {
            Action_SaveEditor();
            string[] _t = FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing.Split("@");
            FABRICK_SCENARIO_MANAGER.ReplaceScenarioModuleData(_t[0], _t[1], FABRICK_SCENARIO_RWDATA.LoadScenario(_t[0], _t[1]));
            FABRICK_SCENARIO_MANAGER.RunScenario(_t[0], _t[1]);
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Testing Scenario: {_t[0]}, Category: {_t[1]}");
        }
        private void DrawLinePinConnector()
        {
            /*
            1. Check Each Module OutputTo
            2. Get Start Position
            3. Check Each OutputTo
            4. Check Each Module if Same as Every OutputTo;
            5. Get End Position
            6. Draw
            */
            foreach (FABRICK_DIALOGUE_EDITOR_MODULE ThisModule in FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList)
            {
                int i_Pos = -1;
                foreach (string ThisKeyOut in ThisModule.ModuleData.OutputToModule.Keys)
                {
                    i_Pos++;
                    Vector2 StartPos = ThisModule.OutputPos[i_Pos];
                    foreach (string ThisOutputToModule in ThisModule.ModuleData.OutputToModule[ThisKeyOut])
                    {
                        foreach (FABRICK_DIALOGUE_EDITOR_MODULE ThisModuleInput in FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList)
                        {
                            if (ThisModuleInput.ModuleId == ThisOutputToModule)
                            {
                                foreach (Vector2 EndPos in ThisModuleInput.InputPos)
                                {
                                    // 1/3
                                    FABRICK_DRAW_SHAPE.DrawFillRect(StartPos, new Vector2((EndPos.X - StartPos.X) / 2, FABRICK_DIALOGUE_EDITOR_MANAGER.PinLineWidth), EDITOR_MANAGER.Color1);
                                    // 2/3
                                    FABRICK_DRAW_SHAPE.DrawFillRect(new Vector2(StartPos.X + (EndPos.X - StartPos.X) / 2, StartPos.Y), new Vector2(FABRICK_DIALOGUE_EDITOR_MANAGER.PinLineWidth, EndPos.Y - StartPos.Y), EDITOR_MANAGER.Color1);
                                    // 3/3
                                    FABRICK_DRAW_SHAPE.DrawFillRect(EndPos, new Vector2((EndPos.X - StartPos.X) / 2, FABRICK_DIALOGUE_EDITOR_MANAGER.PinLineWidth) * -1, EDITOR_MANAGER.Color1);
                                }
                            }
                        }
                    }
                }                
            }
        }
    }
}
using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.SCENE_MANAGER;
using Fabrick.ENGINE.VISUAL_NOVEL;

namespace Fabrick.ENGINE.EDITOR.SCENE
{
    public class FABRICK_DIALOGUE_EDITOR_SCENE : SCENE_DATA
    {
        private TAB_LIST_MENU FDEC_MENU = new();
        private TAB_LIST_MENU OPERATION_MENU = new();
        private List<TAB_LIST_ITEM> FDEC_ITEMS = new();
        private List<TAB_LIST_ITEM> OPERATION_ITEMS = new();

        public FABRICK_DIALOGUE_EDITOR_SCENE()
        {
            SceneName = "@DEV.DIALOGUE_EDITOR";
            InitializeAction = Initialize_Func;
            RunningAction = Update_Func;
            RenderAction = Render_Func;
            LeaveAction = Leave_Func;
        }
        public void Initialize_Func()
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
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Start", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Start)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("End", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.End)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Dialogue", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Dialogue)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Coice", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Coice)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Form", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.Form)));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Change Cast", ()=>Action_AddModule(SCENARIO_MODULE_TYPE.ChangeCast)));
            //ThisScene.Initialize();
        }
        public void Update_Func()
        {
            // Do not put here
            //ThisScene.Update();
        }
        public void Render_Func()
        {
            FABRICK_DIALOGUE_EDITOR.DialogueModuleList.ForEach((ThisModule) => ThisModule.Update());
            if (FABRICK_SCENARIO_MANAGER.VisualNovelRunning)
            {
                FABRICK_DRAW_SHAPE.DrawFillRect_UI(Vector2.Zero, new Vector2(1920, 1080), new MATH.FABRICK_COLOR(0, 0, 0, 125));
            }
            DrawLinePinConnector();
            FDEC_MENU.Update(FDEC_ITEMS);
            OPERATION_MENU.Update(OPERATION_ITEMS);
            //ThisScene.Render();
        }
        public void Leave_Func()
        {
            
        }
        private void Action_AddModule(SCENARIO_MODULE_TYPE ModuleType)
        {
            FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR_SCENE] Add Module: " + ModuleType);
            FABRICK_DIALOGUE_EDITOR.DialogueModuleList.Add(new FABRICK_DIALOGUE_EDITOR_MODULE_ITEM(ModuleType));
        }
        private void Action_SaveEditor()
        {
            string[] ThisScenario = FABRICK_DIALOGUE_EDITOR.CurrentScenarioEditing.Split("@");
            _ = FABRICK_SCENARIO_RWDATA.Editor_Save(ThisScenario[0], ThisScenario[1], FABRICK_DIALOGUE_EDITOR.DialogueModuleList);
        }
        private void Action_TestEditor()
        {
            Action_SaveEditor();
            string[] _t = FABRICK_DIALOGUE_EDITOR.CurrentScenarioEditing.Split("@");
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
            foreach (FABRICK_DIALOGUE_EDITOR_MODULE_ITEM ThisModule in FABRICK_DIALOGUE_EDITOR.DialogueModuleList)
            {
                Vector2 StartPos = ThisModule.OutputPos;
                foreach (string ThisOutput in ThisModule.ModuleData.OutputToModule)
                {
                    foreach (FABRICK_DIALOGUE_EDITOR_MODULE_ITEM ThisModuleIn in FABRICK_DIALOGUE_EDITOR.DialogueModuleList)
                    {
                        if (ThisModuleIn.ModuleId == ThisOutput)
                        {
                            Vector2 EndPos = ThisModuleIn.InputPos;
                            // 1/3
                            FABRICK_DRAW_SHAPE.DrawFillRect(StartPos, new Vector2((EndPos.X - StartPos.X) / 2, FABRICK_DIALOGUE_EDITOR.PinLineWidth), EDITOR_MANAGER.Color1);
                            // 2/3
                            FABRICK_DRAW_SHAPE.DrawFillRect(new Vector2(StartPos.X + (EndPos.X - StartPos.X) / 2, StartPos.Y), new Vector2(FABRICK_DIALOGUE_EDITOR.PinLineWidth, EndPos.Y - StartPos.Y), EDITOR_MANAGER.Color1);
                            // 3/3
                            FABRICK_DRAW_SHAPE.DrawFillRect(EndPos, new Vector2((EndPos.X - StartPos.X) / 2, FABRICK_DIALOGUE_EDITOR.PinLineWidth) * -1, EDITOR_MANAGER.Color1);
                        }
                    }
                }
            }
        }
    }
}
using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.SCENE_MANAGER;
using Fabrick.ENGINE.VISUAL_NOVEL;
using SDL3;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DIALOGUE_EDITOR
    {
        internal LIST_WINDOW LIST_WINDOW = new();
        private static List<MENU_ITEM_ENTITY> ScenarioList = new List<MENU_ITEM_ENTITY>();
        public bool IsRun = true;

        public void Initialize()
        {
            LIST_WINDOW.Initialize("Scenario List", new Vector2(20, 75));
            FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR] Initialized");
        }
        public void Update()
        {
            if (IsRun)
            {
                // Cancel Module Pin Connect 
                if (FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect.Length > 0 && FABRICK_INPUT.IsKeyDown(SDL.Scancode.Escape))
                {
                    FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect = "";
                }
                LIST_WINDOW.Update(ScenarioList);
                EDITOR_MANAGER.SetFocusWindow(LIST_WINDOW.OnFocusClick());
                UpdateLatestItem();
                SaveScenarioHandle();
            }
            else
            {
                foreach (MENU_ITEM_ENTITY ThisKey in ScenarioList)
                {
                    ScenarioList[ScenarioList.IndexOf(ThisKey)].IsFocused = false;
                }
            }
        }
        private void UpdateLatestItem()
        {
            ScenarioList.Clear();

            // Create Scenario List
            foreach (String ThisScenario in FABRICK_SCENARIO_MANAGER.ScenarioList.Keys)
            {
                MENU_ITEM_ENTITY ThisAdd = new(
                    ThisScenario,
                    0,
                    "",
                    false,
                    0,
                    ()=>ActionOnClick(ThisScenario)
                );
                ScenarioList.Add(ThisAdd);
            }

            // Focus Doing
            foreach (MENU_ITEM_ENTITY ThisKey in ScenarioList)
            {
                ScenarioList[ScenarioList.IndexOf(ThisKey)].IsFocused = false;
                if (ScenarioList[ScenarioList.IndexOf(ThisKey)].Name == FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing)
                {
                    ScenarioList[ScenarioList.IndexOf(ThisKey)].IsFocused = true;
                }
            }
        }

        private void ActionOnClick(string ScenarioName)
        {
            if (FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing != ScenarioName)
            {
                FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR] VN Scene: " + FABRICK_SCENARIO_MANAGER.VisualNovelSceneName);
                if (FABRICK_SCENE_MANAGER.OverlaySceneList.ContainsKey(FABRICK_SCENARIO_MANAGER.VisualNovelSceneName) && FABRICK_SCENE_MANAGER.OverlaySceneList[FABRICK_SCENARIO_MANAGER.VisualNovelSceneName].Initialized == false)
                {
                    FABRICK_SCENE_MANAGER.OverlaySceneList[FABRICK_SCENARIO_MANAGER.VisualNovelSceneName].SceneInitialize();
                }
                
                FABRICK_DEBUG.Log("[FABRICK_DIALOGUE_EDITOR] Set Current Scenario Editing to: " + ScenarioName);
                FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing = ScenarioName;
                // Clear First
                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.Clear();
                // Replace All ModuleList
                string[] ThisScenario = FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing.Split("@");
                FABRICK_SCENARIO_RWDATA.Editor_Open(ThisScenario[0], ThisScenario[1], out FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList);
            }
        }

        internal void SaveScenarioHandle()
        {
            if (FABRICK_INPUT.IsKeyDown(SDL.Scancode.LCtrl) && FABRICK_INPUT.IsKeyPressed(SDL.Scancode.S))
            {
                string[] ThisScenario = FABRICK_DIALOGUE_EDITOR_MANAGER.CurrentScenarioEditing.Split("@");
                _ = FABRICK_SCENARIO_RWDATA.Editor_Save(ThisScenario[0], ThisScenario[1], FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList);
            }
        }
    }
}
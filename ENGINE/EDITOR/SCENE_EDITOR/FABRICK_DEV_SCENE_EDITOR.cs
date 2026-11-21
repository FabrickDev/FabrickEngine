using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DEV_SCENE_EDITOR
    {
        public LIST_WINDOW LIST_WINDOW = new();
        internal static List<MENU_ITEM_ENTITY> SceneList = new List<MENU_ITEM_ENTITY>();
        public bool IsRun = true;
        private string CurrentSceneFocus = "";

        public void Initialize()
        {
            LIST_WINDOW.Initialize("Scene List", new Vector2(20, 20));
            LIST_WINDOW.EnableAddingList(true, DataType.Scene);
            InitialFocus();
        }
        public void Update()
        {
            if (IsRun)
            {
                LIST_WINDOW.Update(SceneList);
                EDITOR_MANAGER.SetFocusWindow(LIST_WINDOW.OnFocusClick());
                UpdateLatestItem();
                SaveHandle();
                DeleteHandle();
            }
            else
            {
                foreach (MENU_ITEM_ENTITY ThisKey in SceneList)
                {
                    SceneList[SceneList.IndexOf(ThisKey)].IsFocused = false;
                }
            }
        }
        private void SaveHandle()
        {
            if (FABRICK_INPUT.IsKeyDown(SDL3.SDL.Scancode.LCtrl) && FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.S))
            {
                _ = FABRICK_SCENE_RWDATA.WriteSceneData();
            }
        }
        private void DeleteHandle()
        {
            if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.Delete))
            {
                string CurrentScene = CurrentSceneFocus.Split("@")[1];
                string BackupCurrentScene = CurrentScene;
                string BeforeScene = ""; //Proprity This
                string AfterScene = "";
                bool GetAfterSceneName = false;
                int Iterator = 0;

                foreach (string ThisKey in FABRICK_SCENE_MANAGER.SceneList.Keys)
                {
                    if(ThisKey.Contains(EDITOR_MANAGER.DevKey)) continue;
                    
                    if (GetAfterSceneName)
                    {
                        AfterScene = ThisKey;
                        GetAfterSceneName = false;
                    }
                    if (ThisKey != CurrentScene)
                    {
                        BeforeScene = ThisKey;
                    }
                    else
                    {
                        if (Iterator == 0)
                        {
                            GetAfterSceneName = true;
                        }
                    }
                    Iterator++;
                }

                if (BeforeScene.Length > 0)
                {
                    FABRICK_SCENE_MANAGER.SetCurrentScene(BeforeScene);
                    CurrentSceneFocus = "NonOverlay@" + BeforeScene;
                }
                else
                {
                    FABRICK_SCENE_MANAGER.SetCurrentScene(AfterScene);
                    CurrentSceneFocus = "NonOverlay@" + AfterScene;
                }
                FABRICK_SCENE_MANAGER.RemoveScene(BackupCurrentScene);
            }
        }
        private void InitialFocus()
        {
            foreach (MENU_ITEM_ENTITY ThisKey in SceneList)
            {
                SceneList[SceneList.IndexOf(ThisKey)].IsFocused = false;
                if (SceneList[SceneList.IndexOf(ThisKey)].Name == "NonOverlay@" + FABRICK_SCENE_MANAGER.GetCurrentScene())
                {
                    SceneList[SceneList.IndexOf(ThisKey)].IsFocused = true;
                }
            }
        }
        private void UpdateLatestItem()
        {
            SceneList.Clear();

            // Create Scene List
            string ExtraTag = "NonOverlay@";
            foreach (SCENE_DATA ThisSceneData in FABRICK_SCENE_MANAGER.SceneList.Values)
            {
                if (!ThisSceneData.SceneName.Contains(EDITOR_MANAGER.DevKey))
                {
                    MENU_ITEM_ENTITY ThisAdd = new(
                        ExtraTag + ThisSceneData.SceneName,
                        0,
                        "",
                        false,
                        0,
                        ()=>ActionOnClick(ThisSceneData.SceneName)
                    );
                    SceneList.Add(ThisAdd);
                }
            }
            ExtraTag = "Overlay";
            foreach (SCENE_DATA ThisSceneData in FABRICK_SCENE_MANAGER.OverlaySceneList.Values)
            {
                if (!ThisSceneData.SceneName.Contains(EDITOR_MANAGER.DevKey))
                {
                    MENU_ITEM_ENTITY ThisAdd = new(
                        ExtraTag + ThisSceneData.SceneName,
                        0,
                        "",
                        false,
                        1,
                        ()=>ActionOnClick(ThisSceneData.SceneName)
                    );
                    SceneList.Add(ThisAdd);
                }
            }

            // Focus Doing
            foreach (MENU_ITEM_ENTITY ThisItem in SceneList)
            {
                ThisItem.IsFocused = false;
                if (ThisItem.Name == CurrentSceneFocus)
                {
                    SceneList[SceneList.IndexOf(ThisItem)].IsFocused = true;
                }
            }
        }

        private void ActionOnClick(string SceneName)
        {
            if (FABRICK_SCENE_MANAGER.IsOverlayScene(SceneName))
            {
                FABRICK_SCENE_MANAGER.ClearOverlaySceneRunList();
                FABRICK_SCENE_MANAGER.AddOverlaySceneRunList(SceneName);
                FABRICK_SCENE_MANAGER.StopAllNonOverlayScene();
                CurrentSceneFocus = "Overlay" + SceneName;
            }
            else
            {
                FABRICK_SCENE_MANAGER.ClearOverlaySceneRunList();
                FABRICK_SCENE_MANAGER.StartAllNonOverlayScene();
                EDITOR_MANAGER.LastSceneUsed = SceneName;
                FABRICK_SCENE_MANAGER.SetCurrentScene(SceneName);
                CurrentSceneFocus = "NonOverlay@" + SceneName;
            }
        }
    }
}
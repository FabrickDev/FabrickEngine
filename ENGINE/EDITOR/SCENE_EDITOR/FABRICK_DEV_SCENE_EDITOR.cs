using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DEV_SCENE_EDITOR
    {
        public LIST_WINDOW LIST_WINDOW = new();
        private static List<MENU_ITEM_ENTITY> SceneList = new List<MENU_ITEM_ENTITY>();
        public bool IsRun = true;

        public void Initialize()
        {
            LIST_WINDOW.Initialize("Scene List", new Vector2(20, 20));
        }
        public void Update()
        {
            if (IsRun)
            {
                LIST_WINDOW.Update(SceneList);
                EDITOR_MANAGER.SetFocusWindow(LIST_WINDOW.OnFocusClick());
                UpdateLatestItem();
            }
            else
            {
                foreach (MENU_ITEM_ENTITY ThisKey in SceneList)
                {
                    SceneList[SceneList.IndexOf(ThisKey)].IsFocused = false;
                }
            }
        }
        private void UpdateLatestItem()
        {
            SceneList.Clear();

            // Create Scene List
            foreach (SCENE_DATA ThisSceneData in FABRICK_SCENE_MANAGER.SceneList.Values)
            {
                if (!ThisSceneData.SceneName.Contains(EDITOR_MANAGER.DevKey))
                {
                    MENU_ITEM_ENTITY ThisAdd = new(
                        ThisSceneData.SceneName,
                        0,
                        "",
                        false,
                        0,
                        ()=>ActionOnClick(ThisSceneData.SceneName)
                    );
                    SceneList.Add(ThisAdd);
                }
            }
            foreach (SCENE_DATA ThisSceneData in FABRICK_SCENE_MANAGER.OverlaySceneList.Values)
            {
                if (!ThisSceneData.SceneName.Contains(EDITOR_MANAGER.DevKey))
                {
                    MENU_ITEM_ENTITY ThisAdd = new(
                        ThisSceneData.SceneName,
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
            foreach (MENU_ITEM_ENTITY ThisKey in SceneList)
            {
                SceneList[SceneList.IndexOf(ThisKey)].IsFocused = false;
                if (SceneList[SceneList.IndexOf(ThisKey)].Name == FABRICK_SCENE_MANAGER.GetCurrentScene())
                {
                    SceneList[SceneList.IndexOf(ThisKey)].IsFocused = true;
                }
            }
        }

        private void ActionOnClick(string SceneName)
        {
            if (FABRICK_SCENE_MANAGER.IsOverlayScene(SceneName))
            {
                FABRICK_SCENE_MANAGER.ClearOverlaySceneRunList();
                FABRICK_SCENE_MANAGER.AddOverlaySceneRunList(SceneName);
            }
            else
            {
                EDITOR_MANAGER.LastSceneUsed = SceneName;
                FABRICK_SCENE_MANAGER.SetCurrentScene(SceneName);
                FABRICK_SCENE_MANAGER.RunScene(SceneName);
                FABRICK_SCENE_MANAGER.UpdateScene(SceneName);
            }
        }
    }
}
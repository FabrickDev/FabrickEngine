using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class FABRICK_SCENE_MANAGER
    {
        public static string CurrentSceneName = "";
        private static bool IsPauseAllAcene = false;
        private static bool IsPauseAllOverlayScene = false;
        private static bool IsPauseAllAceneOnceUpdate = true;
        private static bool IsPauseAllOverlaySceneOnceUpdate = true;
        private static string OverlayScene_AlwaysOnTop = "";
        public static Dictionary<string, SCENE_DATA> SceneList = new();
        public static Dictionary<string, SCENE_DATA> OverlaySceneList = new();
        public static List<string> OverlaySceneRunList = new();

        public static void AssignScene(string Name, bool IsMain)
        {
            bool _IsMain = IsMain;
            if (SceneList.Count == 0)
            {
                _IsMain = IsMain;
            }
            else
            {
                foreach (SCENE_DATA ThisScene in SceneList.Values)
                {
                    if (ThisScene.IsMain == true)
                    {
                        _IsMain = false;
                    }
                }
            }
            
            if (SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to Assign new Scene: {Name} already exist!");
                return;
            }
            if (_IsMain)
            {
                SetCurrentScene(Name);
            }
            SceneList[Name] = new FABRICK_SCENE(Name);
            SceneList[Name].SceneName = Name;
            SceneList[Name].IsMain = _IsMain;
            FABRICK_DEBUG.Log("[SCENE MANAGER] Added Scene: " + Name);
        }
        public static void AssignScene(string Name)
        {
            AssignScene(Name, false);
        }
        public static void AssignScene(string Name, SCENE_DATA Scene, bool IsMain = false, bool IsOverlay = false)
        {
            if (IsOverlay)
            {
                if (!OverlaySceneList.ContainsKey(Name))
                {
                    OverlaySceneList[Name]              = Scene;
                    OverlaySceneList[Name].SceneName    = Name;
                    OverlaySceneList[Name].IsMain       = false;
                    OverlaySceneList[Name].IsOverlay    = true;
                    FABRICK_DEBUG.Log("[SCENE MANAGER] Added Overlay Scene: " + Name);
                }
                else
                {
                    FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to Assign new Scene: {Name} already exist!");
                }
                return;
            }

            bool _IsMain = false;
            if (SceneList.Count == 0)
            {
                _IsMain = IsMain;
            }
            foreach (SCENE_DATA ThisScene in SceneList.Values)
            {
                if (ThisScene.IsMain == true)
                {
                    _IsMain = false;
                }
            }
            if (SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to Assign new Scene: {Name} already exist!");
                return;
            }

            SceneList[Name] = Scene;
            SceneList[Name].SceneName = Name;
            SceneList[Name].IsMain = _IsMain;
            FABRICK_DEBUG.Log("[SCENE MANAGER] Added Scene: " + Name);
        }
        public static void InitializeScene(string Name)
        {
            bool _found = false;
            bool _isOverlay = false;
            if (SceneList.ContainsKey(Name))
            {
                _found = true;
            }
            if (OverlaySceneList.ContainsKey(Name))
            {
                _found = true;
                _isOverlay = true;
            }

            if (_found == false)
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Try to initialize this Scene: {Name} didn't exist!");
                return;
            }
            if (_isOverlay)
            {
                OverlaySceneList[Name].Initialize();
            }
            else
            {
                SceneList[Name].Initialize();
            }
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Initialized Scene: {Name}");
        }
        public static void AddOverlaySceneRunList(string SceneName)
        {
            if (!OverlaySceneList.ContainsKey(SceneName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE MANAGER] Cannot Add {SceneName} to AddOverlaySceneRunList, cuz that scene doesn't exist");
            }
            if (OverlaySceneRunList.Contains(SceneName))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE MANAGER] Cannot Add {SceneName} to AddOverlaySceneRunList, cuz its already exist");
                return;
            }
            OverlaySceneRunList.Add(SceneName);
        }
        public static void StopOverlaySceneRun(string SceneName)
        {
            if (!OverlaySceneRunList.Contains(SceneName))
            {
                return;
            }
            FABRICK_DEBUG.Log($"[SCENE MANAGER] Stopping Scene Overlay From Run List: {SceneName}");
            OverlaySceneRunList.Remove(SceneName);
        }
        public static void ClearOverlaySceneRunList()
        {
            OverlaySceneRunList.Clear();
        }

        public static void SetOverlayScene_AlwaysOnTop(string Name)
        {
            if (!OverlaySceneRunList.Contains(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE MANAGER] Cannot set {Name} as Always On Top, cuz that scene doesn't in the run list");
                return;
            }
            OverlayScene_AlwaysOnTop = Name;
        }

        public static void RunScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunScene] Try to run this Scene: {Name} didn't exist!");
                return;
            }
            SceneList[Name].IsRunning = true;
            SceneList[Name].OnLeave = true;
            if (!IsPauseAllAcene || IsPauseAllAceneOnceUpdate)
            {
                UpdateScene(Name);
                IsPauseAllAceneOnceUpdate = false;
            }
        }
        public static void RunOverlayScene(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunOverlayScene] Try to run this Overlay Scene: {Name} didn't exist in OverlaySceneList!");
                return;
            }
            if (!OverlaySceneRunList.Contains(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunOverlayScene] Try to run this Overlay Scene: {Name} didn't exist in OverlaySceneRunList!");
                return;
            }

            OverlaySceneList[Name].IsRunning = true;
            OverlaySceneList[Name].OnLeave = true;
            if (!IsPauseAllOverlayScene || IsPauseAllOverlaySceneOnceUpdate)
            {
                UpdateOverlayScene(Name);
                IsPauseAllOverlaySceneOnceUpdate = false;
            }
        }
        public static void UpdateScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][UpdateScene] Try to update this Scene: {Name} didn't exist!");
                return;
            }
            SceneList[Name].Update();
        }
        public static void UpdateOverlayScene(string Name)
        {
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][UpdateOverlayScene] Try to update this Overlay Scene: {Name} didn't exist!");
                return;
            }
            OverlaySceneList[Name].Update();
        }

        public static void PauseAllScene(bool State)
        {
            if(!State) IsPauseAllAceneOnceUpdate = true;
            IsPauseAllAcene = State;
        }
        public static void PauseAllOverlayScene(bool State)
        {
            if(!State) IsPauseAllOverlaySceneOnceUpdate = true;
            IsPauseAllOverlayScene = State;
        }
        public static void UpdateAllScene(FABRICK_ENGINE ENGINE)
        {
            SetEngine(ENGINE);
            if (SceneList.Count == 0)
            {
                return;
            }
            RunScene(GetCurrentScene());

            // Skip if Overlay Scene Null
            if (OverlaySceneList.Count == 0 || OverlaySceneRunList.Count == 0)
            {
                return;
            }

            //Checking Run List with Available Overlay Scene
            bool _IsNotAvailable = false;
            List<string> _NotAvailable = new();

            foreach (string ThisScene in OverlaySceneRunList)
            {
                if (!OverlaySceneList.ContainsKey(ThisScene))
                {
                    _NotAvailable.Add(ThisScene);
                    _IsNotAvailable = true;
                }
            }
            if (_IsNotAvailable)
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE MANAGER] This Overlay Run List doesn't exist: {_NotAvailable}");
                return;
            }

            //Updating Overlay Scene by Run List
            foreach (string ThisScene in OverlaySceneRunList)
            {
                if (OverlaySceneList[ThisScene].SceneName != OverlayScene_AlwaysOnTop)
                {
                    RunOverlayScene(ThisScene);
                }
            }
            if (OverlaySceneRunList.Contains(OverlayScene_AlwaysOnTop))
            {    
                RunOverlayScene(OverlayScene_AlwaysOnTop);
            }
        }
        public static void RenderAllScene()
        {
            if (SceneList.ContainsKey(GetCurrentScene()))
            {
                SceneList[GetCurrentScene()].Render();
            }
            foreach (string ThisScene in OverlaySceneRunList)
            {
                if (OverlaySceneList[ThisScene].SceneName != OverlayScene_AlwaysOnTop)
                {
                    OverlaySceneList[ThisScene].Render();
                }
            }
            if (!string.IsNullOrWhiteSpace(OverlayScene_AlwaysOnTop) && OverlaySceneRunList.Contains(OverlayScene_AlwaysOnTop))
            {
                OverlaySceneList[OverlayScene_AlwaysOnTop].Render();    
            }
        }
        public static void SetEngine(FABRICK_ENGINE ENGINE)
        {
            if (SceneList.Count == 0)
            {
                return;
            }
            foreach (var ThisKey in SceneList.Keys.ToList())
            {
                SceneList[ThisKey].UpdateEngine(ENGINE);
            }
            if (OverlaySceneList.Count == 0)
            {
                return;
            }
            foreach (var ThisKey in OverlaySceneList.Keys.ToList())
            {
                OverlaySceneList[ThisKey].UpdateEngine(ENGINE);
            }
        }
        public static void RestartScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to restart this Scene: {Name} already didn't exist!");
                return;
            }
            SceneList[Name].RestartScene();
        }
        public static void RestartOverlayScene(string Name)
        {
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to restart this Overlay Scene: {Name} already didn't exist!");
                return;
            }
            OverlaySceneList[Name].RestartScene();
        }
        public static void RemoveScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to remove this Scene: {Name} already didn't exist!");
                return;
            }
            SceneList.Remove(Name);
            FABRICK_DEBUG.Log("Removed scene: " + Name);
        }
        public static void RemoveOverlayScene(string Name)
        {
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to remove this Overlay Scene: {Name} already didn't exist!");
                return;
            }
            OverlaySceneList.Remove(Name);
            FABRICK_DEBUG.Log("Removed Overlay Scene: " + Name);
        }
        public static void RemoveAllSceneList()
        {
            FABRICK_DEBUG.Log("[SCENE_MANAGER] Remove All Scene List!!");
            foreach (string ThisKey in SceneList.Keys)
            {
                SceneList[ThisKey].Leave();
            }
            SceneList.Clear();
            foreach (string ThisKey in OverlaySceneList.Keys)
            {
                OverlaySceneList[ThisKey].Leave();
            }
            OverlaySceneList.Clear();
        }
        public static void UpdateRunningState()
        {
            if (SceneList.Count == 0)
            {
                return;
            }
            foreach (string ThisKey in SceneList.Keys.ToList())
            {
                SceneList[ThisKey].OnLeave = false;
            }

            if (OverlaySceneList.Count == 0)
            {
                return;
            }
            foreach (string ThisKey in OverlaySceneList.Keys.ToList())
            {
                OverlaySceneList[ThisKey].OnLeave = false;
            }
        }
        public static void LeaveSceneHandle()
        {
            if (SceneList.Count == 0)
            {
                return;
            }
            foreach (var ThisKey in SceneList.Keys.ToList())
            {
                if (SceneList[ThisKey].IsRunning && !SceneList[ThisKey].OnLeave)
                {
                    SceneList[ThisKey].RestartScene();
                    SceneList[ThisKey].Leave();
                }
            }
        }
        public static void SetCurrentScene(string SceneName)
        {
            CurrentSceneName = SceneName;
        }
        public static string GetCurrentScene()
        {
            return CurrentSceneName;
        }
        public static bool IsOverlayScene(string SceneName)
        {   
            return OverlaySceneList.ContainsKey(SceneName);
        }
    }
}
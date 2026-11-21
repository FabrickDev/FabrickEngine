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
        public static bool IsAllNonOverlaySceneStopped = false;

        public static bool AssignScene(string Name, bool IsMain, bool IsNew = false, bool NoWarning = false)
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
                if (IsNew && !NoWarning)
                {
                    FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to Assign new Scene: {Name} already exist!");
                    return false;
                }
                FABRICK_DEBUG.Log($"[SCENE_MANAGER] Try to Assign new Scene: {Name} already exist!");
                return false;
            }
            
            SceneList[Name] = new FABRICK_SCENE(Name)
            {
                SceneName = Name,
                IsMain = _IsMain,
                IsNew = IsNew
            };
            if (_IsMain)
            {
                SetCurrentScene(Name);
            }
            FABRICK_DEBUG.Log("[SCENE MANAGER] Added Scene: " + Name);
            return true;
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
            OverlaySceneList[SceneName].SceneLeave();
            OverlaySceneRunList.Remove(SceneName);
        }
        public static void ClearOverlaySceneRunList()
        {
            foreach (string ThisScene in OverlaySceneRunList)
            {
                OverlaySceneList[ThisScene].SceneLeave();
            }
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

        public static void RunScene(string Name, FABRICK_ENGINE Engine)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunScene] Try to run this Scene: {Name} didn't exist!");
                return;
            }

            // Initialize Scene Handle
            if (SceneList[Name].Initialized == false)
            {
                SceneList[Name].SceneInitialize();
            }

            CurrentSceneName = Name;

            if (!IsPauseAllAcene || IsPauseAllAceneOnceUpdate)
            {
                SceneList[Name].SceneUpdate(Engine);
                IsPauseAllAceneOnceUpdate = false;
            }
        }
        public static void RunOverlayScene(string Name, FABRICK_ENGINE Engine)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunScene] Try to run this Scene Overlay: {Name} didn't exist!");
                return;
            }
            // Trigger Current Scene to Leave
            if (OverlaySceneList[Name].Initialized == false)
            {
                OverlaySceneList[Name].SceneInitialize();
            }

            if (!IsPauseAllOverlayScene || IsPauseAllOverlaySceneOnceUpdate)
            {
                OverlaySceneList[Name].SceneUpdate(Engine);
                IsPauseAllOverlaySceneOnceUpdate = false;
            }
        }
        // public static void RunOverlayScene(string Name)
        // {
        //     if (string.IsNullOrWhiteSpace(Name))
        //     {
        //         return;
        //     }
        //     if (!OverlaySceneList.ContainsKey(Name))
        //     {
        //         FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunOverlayScene] Try to run this Overlay Scene: {Name} didn't exist in OverlaySceneList!");
        //         return;
        //     }
        //     if (!OverlaySceneRunList.Contains(Name))
        //     {
        //         FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER][RunOverlayScene] Try to run this Overlay Scene: {Name} didn't exist in OverlaySceneRunList!");
        //         return;
        //     }

        //     OverlaySceneList[Name].IsRunning = true;
        //     if (!IsPauseAllOverlayScene || IsPauseAllOverlaySceneOnceUpdate)
        //     {
        //         UpdateOverlayScene(Name);
        //         IsPauseAllOverlaySceneOnceUpdate = false;
        //     }
        // }

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
        public static void UpdateAllScene(FABRICK_ENGINE Engine)
        {
            if (SceneList.Count == 0)
            {
                return;
            }
            if (!IsAllNonOverlaySceneStopped)
            {
                // Leave Handle
                foreach (string ThisSceneKey in SceneList.Keys)
                {
                    if (ThisSceneKey != CurrentSceneName) SceneList[ThisSceneKey].SceneLeave();
                }

                RunScene(CurrentSceneName, Engine);
            }
            
            //FABRICK_DEBUG.Log($"[FABRICK_SCENE_MANAGER] Updating: {GetCurrentScene()}");

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

            // Leave Handle
            foreach (string ThisOVSceneKey in OverlaySceneList.Keys)
            {
                bool NotInRun = true;
                foreach(string ThisOVSceneRunKey in OverlaySceneRunList)
                {
                    if(ThisOVSceneKey == ThisOVSceneRunKey) NotInRun = false;
                }
                if (NotInRun)
                {
                    OverlaySceneList[ThisOVSceneKey].SceneLeave();
                }
            }
            //Updating Overlay Scene by Run List
            foreach (string ThisScene in OverlaySceneRunList)
            {
                if (OverlaySceneList[ThisScene].SceneName != OverlayScene_AlwaysOnTop)
                {
                    RunOverlayScene(ThisScene, Engine);
                }
            }

            if (OverlaySceneRunList.Contains(OverlayScene_AlwaysOnTop))
            {    
                RunOverlayScene(OverlayScene_AlwaysOnTop, Engine);
            }
        }
        public static void RenderAllScene()
        {
            if (!IsAllNonOverlaySceneStopped && SceneList.ContainsKey(CurrentSceneName))
            {
                SceneList[CurrentSceneName].SceneRender();
                //FABRICK_DEBUG.Log($"[FABRICK_SCENE_MANAGER] Rendering: {GetCurrentScene()}");
            }
            else
            {
                //FABRICK_DEBUG.Log($"[FABRICK_SCENE_MANAGER] Cannot Find to Render: {GetCurrentScene()}");
            }
            foreach (string ThisScene in OverlaySceneRunList)
            {
                if (OverlaySceneList[ThisScene].SceneName != OverlayScene_AlwaysOnTop)
                {
                    OverlaySceneList[ThisScene].SceneRender();
                }
            }
            if (!string.IsNullOrWhiteSpace(OverlayScene_AlwaysOnTop) && OverlaySceneRunList.Contains(OverlayScene_AlwaysOnTop))
            {
                OverlaySceneList[OverlayScene_AlwaysOnTop].SceneRender();    
            }
        }
        public static void RestartOverlayScene(string Name)
        {
            if (!OverlaySceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[SCENE_MANAGER] Try to restart this Overlay Scene: {Name} already didn't exist!");
                return;
            }
            //OverlaySceneList[Name].RestartScene();
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
                FABRICK_DEBUG.Log($"[SCENE_MANAGER] Try to remove this Overlay Scene: {Name} already didn't exist!");
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
                SceneList[ThisKey].SceneLeave();
            }
            SceneList.Clear();
            foreach (string ThisKey in OverlaySceneList.Keys)
            {
                OverlaySceneList[ThisKey].SceneLeave();
            }
            OverlaySceneList.Clear();
        }

        public static void StopAllNonOverlayScene()
        {
            IsAllNonOverlaySceneStopped = true;
            foreach (string ThisKey in SceneList.Keys)
            {
                if (SceneList[ThisKey].IsRunning)
                {
                    SceneList[ThisKey].SceneLeave();
                }
            }
        }
        public static void StartAllNonOverlayScene()
        {
            IsAllNonOverlaySceneStopped = false;
        }

        // public static void LeaveSceneHandle()
        // {
        //     if (SceneList.Count == 0)
        //     {
        //         return;
        //     }
        //     foreach (var ThisKey in SceneList.Keys.ToList())
        //     {
        //         if (SceneList[ThisKey].IsRunning && !SceneList[ThisKey].OnLeave)
        //         {
        //             SceneList[ThisKey].RestartScene();
        //             SceneList[ThisKey].Leave();
        //         }
        //     }
        // }
        public static void SetCurrentScene(string SceneName)
        {
            FABRICK_DEBUG.Log("[SCENE MANAGER] Set Current Scene: " + SceneName);
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
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class SCENE_DATA
    {
        public Action? RunningAction {get; set;}
        public Action? RenderAction {get; set;}
        public Action? LeaveAction {get; set;}
        public Action? InitializeAction {get; set;}

        public string SceneName = "";
        public bool IsMain = false;

        public FABRICK_ENGINE? Engine;
        public bool IsRunning {get; set;}
        public bool OnLeave {get; set;}
        public int InitializeState {get; set;}

        public virtual SCENE_DATA Clone()
        {
            return new();
        }

        public virtual void Initialize()
        {
            FABRICK_SCENE_MANAGER.CurrentSceneName = this.SceneName;
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Initialize Scene: {SceneName}...");
            InitializeAction?.Invoke();
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Total Entity: {FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count}");
        }

        public virtual void Update()
        {
            // IDK but it works
            if (InitializeState < 1)
            {
                Initialize();
                InitializeState++;
            }
            FABRICK_SCENE_MANAGER.CurrentSceneName = this.SceneName;
            RunningAction?.Invoke();
        }
        public virtual void Render()
        {
            RenderAction?.Invoke();
        }
        public virtual void Leave()
        {
            InitializeState = 0;
            FABRICK_ENTITY_MANAGER.EntityOrder = 0;
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Leaving Scene: {SceneName}...");
            LeaveAction?.Invoke();
        }
        public void RestartScene()
        {
            IsRunning = false;
            OnLeave = false;
            InitializeState = 0;
        }
        public void UpdateEngine(FABRICK_ENGINE Engine)
        {
            this.Engine = Engine;
        }
    }
    public class FABRICK_SCENE_MANAGER
    {
        public static string CurrentSceneName = "";
        private static bool IsPauseAllAcene = false;
        private static bool IsPauseAllAceneOnceUpdate = true;
        public static Dictionary<string, SCENE_DATA> SceneList = new Dictionary<string, SCENE_DATA>();
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
        public static void AssignScene(string Name, SCENE_DATA Scene, bool IsMain = false)
        {
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
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Try to initialize this Scene: {Name} didn't exist!");
                return;
            }
            SceneList[Name].Initialize();
        }
        public static void RunScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Try to run this Scene: {Name} didn't exist!");
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
        public static void UpdateScene(string Name)
        {
            if (!SceneList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Try to update this Scene: {Name} didn't exist!");
                return;
            }
            SceneList[Name].Update();
        }
        public static void PauseAllScene(bool State)
        {
            if(!State) IsPauseAllAceneOnceUpdate = true;
            IsPauseAllAcene = State;
        }
        public static void UpdateAllScene(FABRICK_ENGINE ENGINE)
        {
            SetEngine(ENGINE);
            if (SceneList.Count == 0)
            {
                return;
            }
            RunScene(GetCurrentScene());
        }
        public static void RenderAllScene()
        {
            if (SceneList.Count == 0)
            {
                return;
            }
            if (SceneList.ContainsKey(GetCurrentScene()))
            {
                SceneList[GetCurrentScene()].Render();
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
        public static void RemoveAllSceneList()
        {
            FABRICK_DEBUG.Log("[SCENE_MANAGER] Remove All Scene List!!");
            foreach (string ThisKey in SceneList.Keys)
            {
                SceneList[ThisKey].Leave();
            }
            SceneList.Clear();
        }
        public static void UpdateRunningState()
        {
            if (SceneList.Count == 0)
            {
                return;
            }

            foreach (var ThisKey in SceneList.Keys.ToList())
            {
                SceneList[ThisKey].OnLeave = false;
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
    }
}
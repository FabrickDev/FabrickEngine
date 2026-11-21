using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public partial class FABRICK_SCENARIO_MANAGER
    {
        public static void SetVisualNovelScene(string VNScene)
        {
            string _tmp = FABRICK_VISUAL_NOVEL_MANAGER.VisualNovelSceneName + VNScene;
            if (!FABRICK_SCENE_MANAGER.OverlaySceneList.ContainsKey(_tmp))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Set Visual Novel Scene: {VNScene} cus its already exist!!");
                return;
            }
            VisualNovelSceneName = _tmp;
        }
        public static void AddScenario(string ScenarioName, string ScenarioCategory)
        {
            if (!ScenarioList.ContainsKey(ScenarioName + "@" + ScenarioCategory))
            {
                ScenarioList[ScenarioName + "@" + ScenarioCategory] = new();
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Adding Scenario: {ScenarioName}, Category: {ScenarioCategory} cus its already exist!!");
            }
        }
        public static void ReplaceScenarioModuleData(string ScenarioName, string ScenarioCategory, List<SCENARIO_MODULE_DATA> ScenarioData)
        {
            string ThisScenario = ScenarioName + "@" + ScenarioCategory;
            if (ScenarioList.ContainsKey(ThisScenario))
            {
                ScenarioList[ThisScenario] = ScenarioData;
            }
            else
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot ReplaceScenarioModuleData cus scenario: {ScenarioName}, Category: {ScenarioCategory} doesn't exist!!");
                return;
            }
        }
        public static void RunScenario(string ScenarioName, string ScenarioCategory)
        {
            string ThisScenario = ScenarioName + "@" + ScenarioCategory;
            if (!ScenarioList.ContainsKey(ThisScenario))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Run Scenario: {ScenarioName}, Category: {ScenarioCategory} doesn't Exist!!");
                return;
            }
            if (CurrentScenario != ThisScenario)
            {
                FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Run Scenario: {ScenarioName}, Category: {ScenarioCategory}");
                VisualNovelTextNext = true;
                VisualNovelText = "";
                NextModuleToExecute.Clear();
                CoiceList.Clear();
                CurrentScenario = ThisScenario;
                for (int i = 0; i < ScenarioList[ThisScenario].Count; i++)
                {
                    ScenarioList[CurrentScenario][i].IsFinishedModule = false;
                }
            }
        }
        private static void AddNextModuleList(List<string> ModuleList)
        {
            foreach (string ThisModuleId in ModuleList)
            {
                NextModuleToExecute.Add(ThisModuleId);
            }
        }
        private static void SetNextListScenario(List<string> Scenarios)
        {
            FABRICK_DEBUG.Log("[FABRICK_SCENARIO_MANAGER] Next to Execute:");
            foreach (string thisMod in Scenarios)
            {
                FABRICK_DEBUG.Log(thisMod);
            }
            if(NextModuleToExecute.Count > 0) NextModuleToExecute.Clear();
            NextModuleToExecute = Scenarios;
        }
    }
}
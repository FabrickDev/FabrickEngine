using System.Numerics;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.VISUAL_NOVEL;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal class FABRICK_DIALOGUE_EDITOR_MODULE_MANAGER
    {
        public Dictionary<string, string> ModuleIdToExecute = new();

        public void ExecuteModuleIdHandle()
        {
            if (!FABRICK_SCENARIO_MANAGER.CurrentScenario.IsWhiteSpace() && ModuleIdToExecute.ContainsKey(FABRICK_SCENARIO_MANAGER.CurrentScenario))
            {
                //Execute then??
            }
        }
    }
}
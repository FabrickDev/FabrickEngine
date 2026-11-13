using Fabrick.ENGINE.VISUAL_NOVEL;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal class FABRICK_DIALOGUE_SCENARIO_DATA
    {
        internal static List<SCENARIO_MODULE_DATA> ModuleDataList = new();
        internal static void MakeScenario()
        {
            SCENARIO_MODULE_DATA DataTemp = new();
            // Find Start
            foreach (FABRICK_DIALOGUE_EDITOR_MODULE_ITEM ThisModule in FABRICK_DIALOGUE_EDITOR.DialogueModuleList)
            {
                if (ThisModule.ThisType == SCENARIO_MODULE_TYPE.Start)
                {
                    DataTemp.Type = SCENARIO_MODULE_TYPE.Start;
                    DataTemp.ModuleId = ThisModule.ModuleId;
                    ModuleDataList.Add(DataTemp);
                    break;
                }
            }
            // Rest of Them
            foreach (FABRICK_DIALOGUE_EDITOR_MODULE_ITEM ThisModule in FABRICK_DIALOGUE_EDITOR.DialogueModuleList)
            {
                if (ThisModule.ThisType != SCENARIO_MODULE_TYPE.Start)
                {
                    DataTemp.Type = ThisModule.ThisType;
                    DataTemp.ModuleId = ThisModule.ModuleId;
                    
                }
            }
        }
    }
}
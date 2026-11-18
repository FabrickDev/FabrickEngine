using System.CodeDom;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public partial class FABRICK_SCENARIO_MANAGER
    {
        internal static FABRICK_VISUAL_NOVEL_DIALOGUE_TEXT? ThisDialogueText;
        public static string VisualNovelCurrentCast = ""; // Control who speaking
        public static string VisualNovelCurrentCastVariant = "Normal";
        public static string VisualNovelCast = "Narrator"; // This actually just for display
        public static string VisualNovelText = "I'm Appearing...";
        public static void AssignDialogueText(FABRICK_VISUAL_NOVEL_DIALOGUE_TEXT DialogueText)
        {
            ThisDialogueText = DialogueText;
        }
        internal static void DialogueTextInitialize()
        {
            ThisDialogueText?.EntityInitialize();
        }
        internal static void DialogueTextUpdate()
        {
            //FABRICK_DEBUG.Log("DuialogueText Updating");
            if(ThisEngine != null) ThisDialogueText?.EntityUpdate(ThisEngine);
        }
        internal static void DialogueTextRender()
        {
            //FABRICK_DEBUG.Log("DuialogueText Rendering");
            ThisDialogueText?.EntityRender();
        }
    }
}
using System.CodeDom;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public partial class FABRICK_SCENARIO_MANAGER
    {
        internal static FABRICK_VISUAL_NOVEL_COICE? CoiceEntity;
        internal static List<string> CoiceList = new();
        internal static string SelectedCoice = "";
        
        public static void AssignCoice(FABRICK_VISUAL_NOVEL_COICE Coice)
        {
            CoiceEntity = Coice;
        }
        internal static void CoiceInitialize()
        {
            CoiceEntity?.EntityInitialize();
        }
        internal static void CoiceUpdate()
        {
            if(ThisEngine != null) CoiceEntity?.EntityUpdate(ThisEngine);
        }
        internal static void CoiceRender()
        {
            CoiceEntity?.EntityRender();
        }
    }
}
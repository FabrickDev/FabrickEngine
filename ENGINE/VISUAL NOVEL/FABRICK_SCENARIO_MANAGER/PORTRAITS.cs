using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public partial class FABRICK_SCENARIO_MANAGER
    {
        public static Dictionary<string, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR> PortraitsCastList = new();
        public static void AssignPortraitCast(string Name, string DisplayName, FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR PostraitsCastTemplate)
        {
            FABRICK_DEBUG.Log($"[FABRICK_SCENARIO_MANAGER] Assigning Cast: {Name}");
            if (PortraitsCastList.ContainsKey(Name))
            {
                FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Assign Postrais Cast: {Name} cus its already exist!!");
                return;
            }
            PortraitsCastList[Name] = PostraitsCastTemplate;
            PortraitsCastList[Name].DisplayName = DisplayName;
        }
        public static void SetFocusCast(string Cast, string Variant)
        {
            if (!PortraitsCastList.ContainsKey(Cast))
            {
                foreach (string ThisKey in PortraitsCastList.Keys)
                {
                    PortraitsCastList[ThisKey].SetPortraitVisibility(0.5f);
                }
                //FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Set Focus Cast: {Name} cus its doesn't exist!!");
                return;
            }
            foreach (string ThisKey in PortraitsCastList.Keys)
            {
                PortraitsCastList[ThisKey].SetPortraitVisibility(0.5f);
            }
            PortraitsCastList[Cast].SetPortraitVisibility(1f);
            PortraitsCastList[Cast].SetVariant(Cast, Variant);
        }
        internal static void ModifyCastVariant(string Cast, string Variant)
        {
            if (!PortraitsCastList.ContainsKey(Cast))
            {
                //FABRICK_DEBUG.SimpleMessageWarning($"[FABRICK_SCENARIO_MANAGER] Cannot Set Focus Cast: {Name} cus its doesn't exist!!");
                return;
            }
            PortraitsCastList[Cast].SetVariant(Cast, Variant);
        }
        internal static void PortraitInitialize()
        {
            foreach (FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR ThisChar in PortraitsCastList.Values)
            {
                ThisChar.EntityInitialize();
            }
        }
        internal static void PortraitUpdate()
        {
            foreach (FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR ThisChar in PortraitsCastList.Values)
            {
                if(ThisEngine != null) ThisChar.EntityUpdate(ThisEngine);
            }
        }
        internal static void PortraitRender()
        {
            foreach (FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR ThisChar in PortraitsCastList.Values)
            {
                ThisChar.EntityRender();
            }
        }
    }
}
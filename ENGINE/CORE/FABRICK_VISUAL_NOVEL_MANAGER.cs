using System.Configuration;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.CORE
{
    public class FABRICK_VISUAL_NOVEL_MANAGER
    {
        // Internal Things
        internal static string VisualNovelSceneName = "@VISUAL_NOVEL.";
        internal static void Initialize()
        {
            FABRICK_DEBUG.Log("[FABRICK VISUAL NOVEL] Initializing...");
            // What De Heck??
            // What De Dog Doin'?
            FABRICK_DEBUG.Log("[FABRICK VISUAL NOVEL] Initializing Done!!");
        }

        // Public Things
        public static void AssignVisualNovelScene(FABRICK_VISUAL_NOVEL_SCENE VN_Scene)
        {
            FABRICK_SCENE_MANAGER.AssignScene(VisualNovelSceneName + VN_Scene.SceneName, VN_Scene, false, true);
        }
    }
}
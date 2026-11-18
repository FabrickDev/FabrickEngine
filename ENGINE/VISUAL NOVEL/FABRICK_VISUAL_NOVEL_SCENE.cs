using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.VISUAL_NOVEL;

namespace Fabrick.ENGINE.SCENE_MANAGER
{
    public class FABRICK_VISUAL_NOVEL_SCENE : SCENE_DATA
    {
        public float AppearAnimation = 0f;
        public bool ScenePrepared = false;

        public FABRICK_VISUAL_NOVEL_SCENE(string SceneName)
        {
            this.SceneName = SceneName;
            this.IsOverlay = true;
        }

        public override SCENE_DATA Clone()
        {
            return new FABRICK_VISUAL_NOVEL_SCENE(this.SceneName);
        }

        internal override void VN_Initialize()
        {
            //FABRICK_DEBUG.SimpleMessageInfo($"Initialize VN Scene: {this.SceneName}");
            FABRICK_SCENARIO_MANAGER.DialogueTextInitialize();
            FABRICK_SCENARIO_MANAGER.CoiceInitialize();
            FABRICK_SCENARIO_MANAGER.PortraitInitialize();
        }
        internal override void VN_Update()
        {
            if (!ScenePrepared && Engine is not null)
            {
                //FABRICK_DEBUG.Log($"{SceneName} Updating...");
                AppearAnimation += Engine.DeltaTime * 2;
                if (AppearAnimation > 1)
                {
                    FABRICK_SCENARIO_MANAGER.VisualNovelRunning = true;
                    ScenePrepared = true;
                }
                AppearAnimation = FABRICK_MATH.Clamp(AppearAnimation, 0, 1);
                foreach (FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR ThisChar in FABRICK_SCENARIO_MANAGER.PortraitsCastList.Values)
                {
                    ThisChar.SetVisibility(AppearAnimation);
                }
                FABRICK_SCENARIO_MANAGER.ThisDialogueText?.ResetAppear();
            }
            
            if (FABRICK_SCENARIO_MANAGER.VisualNovelEnding && !FABRICK_SCENARIO_MANAGER.VisualNovelEnded && Engine is not null)
            {
                AppearAnimation -= Engine.DeltaTime * 2;
                if (AppearAnimation < 0)
                {
                    FABRICK_SCENARIO_MANAGER.VisualNovelEnded = true;
                    AppearAnimation = 0f;
                    ScenePrepared = false;
                }
                AppearAnimation = FABRICK_MATH.Clamp(AppearAnimation, 0, 1);
                foreach (FABRICK_VISUAL_NOVEL_PORTRAIT_CHAR ThisChar in FABRICK_SCENARIO_MANAGER.PortraitsCastList.Values)
                {
                    ThisChar.SetVisibility(AppearAnimation);
                }
            }
            FABRICK_SCENARIO_MANAGER.DialogueTextUpdate();
            FABRICK_SCENARIO_MANAGER.CoiceUpdate();
            FABRICK_SCENARIO_MANAGER.PortraitUpdate();
        }
        internal override void VN_Render()
        {
            FABRICK_SCENARIO_MANAGER.DialogueTextRender();
            FABRICK_SCENARIO_MANAGER.PortraitRender();
            FABRICK_SCENARIO_MANAGER.CoiceRender(); // Keep on priority!!
        }
    }
}

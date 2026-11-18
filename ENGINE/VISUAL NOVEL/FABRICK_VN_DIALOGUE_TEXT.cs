using System.Numerics;
using System.Runtime.CompilerServices;
using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.VISUAL_NOVEL
{
    public class FABRICK_VISUAL_NOVEL_DIALOGUE_TEXT : FABRICK_ENTITY
    {
        public override FABRICK_ENTITY Clone()
        {
            return new FABRICK_VISUAL_NOVEL_DIALOGUE_TEXT(Name, Id);
        }
        //private string TestText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, \nsed do eiusmod tempor incididunt ut labore et dolore \nmagna aliqua. Ut enim ad minim veniam, \nquis nostrud exercitation ullamco laboris nisi ut aliquip ex ea";
        private bool TextFinishStep = false;
        private float TextSize = 4f;
        private float StepText = 0;
        private float StepSpeed = 0;
        private int MaxSentenceEachLine = 45;
        private bool CacheRemoved = true;
        private string CacheCategory_CastName = "CastName";
        private string CacheCategory_CastDialogue = "CastDialogue";

        public FABRICK_VISUAL_NOVEL_DIALOGUE_TEXT(string Name, int Id)
        {
            this.Category = "VisualNovelDialoguetext";
            this.Name = Name;
            this.Id = Id;
            CacheCategory_CastName = string.Join(CacheCategory_CastName, "@", Name);
            CacheCategory_CastDialogue = string.Join(CacheCategory_CastDialogue, "@", Name);
        }
        public override void Initialize()
        {
            
        }
        public override void Update()
        {
            if (CacheRemoved && !FABRICK_SCENARIO_MANAGER.VisualNovelRunning)
            {
                FABRICK_DRAW_TEXT.RemoveCacheByCategory(CacheCategory_CastName);
                FABRICK_DRAW_TEXT.RemoveCacheByCategory(CacheCategory_CastDialogue);
                CacheRemoved = false;
            }
            if (FABRICK_SCENARIO_MANAGER.VisualNovelRunning)
            {
                CacheRemoved = true;
            }
            StepText += StepSpeed * DeltaTime;
            StepText = FABRICK_MATH.Clamp(StepText, 0, FABRICK_SCENARIO_MANAGER.VisualNovelText.Length);
            if (StepText >= FABRICK_SCENARIO_MANAGER.VisualNovelText.Length)
            {
                TextFinishStep = true;
            }
            else
            {
                TextFinishStep = false;
            }
            if (FABRICK_INPUT.IsKeyPressed(FABRICK_GLOBAL_VARIABLE_MANAGER.GlobalVariableDataList["Engine.VisualNovelKeyNext"].variableValue_KEYBOARD))
            {
                StepText = 0;
                FABRICK_SCENARIO_MANAGER.VisualNovelTextNext = true;
            }
        }
        public override void Render()
        {
            FABRICK_DRAW_TEXT.DrawTextAdvanced(FABRICK_SCENARIO_MANAGER.VisualNovelCast, FABRICK_DRAW_TEXT.GetDefaultFontName(), Transform.GetPosition() + new Vector2(-80, -60), 4, FABRICK_COLOR.Black(), Algin.Left, out _, MaxSentenceEachLine, 0, true, true, true, CacheCategory_CastName);
            if(FABRICK_SCENARIO_MANAGER.VisualNovelText.Length == 0) return;
            //FABRICK_DRAW_TEXT.DrawTextAdvanced(FABRICK_SCENARIO_MANAGER.VisualNovelText, FABRICK_DRAW_TEXT.GetDefaultFontName(), Transform.GetPosition(), 4, FABRICK_COLOR.Black(), Algin.Left, out _, MaxSentenceEachLine, 0, true, true, true, CacheCategory_CastName);
            FABRICK_DRAW_TEXT.DrawTextEachLetter(FABRICK_SCENARIO_MANAGER.VisualNovelText, (int)StepText, FABRICK_DRAW_TEXT.GetDefaultFontName(), Transform.GetPosition(), TextSize, FABRICK_COLOR.Black(), Algin.Left, out _, MaxSentenceEachLine, 0, true, true, CacheCategory_CastDialogue);
        }
        // public void SetText(string Text)
        // {
        //     TestText = Text;
        //     StepText = 0;
        //     TextFinishStep = false;
        // }
        // Default is 4
        public void SetTextSize(float TextSize)
        {
            this.TextSize = TextSize;
        }
        public void ResetAppear()
        {
            StepText = 0;
        }
        public void SetAppearSpeed(float Speed)
        {
            StepSpeed = Speed;
        }
        public bool IsTextFinishedStep()
        {
            return TextFinishStep;
        }
    }
}
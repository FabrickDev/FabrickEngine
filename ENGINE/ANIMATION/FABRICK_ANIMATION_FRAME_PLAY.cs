using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.ANIMATION
{
    public class FABRICK_ANIMATION_FRAME_PLAY
    {
        private struct FABRICK_ANIMATION_FRAME_STRUCT
        {
            public int CurrentFrame = new();
            public float AnimTime = new();
            public bool IsPlaying = false;

            public FABRICK_ANIMATION_FRAME_STRUCT()
            {
                CurrentFrame = 0;
                AnimTime = 0.0f;
                IsPlaying = false;
            }
        }
        private static FABRICK_ENGINE? ThisEngine;
        private static List<FABRICK_ANIMATION_FRAME_STRUCT> AnimationPlayList = new();

        public static void SetEngine(FABRICK_ENGINE ENGINE)
        {
            ThisEngine = ENGINE;
        }
        public static int PlayAnimation(int Frame, int TotalFrames, float TargetFPS)
        {
            if (ThisEngine is null)
            {
                FABRICK_DEBUG.SimpleMessageError($"[ANIMATION_FRAME_PLAY] Engine is null!!");
                return 0;
            }

            // Check if the play list is null
            if (AnimationPlayList.Count == 0)
            {
                FABRICK_DEBUG.Log($"[ANIMATION_FRAME_PLAY] AnimationPlayList is null, creating new list...");
                AnimationPlayList.Add(new FABRICK_ANIMATION_FRAME_STRUCT());
            }

            int IndexOfUnusedPlayList = 0;

            // Check if there is an unused play list
            foreach (FABRICK_ANIMATION_FRAME_STRUCT ThisPlayList in AnimationPlayList)
            {
                if (ThisPlayList.IsPlaying == false)
                {
                    IndexOfUnusedPlayList = AnimationPlayList.IndexOf(ThisPlayList);
                }
            }

            // Execute
            FABRICK_ANIMATION_FRAME_STRUCT ThisPlayListStruct = AnimationPlayList[IndexOfUnusedPlayList];

            ThisPlayListStruct.AnimTime += (float)ThisEngine.DeltaTime;
            float TimeFloat = ThisPlayListStruct.AnimTime * TargetFPS;
            ThisPlayListStruct.CurrentFrame = (int)TimeFloat % TotalFrames;

            AnimationPlayList[IndexOfUnusedPlayList] = ThisPlayListStruct;

            return ThisPlayListStruct.CurrentFrame;
        }
        public static void ForceStopAnimation(int PlayListIndex)
        {
            // Check if the play list is null
            if (AnimationPlayList.Count == 0)
            {
                FABRICK_DEBUG.Log($"[ANIMATION_FRAME_PLAY] AnimationPlayList is null, creating new list...");
                return;
            }

            if (PlayListIndex < 0 || PlayListIndex >= AnimationPlayList.Count)
            {
                FABRICK_DEBUG.SimpleMessageError($"[ANIMATION_FRAME_PLAY] Invalid PlayListIndex: {PlayListIndex}!!");
                return;
            }
            FABRICK_ANIMATION_FRAME_STRUCT ThisPlayListStruct = AnimationPlayList[PlayListIndex];
            ThisPlayListStruct.IsPlaying = false;
            AnimationPlayList[PlayListIndex] = ThisPlayListStruct;
        }
        public static void Update()
        {
            for (int i = 0; i < AnimationPlayList.Count; i++)
            {
                if (AnimationPlayList[i].IsPlaying == false)
                {
                    AnimationPlayList.RemoveAt(i);
                }
            }
        }
    }
}
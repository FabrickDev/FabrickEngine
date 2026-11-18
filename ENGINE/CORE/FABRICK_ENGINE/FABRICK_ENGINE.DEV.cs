using SDL3;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.EDITOR;
using Fabrick.ENGINE.SCENE_MANAGER;


namespace Fabrick.ENGINE.CORE
{
    public partial class FABRICK_ENGINE
    {
        // Private
        private bool IsDevMode = false;
        private bool LeavingDevMode = true;
        internal bool IsEditorMode = false;

        private void DevEditorListener()
        {
            // ========================== Render Phase ========================== //
            // Check if Render Order Available
            if (!RenderList_Error && (RenderList == null || !RenderList.Any()))
            {
                RenderList_Error = true;
                if (IsDevMode)
                {
                    FABRICK_DEBUG.SimpleMessageError("[FABRICK_ENGINE] Nothing to Render?!");
                }
            }

            if (IsDevMode)
            {
                if (FABRICK_INPUT.IsKeyPressed(SDL.Scancode.F1))
                {
                    IsEditorMode = !IsEditorMode;
                    if (IsEditorMode)
                    {
                        // Devmode On
                        if (LeavingDevMode)
                        {
                            EDITOR_MANAGER.LastSceneUsed = FABRICK_SCENE_MANAGER.GetCurrentScene();
                            LeavingDevMode = false;
                        }
                        EDITOR_MANAGER.LastEngineCamera = FABRICK_CAMERA_MANAGER.GetCurrentCamera();
                        if (EDITOR_MANAGER.LastEditorCamera != "@DEV.CAMNULL")
                        {
                            FABRICK_CAMERA_MANAGER.SetCurrentCamera(EDITOR_MANAGER.LastEditorCamera);
                        }
                        FABRICK_DEBUG.Log("[FABRICK_ENGINE] Editor Mode: On");
                    }
                    else
                    {
                        // Devmode Off
                        if (!LeavingDevMode)
                        {
                            if (FABRICK_SCENE_MANAGER.GetCurrentScene() != EDITOR_MANAGER.LastSceneUsed)
                            {
                                FABRICK_SCENE_MANAGER.SetCurrentScene(EDITOR_MANAGER.LastSceneUsed);
                            }
                            LeavingDevMode = true;
                        }
                        EDITOR_MANAGER.LastEditorCamera = FABRICK_CAMERA_MANAGER.GetCurrentCamera();
                        FABRICK_CAMERA_MANAGER.SetCurrentCamera(EDITOR_MANAGER.LastEngineCamera);
                        FABRICK_DRAW_TEXT.RemoveCacheByCategory(FABRICK_DRAW_TEXT.DevTextTagCache);
                        FABRICK_DEBUG.Log("[FABRICK_ENGINE] Editor Mode: Off");
                    }
                }
            }
            // Developer Component Update
            if (IsEditorMode)
            {
                DeveloperEditorComponentUpdate();
            }
        }
        private void DeveloperEditorComponentUpdate()
        {
            EDITOR_MANAGER.Update((float)DeltaTime);
        }
    }
}
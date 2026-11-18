using System.Numerics;
using System.Reflection;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.VISUAL_NOVEL;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.DIALOGUE_EDITOR
{
    internal partial class FABRICK_DIALOGUE_EDITOR_MODULE
    {
        private void UpdateInputPin(out float LowerPos)
        {
            InputPos.Clear();
            FABRICK_DRAW_TEXT.DrawDevText("Input",  new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2((PinRadius * 1.75f) - (MoverSize.X / 2),  MoverSize.Y + (PinRadius * 1.25f)), 2f, EDITOR_MANAGER.Color3, Algin.Left);
            Box _temp = new Box(                    new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2((PinRadius / 2.00f) - (MoverSize.X / 2),  MoverSize.Y + (PinRadius * 1.25f)), Vector2.Zero, new Vector2(PinRadius));
            InputPos.Add(_temp.GetPos() + new Vector2(PinRadius / 2, PinRadius / 2));
            LowerPos = _temp.GetPos().Y + PinRadius / 2;
            
            FABRICK_VISUALIZE_SHAPE.Visualize(_temp, EDITOR_MANAGER.Color3, true);

            // Receive Input
            if(FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect.Length == 0) return;

            string[] _strtemp = FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect.Split("@");
            string OutputTag = _strtemp[0];
            string ModuleWantToConnect = _strtemp[1];

            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                if (ModuleWantToConnect == ModuleId)
                {
                    // Abort
                    FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect = "";
                }
                else
                {
                    for (int i = 0; i < FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList.Count; i++)
                    {
                        if (FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList[i].ModuleId == ModuleWantToConnect)
                        {
                            if (!FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList[i].ModuleData.OutputToModule.ContainsKey(OutputTag))
                            {
                                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList[i].ModuleData.OutputToModule.Add(OutputTag, new(){ModuleId});
                            }
                            else
                            {
                                FABRICK_DIALOGUE_EDITOR_MANAGER.DialogueModuleList[i].ModuleData.OutputToModule[OutputTag].Add(ModuleId);
                            }
                            FABRICK_DEBUG.Log($"[MODULE: {FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect} Was Connected to {ModuleId}]");
                            FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect = "";
                        }
                    }
                }
            }
        }
        
        private void UpdateOutputPin(string[] OutputTag, out float LowerPos)
        {
            if(OutputPos.Count > 0) OutputPos.Clear();
            float _YOffset = 0;
            foreach (string ThisKey in OutputTag)
            {
                Box _temp = new Box             (           new Vector2(0, _YOffset) + new Vector2(0, IOYOffset) + Pos + new Vector2(0, IOYOffsetEachLine) + new Vector2(MoverSize.X - (PinRadius * 1.25f) - (MoverSize.X / 2), MoverSize.Y + (PinRadius * 1.25f)),                                        Vector2.Zero, new Vector2(PinRadius));
                
                FABRICK_DRAW_TEXT.DrawDevText   ("Output: " + ThisKey,  _temp.GetPos() - new Vector2(PinRadius / 2, 0), 2f, EDITOR_MANAGER.Color3, Algin.Right);
                _YOffset += _temp.GetSize().Y + 2;
                
                OutputPos.Add(_temp.GetPos() + new Vector2(PinRadius / 2, PinRadius / 2));
                FABRICK_VISUALIZE_SHAPE.Visualize(_temp, EDITOR_MANAGER.Color3, true);

                // Send Channel
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
                {
                    FABRICK_DEBUG.Log($"[MODULE: {ModuleId} w Key: {ThisKey} Want to Connect]");
                    FABRICK_DIALOGUE_EDITOR_MANAGER.ModuleWantToConnect = ThisKey + "@" + ModuleId;
                }

                // Remove Channel on That Module
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), _temp) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Right))
                {
                    if(!ModuleData.OutputToModule.ContainsKey(ThisKey)) continue;
                    if(ModuleData.OutputToModule[ThisKey].Count > 0) ModuleData.OutputToModule[ThisKey].Clear();
                }
            }
            LowerPos = _YOffset;
        }
    }
}
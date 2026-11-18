using System.Numerics;
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
        private void Dialogue_DialogueButton()
        {
            // Draw BG
            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            Vector2 _Size = new Vector2(SizeModule.X - 20, 110);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos, _Size, EDITOR_MANAGER.Color3);

            // Draw Text
            if (ModuleData.DialogueText.Length > 0)
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced(ModuleData.DialogueText, "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 40, 0, true, false, true, "ModuleEditor");
            }
            else
            {
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Enter Dialogue Here...", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 40, 0, true, false, true, "ModuleEditor");
            }
            // Response
            if(!IsFocushed) return;
            Box box = new(_Pos, Vector2.Zero, _Size);
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                ModuleData.DialogueText = FABRICK_DEBUG.DialogueInputText("Dialogue", "Input text here...", ModuleData.DialogueText, true);
            }
        }
    }
}
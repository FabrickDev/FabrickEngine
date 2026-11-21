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
        private void ModifyCast()
        {
            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            Vector2 _Size = new Vector2(SizeModule.X - 20, 30);
            Vector2 _Pos2 = _Pos + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            Vector2 _Pos3 = _Pos2 + new Vector2(0, _Size.Y) + new Vector2(0, 5);
            
            List<string> MainOrOpp = new ()
            {
                "Opponent", 
                "Main"
            };
            List<string> LString = new();
            List<string> VariantString = new();
            // Draw BG
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos2, _Size, EDITOR_MANAGER.Color3);
            FABRICK_DRAW_SHAPE.DrawFillRect(_Pos3, _Size, EDITOR_MANAGER.Color3);
            
            string ThisCast = "Select Cast...";
            string ThisCastVariant = "Select Cast Variant...";
            if (ModuleData.DialogueCast.Length > 0)
            {
                ThisCast = ModuleData.DialogueCast;
            }
            if (ModuleData.DialogueCastVariant.Length > 0)
            {
                ThisCastVariant = ModuleData.DialogueCastVariant;
            }
            
            // Draw Text
            if (!ModuleData.IsMainCast)
            { // If False Is Main
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Main Cast", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            }
            else
            { // If True Is Opponent
                FABRICK_DRAW_TEXT.DrawTextAdvanced("Opponent Cast", "DefaultFont", _Pos, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            }
            
            // Name Char
            foreach (string ThisChar in FABRICK_SCENARIO_MANAGER.PortraitsCastList.Keys)
            {
                if (FABRICK_SCENARIO_MANAGER.PortraitsCastList[ThisChar].IsMain() != ModuleData.IsMainCast)
                {
                    LString.Add(ThisChar);
                }
            }

            // Variant Char
            if (ModuleData.DialogueCast.Length > 0)
            {
                foreach (string ThisVariant in FABRICK_SCENARIO_MANAGER.PortraitsCastList[ModuleData.DialogueCast].VariantPortrait.Keys)
                {
                    string[] vari = ThisVariant.Split(".");
                    if (vari[0] != ModuleData.DialogueCast)
                    {
                        continue;
                    }

                    VariantString.Add(vari[1]);
                }
            }
            
            FABRICK_DRAW_TEXT.DrawTextAdvanced(ThisCast, "DefaultFont", _Pos2, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            FABRICK_DRAW_TEXT.DrawTextAdvanced(ThisCastVariant, "DefaultFont", _Pos3, 2f, EDITOR_MANAGER.Color1, Algin.Left, out _, 0, 0, true, false, true, "ModuleEditor");
            // Input Response
            if(!IsFocushed) return;
            Box box1 = new(_Pos, Vector2.Zero, _Size);
            Box box2 = new(_Pos2, Vector2.Zero, _Size);
            Box box3 = new(_Pos3, Vector2.Zero, _Size);
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box1) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                int Selected = FABRICK_DEBUG.DialogueList("Select Cast", "Cast: ", ref MainOrOpp);
                if(Selected != -1)
                {
                    ModuleData.IsMainCast = !Convert.ToBoolean(Selected);
                    ModuleData.DialogueCast = "";
                    ModuleData.DialogueCastVariant = "";
                }
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box2) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                int Selected = FABRICK_DEBUG.DialogueList("Select Cast", "Cast: ", ref LString);
                if (Selected != -1)
                {
                    ModuleData.DialogueCast = LString[Selected];
                    ModuleData.DialogueCastVariant = "";
                    FABRICK_DEBUG.Log($"[{ModuleId}]Selected Cast: " + ModuleData.DialogueCast);
                }
            }
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos(), box3) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
            {
                int val = FABRICK_DEBUG.DialogueList("Select Cast Variant", "Variant: ", ref VariantString);
                if (val != -1)
                {
                    ModuleData.DialogueCastVariant = VariantString[val];
                    FABRICK_DEBUG.Log($"[{ModuleId}]Selected Cast Variant: " + ModuleData.DialogueCastVariant);
                }
            }

        }
    }
}
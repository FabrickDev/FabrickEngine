using System.Numerics;
using System.Reflection;
using Fabrick.ENGINE.CORE;
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
        private void Coice(out float LowerPos)
        {
            LowerPos = 0f;
            Vector2 _Size = new Vector2(SizeModule.X - 20, 30);
            Vector2 _Pos = Pos + new Vector2(10, 10 + MoverSize.Y) - new Vector2(SizeModule.X / 2, 0);
            int ModifyIndex = -1;
            int Iterator = 0;
            string TempNewKey = "";
            foreach (string ThisCoice in ModuleData.CoiceList)
            {
                Box box = new(_Pos, Vector2.Zero, _Size);
                FABRICK_VISUALIZE_SHAPE.Visualize(box, EDITOR_MANAGER.Color3, true);
                FABRICK_DRAW_TEXT.DrawDevText(ThisCoice, new Vector2(_Pos.X + (_Size.X / 2), _Pos.Y), 2f, EDITOR_MANAGER.Color1, Algin.Center, true);
                _Pos = _Pos + new Vector2(0, _Size.Y + 4);
                if (IsFocushed && FABRICK_COLLISIONS.MouseClickToBox_Pressed(box, SDL.MouseButtonFlags.Left, false))
                {
                    string _temp = FABRICK_DEBUG.DialogueInputText("Modify Coice", "Write Coice Here", "", true);
                    if (_temp.Length > 0)
                    {
                        TempNewKey = _temp;
                        ModifyIndex = Iterator;
                    }
                }
                Iterator++;
            }

            if (ModifyIndex != -1)
            {
                List<string> TempList = new();
                for (int i = 0; i < ModuleData.CoiceList.Count; i++)
                {
                    if (i == ModifyIndex)
                    {
                        TempList.Add(TempNewKey);
                    }
                    else
                    {
                        TempList.Add(ModuleData.CoiceList[i]);
                    }
                }
                ModuleData.CoiceList.Clear();
                ModuleData.CoiceList = TempList;
            }

            Box AddBox = new(_Pos, Vector2.Zero, _Size);
            FABRICK_VISUALIZE_SHAPE.Visualize(AddBox, EDITOR_MANAGER.Color3, true);
            FABRICK_DRAW_TEXT.DrawDevText("+ Add +", new Vector2(_Pos.X + (_Size.X / 2), _Pos.Y), 2f, EDITOR_MANAGER.Color2, Algin.Center, true);
            if (IsFocushed && FABRICK_COLLISIONS.MouseClickToBox_Pressed(AddBox, SDL.MouseButtonFlags.Left, false))
            {
                string _temp = FABRICK_DEBUG.DialogueInputText("Add Coice", "Write Coice Here", "", true);
                if (_temp.Length > 0)
                {
                    ModuleData.CoiceList.Add(_temp);
                    IOYOffset += (int)_Size.Y;
                    SizeModule.Y += (int)_Size.Y;
                }
            }
        }
    }
}
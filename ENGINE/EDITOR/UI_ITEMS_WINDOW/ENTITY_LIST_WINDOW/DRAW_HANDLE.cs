using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public partial class LIST_WINDOW
    {
        private void Draw_MainArea()
        {
            // Draw Main Window Area
            MainAreaPos = new Vector2(Pos.X, Pos.Y + MoveListArea.Y);
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(MainAreaPos, SizeWindow, EDITOR_MANAGER.Color2);
        }
        private void Draw_ButtonListHandle(List<MENU_ITEM_ENTITY> List)
        {
            if (List is null) return;

            float BoxOffsetX = 4f;
            float BoxOffsetY = 24f;
            float PaddingEachBox = 4;

            if (IsEnableAdding)
            {
                switch (AddingType)
                {
                    case DataType.Entity:
                    break;
                    case DataType.Scene:
                        MENU_ITEM_ENTITY _MenuItem = new(
                            "+ ADD NEW +",
                            0,
                            "",
                            false,
                            0,
                            ()=>Adding_AddSceneHandle(FABRICK_DEBUG.DialogueInputText("Create New Scene", "Enter Scene Name", "", false))
                        );
                        List.Add(_MenuItem);
                    break;
                }
            }

            for (int i = 0; i < List.Count; i++)
            {
                Vector2 _Pos = new Vector2(MainAreaPos.X + BoxOffsetX, MainAreaPos.Y + (BoxOffsetY * i) + (PaddingEachBox * (i + 1)));
                Vector2 _Size = new Vector2(SizeWindow.X - BoxOffsetX * 3, BoxOffsetY);

                if (List[i].IsFocused)
                {
                    FABRICK_DRAW_SHAPE.DrawFillRect_UI(_Pos, _Size, EDITOR_MANAGER.Color4);
                }
                else
                {
                    FABRICK_DRAW_SHAPE.DrawFillRect_UI(_Pos, _Size, EDITOR_MANAGER.Color1);
                }

                Vector2 _PosText = new Vector2(MainAreaPos.X + BoxOffsetX * 2, MainAreaPos.Y + (BoxOffsetY * i) + (PaddingEachBox * (i + 1)));
                Vector2 _PosText2 = new Vector2(MainAreaPos.X + SizeWindow.X - (BoxOffsetX * 6), MainAreaPos.Y + (BoxOffsetY * i) + (PaddingEachBox * (i + 1)));

                FABRICK_DRAW_TEXT.DrawDevText_UI(List[i].Name, _PosText, 2, EDITOR_MANAGER.Color3, Algin.Left);
                FABRICK_DRAW_TEXT.DrawDevText_UI(List[i].EntityOrder.ToString(), _PosText2, 2, EDITOR_MANAGER.Color3, Algin.Right);

                // OnClick Handle
                if (IsFocused && !MoveAreaState)
                {
                    Box _B = new Box(_Pos, Vector2.Zero, _Size);
                    if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), _B))
                    {
                        if(FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
                        {
                            if (List[i].Action is not null)
                            {
                                List[i].Action!();
                            }
                        }
                        if (FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Right))
                        {
                            List[i].IsFocused = false;
                        }
                    }
                }
            }
        }
    }
}
/*
Color usage:
Dragablearea/Unfocus: Color1
Main Area: Color2
Text: Color3
Focushed: Color4
*/

using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public class MENU_ITEM_ENTITY
    {
        public string Name = "";
        public int Id = 0;
        public string Category = "";
        public int EntityOrder = 0;
        public bool Unremovable = false;
        public Action? Action = null;
        public bool IsFocused = false;

        public MENU_ITEM_ENTITY(string Name, int Id, string Category, bool Unremovable, int EntityOrder, Action Action)
        {
            this.Name = Name;
            this.Id = Id;
            this.Category = Category;
            this.Unremovable = Unremovable;
            this.EntityOrder = EntityOrder;
            this.Action = Action;
        }
        public virtual void ActionFunc()
        {
            Action?.Invoke();
        }
    }
    public class LIST_WINDOW
    {
        private string WindowName = "";
        private bool IsFocused = false;
        private Vector2 Pos, PosDrag = new();
        private Vector2 MoveListArea = new Vector2(250, 25);
        private Vector2 MainAreaPos = new Vector2();
        private Vector2 SizeWindow = new Vector2(250, 400);

        private bool MoveAreaState = false;
        private bool IsHide = false;
        private Vector2 MoveAreaOffset;
        
        public void Initialize(string WindowName)
        {
            this.WindowName = WindowName;
        }
        public void Initialize(string WindowName, Vector2 Pos)
        {
            this.WindowName = WindowName;
            SetPos(Pos);
        }
        public void SetPos(Vector2 Pos)
        {
            this.Pos = Pos;
        }
        public void SetSize(Vector2 Size)
        {
            MoveListArea.X = Size.X;
            MoveListArea.Y = Size.Y / 16;
            SizeWindow.X = Size.X;
            SizeWindow.Y = Size.Y;
        }
        public void Update(List<MENU_ITEM_ENTITY> List)
        {
            Pos = PosDrag;
            Pos.X = FABRICK_MATH.Clamp(Pos.X, EDITOR_MANAGER.GetWindowMinLimit().X, EDITOR_MANAGER.GetWindowMaxLimit().X - MoveListArea.X);
            Pos.Y = FABRICK_MATH.Clamp(Pos.Y, EDITOR_MANAGER.GetWindowMinLimit().Y, EDITOR_MANAGER.GetWindowMaxLimit().Y - MoveListArea.Y - SizeWindow.Y);
            FocusHandler();
            if (IsFocused)
            {
                DragHandle();
                RightClick_HideHandle();   
            }

            // Draw Moveable Window Area
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(Pos, MoveListArea, EDITOR_MANAGER.Color1);
            FABRICK_DRAW_TEXT.DrawDevText_UI(WindowName, new Vector2(Pos.X + MoveListArea.X / 2, Pos.Y), 2, EDITOR_MANAGER.Color3, Algin.Center);

            if (!IsHide)
            {
                Draw_MainArea();
                Draw_ButtonListHandle(List);
            }
        }
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

            for (int i = 0; i < List.Count; i++)
            {
                Vector2 _Pos = new Vector2(MainAreaPos.X + BoxOffsetX, MainAreaPos.Y + (BoxOffsetY * i) + (PaddingEachBox * (i + 1)));
                Vector2 _Size = new Vector2(SizeWindow.X - BoxOffsetX * 3, BoxOffsetY);

                if (List[i].IsFocused)
                {
                    //FABRICK_DEBUG.Log("Tesst@@@@: " + List[i].Name + " Is Focused");
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
                    Box _B = new Box(_Pos, _Pos, _Size);
                    if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), _B) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left))
                    {
                        if (List[i].Action is not null)
                        {
                            List[i].Action!();
                        }
                    }
                }
            }
        }
        private void DragHandle()
        {
            Box MoveableArea = new Box(Pos, Vector2.Zero, MoveListArea);
            if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), MoveableArea) && FABRICK_INPUT.IsMouseDown(SDL.MouseButtonFlags.Left) && !MoveAreaState)
            {
                MoveAreaState = true;
                MoveAreaOffset = PosDrag - FABRICK_INPUT.GetMousePos_UI();
            }
            if (FABRICK_INPUT.IsMouseReleased(SDL.MouseButtonFlags.Left))
            {
                MoveAreaState = false;
            }
            if (MoveAreaState)
            {
                EDITOR_MANAGER.ChangeableFocusWindow = false;
                PosDrag = FABRICK_INPUT.GetMousePos_UI() + MoveAreaOffset;
            }
        }
        private void RightClick_HideHandle()
        {
             Box MoveableArea = new Box(Pos, Vector2.Zero, MoveListArea);
            if (FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Right) && FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), MoveableArea))
            {
                IsHide = !IsHide;
            }
        }
        public string OnFocusClick()
        {
            string name = EDITOR_MANAGER.FocusWindow;
            if (FABRICK_INPUT.IsMouseDown(SDL.MouseButtonFlags.Left)
                && FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), GetWindowBox())
                && EDITOR_MANAGER.FocusWindow != WindowName
                && EDITOR_MANAGER.ChangeableFocusWindow)
            {
                name = WindowName;
            }
            return name;
        }
        private void FocusHandler()
        {
            if (EDITOR_MANAGER.FocusWindow == WindowName)
            {
                IsFocused = true;
            }
            else
            {
                IsFocused = false;
            }
        }
        public Box GetWindowBox()
        {
            return new Box(this.Pos, Vector2.Zero, new Vector2(SizeWindow.X, MoveListArea.Y + SizeWindow.Y));
        }
    }
}
using System.Numerics;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;
using SDL3;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public partial class LIST_WINDOW
    {
        private void DragHandle()
        {
            Box MoveableArea = new Box(Pos, Vector2.Zero, MoveListArea);
            if (FABRICK_INPUT.IsKeyUp(SDL.Scancode.LAlt) && FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), MoveableArea) && FABRICK_INPUT.IsMousePressed(SDL.MouseButtonFlags.Left) && !MoveAreaState)
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
            if (FABRICK_COLLISIONS.MouseClickToBox_Pressed(GetWindowBox(), SDL.MouseButtonFlags.Left, true)
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
        public bool IsDragged()
        {
            return MoveAreaState;
        }
    }
}
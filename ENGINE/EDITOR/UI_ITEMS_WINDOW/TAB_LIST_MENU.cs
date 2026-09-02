using System.Numerics;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.EDITOR.ITEMS
{
    public class TAB_LIST_ITEM
    {
        public string Name = "";
        public Action? Action = null;
        public bool IsFocused = false;

        public TAB_LIST_ITEM(string Name, Action Action)
        {
            this.Name = Name;
            this.Action = Action;
        }
        public virtual void ActionFunc()
        {
            Action?.Invoke();
        }
    }

    public class TAB_LIST_MENU
    {
        private Vector2 Pos = Vector2.Zero;
        private Vector2 Size = new Vector2(1920, 25);
        private Vector2 Margin_TopLeft = new Vector2(2, 2);
        private Vector2 Margin_BottomRight = new Vector2(2, 4);
        private float MarginFromText = 4;
        private float MarginEachButton = 5;
        private float TextSize = 2;
        private List<Box> CollisionBox = new List<Box>();

        public void Initialize()
        {
            
        }
        public void Update(List<TAB_LIST_ITEM> List)
        {
            RenderBackground();
            UpdateRenderButton(List);
            OnClickHandle(List);
        }
        public Vector2 GetMaxSize()
        {
            return Size;
        }

        // Everything Purpose
        private void RenderBackground()
        {
            FABRICK_DRAW_SHAPE.DrawFillRect_UI(Pos, Size, EDITOR_MANAGER.Color2);
        }
        private void UpdateRenderButton(List<TAB_LIST_ITEM> List)
        {
            if(List is null) return;
            CollisionBox.Clear();
            float _Offset = 0;
            for (int i = 0; i < List.Count; i++)
            {
                // Calculate Collision Pos & Size
                
                if (i != 0)
                {
                    _Offset += CollisionBox[i - 1].GetSize().X + MarginEachButton;
                }

                // Calculate Collision and Text Position
                Vector2 _Pos = new Vector2(Margin_TopLeft.X + _Offset, Margin_TopLeft.Y);
                Vector2 _Size = FABRICK_DRAW_TEXT.GetSizeDrawDevText_UI(List[i].Name, TextSize, Algin.Left) + new Vector2(MarginFromText * 2, 0);
                Vector2 _PosText = _Pos + new Vector2(MarginFromText, 0);

                // Create Collision Button
                Box _b = new(_Pos, Vector2.Zero, _Size);
                CollisionBox.Add(_b);

                // On Focus Handle and Draw Button
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), CollisionBox[i]))
                {
                    FABRICK_DRAW_SHAPE.DrawFillRect_UI(_Pos, _Size, EDITOR_MANAGER.Color4);
                }
                else
                {
                    FABRICK_DRAW_SHAPE.DrawFillRect_UI(_Pos, _Size, EDITOR_MANAGER.Color1);
                }

                // Draw Text
                FABRICK_DRAW_TEXT.DrawDevText_UI(List[i].Name, _PosText, TextSize, EDITOR_MANAGER.Color3, Algin.Left, out _Size);
                // Update Tab Size
                Size = new Vector2(Size.X, _Size.Y + Margin_BottomRight.Y);
            }
        }

        private void OnClickHandle(List<TAB_LIST_ITEM> List)
        {
            if (List.Count == 0 || CollisionBox.Count == 0) return;
            if(List.Count != CollisionBox.Count) return;

            for (int i = 0; i < List.Count; i++)
            {
                if (FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), CollisionBox[i]) && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left) && EDITOR_MANAGER.ChangeableFocusWindow)
                {
                    if(List[i] is not null) List[i].Action!();
                }
            }
        }
    }
}
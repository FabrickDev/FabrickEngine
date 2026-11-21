/*
Color usage:
Dragablearea/Unfocus: Color1
Main Area: Color2
Text: Color3
Focushed: Color4
*/

using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;
using Fabrick.ENGINE.SCENE_MANAGER;
using Fabrick.ENGINE.VISUAL_NOVEL;

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
    public enum DataType
    {
        Entity = 0,
        Scene = 1
    }
    public partial class LIST_WINDOW
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

        private bool IsEnableAdding = false;
        private DataType AddingType = DataType.Entity;
        
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
            this.PosDrag = Pos;
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
        public void EnableAddingList(bool State, DataType InputDialogueType)
        {
            IsEnableAdding = State;
            AddingType = InputDialogueType;
        }

        private void Adding_AddSceneHandle(string SceneName)
        {
            if (SceneName.IsWhiteSpace())
            {
                FABRICK_DEBUG.SimpleMessageError("[ENTITY_LIST_WINDOW] Cannot create new scene cuz SceneName is WhiteSpace!!");
                return;
            }
            if (FABRICK_SCENE_MANAGER.AssignScene(SceneName, false, true))
            {
                FABRICK_SCENE_MANAGER.SetCurrentScene(SceneName);
                for (int i = 0; i < FABRICK_DEV_SCENE_EDITOR.SceneList.Count; i++)
                {
                    if (FABRICK_DEV_SCENE_EDITOR.SceneList[i].Name == "NonOverlay@" + SceneName)
                    {
                        FABRICK_DEV_SCENE_EDITOR.SceneList[i].IsFocused = true;
                    }
                }
                
            }
        }
    }
}
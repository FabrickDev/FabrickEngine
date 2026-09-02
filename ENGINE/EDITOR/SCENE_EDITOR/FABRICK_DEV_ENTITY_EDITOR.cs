using System.Numerics;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.RENDERER;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DEV_ENTITY_EDITOR
    {
        public LIST_WINDOW LIST_WINDOW = new LIST_WINDOW();
        private MANIPULATOR MANIPULATOR = new();
        private static List<MENU_ITEM_ENTITY> ObjectList = new List<MENU_ITEM_ENTITY>();
        public bool IsRun = true;

        public void Initialize()
        {
            LIST_WINDOW.Initialize("Scene Entity List", new Vector2(20, 20));
            MANIPULATOR.Initialize();
        }
        public void Update()
        {
            if (IsRun)
            {
                Hotkeys();
                MANIPULATOR.Update();
                EngineEditorHandler();
                UpdateLatestItem();

                MANIPULATOR.Render();

                LIST_WINDOW.Update(ObjectList);
                EDITOR_MANAGER.SetFocusWindow(LIST_WINDOW.OnFocusClick());
            }
            else
            {
                foreach (MENU_ITEM_ENTITY ThisKey in ObjectList)
                {
                    ObjectList[ObjectList.IndexOf(ThisKey)].IsFocused = false;
                }
            }
        }
        private void Hotkeys()
        {
            if (FABRICK_INPUT.IsKeyPressed(SDL3.SDL.Scancode.F))
            {
                // Getting Position of Focused Entity
                Vector2 ThisEntityPos = Vector2.Zero;
                foreach(MENU_ITEM_ENTITY ThisObject in ObjectList){
                if (ThisObject.IsFocused == true)
                {
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.ENTITY_LIST)
                    {
                        if (ThisObject.Name == ThisEntity.Name && ThisObject.Id == ThisEntity.Id && ThisObject.Category == ThisEntity.Category && ThisObject.Unremovable == ThisEntity.Unremovable && ThisObject.EntityOrder == ThisEntity.EntityOrder)
                        {
                            ThisEntityPos = ThisEntity.Transform.GetPosition();
                        }
                    }
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST)
                    {
                        if (ThisObject.Name == ThisEntity.Name && ThisObject.Id == ThisEntity.Id && ThisObject.Category == ThisEntity.Category && ThisObject.Unremovable == ThisEntity.Unremovable && ThisObject.EntityOrder == ThisEntity.EntityOrder)
                        {
                           ThisEntityPos = ThisEntity.Transform.GetPosition();
                        }
                    }
                }
            }
                FABRICK_CAMERA_MANAGER.CurrentCameraList[FABRICK_CAMERA_MANAGER.GetCurrentCamera()].SetCameraPos(ThisEntityPos);
            }
        }
        public static void AddObjectList(FABRICK_ENTITY Entity)
        {
            bool Cancel = false;
            MENU_ITEM_ENTITY Item = new MENU_ITEM_ENTITY(Entity.Name, Entity.Id, Entity.Category, Entity.Unremovable, Entity.EntityOrder, ()=>ActionOnClick(Entity.Name, Entity.Id, Entity.Category, Entity.Unremovable, Entity.EntityOrder));

            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i].Name == Item.Name && ObjectList[i].Id == Item.Id && ObjectList[i].Category == Item.Category && Item.Unremovable == ObjectList[i].Unremovable && Item.EntityOrder == ObjectList[i].EntityOrder)
                {
                    Cancel = false;
                }
            }
            if (Cancel)
            {
                return;
            }
            ObjectList.Add(Item);
        }
        public static void ActionOnClick(string Name, int Id, string Category, bool Unremovable, int EntityOrder)
        {
            for (int i = 0; i < ObjectList.Count; i++){
                ObjectList[i].IsFocused = false;
                if (ObjectList[i].Name == Name && ObjectList[i].Id == Id && ObjectList[i].Category == Category && ObjectList[i].Unremovable == Unremovable && ObjectList[i].EntityOrder == EntityOrder)
                {
                    ObjectList[i].IsFocused = true;
                }
            }
        }
        private void EngineEditorHandler()
        {
            foreach(MENU_ITEM_ENTITY ThisObject in ObjectList){
                if (ThisObject.IsFocused == true)
                {
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.ENTITY_LIST)
                    {
                        if (ThisObject.Name == ThisEntity.Name && ThisObject.Id == ThisEntity.Id && ThisObject.Category == ThisEntity.Category && ThisObject.Unremovable == ThisEntity.Unremovable && ThisObject.EntityOrder == ThisEntity.EntityOrder)
                        {
                            FABRICK_DRAW_SHAPE.DrawRect(ThisEntity.Transform.GetPosition() - new Vector2(15, 15), new Vector2(30, 30), new Color(255, 0, 255, 255));
                            MANIPULATOR.SetPosition(ThisEntity.Transform.GetPosition());
                            MANIPULATOR.SetActiveState(true);
                            FABRICK_ENTITY_MANAGER.ENTITY_LIST[FABRICK_ENTITY_MANAGER.ENTITY_LIST.IndexOf(ThisEntity)].Transform = MANIPULATOR.UpdateEntity(ThisEntity.Transform);
                        }
                    }
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST)
                    {
                        if (ThisObject.Name == ThisEntity.Name && ThisObject.Id == ThisEntity.Id && ThisObject.Category == ThisEntity.Category && ThisObject.Unremovable == ThisEntity.Unremovable && ThisObject.EntityOrder == ThisEntity.EntityOrder)
                        {
                            FABRICK_DRAW_SHAPE.DrawRect(ThisEntity.Transform.GetPosition() - new Vector2(15, 15), new Vector2(30, 30), new Color(255, 0, 255, 255));
                            MANIPULATOR.SetPosition(ThisEntity.Transform.GetPosition());
                            MANIPULATOR.SetActiveState(true);
                            FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST.IndexOf(ThisEntity)].Transform = MANIPULATOR.UpdateEntity(ThisEntity.Transform);
                        }
                    }
                }
            }
        }
        private void UpdateLatestItem()
        {
            InitialGetItem();
            List<bool> IsFocushedSave = new List<bool>();
            
            int PrevCount = 0;
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count; i++)
            {
                IsFocushedSave.Add(ObjectList[i].IsFocused);
                PrevCount++;
            }
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                IsFocushedSave.Add(ObjectList[PrevCount + i].IsFocused);
                PrevCount++;
            }

            ObjectList.Clear();
            PrevCount = 0;

            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.ENTITY_LIST[i]);
                ObjectList[i].IsFocused = IsFocushedSave[i];
                PrevCount++;
            }
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[i]);
                ObjectList[PrevCount + i].IsFocused = IsFocushedSave[PrevCount + i];
                PrevCount++;
            }
        }
        private void InitialGetItem()
        {
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.ENTITY_LIST[i]);
            }
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[i]);
            }
        }
    }
}
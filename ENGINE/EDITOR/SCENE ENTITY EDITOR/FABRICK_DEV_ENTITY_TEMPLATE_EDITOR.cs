using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;

namespace Fabrick.ENGINE.EDITOR
{
    public class FABRICK_DEV_ENTITY_TEMPLATE_EDITOR
    {
        public LIST_WINDOW LIST_WINDOW = new LIST_WINDOW();
        private static List<MENU_ITEM_ENTITY> ObjectList = new List<MENU_ITEM_ENTITY>();
        public bool IsRun = true;

        public void Initialize()
        {
            LIST_WINDOW.Initialize("Entity Template List", new Vector2(20, 20));
        }
        public void Update()
        {
            if (IsRun)
            {
                UpdateLatestItem();
                LIST_WINDOW.Update(ObjectList);
                EDITOR_MANAGER.SetFocusWindow(LIST_WINDOW.OnFocusClick());
                AddToSceneHandle();
            }
            else
            {
                foreach (MENU_ITEM_ENTITY ThisKey in ObjectList)
                {
                    ObjectList[ObjectList.IndexOf(ThisKey)].IsFocused = false;
                }
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
                if (ObjectList[i].Name == Name && 
                    ObjectList[i].Id == Id && 
                    ObjectList[i].Category == Category && 
                    ObjectList[i].Unremovable == Unremovable && 
                    ObjectList[i].EntityOrder == EntityOrder)
                {
                    ObjectList[i].IsFocused = true;
                }
            }
        }
        private void UpdateLatestItem()
        {
            InitialGetItem();
            List<bool> IsFocushedSave = new List<bool>();
            
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE.Count; i++)
            {
                IsFocushedSave.Add(ObjectList[i].IsFocused);
            }

            ObjectList.Clear();

            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE[i]);
                ObjectList[i].IsFocused = IsFocushedSave[i];
            }
        }
        private void InitialGetItem()
        {
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE.Count; i++)
            {
                AddObjectList(FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE[i]);
            }
        }

        // Add to Scene Handle
        private void AddToSceneHandle()
        {
            bool _action = false;
            string _name = "";
            int _id = 0;
            string _category = "";
            bool _unremovable = false;
            int _entityOrder = 0;
            
            for (int i = 0; i < ObjectList.Count; i++)
            {
                if (ObjectList[i].IsFocused)
                {
                    _action = true;

                    _name = ObjectList[i].Name;
                    _id = ObjectList[i].Id;
                    _category = ObjectList[i].Category;
                    _unremovable = ObjectList[i].Unremovable;
                }
            }

            // Cancel when no item focused
            if (_action == false)
            {
                return;
            }

            if (!(FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), LIST_WINDOW.GetWindowBox()) || ENTITY_SCENE_EDITOR_MANAGER.Is_ENTITY_EDITOR_MENU_CollidedPoint()) && FABRICK_INPUT.IsMousePressed(SDL3.SDL.MouseButtonFlags.Left))
            {
                foreach (FABRICK_ENTITY ThisEntityTemplate in FABRICK_ENTITY_MANAGER.ENTITY_LIST_TEMPLATE)
                {
                    if (ThisEntityTemplate.Name == _name &&
                        ThisEntityTemplate.Id == _id &&
                        ThisEntityTemplate.Category == _category)
                    {
                        if (!_unremovable)
                        {
                            _entityOrder = FABRICK_ENTITY_MANAGER.EntityOrder;
                            FABRICK_ENTITY_MANAGER.EntityOrder++;
                        }
                        else
                        {
                            _entityOrder = FABRICK_ENTITY_MANAGER.UnremovableEntityOrder;
                            FABRICK_ENTITY_MANAGER.UnremovableEntityOrder++;
                        }
                        FABRICK_ENTITY_MANAGER.AssignEntityList(_name, _id, _category, _entityOrder, false);
                        
                        if (!_unremovable)
                        {
                            FABRICK_ENTITY_MANAGER.ENTITY_LIST[FABRICK_ENTITY_MANAGER.GetEntityIndex(_name, _id, _category, _entityOrder)].Transform.SetPosition(FABRICK_INPUT.GetMousePos());
                        }
                        else
                        {
                            FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[FABRICK_ENTITY_MANAGER.GetUnremovableEntityIndex(_name, _id, _category, _entityOrder)].Transform.SetPosition(FABRICK_INPUT.GetMousePos());
                        }
                    }
                }
                FABRICK_DEBUG.Log($"[FABRICK_DEV_ENTITY_TEMPLATE_EDITOR] Added Entity: {_name}, ID: {_id}, Category: {_category}, Order: {_entityOrder}, Unremovable: {_unremovable}");
            }
            /*
                ObjectList[i].Name == Name && 
                ObjectList[i].Id == Id && 
                ObjectList[i].Category == Category && 
                ObjectList[i].Unremovable == Unremovable && 
                ObjectList[i].EntityOrder == EntityOrder
            */
        }
    }
}
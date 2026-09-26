using System.Diagnostics.Tracing;
using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.INPUT;
using Fabrick.ENGINE.MATH;
using Fabrick.ENGINE.PHYSICS;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR
{
    public class ENTITY_SCENE_EDITOR_MANAGER
    {
        // Parts
        private static FABRICK_DEV_ENTITY_EDITOR ENTITY_EDITOR_MENU = new();
        private static FABRICK_DEV_ENTITY_TEMPLATE_EDITOR ENTITY_TEMPLATE_EDITOR_MENU = new();
        public static void Initialize()
        {
            ENTITY_EDITOR_MENU.Initialize();
            ENTITY_TEMPLATE_EDITOR_MENU.Initialize();
            Connect_Entity_w_EntityTemplate_InitializeHandle();
        }
        public static void Update()
        {
            UpdateableCurrentEditorType();
            ENTITY_EDITOR_MENU.Update();
            ENTITY_TEMPLATE_EDITOR_MENU.Update(); 
            Connect_Entity_w_EntityTemplate_UpdateHandle();
        }

        public static bool Is_ENTITY_EDITOR_MENU_CollidedPoint()
        {
            return FABRICK_COLLISIONS.IntersectCollisionPointBox(FABRICK_INPUT.GetMousePos_UI(), ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox());
        }

        private static void UpdateableCurrentEditorType()
        {
            if (FABRICK_SCENE_MANAGER.GetCurrentScene().Contains(EDITOR_MANAGER.DevKey))
            {
                ENTITY_EDITOR_MENU.IsRun = false;
                ENTITY_TEMPLATE_EDITOR_MENU.IsRun = false;
            }
            else
            {
                ENTITY_EDITOR_MENU.IsRun = true;
                ENTITY_TEMPLATE_EDITOR_MENU.IsRun = true;
            }
        }

        private static void Connect_Entity_w_EntityTemplate_InitializeHandle()
        {
            ENTITY_EDITOR_MENU.Update();
            ENTITY_TEMPLATE_EDITOR_MENU.Update();

            // Entity Template Editor Left Side - Entity Editor Right Side
            ENTITY_EDITOR_MENU.LIST_WINDOW.SetPos(ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos() + 
                                                  new Vector2(ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetSize().X, 0));
        }
        private static void Connect_Entity_w_EntityTemplate_UpdateHandle()
        {
            // Update Their Position
            if (ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.IsDragged())
            {
                ENTITY_EDITOR_MENU.LIST_WINDOW.SetPos(ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos() + 
                                                  new Vector2(ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetSize().X, 0));
            }
            if (ENTITY_EDITOR_MENU.LIST_WINDOW.IsDragged())
            {
                ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.SetPos(ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos() - 
                                                  new Vector2(ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetSize().X, 0));
            }

            // Clamp Their Position X
            // float _Pos = ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos().X;
            // float _Size = ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetSize().X + ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetSize().X;

            // ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.SetPos(new Vector2(FABRICK_MATH.Clamp(_Pos, EDITOR_MANAGER.GetWindowMinLimit().X, EDITOR_MANAGER.GetWindowMaxLimit().X - _Size),
            //                                                 ENTITY_TEMPLATE_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos().Y));
            
            //ENTITY_EDITOR_MENU.LIST_WINDOW.SetPos(new Vector2(_Pos + _Size, ENTITY_EDITOR_MENU.LIST_WINDOW.GetWindowBox().GetPos().Y));
        }
    }
}
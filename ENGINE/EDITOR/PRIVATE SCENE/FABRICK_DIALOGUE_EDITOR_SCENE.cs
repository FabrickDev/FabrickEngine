using System.Numerics;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.EDITOR.ITEMS;
using Fabrick.ENGINE.ENTITY;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.EDITOR.SCENE
{
    public class FABRICK_DIALOGUE_EDITOR_SCENE : SCENE_DATA
    {
        private TAB_LIST_MENU FDEC_MENU = new();
        private List<TAB_LIST_ITEM> FDEC_ITEMS = new();

        public FABRICK_DIALOGUE_EDITOR_SCENE()
        {
            SceneName = "@DEV.DIALOGUE_EDITOR";
            InitializeAction = Initialize_Func;
            RunningAction = Update_Func;
            RenderAction = Render_Func;
            LeaveAction = Leave_Func;
        }
        public void Initialize_Func()
        {
            FABRICK_ENTITY_MANAGER.RemoveCompleteAllEntityList();
            FDEC_MENU.Initialize(new Vector2(0, 26));
            FDEC_ITEMS.Add(new TAB_LIST_ITEM("Hello", ()=>Action1()));
            //ThisScene.Initialize();
        }
        public void Update_Func()
        {
            // Do not put here
            //ThisScene.Update();
        }
        public void Render_Func()
        {
            FDEC_MENU.Update(FDEC_ITEMS);
            //ThisScene.Render();
        }
        public void Leave_Func()
        {
            
        }
        private void Action1()
        {
            FABRICK_DEBUG.SimpleMessageInfo("Helloooooo");
        }
    }
}
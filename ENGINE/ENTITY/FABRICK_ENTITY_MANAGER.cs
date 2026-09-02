using Fabrick.ENGINE.CORE;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.ENTITY
{
    public class FABRICK_ENTITY_MANAGER
    {
        private static List<FABRICK_ENTITY> ENTITY_LIST_TEMPLATE = new List<FABRICK_ENTITY>();
        public static List<FABRICK_ENTITY> ENTITY_LIST = new List<FABRICK_ENTITY>();
        public static List<FABRICK_ENTITY> UNREMOVABLE_ENTITY_LIST = new List<FABRICK_ENTITY>();
        public static int EntityOrder = 0; //Add 1 every write to json // Then Back to 0 when change scene
        public static int UnremovableEntityOrder = 0;

        public static void AssignEntityTemplate(FABRICK_ENTITY Entity)
        {
            foreach (FABRICK_ENTITY ThisEntity in ENTITY_LIST_TEMPLATE)
            {
                if (Entity.Name == ThisEntity.Name && Entity.Id == ThisEntity.Id && Entity.Category == ThisEntity.Category)
                {
                    FABRICK_DEBUG.SimpleMessageWarning($"Cannot assign this entity: {Entity.Name} !!");
                    return;
                }
            }
            ENTITY_LIST_TEMPLATE.Add(Entity);
        }
        public static void AssignEntityList(string Name, int Id, string Category, int EntityOrder, bool Unremovable = false)
        {
            for (int i = 0; i < ENTITY_LIST_TEMPLATE.Count; i++)
            {
                if (ENTITY_LIST_TEMPLATE[i].Name == Name && ENTITY_LIST_TEMPLATE[i].Id == Id && ENTITY_LIST_TEMPLATE[i].Category == Category)
                {
                    var ENTITYY = ENTITY_LIST_TEMPLATE[i].Clone();

                    FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Adding Entity List: {Name}; Id: {Id}; Category: {Category};");
                    if (Unremovable)
                    {
                        UNREMOVABLE_ENTITY_LIST.Add(ENTITYY);
                        UNREMOVABLE_ENTITY_LIST[UNREMOVABLE_ENTITY_LIST.Count - 1].EntityOrder = EntityOrder;
                        UNREMOVABLE_ENTITY_LIST[UNREMOVABLE_ENTITY_LIST.Count - 1].Unremovable = true;
                        FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Assigned to list this entity as Unremovable Entity: {Name}; Id: {Id}; Category: {Category};");
                    }
                    else
                    {
                        ENTITY_LIST.Add(ENTITYY);
                        ENTITY_LIST[ENTITY_LIST.Count - 1].EntityOrder = EntityOrder;
                        ENTITY_LIST[ENTITY_LIST.Count - 1].Unremovable = false;
                        FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Assigned to list this entity as Normal Entity: {Name}; Id: {Id}; Category: {Category};");
                    }
                    return;
                }
            }
            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Cannot assign this entity: {Name}; Id: {Id}; Category: {Category};");
            return;
        }
        public static void RemoveAllEntityList()
        {
            FABRICK_DEBUG.Log("[ENTITY_MANAGER] Remove All Normal Entity List...");
            ENTITY_LIST.Clear();
        }
        public static void RemoveCompleteAllEntityList()
        {
            RemoveAllEntityList();
            FABRICK_DEBUG.Log("[ENTITY_MANAGER] Remove All Unremovable Entity List...");
            UNREMOVABLE_ENTITY_LIST.Clear();
        }
        public static void InitializeAllEntity()
        {
            for (int i = 0; i < ENTITY_LIST.Count; i++)
            {
                ENTITY_LIST[i].EntityInitialize();
            }
            for (int i = 0; i < UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                UNREMOVABLE_ENTITY_LIST[i].EntityInitialize();
            }
        }
        public static void UpdateAllEntity(FABRICK_ENGINE Engine)
        {
            for (int i = 0; i < ENTITY_LIST.Count; i++)
            {
                ENTITY_LIST[i].EntityUpdate(Engine);
            }
            for (int i = 0; i < UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                UNREMOVABLE_ENTITY_LIST[i].EntityUpdate(Engine);
            }
        }
        public static void RenderAllEntity()
        {
            for (int i = 0; i < ENTITY_LIST.Count; i++)
            {
                ENTITY_LIST[i].EntityRender();
            }
            for (int i = 0; i < UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                UNREMOVABLE_ENTITY_LIST[i].EntityRender();
            }
        }
        public static FABRICK_ENTITY GetEntity(string Name, int Id, string Category)
        {
            FABRICK_ENTITY E = new FABRICK_ENTITY();
            foreach (FABRICK_ENTITY ThisEntity in ENTITY_LIST)
            {
                if (ThisEntity.Name == Name && ThisEntity.Id == Id && ThisEntity.Category == Category)
                {
                    return ThisEntity;
                }
            }
            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Cnnot GetEntity: {Name}; Id: {Id}; Category{Category};");
            return E;
        }
        public static int GetEntityIndex(string Name, int Id, string Category)
        {
            if (ENTITY_LIST.Count < 1)
            {
                return 0;
            }
            for (int i = 0; i < ENTITY_LIST.Count; i++)
            {
                if (ENTITY_LIST[i].Name == Name && ENTITY_LIST[i].Id == Id && ENTITY_LIST[i].Category == Category)
                {
                    return i;
                }
            }
            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Cnnot GetEntityIndex: {Name}; Id: {Id}; Category{Category};");
            return 0;
        }
    }
}
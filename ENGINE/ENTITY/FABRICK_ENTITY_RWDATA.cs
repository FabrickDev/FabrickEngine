using System.Text.Json;
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.ENTITY
{
    public struct ENTITY_RWDATA_TRANSFORM_DATA
    {
        public float PosX {get; set;} 
        public float PosY {get; set;}
        public float SizeX {get; set;}
        public float SizeY {get; set;}
        public float Angle {get; set;}
    }
    public class FABRICK_ENTITY_RWDATA_STRUCT
    {
        public string? Name {get; set; }
        public string? Category {get; set;}
        public int Id {get; set;}

        public int EntityOrder {get; set;}

        public bool Unremovable {get; set;}
        public float PosX {get; set;} 
        public float PosY {get; set;}
        public float SizeX {get; set;}
        public float SizeY {get; set;}
        public float Angle {get; set;}
    }
    public class FABRICK_ENTITY_RWDATA
    {
        public static string EngineDataPath = "";
        private static string FolderName = "Scene";
        private static string FileExtension = ".json";
        public static void SetEngineDataPath(string Path)
        {
            EngineDataPath = Path + "\\";
        }
        public static async Task WriteEntityData(string Name)
        {
            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Write Entity Data for {Name} ...");
            string PT = "";
            
            PT = Path.Combine(EngineDataPath, FolderName);
            if (!Directory.Exists(PT))
            {
                FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Creating new directory cuz ts didnt exist: {PT}...");
                Directory.CreateDirectory(PT);
            }
            PT = Path.Combine(PT, Name + FileExtension);

            var JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Writing...");
            int EntityIteratorListCount = 0;
            List<FABRICK_ENTITY_RWDATA_STRUCT> L = new List<FABRICK_ENTITY_RWDATA_STRUCT>();
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.ENTITY_LIST.Count; i++)
            {
                L.Add(EntityToRWData(FABRICK_ENTITY_MANAGER.ENTITY_LIST[i]));
                L[i].EntityOrder = FABRICK_ENTITY_MANAGER.ENTITY_LIST[i].EntityOrder;
                L[i].Unremovable = false;
                EntityIteratorListCount++;
            }
            for (int i = 0; i < FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST.Count; i++)
            {
                L.Add(EntityToRWData(FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[i]));
                L[EntityIteratorListCount + i].EntityOrder = FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[i].EntityOrder;
                L[EntityIteratorListCount + i].Unremovable = true;
            }

            string EntityDataString = JsonSerializer.Serialize(L);
            
            File.WriteAllText(PT, EntityDataString);

            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Write Entity Data for {Name} finished!!");
        }
        public static bool ReadEntityData(string Name)
        {
            FABRICK_DEBUG.Log($"[FABRICK_ENTITY_RWDATA] Reading Entity Data for {Name} ...");
            string PT = "";
            PT = Path.Combine(EngineDataPath, FolderName);
            if (!Directory.Exists(PT))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_ENTITY_RWDATA] Directory: {PT} not found!!");
                return false;
            }
            PT = Path.Combine(PT, Name + FileExtension);
            if (!File.Exists(PT))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_ENTITY_RWDATA] File: {PT} not found");
                return false;
            }

            string EntityDataString = File.ReadAllText(PT);
            List<FABRICK_ENTITY_RWDATA_STRUCT> I = JsonSerializer.Deserialize<List<FABRICK_ENTITY_RWDATA_STRUCT>>(EntityDataString) ?? new List<FABRICK_ENTITY_RWDATA_STRUCT>();

            FABRICK_ENTITY_MANAGER.RemoveAllEntityList();
            
            for (int i = 0; i < I.Count; i++)
            {
                FABRICK_DEBUG.Log($"[FABRICK_ENTITY_RWDATA] Read Entity Data, Name: {I[i].Name}");
                if (I[i].Unremovable) //Bagaimana ngecek kalau sudah ada di memory biar enggak Assign lagi?
                {
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST)
                    {
                        FABRICK_ENTITY_MANAGER.UnremovableEntityOrder = Math.Max(FABRICK_ENTITY_MANAGER.UnremovableEntityOrder, I[i].EntityOrder + 1);

                        if(ThisEntity.EntityOrder == I[i].EntityOrder && 
                            ThisEntity.Name == I[i].Name &&
                            ThisEntity.Id == I[i].Id &&
                            ThisEntity.Category == I[i].Category)
                        {
                            //This Entity Order Already in Unremovable!! Then shouldnt be add again
                            FABRICK_DEBUG.Log($"[FABRICK_ENTITY_RWDATA] This Entity Order already on the Unremovable List!! : {ThisEntity.EntityOrder}, {ThisEntity.Name}, {ThisEntity.Id}");
                            return false;
                        }
                    }
                    int U = I[i].EntityOrder;

                    FABRICK_ENTITY_MANAGER.AssignEntityList(I[i].Name, I[i].Id, I[i].Category, I[i].EntityOrder, true);
                    FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[U].Transform.SetPosition(new System.Numerics.Vector2(I[i].PosX, I[i].PosY));
                    FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[U].Transform.SetSize(new System.Numerics.Vector2(I[i].SizeX, I[i].SizeY));
                    FABRICK_ENTITY_MANAGER.UNREMOVABLE_ENTITY_LIST[U].Transform.SetAngle(I[i].Angle);
                }
                else
                {
                    foreach (FABRICK_ENTITY ThisEntity in FABRICK_ENTITY_MANAGER.ENTITY_LIST)
                    {
                        FABRICK_ENTITY_MANAGER.EntityOrder = Math.Max(FABRICK_ENTITY_MANAGER.EntityOrder, I[i].EntityOrder + 1);
        
                        if(ThisEntity.EntityOrder == I[i].EntityOrder &&
                            ThisEntity.Name == I[i].Name &&
                            ThisEntity.Id == I[i].Id &&
                            ThisEntity.Category == I[i].Category)
                        {
                            //This Entity Order Already in Normal List!! Then shouldnt be add again
                            FABRICK_DEBUG.Log($"[ENTITY MANAGER] This Entity Order already on the Normal List!! : {ThisEntity.EntityOrder}");
                            return false;
                        }
                    }
                    if(I[i].Name is not null && I[i].Category is not null)
                    {
                        FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Assigning Entity to Normal List, Name: {I[i].Name}, Id: {I[i].Id}, Category: {I[i].Category}, Entity Order: {I[i].EntityOrder}");
                    }
                    else
                    {
                        FABRICK_DEBUG.SimpleMessageError($"[ENTITY_MANAGER] Entity Name or Category is null!!");
                        return false;
                    }
                    int U = I[i].EntityOrder;
                    FABRICK_ENTITY_MANAGER.AssignEntityList(I[i].Name, I[i].Id, I[i].Category, I[i].EntityOrder, false);
                    FABRICK_ENTITY_MANAGER.ENTITY_LIST[U].Transform.SetPosition(new System.Numerics.Vector2(I[i].PosX, I[i].PosY));
                    FABRICK_ENTITY_MANAGER.ENTITY_LIST[U].Transform.SetSize(new System.Numerics.Vector2(I[i].SizeX, I[i].SizeY));
                    FABRICK_ENTITY_MANAGER.ENTITY_LIST[U].Transform.SetAngle(I[i].Angle);
                }
            }
            FABRICK_DEBUG.Log($"[ENTITY_MANAGER] Reading Entity Data for {Name} finished!!");
            return true;
        }

        private static FABRICK_ENTITY_RWDATA_STRUCT EntityToRWData(FABRICK_ENTITY Entity)
        {
            FABRICK_ENTITY_RWDATA_STRUCT t = new FABRICK_ENTITY_RWDATA_STRUCT();
            t.Name      = Entity.Name;
            t.Category  = Entity.Category;
            t.Id        = Entity.Id;

        // Transform Data 
            t.PosX = Entity.Transform.PositionX;
            t.PosY = Entity.Transform.PositionY;
            t.SizeX = Entity.Transform.SizeX;
            t.SizeY = Entity.Transform.SizeY;
            t.Angle = Entity.Transform.Angle;

            return t;
        }
    }
}
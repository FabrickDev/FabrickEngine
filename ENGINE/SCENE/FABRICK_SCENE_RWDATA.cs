using System.Text.Json;
using Fabrick.ENGINE.DEBUG;
using Fabrick.ENGINE.SCENE_MANAGER;

namespace Fabrick.ENGINE.ENTITY
{
    public class FABRICK_SCENE_RWDATA_STRUCT
    {
        public string? Name { get; set; }
        public bool IsMain { get; set; }
    }
    public class FABRICK_SCENE_RWDATA
    {
        public static string EngineDataPath = "";
        private static string FolderName = "Scene";
        private static string FileName = "SceneList";
        private static string FileExtension = ".json";
        public static void SetEngineDataPath(string Path)
        {
            EngineDataPath = Path + "\\";
        }
        public static async Task WriteSceneData()
        {
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Write Scene List Data...");
            string PT = "";

            PT = Path.Combine(EngineDataPath, FolderName);
            if (!Directory.Exists(PT))
            {
                FABRICK_DEBUG.Log($"[SCENE_MANAGER] Creating new directory cuz ts didnt exist: {PT}...");
                Directory.CreateDirectory(PT);
            }
            PT = Path.Combine(PT, FileName + FileExtension);

            var JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Writing...");
            List<FABRICK_SCENE_RWDATA_STRUCT> L = new List<FABRICK_SCENE_RWDATA_STRUCT>();

            foreach (SCENE_DATA ThisScene in FABRICK_SCENE_MANAGER.SceneList.Values)
            {
                L.Add(SceneToRWData(ThisScene));
            }

            string SceneDataString = JsonSerializer.Serialize(L);

            File.WriteAllText(PT, SceneDataString);

            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Write Scene List Data in {PT} finished!!");
        }
        public static bool ReadSceneData()
        {
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Reading Scene List Data ...");
            string PT = "";
            PT = Path.Combine(EngineDataPath, FolderName);
            if (!Directory.Exists(PT))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Directory: {PT} not found!!");
                return false;
            }
            PT = Path.Combine(PT, FileName + FileExtension);
            if (!File.Exists(PT))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] File: {PT} not found");
                return false;
            }

            string SceneDataString = File.ReadAllText(PT);
            if (string.IsNullOrWhiteSpace(SceneDataString))
            {
                FABRICK_DEBUG.SimpleMessageError($"[SCENE_MANAGER] Scene Data Invalid!!");
                return false;
            }

            List<FABRICK_SCENE_RWDATA_STRUCT> I = JsonSerializer.Deserialize<List<FABRICK_SCENE_RWDATA_STRUCT>>(SceneDataString) ?? new List<FABRICK_SCENE_RWDATA_STRUCT>();

            FABRICK_SCENE_MANAGER.RemoveAllSceneList();

            for (int i = 0; i < I.Count; i++)
            {
                bool _fullfill = false;
                string _name = "";
                if (I[i].Name is not null)
                {
                    _fullfill = true;
                    _name = I[i].Name ?? "";
                }

                FABRICK_DEBUG.Log($"[SCENE_MANAGER] Read Scene Data, Name: {I[i].Name}");
                if (I[i].IsMain)
                {
                    if(_fullfill)
                    {
                        FABRICK_SCENE_MANAGER.SetCurrentScene(_name);
                    }
                }
                if(_fullfill)
                {
                    FABRICK_SCENE_MANAGER.AssignScene(_name, I[i].IsMain);
                }
            }
            FABRICK_DEBUG.Log($"[SCENE_MANAGER] Reading Scene Data finished!!");
            return true;
        }

        private static FABRICK_SCENE_RWDATA_STRUCT SceneToRWData(SCENE_DATA Scene)
        {
            FABRICK_SCENE_RWDATA_STRUCT t = new FABRICK_SCENE_RWDATA_STRUCT();
            t.Name = Scene.SceneName;
            t.IsMain = Scene.IsMain;

            return t;
        }
    }
}
using Fabrick.ENGINE.DEBUG;

namespace Fabrick.ENGINE.IOFILES
{
    public class FABRICK_IOFILES_MANAGER
    {
        internal static string ThisEnginePath = "";
        public static List<string> GetFiles(string path, bool UseEnginePath = true, bool RemoveEnginePath = true)
        {
            string ThisDirectory = "";
            if (UseEnginePath)
            {
                ThisDirectory = Path.Combine(ThisEnginePath, path);
            }
            else
            {
                ThisDirectory = path;
            }

            if (!Directory.Exists(ThisDirectory))
            {
                FABRICK_DEBUG.SimpleMessageError($"[FABRICK_IOFILES_MANAGER] Directory Doesn't Exist: {ThisDirectory}");
                return new();
            }
            return GetFiles_NoChecking(ThisDirectory, RemoveEnginePath);
        }
        public static List<string> GetFiles_NoChecking(string path, bool RemoveEnginePath = true)
        {
            List<string> _tempFiles = new();
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string ThisFile in files)
            {
                string _temp = "";
                if (RemoveEnginePath)
                {
                    for(int i = 1; i < ThisFile.Replace(Path.Combine(ThisEnginePath, path), "").Length; i++)
                    {
                        _temp += ThisFile.Replace(Path.Combine(ThisEnginePath, path), "")[i];
                    }
                }
                _tempFiles.Add(_temp);
            }
            return _tempFiles;
        }
    }
}
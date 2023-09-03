using System;
using Directory = System.IO.Directory;

namespace Util
{
    public static class UtilJsonFile
    {
        public static string[] GetSaveFiles()
        {
#if UNITY_EDITOR
            const string path = Settings.DevSaveDirectory + "/";
#else
         string path = Application.persistentDataPath + "/";
#endif
            var files = Directory.GetFiles(path, "*" + Settings.SaveFileSuffix);
            Array.Sort(files);
            return files;
        }

        public static string GetJsonFromFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    }
}
using System;
using Directory = System.IO.Directory;

namespace Util
{
    /**
     * Problem: Provide helper methods for save file IO.
     * Goal: Locate save files and read JSON contents.
     * Approach: Use System.IO APIs with platform paths.
     * Time: O(n) to list files.
     * Space: O(n) for file list.
     */
    public static class UtilJsonFile
    {
        public static string[] GetSaveFiles()
        {
#if UNITY_EDITOR
            const string path = Settings.DevSaveDirectory + "/";
#else
         string path = Application.persistentDataPath + "/";
#endif
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return Array.Empty<string>();
            }

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

using System;
using UnityEngine;
using Directory = System.IO.Directory;

public static class UtilJSONFile
{
    public static string[] GetSaveFiles()
    {
#if UNITY_EDITOR
        string path = Settings.DevSaveDirectory + "/";
#else
         string path = Application.persistentDataPath + "/";
#endif   
        string[] files = Directory.GetFiles(path, "*" + Settings.SaveFileSuffix);
        Array.Sort(files);
        return files;
    }

    public static string GetJsonFromFile(string path)
    {
        return System.IO.File.ReadAllText(path);
    }
}
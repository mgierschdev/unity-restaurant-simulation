using System;
using UnityEngine;
using Directory = System.IO.Directory;

public static class UtilJSONFile
{
    public static string[] GetSaveFiles()
    {
        string path = Settings.devEnv ? Settings.DevSaveDirectory + "/" : Application.persistentDataPath + "/";
        string[] files = Directory.GetFiles(path, "*" + Settings.SaveFileSuffix);
        Array.Sort(files);
        return files;
    }

    public static string GetJsonFromFile(string path)
    {
        return System.IO.File.ReadAllText(path);
    }
}
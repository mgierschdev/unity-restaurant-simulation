using UnityEngine;
using Directory = System.IO.Directory;

public static class UtilJSONFile
{
    public static string[] GetSaveFiles()
    {
        string path = Settings.DevEnv ? Settings.DevSaveDirectory + "/" : Application.persistentDataPath + "/";
        string[] files = Directory.GetFiles(path, "*" + Settings.SaveFileSuffix);
        return files;
    }

    public static string GetJsonFromFile(string path)
    {
        string json = System.IO.File.ReadAllText(path);
        return json;
    }
}
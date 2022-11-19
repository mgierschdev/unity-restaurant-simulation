using UnityEngine;
using Directory = System.IO.Directory;

public static class UtilSaveFile
{
    public static string[] GetSaveFiles()
    {
        string path = Settings.DevEnv ? Settings.DevSaveDirectory + "/" : Application.persistentDataPath + "/";
        string[] files = Directory.GetFiles(path, "*" + Settings.SaveFileSuffix);
        return files;
    }
}
using UnityEngine;
using Directory = System.IO.Directory;

public static class UtilSaveFile
{
    public static string[] GetSaveFiles()
    {
        string path = Settings.DevEnv ? Settings.TestSaveDirectory + "/" : Application.persistentDataPath + "/";
        string[] files = Directory.GetFiles(path, "*save.json");

        Debug.Log("GetLatestSaveFile()");

        foreach (string file in files)
        {
            Debug.Log(file);
        }

        return files;
    }
}
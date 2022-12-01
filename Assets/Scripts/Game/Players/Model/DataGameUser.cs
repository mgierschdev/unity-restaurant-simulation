using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable] // The same as [System.Serializable]
public class DataGameUser
{
    [SerializeField]
    public string NAME;
    [SerializeField]
    public Double GAME_MONEY;
    [SerializeField]
    public Double GEMS;
    [SerializeField]
    public Double EXPERIENCE;
    [SerializeField]
    public int LEVEL;
    [SerializeField]
    public int GRID_SIZE;
    [SerializeField]
    public string LANGUAGE_CODE;
    [SerializeField]
    public string INTERNAL_ID;
    [SerializeField]
    public string FIREBASE_AUTH_ID;
    [SerializeField]
    public string EMAIL;
    [SerializeField]
    public int AUTH_TYPE;
    [SerializeField]
    public DateTime LAST_LOGIN;
    [SerializeField]
    public DateTime CREATED_AT;
    [SerializeField]
    public List<DataGameObject> OBJECTS;
    public DataStatsGameObject DATA_STATS;

    // Convert to JSON string
    public string ToJSONString(Boolean pretty)
    {
        return JsonUtility.ToJson(this, pretty);
    }

    // Create object from JSON
    public static DataGameUser CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DataGameUser>(jsonString);
    }

    // load from JSON data
    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }

    // Load object from json file inside the resource folder
    public void LoadFromJSONFile(string file)
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>(file);
        Load(jsonTextFile.text);
    }

    // Saves to a json file 
    private async void SaveToJSONFile()
    {
        await System.IO.File.WriteAllTextAsync(GetSaveFileName(), ToJSONString(Settings.devEnv));
    }

    public void SaveToJSONFileAsync()
    {
        SaveToJSONFile();
    }

    public static string GetSaveFileName()
    {
        DateTime date = DateTime.Now;
        string today = date.Day + "-" + date.Month + "-" + date.Year;
        string name = today + Settings.SaveFileSuffix;
        return Settings.devEnv ? Settings.DevSaveDirectory + "/" + name : Application.persistentDataPath + "/" + name;
    }
}
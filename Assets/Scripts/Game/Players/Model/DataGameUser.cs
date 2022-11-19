using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable] // The same as [System.Serializable]
public class DataGameUser : MonoBehaviour
{
    public string NAME;
    public Double GAME_MONEY;
    public Double GEMS;
    public Double EXPERIENCE;
    public int LEVEL;
    public int GRID_SIZE;
    public string LANGUAGE_CODE;
    public string INTERNAL_ID;
    public string FIREBASE_AUTH_ID;
    public string EMAIL;
    public int AUTH_TYPE;
    public DateTime LAST_LOGIN;
    public DateTime CREATED_AT;
    public List<DataGameObject> OBJECTS;

    // Convert to JSON string
    public string ToJSONString()
    {
        return JsonUtility.ToJson(this);
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
        Debug.Log("User " + NAME + " " + GAME_MONEY);

        double date = DateTime.Now.ToOADate();
        string name = date + Settings.SaveFileSuffix;
        string path = Settings.DevEnv ? Settings.DevSaveDirectory + "/" + name : Application.persistentDataPath + "/" + name;
        await System.IO.File.WriteAllTextAsync(path, ToJSONString());
    }

    public void SaveToJSONFileAsync()
    {
        SaveToJSONFile();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = UnityEngine.Coroutine;
// The same as [System.Serializable]
[Serializable]
public class DataGameUser
{
    public string NAME { get; set; }
    public Double GAME_MONEY { get; set; }
    public Double GEMS { get; set; }
    public Double EXPERIENCE { get; set; }
    public int LEVEL { get; set; }
    public int GRID_SIZE { get; set; }
    public string LANGUAGE_CODE { get; set; }
    public string INTERNAL_ID { get; set; }
    public string FIREBASE_AUTH_ID { get; set; }
    public string EMAIL { get; set; }
    public int AUTH_TYPE { get; set; }
    public DateTime LAST_LOGIN { get; set; }
    public DateTime CREATED_AT { get; set; }
    public List<DataGameObject> OBJECTS { get; set; }


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
    // Behaviour intended without await to the Task object
    private async void SaveToJSONFile()
    {
        //Application persistent data Application.persistentDataPath
        //System.IO.File.WriteAllText(Application.persistentDataPath + "/userData.json", ToJSONString());
        //TODO: change user nada for the datta in which the save is taking place 
        DateTime date = DateTime.Now;
        string name = "save" + date + ".json";
        string path = Settings.DevEnv ? "Data/" + name : Application.persistentDataPath + "/" + name;
        System.IO.File.WriteAllTextAsync(path, ToJSONString());
    }

    public void SaveToJSONFileAsync()
    {
        SaveToJSONFile();
    }
}
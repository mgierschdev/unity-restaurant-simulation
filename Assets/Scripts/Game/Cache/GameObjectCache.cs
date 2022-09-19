using System.Collections.Generic;
using UnityEngine;

//TODO: sWill handle all GetComponent and Find Calls and cache the items for future reference
public class GameObjectCache : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectsMap;

    public void Awake()
    {
        gameObjectsMap = new Dictionary<string, GameObject>();
    }

    public GameObject Find(string name)
    {
        if (ContainsKey(name))
        {
            return gameObjectsMap[name];
        }
        else
        {
            return null;
        }
    }

    public bool ContainsKey(string name)
    {
        return gameObjectsMap.ContainsKey(name);
    }

    public void Put(GameObject obj)
    {
        gameObjectsMap.TryAdd(obj.name, obj);
    }
}
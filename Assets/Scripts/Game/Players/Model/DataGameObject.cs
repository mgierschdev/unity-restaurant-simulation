using System;
using UnityEngine;

[Serializable]
public class DataGameObject
{
    [SerializeField]
    public int ID;//StoreItemType
    [SerializeField]
    public int[] POSITION;
    [SerializeField]
    public bool IS_STORED;
    [SerializeField]
    public int ROTATION;

    public StoreItemType GetType()
    {
        return (StoreItemType)ID;
    }

    // We need to update everytime we add a new counter to the model
    public bool IsCounter()
    {
        return GetType() == StoreItemType.COUNTER;
    }
}
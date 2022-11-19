using System;
using UnityEngine;

[Serializable]
public class DataGameObject : MonoBehaviour
{
    public int ID;//StoreItemType
    public int[] POSITION;
    public bool IS_STORED;
    public int ROTATION;

    public override string ToString()
    {
        return ID + "-" + POSITION + "-" + IS_STORED + "-" + ROTATION;
    }
}
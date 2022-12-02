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
}
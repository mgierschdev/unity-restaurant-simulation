using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class DataGameObject
{
    public int ID { get; set; } //StoreItemType
    public int[] POSITION { get; set; }
    public bool IS_STORED { get; set; }
    public int ROTATION { get; set; }

    public override string ToString()
    {
        return ID + "-" + POSITION + "-" + IS_STORED + "-" + ROTATION;
    }
}
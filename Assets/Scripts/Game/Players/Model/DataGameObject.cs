using System;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game.Players.Model
{
    [Serializable]
    public class DataGameObject
    {
        public int id;
        
        public int[] position;
        
        public bool isStored;
        
        public int rotation;

        public StoreItemType GetStoreItemType()
        {
            return (StoreItemType)id;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game.Players.Model
{
    [Serializable]
    /**
     * Problem: Persist grid object data for save/load.
     * Goal: Store position, rotation, and storage state.
     * Approach: Serialize fields for JSON persistence.
     * Time: O(1) per access.
     * Space: O(1).
     */
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

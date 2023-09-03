using System;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game.Players.Model
{
    [Serializable]
    public class DataGameObject
    {
        [FormerlySerializedAs("ID")] [SerializeField]
        public int id; //StoreItemType

        [FormerlySerializedAs("POSITION")] [SerializeField]
        public int[] position;

        [FormerlySerializedAs("IS_STORED")] [SerializeField]
        public bool isStored;

        [FormerlySerializedAs("ROTATION")] [SerializeField]
        public int rotation;

        public StoreItemType GetStoreItemType()
        {
            return (StoreItemType)id;
        }
    }
}
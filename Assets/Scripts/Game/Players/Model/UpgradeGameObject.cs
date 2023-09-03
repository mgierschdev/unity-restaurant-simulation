using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Players.Model
{
    [Serializable]
    public class UpgradeGameObject
    {
        [FormerlySerializedAs("ID")] [SerializeField]
        public int id; // UPGRADE ID

        [FormerlySerializedAs("UPGRADE_NUMBER")] [SerializeField]
        public int upgradeNumber;
    }
}
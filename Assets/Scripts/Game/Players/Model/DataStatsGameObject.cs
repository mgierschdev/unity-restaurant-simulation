using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Players.Model
{
    [Serializable]
    public class DataStatsGameObject
    {
        [FormerlySerializedAs("MONEY_EARNED")] [SerializeField]
        public Double moneyEarned;

        [FormerlySerializedAs("MONEY_SPENT")] [SerializeField]
        public Double moneySpent;

        [FormerlySerializedAs("CLIENTS_ATTENDED")] [SerializeField]
        public int clientsAttended;

        [FormerlySerializedAs("ITEMS_BOUGHT")] [SerializeField]
        public int itemsBought;
    }
}
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Players.Model
{
    [Serializable]
    public class DataStatsGameObject
    {
        public Double moneyEarned;
        
        public Double moneySpent;
        
        public int clientsAttended;
        
        public int itemsBought;
    }
}
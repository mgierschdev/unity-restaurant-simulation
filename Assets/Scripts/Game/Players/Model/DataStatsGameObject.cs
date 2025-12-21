using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Players.Model
{
    [Serializable]
    /**
     * Problem: Persist player statistics for saves.
     * Goal: Store aggregated stats like money and counts.
     * Approach: Serialize simple numeric fields.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class DataStatsGameObject
    {
        public Double moneyEarned;
        
        public Double moneySpent;
        
        public int clientsAttended;
        
        public int itemsBought;
    }
}

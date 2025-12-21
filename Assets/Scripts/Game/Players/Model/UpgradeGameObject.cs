using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Players.Model
{
    [Serializable]
    /**
     * Problem: Persist upgrade state data for saving.
     * Goal: Store upgrade identifier and current level.
     * Approach: Serialize simple fields for JSON persistence.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class UpgradeGameObject
    {
        public int id;
        
        public int upgradeNumber;
    }
}

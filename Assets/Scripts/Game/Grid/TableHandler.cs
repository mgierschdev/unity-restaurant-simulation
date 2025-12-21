using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Grid
{
    /**
     * Problem: Track business grid objects available for NPC usage.
     * Goal: Maintain a concurrent map of grid objects and occupancy state.
     * Approach: Use a ConcurrentDictionary as a shared registry.
     * Time: O(1) for map operations on average.
     * Space: O(n) for tracked objects.
     */
    public static class TableHandler
    {
        private static ConcurrentDictionary<GameGridObject, byte> _bussQueueMap;

        public static void Init()
        {
            _bussQueueMap = new ConcurrentDictionary<GameGridObject, byte>();
        }

        public static KeyValuePair<GameGridObject, byte>[] GetFreeBusinessSpots()
        {
            return _bussQueueMap.ToArray();
        }

        public static ConcurrentDictionary<GameGridObject, byte> GetBussQueueMap()
        {
            return _bussQueueMap;
        }
    }
}

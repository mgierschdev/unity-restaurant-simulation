using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Grid
{
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
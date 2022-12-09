using System.Collections.Concurrent;
using System.Collections.Generic;

public static class TableHandler
{
    private static ConcurrentDictionary<GameGridObject, byte> BussQueueMap;

    public static void Init()
    {
        BussQueueMap = new ConcurrentDictionary<GameGridObject, byte>();
    }

    public static KeyValuePair<GameGridObject, byte>[] GetFreeBusinessSpots()
    {
        return BussQueueMap.ToArray();
    }

    public static ConcurrentDictionary<GameGridObject, byte> GetBussQueueMap()
    {
        return BussQueueMap;
    }
}

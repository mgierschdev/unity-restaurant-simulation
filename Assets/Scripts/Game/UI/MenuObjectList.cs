using System.Collections.Generic;
using UnityEngine;

public class MenuObjectList
{

    public List<GameGridObject> Tables { get; set; }
    public List<GameGridObject> Waiters { get; set; }

    public MenuObjectList()
    {
        Tables = new List<GameGridObject>(){
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SingleWoodenTable),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SingleWoodenTable),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SingleWoodenTable),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SingleWoodenTable),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SingleWoodenTable),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SingleWoodenTable),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SingleWoodenTable),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SingleWoodenTable),
        new GameGridObject("Iron table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 100, Settings.SingleWoodenTable)
        };
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MenuObjectList : MonoBehaviour
{

    public List<GameGridObject> Tables { get; set; }
    public List<GameGridObject> Waiters { get; set; }

    public MenuObjectList()
    {
        Tables = new List<GameGridObject>(){
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40, Settings.SINGLE_WOODEN_TABLE),
        new GameGridObject("Iron table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 100, Settings.SINGLE_WOODEN_TABLE)
        };
    }
}

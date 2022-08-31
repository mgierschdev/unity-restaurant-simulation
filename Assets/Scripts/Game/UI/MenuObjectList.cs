using System.Collections.Generic;
using UnityEngine;

public class MenuObjectList
{

    List<GameGridObject> Tables { get; set; }

    List<GameGridObject> Waiters { get; set; }

    public MenuObjectList()
    {
        Tables = new List<GameGridObject>(){
        new GameGridObject("Wooden table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 20),
        new GameGridObject("Dark wood table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 40),
        new GameGridObject("Iron table", Vector3.zero, Vector3Int.zero, Vector3Int.zero, ObjectType.NPC_TABLE, TileType.UNDEFINED, 100) 
        };
    }
}

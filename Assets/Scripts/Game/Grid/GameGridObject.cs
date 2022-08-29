using Codice.Client.BaseCommands.Merge.MergeTo;
using UnityEngine;
public class GameGridObject : GameObjectBase
{
    public bool Busy { get; set; } //Being used by an NPC
    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType)
    {
        this.TileType = tileType;
        this.Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;

        if (type == ObjectType.NPC_COUNTER)
        {
            ActionGridPosition = gridPosition + new Vector3Int(0, 1, 0);
        }

        if (type == ObjectType.NPC_TABLE)
        {
            ActionGridPosition = gridPosition + new Vector3Int(0, 1, 0);
        }
    }
}
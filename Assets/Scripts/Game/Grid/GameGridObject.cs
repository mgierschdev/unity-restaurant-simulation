using UnityEngine;
public class GameGridObject : GameObjectBase
{
    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType)
    {
        this.TileType = tileType;
        this.Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
    }
}
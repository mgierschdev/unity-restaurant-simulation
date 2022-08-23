using UnityEngine;
public class GameGridObject : GameObjectBase
{
    public GameGridObject(Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type)
    {
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
    }
}
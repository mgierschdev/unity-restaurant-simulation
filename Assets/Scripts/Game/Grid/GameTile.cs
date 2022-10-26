using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : GameObjectBase
{
    public GameTile(Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, TileType name, ObjectType type, TileBase unityTileBase)
    {
        // Grid position, first position = 0, 0
        GridPosition = gridPosition; 
        // World position on Unity coords
        WorldPosition = worldPosition;
        LocalGridPosition = localGridPosition;
        TileType = name;
        Type = type;
        UnityTileBase = unityTileBase;
    }
}
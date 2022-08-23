using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : GameObjectBase
{
    public TileBase UnityTileBase { get; set; }
    public TileType Name { get; set; }

    public GameTile(Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, TileType name, ObjectType type, TileBase unityTileBase)
    {
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Name = name;
        Type = type;
        UnityTileBase = unityTileBase;
    }
}
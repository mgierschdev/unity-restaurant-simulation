using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : GameObjectBase
{
    public TileType Name { get; set; }
    public ObjectType Type { get; set; }
    public TileBase UnityTileBase { get; set; }
    public Vector3Int LocalGridPosition { get; set; } // Local grid position, can be negatice -20,20

    public GameTile(Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, TileType name, ObjectType type, TileBase unityTileBase)
    {
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Name = name;
        Type = type;
        UnityTileBase = unityTileBase;
        UpdateSortingLayer();
    }
}
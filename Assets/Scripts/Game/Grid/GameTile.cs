using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTile : GameObjectBase
{
    public TileType Name { get; set; }
    public ObjectType Type { get; set; }
    public TileBase UnityTileBase { get; set; }

    public GameTile(Vector3 worldPosition, TileType name, ObjectType type, TileBase unityTileBase)
    {
        WorldPosition = worldPosition;
        Name = name;
        Type = type;
        UnityTileBase = unityTileBase;
        UpdatePositionInGrid(); // Sets X and Y
    }
}
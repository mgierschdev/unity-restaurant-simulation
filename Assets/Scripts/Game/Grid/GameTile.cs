using UnityEngine;

public class GameTile : GameObjectBase
{
    public TileType Type {get; set;}

    public GameTile(Vector3 worldPosition, TileType type){
        WorldPosition = worldPosition;
        Type = type;
        UpdatePositionInGrid(); // Sets X and Y
    }
}
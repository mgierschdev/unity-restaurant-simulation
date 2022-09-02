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

        SetActionPoints();
    }

    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType, int cost, string menuItemFoto)
    {
        MenuItemSprite = menuItemFoto;
        this.TileType = tileType;
        this.Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
        Cost = cost;

        SetActionPoints();
    }

    private void SetActionPoints()
    {
        if (Type == ObjectType.NPC_COUNTER)
        {
            ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
        }

        if (Type == ObjectType.NPC_TABLE)
        {
            ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
        }
    }
    
    public bool IsLastPositionEqual(Vector3Int ActionGridPosition)
    {
        return this.ActionGridPosition == ActionGridPosition;
    }

    public void UpdateCoords(Vector3Int GridPosition, Vector3Int LocalGridPosition, Vector3 WorldPosition)
    {
        this.GridPosition = GridPosition;
        this.LocalGridPosition = LocalGridPosition;
        this.WorldPosition = WorldPosition;

        if (Type == ObjectType.NPC_TABLE)
        {
            ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
        }
    }

}
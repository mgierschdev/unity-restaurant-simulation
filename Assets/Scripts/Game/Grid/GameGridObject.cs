using System;
using UnityEngine;

public class GameGridObject : GameObjectBase
{
    public bool Busy { get; set; } //Being used by an NPC

    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType)
    {
        TileType = tileType;
        Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;

        SetActionPoints();
    }

    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType, int cost, string menuItemSprite)
    {
        MenuItemSprite = menuItemSprite;
        TileType = tileType;
        Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
        Cost = cost;

        SetActionPoints();
    }

    private void SetActionPoints()
    {
        switch (Type)
        {
            case ObjectType.OBSTACLE:
                break;
            case ObjectType.NPC:
                break;
            case ObjectType.PLAYER:
                break;
            case ObjectType.EMPLOYEE:
                break;
            case ObjectType.NPC_TABLE:
                ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
                break;
            case ObjectType.NPC_COUNTER:
                ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
                break;
            case ObjectType.FLOOR:
                break;
            case ObjectType.UNDEFINED:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public bool IsLastPositionEqual(Vector3Int actionGridPosition)
    {
        return this.ActionGridPosition == actionGridPosition;
    }

    public void UpdateCoords(Vector3Int gridPosition, Vector3Int localGridPosition, Vector3 worldPosition)
    {
        GridPosition = gridPosition;
        LocalGridPosition = localGridPosition;
        WorldPosition = worldPosition;

        if (Type == ObjectType.NPC_TABLE)
        {
            ActionGridPosition = GridPosition + new Vector3Int(0, 1, 0);
        }
    }

}
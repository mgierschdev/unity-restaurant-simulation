using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGridObject : GameObjectBase
{
    public SpriteRenderer GameGridObjectSpriteRenderer { get; set; }
    public bool Busy { get; set; } //Being used by an NPC
    public NPCController UsedBy { get; set; }
    //public Vector3Int ActionGridPosition { get; set; } // Cell position that NPC has to move tos
    public List<GameObject> ActionTiles { get; set; }


    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType)
    {
        TileType = tileType;
        Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
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
    }

    public bool IsLastPositionEqual(Vector3 actionGridPosition)
    {
        return Util.IsAtDistanceWithObject(GetFirstActionTile(), actionGridPosition);
    }

    public void UpdateCoords(Vector3Int gridPosition, Vector3Int localGridPosition, Vector3 worldPosition)
    {
        GridPosition = gridPosition;
        LocalGridPosition = localGridPosition;
        WorldPosition = worldPosition;
    }

    public void Hide()
    {
        GameGridObjectSpriteRenderer.color = Util.Free;
    }

    public void Show()
    {
        GameGridObjectSpriteRenderer.color = Util.Available;
    }

    public void SetUsed(NPCController npc)
    {
        UsedBy = npc;
        Busy = true;
    }

    public void FreeObject()
    {
        UsedBy = null;
        Busy = false;
    }

    public Vector3 GetFirstActionTile()
    {
        return ActionTiles[0].transform.position;
    }

    public Vector3 GetSecondActionTile()
    {
        return ActionTiles[1].transform.position;
    }
}
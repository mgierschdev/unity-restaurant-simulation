using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectBase
{
    public string Name { get; set; }
    public SortingGroup SortingLayer { get; set; }
    public Vector3Int GridPosition { get; set; } //In Pathfinding grid
    public Vector3Int LocalGridPosition { get; set; } // Local grid position, can be negatice -20,20
    public Vector3 WorldPosition { get; set; }
    public ObjectType Type { get; set; }
    public TileType TileType { get; set; }
    private Vector3 tileOffset = new Vector3(0, 0.25f, 0);
    
    public void UpdateSortingLayer()
    {
        // Some objects may not have sorting layer defined
        if (SortingLayer != null)
        {
            SortingLayer.sortingOrder = 1;
        }
    }

    public Vector3 GetWorldPositionWithOffset()
    {
        return WorldPosition + tileOffset;
    }
}
using UnityEngine;
using UnityEngine.Rendering;
 
public abstract class GameObjectBase
{
    public Vector3Int GridPosition { get; set; }
    public Vector3 WorldPosition { get; set; }

    // Sprite level ordering
    protected SortingGroup sortingLayer;

    protected void UpdateSortingLayer()
    {
        // Some objects may not have sorting layer defined
        if (sortingLayer != null)
        {
            sortingLayer.sortingOrder = GridPosition.y * -1;
        }
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { GridPosition.x, GridPosition.y };
    }
}
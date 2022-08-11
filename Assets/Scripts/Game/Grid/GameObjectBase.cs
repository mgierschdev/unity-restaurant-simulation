using UnityEngine;
using UnityEngine.Rendering;
 
public abstract class GameObjectBase
{
    public Vector3Int GridPosition { get; set; }
    public Vector3 WorldPosition { get; set; }

    // Sprite level ordering
    protected SortingGroup sortingLayer;

    protected void UpdatePositionInGrid()
    {
        Vector2Int pos = Util.GetIsometricXYInGameMap(WorldPosition);
        // Some objects may not have sorting layer defined
        if (sortingLayer != null)
        {
            sortingLayer.sortingOrder = pos.y * -1;
        }
        GridPosition = new Vector3Int(pos.x, pos.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { GridPosition.x, GridPosition.y };
    }
}
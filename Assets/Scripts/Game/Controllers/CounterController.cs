using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class CounterController : BaseObjectController
{
    GameGridObject counter;
    
    private void Start()
    {
        Transform transformObject = gameObject.transform;
        Vector3 transformPosition = transformObject.position;
        counter = new GameGridObject(transformObject.name, transformPosition, grid.GetPathFindingGridFromWorldPosition(transformPosition), grid.GetLocalGridFromWorldPosition(transformPosition), ObjectType.NPC_COUNTER, TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT);
        counter.SortingLayer = GetComponent<SortingGroup>();

        if (!Util.IsNull(grid, "CounterController/IsometricGridController null"))
        {
            grid.SetGridObject(counter);
        }
    }
}
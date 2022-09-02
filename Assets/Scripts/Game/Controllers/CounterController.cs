using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class CounterController : BaseObjectController
{
    GameGridObject counter;


    void Start()
    {
        counter = new GameGridObject(transform.name, transform.position, grid.GetPathFindingGridFromWorldPosition(transform.position), grid.GetLocalGridFromWorldPosition(transform.position), ObjectType.NPC_COUNTER, TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT);
        counter.SortingLayer = GetComponent<SortingGroup>();


        if (!Util.IsNull(grid, "CounterController/IsometricGridController null"))
        {
            grid.SetGridObject(counter);
        }
    }
}
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
        counter = new GameGridObject(transformObject.name, transformPosition, Grid.GetPathFindingGridFromWorldPosition(transformPosition), Grid.GetLocalGridFromWorldPosition(transformPosition), ObjectType.NPC_COUNTER, TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT);
        counter.SortingLayer = GetComponent<SortingGroup>();

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(counter);
        }
    }
}
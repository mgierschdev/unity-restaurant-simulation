using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class CounterController : MonoBehaviour
{
    GameGridObject counter;
    GridController grid;

    void Start()
    {
        GameObject gridGameObject = GameObject.Find(Settings.GAME_GRID).gameObject;
        grid = gridGameObject.GetComponent<GridController>();
        counter = new GameGridObject(transform.name, transform.position, grid.GetPathFindingGridFromWorldPosition(transform.position), grid.GetLocalGridFromWorldPosition(transform.position), ObjectType.NPC_COUNTER, TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT);
        counter.SortingLayer = GetComponent<SortingGroup>();
        //counter.UpdateSortingLayer();

        if (!Util.IsNull(grid, "CounterController/IsometricGridController null"))
        {
            grid.SetGridObject(counter);
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class TableController : MonoBehaviour
{
    GameGridObject table;
    GridController grid;
    // Table state
    bool free = true;

    void Start()
    {
        GameObject gridGameObject = GameObject.Find(Settings.GAME_GRID).gameObject;
        grid = gridGameObject.GetComponent<GridController>();
        table = new GameGridObject(transform.name, transform.position, grid.GetPathFindingGridFromWorldPosition(transform.position), grid.GetLocalGridFromWorldPosition(transform.position), ObjectType.NPC_TABLE);
        table.SortingLayer = GetComponent<SortingGroup>();
        table.UpdateSortingLayer();

        if (!Util.IsNull(grid, "TableController/IsometricGridController null"))
        {
            grid.SetGridObject(table);
        }
    }
}
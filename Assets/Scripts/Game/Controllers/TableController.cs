using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class TableController : BaseObjectController
{
    GameGridObject table;
    void Start()
    {
        table = new GameGridObject(transform.name, transform.position, grid.GetPathFindingGridFromWorldPosition(transform.position), grid.GetLocalGridFromWorldPosition(transform.position), ObjectType.NPC_TABLE, TileType.ISOMETRIC_FOUR_SQUARE_OBJECT);
        table.SortingLayer = GetComponent<SortingGroup>();
        Type = table.Type;
        gameGridObject = table;

        if (!Util.IsNull(grid, "TableController/IsometricGridController null"))
        {
            grid.SetGridObject(table);
        }
    }
}
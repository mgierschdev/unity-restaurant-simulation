using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class TableController : BaseObjectController
{
    private GameGridObject table;
    private void Start()
    {
        Vector3 transformPosition = transform.position;
        table = new GameGridObject(name, transformPosition, Grid.GetPathFindingGridFromWorldPosition(transformPosition), Grid.GetLocalGridFromWorldPosition(transformPosition), ObjectType.NPC_TABLE, TileType.ISOMETRIC_FOUR_SQUARE_OBJECT)
            {
                SortingLayer = GetComponent<SortingGroup>()
            };
        Type = table.Type;
        GameGridObject = table;

        if (!Util.IsNull(Grid, "TableController/IsometricGridController null"))
        {
            Grid.SetGridObject(table);
        }
    }
}
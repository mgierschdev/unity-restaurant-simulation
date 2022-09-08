using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class TableController : BaseObjectController
{
    private GameGridObject table;
    private void Start()
    {
        Type = ObjectType.NPC_SINGLE_TABLE;
        gameGridObject.Type = ObjectType.NPC_SINGLE_TABLE;
        
        // we set the object in the grid
        if (!Util.IsNull(Grid, "TableController/IsometricGridController null"))
        {
            Grid.SetGridObject(table);
        }
    }
}
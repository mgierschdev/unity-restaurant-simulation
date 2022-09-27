using UnityEngine;

// Name sof Objects should be unique
public class TableController : BaseObjectController
{
    private void Start()
    {
        gameGridObject = new GameGridObject(transform, Grid, InitialObjectRotation, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.WOODEN_TABLE_SINGLE));

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            gameGridObject.Init();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
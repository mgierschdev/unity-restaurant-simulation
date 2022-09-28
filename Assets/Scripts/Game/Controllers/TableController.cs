public class TableController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, Grid, InitialObjectRotation, Grid.GetObjectListConfiguration().GetStoreObject(StoreItemType.WOODEN_TABLE_SINGLE));

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            gameGridObject.Init();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
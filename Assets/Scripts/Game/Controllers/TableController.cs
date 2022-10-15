public class TableController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, BussGrid.GetObjectListConfiguration().GetStoreObject(StoreItemType.WOODEN_TABLE_SINGLE));

        // if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        // {
            gameGridObject.Init();
            BussGrid.SetGridObject(gameGridObject);
       //}
    }
}
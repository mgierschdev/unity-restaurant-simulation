public class BaseContainerController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, BussGrid.GetObjectListConfiguration().GetStoreObject(StoreItemType.WOODEN_BASE_CONTAINER));

        // if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        // {
            gameGridObject.Init();
            BussGrid.SetGridObject(gameGridObject);
        // }
    }
}
public class BaseContainerController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, Grid, InitialObjectRotation, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.WOODEN_BASE_CONTAINER));

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            gameGridObject.Init();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
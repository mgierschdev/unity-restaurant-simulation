public class CounterController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, Grid, InitialObjectRotation, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.COUNTER));
        
        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            gameGridObject.Init();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
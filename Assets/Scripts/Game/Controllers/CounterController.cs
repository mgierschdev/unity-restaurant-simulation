public class CounterController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, BussGrid.GetObjectListConfiguration().GetStoreObject(StoreItemType.COUNTER));
        
        // if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        // {
            gameGridObject.Init();
            BussGrid.SetGridObject(gameGridObject);
        //}
    }
}
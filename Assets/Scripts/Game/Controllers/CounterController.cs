public class CounterController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, BussGrid.GetObjectListConfiguration().GetStoreObject(StoreItemType.COUNTER));
        gameGridObject.Init();
        BussGrid.SetGridObject(gameGridObject);
    }
}
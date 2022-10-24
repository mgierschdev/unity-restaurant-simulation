public class CounterController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, MenuObjectList.GetStoreObject(StoreItemType.COUNTER));
        BussGrid.SetGridObject(gameGridObject);
    }
}
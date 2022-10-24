public class CounterController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, MenuObjectList.GetStoreObject(StoreItemType.COUNTER));
        BussGrid.SetGridObject(gameGridObject);
    }
}
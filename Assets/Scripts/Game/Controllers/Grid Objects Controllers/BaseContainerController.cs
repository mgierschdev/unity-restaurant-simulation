public class BaseContainerController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, MenuObjectList.GetStoreObject(StoreItemType.WOODEN_BASE_CONTAINER));
        BussGrid.SetGridObject(gameGridObject);
    }
}
public class TableController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, MenuObjectList.GetStoreObject(StoreItemType.WOODEN_TABLE_SINGLE));
        BussGrid.SetGridObject(gameGridObject);
    }
}
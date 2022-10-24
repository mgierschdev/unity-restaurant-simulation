public class TableController : BaseObjectController
{
    private void Start()
    {
        Init();
        gameGridObject = new GameGridObject(transform, InitialObjectRotation, MenuObjectList.GetStoreObject(StoreItemType.WOODEN_TABLE_SINGLE));
        BussGrid.SetGridObject(gameGridObject);
    }
}
using UnityEngine;
public class CounterController : BaseObjectController
{
    private const int COST = 999;

    private void Start()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();

        gameGridObject = new GameGridObject(transform, Grid, ObjectRotation.FRONT, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.COUNTER));
        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(gameGridObject);
        }
    }
}
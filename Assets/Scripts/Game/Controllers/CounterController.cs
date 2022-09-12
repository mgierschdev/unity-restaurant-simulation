using UnityEngine;
public class CounterController : BaseObjectController
{
    private const int COST = 999;

    private void Start()
    {
        //Edit Panel Disable
        GameObject EditPanel = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        EditPanel.SetActive(false);

        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();

        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();
        gameGridObject = new GameGridObject(this.transform, Grid.GetPathFindingGridFromWorldPosition(transform.position), Grid.GetLocalGridFromWorldPosition(transform.position), COST, ObjectRotation.FRONT, ObjectType.NPC_COUNTER, EditPanel);

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(gameGridObject);
        }
    }
}
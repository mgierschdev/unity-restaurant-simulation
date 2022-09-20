using UnityEngine;

// Name sof Objects should be unique
public class TableController : BaseObjectController
{
    private const int COST = 20;
    private void Start()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();

        //Edit Panel Disable
        GameObject EditPanel = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        gameGridObject = new GameGridObject(transform, Grid.GetPathFindingGridFromWorldPosition(transform.position), Grid.GetLocalGridFromWorldPosition(transform.position), COST, ObjectRotation.FRONT, ObjectType.NPC_SINGLE_TABLE, EditPanel, Grid);

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            string id = Grid.GetObjectCount() + 1 + "-" + Time.frameCount;
            name = "SingleTable." + id;
            gameGridObject.Name = name;
            gameGridObject.Hide();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
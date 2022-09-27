using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseContainerController : BaseObjectController
{
    private const int COST = 20;
    private void Start()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();
        ObjectRotation rotation = ObjectRotation.FRONT;

        //Edit Panel Disable
        if (transform.name.Contains(Settings.SingleTableRotationFrontInverted))
        {
            rotation = ObjectRotation.FRONT_INVERTED;
        }

        gameGridObject = new GameGridObject(transform, Grid, rotation, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.WOODEN_BASE_CONTAINER));

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            string id = Grid.GetObjectCount() + 1 + "-" + Time.frameCount;
            name = "BaseContainer." + id;
            gameGridObject.Name = name;
            gameGridObject.Hide();
            Grid.SetGridObject(gameGridObject);
        }
    }
}

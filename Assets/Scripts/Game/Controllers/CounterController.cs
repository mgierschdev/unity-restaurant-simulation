using UnityEngine;
public class CounterController : BaseObjectController
{
    private const int COST = 999;

    private void Start()
    {
        gameGridObject = new GameGridObject(transform, Grid, InitialObjectRotation, Grid.ObjectListConfiguration.GetStoreObject(StoreItemType.COUNTER));
        
        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            gameGridObject.Init();
            Grid.SetGridObject(gameGridObject);
        }
    }
}
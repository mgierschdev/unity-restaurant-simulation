using UnityEngine;
public class CounterController : BaseObjectController
{
    private const int COST = 999;

    private void Start()
    {
        //Edit Panel Disable
        GameObject EditPanel = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        EditPanel.SetActive(false);

        gameGridObject = new GameGridObject(gameObject.transform, Grid.GetPathFindingGridFromWorldPosition(transform.position), Grid.GetLocalGridFromWorldPosition(transform.position), COST, ObjectRotation.FRONT, ObjectType.NPC_COUNTER, EditPanel, Grid);
        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(gameGridObject);
        }
    }
}
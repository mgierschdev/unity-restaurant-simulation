using UnityEngine;
public class CounterController : BaseObjectController
{
    private const int COST = 999;

    private void Start()
    {
        //Edit Panel Disable
        GameObject EditPanel = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        EditPanel.SetActive(false);

        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();
        //    public GameGridObject(Transform transform, Vector3Int gridPosition, Vector3Int localGridPosition, int cost, ObjectRotation position, ObjectType type, GameObject editMenu, GridController gridController)
    
        gameGridObject = new GameGridObject(gameObject.transform, Grid.GetPathFindingGridFromWorldPosition(transform.position), Grid.GetLocalGridFromWorldPosition(transform.position), COST, ObjectRotation.FRONT, ObjectType.NPC_COUNTER, EditPanel, Grid);

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(gameGridObject);
        }
    }
}
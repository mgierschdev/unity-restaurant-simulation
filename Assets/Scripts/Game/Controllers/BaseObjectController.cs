using UnityEngine;

public class BaseObjectController : MonoBehaviour
{
    protected UIHandler Menu { get; set; }
    private Vector3 initialPosition;
    private Vector3 mousePosition;
    //Initial object position
    private Vector3Int initialActionTileOne;
    private const int COST = 20; // temporal
    protected GameGridObject gameGridObject;
    protected GridController Grid { get; set; }

    private void Awake()
    {
        GameObject gameObject = GameObject.Find("UI(New)");
        Menu = gameObject.GetComponent<UIHandler>();
        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();
    }

    private void Update()
    {
        if (!Menu || !Grid || gameGridObject == null || Grid.DraggingObject)
        {
            return;
        }

        if (Menu.IsEditPanelOpen())
        {
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            gameGridObject.HideUnderTiles();
        }
    }

    private void OnMouseDown()
    {
        if (!Menu || !Grid || !Menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        Grid.SetActiveGameGridObject(gameGridObject);
        mousePosition = gameObject.transform.position - Util.GetMouseInWorldPosition();
        initialActionTileOne = Grid.GetPathFindingGridFromWorldPosition(gameGridObject.GetActionTile());
        initialPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        if (!Menu || !Menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }
        //If dragging clean previous position on the grid
        Grid.FreeCoord(Grid.GetPathFindingGridFromWorldPosition(initialPosition));
        Grid.FreeCoord(initialActionTileOne);

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        Vector3 currentPos = Grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        //So it will overlay over the rest of the items while dragging
        gameGridObject.SortingLayer.sortingOrder = 2;


        if (Grid.IsValidBussPosition(gameGridObject, currentPos))
        {
            gameGridObject.SpriteRenderer.color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            gameGridObject.LightOccupiedUnderTiles();
            gameGridObject.SpriteRenderer.color = Util.Occupied;
        }
        Grid.DraggingObject = true;
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!Menu || !Menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        Vector3 finalPos = Grid.GetNearestGridPositionFromWorldMap(transform.position);
        Grid.DraggingObject = false;

        if (!Grid.IsValidBussPosition(gameGridObject, finalPos))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
            gameGridObject.SpriteRenderer.color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            gameGridObject.UpdateCoords();
            initialPosition = new Vector3(finalPos.x, finalPos.y, 1);
        }

        //So it will overlay over the rest of the items while dragging
        gameGridObject.SortingLayer.sortingOrder = 0;
        Grid.UpdateObjectPosition(gameGridObject);
    }

    private bool IsDraggable()
    {
        if (!Menu || !Menu.IsEditPanelOpen() || gameGridObject == null)
        {
            return false;
        }

        if (Grid.IsTableBusy(gameGridObject))
        {
            // GameLog.Log("Moving Busy object" + gameGridObject.Name); // To Show in the UI
            // GameLog.Log("Used by " + gameGridObject.UsedBy.name);
            gameGridObject.UsedBy.GoToFinalState();
            gameGridObject.FreeObject();
            Grid.AddFreeBusinessSpots(gameGridObject);
        }

        return gameGridObject.Type != ObjectType.UNDEFINED &&
        gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE &&
        Menu.IsEditPanelOpen() &&
        !Grid.IsTableBusy(gameGridObject);
    }
}
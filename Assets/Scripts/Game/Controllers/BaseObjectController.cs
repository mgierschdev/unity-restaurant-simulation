using UnityEngine;
using UnityEngine.Rendering;

public class BaseObjectController : MonoBehaviour
{
    private MenuHandlerController menu;
    private SpriteRenderer spriteRenderer;
    private Vector3 initialPosition;
    private Vector3 mousePosition;
    private SortingGroup sortLayer;
    //Initial object position
    private Vector3Int initialActionTileOne;
    private const int COST = 20; // temporal
    protected GameGridObject gameGridObject;
    protected GridController Grid;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        menu = menuHandler.GetComponent<MenuHandlerController>();
    }

    private void Update()
    {
        Debug.Log("Grid "+Grid);

        if (!Grid || Grid.DraggingObject)
        {
            return;
        }

        if (menu.IsEditPanelOpen())
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
        if (!menu.IsEditPanelOpen() || !IsDraggable())
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
        if (!menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        Vector3 currentPos = Grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        sortLayer.sortingOrder = 1;

        if (Grid.IsValidBussPosition(currentPos, gameGridObject, initialActionTileOne))
        {
            spriteRenderer.color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            gameGridObject.LightOccupiedUnderTiles();
            spriteRenderer.color = Util.Occupied;
        }
        Grid.DraggingObject = true;
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        Vector3 finalPos = Grid.GetNearestGridPositionFromWorldMap(transform.position);
        Grid.DraggingObject = false;
        sortLayer.sortingOrder = 0;

        if (!Grid.IsValidBussPosition(finalPos, gameGridObject, initialActionTileOne))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
            spriteRenderer.color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            Vector3Int init = gameGridObject.GridPosition;
            initialPosition = new Vector3(finalPos.x, finalPos.y, 1);
            gameGridObject.UpdateCoords(Grid.GetPathFindingGridFromWorldPosition(finalPos), Grid.GetLocalGridFromWorldPosition(finalPos), finalPos);
            Grid.UpdateGridPosition(init, gameGridObject);
        }
    }

    private bool IsDraggable()
    {
        if (!menu.IsEditPanelOpen())
        {
            return false;
        }

        if (gameGridObject != null && Grid.IsTableBusy(gameGridObject))
        {
            // GameLog.Log("Moving Busy object" + gameGridObject.Name); // To Show in the UI
            // GameLog.Log("Used by " + gameGridObject.UsedBy.name);
            gameGridObject.UsedBy.GoToFinalState();
            gameGridObject.FreeObject();
            Grid.AddFreeBusinessSpots(gameGridObject);
        }
        return gameGridObject.Type != ObjectType.UNDEFINED && gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE && menu.IsEditPanelOpen() && !Grid.IsTableBusy(gameGridObject);
    }
}
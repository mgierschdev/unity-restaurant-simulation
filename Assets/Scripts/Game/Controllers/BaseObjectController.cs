using UnityEngine;

public class BaseObjectController : MonoBehaviour
{
    protected MenuHandlerController Menu { get; set; }
    private Vector3 initialPosition;
    private Vector3 mousePosition;
    //Initial object position
    private Vector3Int initialActionTileOne;
    private const int COST = 20; // temporal
    protected GameGridObject gameGridObject;
    protected GridController Grid { get; set; }

    private void Update()
    {
        if (!Menu || !Grid || Grid.DraggingObject)
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
        if (!Menu || !Menu.IsEditPanelOpen() || !IsDraggable())
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

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        Vector3 currentPos = Grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);

        if (Grid.IsValidBussPosition(gameGridObject, currentPos, initialActionTileOne))
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

        if (!Grid.IsValidBussPosition(gameGridObject, finalPos, initialActionTileOne))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
            gameGridObject.SpriteRenderer.color = Util.Available;
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
        if (!Menu || !Menu.IsEditPanelOpen())
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
        return gameGridObject.Type != ObjectType.UNDEFINED && gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE && Menu.IsEditPanelOpen() && !Grid.IsTableBusy(gameGridObject);
    }
}
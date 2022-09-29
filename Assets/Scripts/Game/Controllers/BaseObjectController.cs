using UnityEngine;

public class BaseObjectController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 currentPos; //Current position of the object including while dragging
    private bool currentValidPos; //Current valid position for the object including while dragging
    //Initial object position
    private Vector3Int initialActionTileOne;
    protected GameGridObject gameGridObject;
    protected GridController Grid { get; set; }
    protected ObjectRotation InitialObjectRotation;
    protected MenuHandlerController Menu { get; set; }

    private void Update()
    {
        if (!Menu || !Grid || gameGridObject == null || Grid.GetDragginObject())
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

    protected void Init()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        GameObject gameGrid = GameObject.Find(Settings.GameGrid).gameObject;
        Grid = gameGrid.GetComponent<GridController>();
        InitialObjectRotation = ObjectRotation.FRONT;
        //Edit Panel Disable
        if (transform.name.Contains(Settings.ObjectRotationFrontInverted))
        {
            InitialObjectRotation = ObjectRotation.FRONT_INVERTED;
        }
    }

    private void OnMouseDown()
    {
        if (!Menu || !Grid || !Menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        Grid.SetActiveGameGridObject(gameGridObject);
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
        currentPos = Grid.GetGridWorldPositionMapMouseDrag();
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        //So it will overlay over the rest of the items while dragging
        Vector3Int currentGridPosition = Grid.GetPathFindingGridFromWorldPosition(transform.position);

        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(currentGridPosition);

        if (Grid.IsValidBussPosition(gameGridObject, currentPos) && !IsOverNPC())
        {
            currentValidPos = true;
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            currentValidPos = false;
            gameGridObject.LightOccupiedUnderTiles();
            gameGridObject.GetSpriteRenderer().color = Util.Occupied;
        }
        Grid.SetDraggingObject(true);
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!Menu || !Menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }

        if (currentValidPos)
        {
            initialPosition = currentPos;
        }
        else
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 0);
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }

        gameGridObject.UpdateCoords();
        Grid.SetDraggingObject(false);
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(gameGridObject.GridPosition);
    }

    private bool IsDraggable()
    {
        if (!Menu || !Menu.IsEditPanelOpen() || gameGridObject == null)
        {
            return false;
        }

        // If you move a table while busy the NPC will self destroy
        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE && Grid.IsTableBusy(gameGridObject))
        {
            GameLog.Log("Moving Busy object" + gameGridObject.Name); // To Show in the UI
            // GameLog.Log("Used by " + gameGridObject.UsedBy.name);
            gameGridObject.GetUsedBy().RecalculateGoTo();
            // gameGridObject.FreeObject();
            // Grid.AddFreeBusinessSpots(gameGridObject);
        }
        // If overlaps with any UI button 
        return gameGridObject.Type != ObjectType.UNDEFINED && Menu.IsEditPanelOpen() && !IsClickingButton();
    }

    // If overlaps with any UI button
    private bool IsClickingButton()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(Settings.Button))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOverNPC()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(Settings.PrefabNpcClient) || r.name.Contains(Settings.PrefabNpcEmployee))
            {
                return true;
            }
        }
        return false;
    }
}
using UnityEngine;

// Here we control the object drag and drop and the state of the NPCs during the drag
public class BaseObjectController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3Int initialGridPosition;
    [SerializeField]
    private Vector3 currentPos; // Current position of the object including while dragging
    private bool iscurrentValidPos; // Is the current position valid for the object including while dragging
    // Initial object position
    [SerializeField]
    private Vector3Int initialActionTileOne;
    [SerializeField]
    protected GameGridObject gameGridObject;
    protected ObjectRotation InitialObjectRotation;
    protected MenuHandlerController Menu { get; set; }

    private void Update()
    {
        if (!Menu || gameGridObject == null || BussGrid.GetDragginObject())
        {
            return;
        }

        // if (Menu.IsEditPanelOpen())
        // {
        //     gameGridObject.LightAvailableUnderTiles();
        // }
        // else
        // {
        //     gameGridObject.HideUnderTiles();
        // }
    }

    protected void Init()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        InitialObjectRotation = ObjectRotation.FRONT;
        //Edit Panel Disable
        if (transform.name.Contains(Settings.ObjectRotationFrontInverted))
        {
            InitialObjectRotation = ObjectRotation.FRONT_INVERTED;
        }
    }

    private void OnMouseDown()
    {
        if (!Menu || !IsDraggable())
        {
            return;
        }

        BussGrid.SetActiveGameGridObject(gameGridObject);
        initialActionTileOne = gameGridObject.GetActionTileInGridPosition();
        initialGridPosition = gameGridObject.GridPosition;
        initialPosition = transform.position;
        gameGridObject.SetIsObjectBeingDragged(true);

        // If you move a table while busy the NPC will self destroy
        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE)
        {
            // Restart NPCs states
            NPCController npc = gameGridObject.GetUsedBy();
            EmployeeController employee = gameGridObject.GetAttendedBy();

            if (npc != null)
            {
                npc.GoToFinalState();
            }

            if (employee != null)
            {
                employee.RecalculateState(gameGridObject);
            }

            gameGridObject.FreeObject(); // So it will be removed while dragging   
        }
    }

    private void OnMouseDrag()
    {
        if (!Menu || !IsDraggable())
        {
            return;
        }

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        currentPos = BussGrid.GetGridWorldPositionMapMouseDrag(Util.GetMouseInWorldPosition());
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);

        // So it will overlay over the rest of the items while dragging
        Vector3Int currentGridPosition = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(currentGridPosition);

        if (BussGrid.IsValidBussPosition(gameGridObject, currentPos) && !IsOverNPC())
        {
            iscurrentValidPos = true;
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            iscurrentValidPos = false;
            gameGridObject.LightOccupiedUnderTiles();
            gameGridObject.GetSpriteRenderer().color = Util.Occupied;
        }

        // If dragging clean previous position on the grid
        BussGrid.FreeCoordWhileDragging(initialGridPosition, initialActionTileOne, gameGridObject);
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!Menu)
        {
            return;
        }

        if (iscurrentValidPos)
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
        BussGrid.SetDraggingObject(false);
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(gameGridObject.GridPosition);

        //We recalculate Paths once the object is placed
        BussGrid.GameController.ReCalculateNpcStates(gameGridObject);

        //if it was a table we re-add it to the freeBussList
        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE)
        {
            gameGridObject.SetIsObjectBeingDragged(false);
        }

        //Re-evaluate all the objects currently in the grid in case of the Unity OnMouseUp failling to update
        // or updating in an inconsistent way
        BussGrid.RecalculateBussGrid();
    }

    private bool IsDraggable()
    {
        if (!Menu || gameGridObject == null)
        {
            return false;
        }
        // If overlaps with any UI button 
        return gameGridObject.Type != ObjectType.UNDEFINED && !IsClickingButton();
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

    public GameGridObject GetGameGridObject()
    {
        return gameGridObject;
    }
}
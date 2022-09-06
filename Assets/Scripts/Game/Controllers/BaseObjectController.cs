using System;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class BaseObjectController : MonoBehaviour
{
    private MenuHandlerController menu;
    private SpriteRenderer spriteRenderer;
    protected GameGridObject gameGridObject;
    protected GridController Grid;
    protected ObjectType Type;
    private Vector3 initialPosition;
    private Vector3 mousePosition;
    private SortingGroup sortLayer;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GameGrid).gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Util.IsNull(gameGridObject, "BaseObjectController/GridController null");
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        menu = menuHandler.GetComponent<MenuHandlerController>();
        Grid = gameGridObject.GetComponent<GridController>();
        sortLayer = GetComponent<SortingGroup>();
        Type = ObjectType.UNDEFINED;
        this.gameGridObject = null;
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (!menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }
        gameGridObject.GameGridObjectSpriteRenderer = spriteRenderer;
        spriteRenderer.color = Util.Available;
        Grid.SetActiveGameGridObject(gameGridObject);
        mousePosition = gameObject.transform.position - Util.GetMouseInWorldPosition();
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
        Vector3 initPos = Grid.GetNearestGridPositionFromWorldMap(initialPosition);
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        sortLayer.sortingOrder = 1;
        spriteRenderer.color = Grid.IsValidBussPosition(currentPos, gameGridObject) ? Util.Available : Util.Occupied;
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
        Vector3 initPos = Grid.GetNearestGridPositionFromWorldMap(initialPosition);
        Grid.DraggingObject = false;
        sortLayer.sortingOrder = 0;

        if (!Grid.IsValidBussPosition(finalPos, gameGridObject))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
        }
        else
        {
            Vector3Int init = gameGridObject.GridPosition;
            initialPosition = new Vector3(finalPos.x, finalPos.y, 1);
            gameGridObject.UpdateCoords(Grid.GetPathFindingGridFromWorldPosition(finalPos), Grid.GetLocalGridFromWorldPosition(finalPos), finalPos);
            Grid.UpdateGridPosition(init, gameGridObject);
        }
        
        spriteRenderer.color = Grid.IsThisSelectedObject(gameGridObject.Name) ? Util.Available : Util.Free;
    }

    private bool IsDraggable()
    {
        if (!menu.IsEditPanelOpen())
        {
            return false;
        }
        
        if (gameGridObject != null && Grid.IsTableBusy(gameGridObject))
        {
            GameLog.Log("Table is Busy "+gameGridObject.Name);
        }
        return Type != ObjectType.UNDEFINED && Type == ObjectType.NPC_TABLE && menu.IsEditPanelOpen() && !Grid.IsTableBusy(gameGridObject);
    }
}
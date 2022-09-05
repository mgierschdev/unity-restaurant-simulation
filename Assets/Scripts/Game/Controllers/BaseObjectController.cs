using UnityEngine;
using UnityEngine.Rendering;

public class BaseObjectController : MonoBehaviour
{
    private MenuHandlerController menu;
    private SpriteRenderer spriteRenderer;
    protected GameGridObject GameGridObject;
    protected GridController Grid;
    protected ObjectType Type;
    private Color available;
    private Color occupied;
    private Color free;
    private Vector3 initialPosition;
    private Vector3 mousePosition;
    private SortingGroup sortLayer;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GameGrid).gameObject;
        Util.IsNull(gameGridObject, "BaseObjectController/GridController null");
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        menu = menuHandler.GetComponent<MenuHandlerController>();
        Grid = gameGridObject.GetComponent<GridController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortLayer = GetComponent<SortingGroup>();
        Type = ObjectType.UNDEFINED;
        available = new Color(0, 1, 0, 0.4f);
        occupied = new Color(1, 0, 0, 0.4f);
        free = new Color(1, 1, 1, 1);
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (!IsDraggable())
        {
            return;
        }

        mousePosition = gameObject.transform.position - Util.GetMouseInWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (!IsDraggable())
        {
            return;
        }

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        Vector3 currentPos = Grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
        Vector3 initPos = Grid.GetNearestGridPositionFromWorldMap(initialPosition);
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        sortLayer.sortingOrder = 1;
        spriteRenderer.color = Grid.IsValidBussPosition(currentPos, initPos) ? available : occupied;
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!IsDraggable())
        {
            return;
        }

        Vector3 finalPos = Grid.GetNearestGridPositionFromWorldMap(transform.position);
        Vector3 initPos = Grid.GetNearestGridPositionFromWorldMap(initialPosition);
        sortLayer.sortingOrder = 0;

        if (!Grid.IsValidBussPosition(finalPos, initPos))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
        }
        else
        {
            initialPosition = new Vector3(finalPos.x, finalPos.y, 1);
            Vector3Int init = GameGridObject.GridPosition;
            GameGridObject.UpdateCoords(Grid.GetPathFindingGridFromWorldPosition(finalPos), Grid.GetLocalGridFromWorldPosition(finalPos), finalPos);
            Grid.UpdateGridPosition(init, GameGridObject.GridPosition);
        }
        spriteRenderer.color = free;
    }

    private bool IsDraggable()
    {
        return Type != ObjectType.UNDEFINED && Type == ObjectType.NPC_TABLE && menu.IsEditPanelOpen();
    }
}
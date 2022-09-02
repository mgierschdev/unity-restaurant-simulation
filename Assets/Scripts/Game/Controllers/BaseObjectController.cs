using UnityEngine;
using UnityEngine.Rendering;
public class BaseObjectController : MonoBehaviour
{
    private MenuHandlerController menu;
    private Vector3 mousePosition;
    private SpriteRenderer spriteRenderer;
    private Color available;
    private Color ocupied;
    private Color free;
    private Vector3 initialPosition;
    private SortingGroup sortLayer;
    protected GameGridObject gameGridObject;
    protected ObjectType Type;
    protected GridController grid;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GAME_GRID).gameObject;
        Util.IsNull(gameGridObject, "BaseObjectController/GridController null");
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        menu = menuHandler.GetComponent<MenuHandlerController>();
        grid = gameGridObject.GetComponent<GridController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortLayer = GetComponent<SortingGroup>();
        Type = ObjectType.UNDEFINED;
        available = new Color(0, 1, 0, 0.4f);
        ocupied = new Color(1, 0, 0, 0.4f);
        free = new Color(1, 1, 1, 1);
        initialPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (IsDragable())
        {
            mousePosition = gameObject.transform.position - Util.GetMouseInWorldPosition();
        }
    }

    private void OnMouseDrag()
    {
        if (IsDragable())
        {
            // Change Overlay color depending if can place or not
            // Mark 2 tiles of the object action tile and position tile
            Vector3 currentPos = grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
            Vector3 initPos = grid.GetNearestGridPositionFromWorldMap(initialPosition);
            transform.position = new Vector3(currentPos.x, currentPos.y, 1);
            sortLayer.sortingOrder = 1;

            if (grid.IsValidBussPosition(currentPos, initPos))
            {
                spriteRenderer.color = available;
            }
            else
            {
                spriteRenderer.color = ocupied;
            }
        }
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (IsDragable())
        {
            Vector3 finalPos = grid.GetNearestGridPositionFromWorldMap(transform.position);
            Vector3 initPos = grid.GetNearestGridPositionFromWorldMap(initialPosition);
            sortLayer.sortingOrder = 0;

            if (!grid.IsValidBussPosition(finalPos, initPos))
            {
                transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
            }
            else
            {
                initialPosition = new Vector3(finalPos.x, finalPos.y, 1);
                Vector3Int init = gameGridObject.GridPosition;
                gameGridObject.UpdateCoords(grid.GetPathFindingGridFromWorldPosition(finalPos), grid.GetLocalGridFromWorldPosition(finalPos), finalPos);
                grid.UpdateGridPosition(init, gameGridObject.GridPosition);
            }

            spriteRenderer.color = free;
        }
    }

    private bool IsDragable()
    {
        return Type != ObjectType.UNDEFINED && Type == ObjectType.NPC_TABLE && menu.IsEditPanelOpen();
    }
}
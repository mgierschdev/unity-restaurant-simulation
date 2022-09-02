using UnityEngine;
public class BaseObjectController : MonoBehaviour
{
    private MenuHandlerController menu;
    protected ObjectType Type;
    protected GridController grid;
    private Vector3 mousePosition;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GAME_GRID).gameObject;
        Util.IsNull(gameGridObject, "BaseObjectController/GridController null");
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        menu = menuHandler.GetComponent<MenuHandlerController>();
        grid = gameGridObject.GetComponent<GridController>();
        Type = ObjectType.UNDEFINED;
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
            transform.position = grid.GetNearestGridPositionFromWorldMap(Util.GetMouseInWorldPosition() + mousePosition);
        }
    }

    private bool IsDragable()
    {
        return Type != ObjectType.UNDEFINED && Type == ObjectType.NPC_TABLE && menu.IsEditPanelOpen();
    }
}
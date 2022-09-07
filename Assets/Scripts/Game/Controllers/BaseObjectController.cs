using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    private List<SpriteRenderer> tiles;
    public List<GameObject> ActionTiles {get; set;}
    public bool[] BusyActionTiles {get; set;}

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GameGrid).gameObject;
        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        GameObject objectActionTile = transform.Find(Settings.BaseObjectActionTile).gameObject;

        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            BusyActionTiles = new bool[1];
            tiles = new List<SpriteRenderer>(){
              objectTileUnder.GetComponent<SpriteRenderer>(),
              objectActionTile.GetComponent<SpriteRenderer>()};

            ActionTiles = new List<GameObject>(){
                objectActionTile
              };
        }
        else if (Type == ObjectType.NPC_DOUBLE_TABLE)
        {
            BusyActionTiles = new bool[2];
            GameObject objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2).gameObject;
            tiles = new List<SpriteRenderer>(){
              objectTileUnder.GetComponent<SpriteRenderer>(),
              objectActionTile.GetComponent<SpriteRenderer>(),
              objectSecondActionTile.GetComponent<SpriteRenderer>()};

            ActionTiles = new List<GameObject>(){
                objectActionTile,
                objectSecondActionTile
              };
        }
        else
        {
            tiles = new List<SpriteRenderer>(){
              objectTileUnder.GetComponent<SpriteRenderer>()
              };
            ActionTiles = new List<GameObject>();
            BusyActionTiles = new bool[1];
        }

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

    private void Update()
    {
        if (Grid.DraggingObject)
        {
            return;
        }

        if (menu.IsEditPanelOpen())
        {
            LightAvailableUnderTiles();
        }
        else
        {
            HideUnderTiles();
        }
    }

    private void OnMouseDown()
    {
        if (!menu.IsEditPanelOpen() || !IsDraggable())
        {
            return;
        }
        gameGridObject.GameGridObjectSpriteRenderer = spriteRenderer;
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
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);
        sortLayer.sortingOrder = 1;

        if (Grid.IsValidBussPosition(currentPos, gameGridObject))
        {
            spriteRenderer.color = Util.Available;
            LightAvailableUnderTiles();
        }
        else
        {
            LightOccupiedUnderTiles();
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
        return Type != ObjectType.UNDEFINED && Type == ObjectType.NPC_SINGLE_TABLE && menu.IsEditPanelOpen() && !Grid.IsTableBusy(gameGridObject);
    }

    private void HideUnderTiles()
    {
        if (gameGridObject == null)
        {
            return;
        }

        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE || gameGridObject.Type == ObjectType.NPC_COUNTER)
        {
            tiles[1].color = Util.Hidden;
        }
        tiles[0].color = Util.Hidden;
    }

    private void LightOccupiedUnderTiles()
    {
        if (gameGridObject == null)
        {
            return;
        }

        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE || gameGridObject.Type == ObjectType.NPC_COUNTER)
        {
            tiles[1].color = Util.LightOccupied;
        }
        tiles[0].color = Util.LightOccupied;
    }

    private void LightAvailableUnderTiles()
    {
        if (gameGridObject == null)
        {
            return;
        }

        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE || gameGridObject.Type == ObjectType.NPC_COUNTER)
        {
            tiles[1].color = Util.LightAvailable;
        }
        tiles[0].color = Util.LightAvailable;
    }
}
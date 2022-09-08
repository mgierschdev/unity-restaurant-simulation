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
    protected List<GameObject> ActionTiles { get; set; }
    //Initial object position
    private Vector3Int actionTileOne;

    public void Awake()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        GameObject gameGridObject = GameObject.Find(Settings.GameGrid).gameObject;
        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Util.IsNull(gameGridObject, "BaseObjectController/GridController null");
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");

        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        ActionTiles = new List<GameObject>();
        tiles = new List<SpriteRenderer>(){
            tileUnder
        };

        if (objectActionTile)
        {
            SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
            tiles.Add(actionTileSpriteRenderer);
            ActionTiles.Add(objectActionTile.gameObject);
        }

        if (objectSecondActionTile)
        {
            SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();
            tiles.Add(secondActionTileSprite);
            ActionTiles.Add(objectSecondActionTile.gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        menu = menuHandler.GetComponent<MenuHandlerController>();
        Grid = gameGridObject.GetComponent<GridController>();
        sortLayer = GetComponent<SortingGroup>();
        Type = ObjectType.UNDEFINED;
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
        actionTileOne = Grid.GetPathFindingGridFromWorldPosition(gameGridObject.GetFirstActionTile());
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

        if (Grid.IsValidBussPosition(currentPos, gameGridObject, actionTileOne))
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

        if (!Grid.IsValidBussPosition(finalPos, gameGridObject, actionTileOne))
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 1);
            spriteRenderer.color = Util.Available;
            LightAvailableUnderTiles();
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
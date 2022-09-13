using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameGridObject : GameObjectBase
{
    public SpriteRenderer SpriteRenderer { get; set; }
    private List<GameObject> actionTiles;
    private List<SpriteRenderer> tiles;
    private ObjectRotation position; // Facing position
    private Transform objectTransform;
    private int actionTile;
    public bool Busy { get; set; } //Being used by an NPC
    public NPCController UsedBy { get; set; }
    public GameObject EditMenu { get; set; }
    private GridController gridController;
    private GameObject objectWithSprite;

    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType)
    {
        position = ObjectRotation.FRONT;
        TileType = tileType;
        Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
    }

    public GameGridObject(string name, Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, ObjectType type, TileType tileType, int cost, string menuItemSprite)
    {
        position = ObjectRotation.FRONT;
        MenuItemSprite = menuItemSprite;
        TileType = tileType;
        Name = name;
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        WorldPosition = worldPosition; // World position on Unity coords
        LocalGridPosition = localGridPosition;
        Type = type;
        Cost = cost;
    }

    public GameGridObject(Transform transform, Vector3Int gridPosition, Vector3Int localGridPosition, int cost, ObjectRotation position, ObjectType type, GameObject editMenu, GridController gridController)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        LocalGridPosition = localGridPosition;
        Type = type;
        Cost = cost;
        this.position = position;
        objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
        SpriteRenderer = objectWithSprite.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();
        EditMenu = editMenu;
        this.gridController = gridController;

        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

        actionTiles = new List<GameObject>(){
            objectActionTile.gameObject,
            objectSecondActionTile.gameObject
        };

        tiles = new List<SpriteRenderer>(){
            tileUnder,
            actionTileSpriteRenderer,
            secondActionTileSprite
        };

        UpdateRotation(position);
        SetEditPanelClickListeners();
    }

    private void SetEditPanelClickListeners()
    {
        GameObject saveObj = EditMenu.transform.Find(Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObj.GetComponent<Button>();
        save.onClick.AddListener(StoreInInventory);
        GameObject rotateObj = EditMenu.transform.Find(Settings.ConstEditStoreMenuRotate).gameObject;
        Button rotate = rotateObj.GetComponent<Button>();
        rotate.onClick.AddListener(RotateObject);
    }

    private void StoreInInventory()
    {
        GameLog.Log("Storing item in Inventory " + Name);
        //Show POPUP confirming action
        gridController.FreeObject(this);
        Object.Destroy(objectTransform.gameObject);
    }

    public bool IsLastPositionEqual(Vector3 actionGridPosition)
    {
        return Util.IsAtDistanceWithObject(GetActionTile(), actionGridPosition);
    }

    public void UpdateCoords()
    {
        GridPosition = gridController.GetPathFindingGridFromWorldPosition(objectTransform.position);
        LocalGridPosition = gridController.GetLocalGridFromWorldPosition(objectTransform.position);
        WorldPosition = objectTransform.position;
    }

    public void Hide()
    {
        SpriteRenderer.color = Util.Free;
        EditMenu.SetActive(false);
    }

    public void Show()
    {
        SpriteRenderer.color = Util.Available;
        EditMenu.SetActive(true);
    }

    public void SetUsed(NPCController npc)
    {
        UsedBy = npc;
        Busy = true;
    }

    public void FreeObject()
    {
        UsedBy = null;
        Busy = false;
    }

    public Vector3 GetActionTile()
    {
        return actionTiles[actionTile].transform.position;
    }

    public void RotateObject()
    {
        Vector3Int prev = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        position++;
        int pos = (int)position % 4 == 0 ? 4 : (int)position % 4;
        ObjectRotation newPos = (ObjectRotation)pos;
        UpdateRotation(newPos);
        Vector3Int post = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        gridController.SwapCoords(prev.x, prev.y, post.x, post.y);
        UpdateCoords();
    }

    private void UpdateRotation(ObjectRotation newPosition)
    {

        if (Type != ObjectType.NPC_SINGLE_TABLE)
        {
            return;
        }

        Sprite singleSpriteWood = Resources.Load<Sprite>(Settings.SingleWoodenTableSprite);
        Sprite singleSpriteWoodMirror = Resources.Load<Sprite>(Settings.SingleWoodenTableSpriteMirror);

        switch (newPosition)
        {
            case ObjectRotation.FRONT:
                SpriteRenderer.sprite = singleSpriteWood;
                objectWithSprite.transform.localScale = new Vector3(1, 1, 1);
                actionTile = 0;
                tiles[1].color = Util.LightAvailable;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.FRONT_INVERTED:
                SpriteRenderer.sprite = singleSpriteWood;
                objectWithSprite.transform.localScale = new Vector3(-1, 1, 1);
                actionTile = 0;
                tiles[1].color = Util.LightAvailable;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.BACK:
                SpriteRenderer.sprite = singleSpriteWoodMirror;
                objectWithSprite.transform.localScale = new Vector3(1, 1, 1);
                actionTile = 1;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.LightAvailable;
                return;
            case ObjectRotation.BACK_INVERTED:
                SpriteRenderer.sprite = singleSpriteWoodMirror;
                objectWithSprite.transform.localScale = new Vector3(-1, 1, 1);
                actionTile = 1;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.LightAvailable;
                return;
        }
    }

    public void HideUnderTiles()
    {
        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[actionTile].color = Util.Hidden;
            tiles[actionTile + 1].color = Util.Hidden;
        }
        else
        {
            tiles[actionTile + 1].color = Util.Hidden;
        }
    }

    public void LightOccupiedUnderTiles()
    {
        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[0].color = Util.LightOccupied;
            tiles[actionTile + 1].color = Util.LightOccupied;
        }
        else
        {
            tiles[actionTile + 1].color = Util.LightOccupied;
        }
    }

    public void LightAvailableUnderTiles()
    {
        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[0].color = Util.LightAvailable;
            tiles[actionTile + 1].color = Util.LightAvailable;
        }
        else
        {
            tiles[actionTile + 1].color = Util.LightAvailable;
        }
    }
}
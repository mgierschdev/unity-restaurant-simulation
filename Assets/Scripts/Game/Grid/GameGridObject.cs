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
    GameObject EditMenu { get; set; }

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

    public GameGridObject(Transform transform, Vector3Int gridPosition, Vector3Int localGridPosition, int cost, ObjectRotation position, ObjectType type, GameObject EditMenu)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = gridPosition; // Grid position, first position = 0, 0
        LocalGridPosition = localGridPosition;
        Type = type;
        Cost = cost;
        this.position = position;
        SpriteRenderer = transform.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();

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
    }

    private void SetEditPanelCLickListeners()
    {
        GameObject saveObj = objectTransform.Find(Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObj.GetComponent<Button>();
        save.onClick.AddListener(() => StoreInInventory());
        GameObject rotateObj = objectTransform.Find(Settings.ConstEditStoreMenuRotate).gameObject;
        Button rotate = rotateObj.GetComponent<Button>();
        rotate.onClick.AddListener(() => RotateObject());
    }

    private void StoreInInventory()
    {
        GameLog.Log("Storing item in Inventory " + Name);
        //Show POPUP confirming action
        UnityEngine.Object.Destroy(objectTransform);
        
    }

    public bool IsLastPositionEqual(Vector3 actionGridPosition)
    {
        return Util.IsAtDistanceWithObject(GetActionTile(), actionGridPosition);
    }

    public void UpdateCoords(Vector3Int gridPosition, Vector3Int localGridPosition, Vector3 worldPosition)
    {
        GridPosition = gridPosition;
        LocalGridPosition = localGridPosition;
        WorldPosition = worldPosition;
    }

    public void Hide()
    {
        SpriteRenderer.color = Util.Free;
    }

    public void Show()
    {
        SpriteRenderer.color = Util.Available;
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
        int pos = ((int)position + 1) % 4;
        ObjectRotation newPos = (ObjectRotation)pos;
        UpdateRotation(newPos);
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
                objectTransform.localScale = new Vector3(1, 1, 1);
                actionTile = 0;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.FRONT_INVERTED:
                SpriteRenderer.sprite = singleSpriteWood;
                objectTransform.localScale = new Vector3(-1, 1, 1);
                actionTile = 0;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.BACK:
                SpriteRenderer.sprite = singleSpriteWoodMirror;
                objectTransform.localScale = new Vector3(1, 1, 1);
                actionTile = 1;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.BACK_INVERTED:
                SpriteRenderer.sprite = singleSpriteWoodMirror;
                objectTransform.localScale = new Vector3(-1, 1, 1);
                actionTile = 1;
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.Hidden;
                return;
        }
    }

    public void HideUnderTiles()
    {
        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[actionTile + 1].color = Util.Hidden;
        }
        tiles[actionTile + 1].color = Util.Hidden;
    }

    public void LightOccupiedUnderTiles()
    {

        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[actionTile + 1].color = Util.LightOccupied;
        }
        tiles[actionTile + 1].color = Util.LightOccupied;
    }

    public void LightAvailableUnderTiles()
    {

        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            tiles[actionTile + 1].color = Util.LightAvailable;
        }
        tiles[actionTile + 1].color = Util.LightAvailable;
    }
}
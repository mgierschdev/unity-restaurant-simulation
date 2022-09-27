using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class GameGridObject : GameObjectBase
{
    private List<GameObject> actionTiles;
    private List<SpriteRenderer> tiles;
    private GridController gridController;
    private GameObject objectWithSprite;
    private SpriteResolver spriteResolver;
    private Transform objectTransform;
    private int actionTile;
    public StoreGameObject StoreGameObject { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public ObjectRotation FacingPosition { get; set; } // Facing position
    public bool Busy { get; set; } //Being used by an NPC
    public NPCController UsedBy { get; set; }
    public GameObject EditMenu { get; set; }

    public GameGridObject(Transform transform, GridController gridController, ObjectRotation position, StoreGameObject storeGameObject)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = gridController.GetPathFindingGridFromWorldPosition(transform.position); // Grid position, first position = 0, 0
        LocalGridPosition = gridController.GetLocalGridFromWorldPosition(transform.position);
        Type = storeGameObject.Type;
        this.StoreGameObject = storeGameObject;
        objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
        SpriteRenderer = objectWithSprite.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();
        SortingLayer.sortingOrder = -1 * GridPosition.y;
        this.gridController = gridController;
        FacingPosition = position;

        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

        spriteResolver = objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();//TEST TODO
        spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);

        //Edit Panel Disable
        EditMenu = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        EditMenu.SetActive(false);

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

    public void Init()
    {
        string id = gridController.GetObjectCount() + 1 + "-" + Time.frameCount;
        objectTransform.name = StoreGameObject.Type + "." + id;
        Name = objectTransform.name;
        Hide();
    }

    private void SetEditPanelClickListeners()
    {
        GameObject saveObj = EditMenu.transform.Find(Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObj.GetComponent<Button>();
        save.onClick.AddListener(StoreInInventory);

        GameObject rotateObjLeft = EditMenu.transform.Find(Settings.ConstEditStoreMenuRotateLeft).gameObject;
        Button rotateLeft = rotateObjLeft.GetComponent<Button>();
        rotateLeft.onClick.AddListener(RotateObjectLeft);

        GameObject rotateObjRight = EditMenu.transform.Find(Settings.ConstEditStoreMenuRotateRight).gameObject;
        Button rotateRight = rotateObjRight.GetComponent<Button>();
        rotateRight.onClick.AddListener(RotateObjectRight);
    }

    private void StoreInInventory()
    {
        //TODO: Show POPUP confirming action
        //GameLog.Log("TODO: Storing item in Inventory " + Name);
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
        HideUnderTiles();
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
        // if doesnt have action point returns the object actual position
        if (!StoreGameObject.HasActionPoint)
        {
            return tiles[0].transform.position;
        }

        return actionTiles[actionTile].transform.position;
    }

    public void RotateObjectRight()
    {
        if (!IsValidRotation(1)) //right
        {
            //TODO: Show popup message
            GameLog.Log("Rotation is invalid");
            return;
        }

        Vector3Int prev = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        FacingPosition++;

        if ((int)FacingPosition >= 5)
        {
            FacingPosition = ObjectRotation.FRONT;
        }
        UpdateRotation(FacingPosition);
        Vector3Int post = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        gridController.SwapCoords(prev.x, prev.y, post.x, post.y);
        UpdateCoords();
    }

    public void RotateObjectLeft()
    {
        if (!IsValidRotation(0)) //left
        {
            //TODO: Show popup message
            GameLog.Log("Rotation is invalid");
            return;
        }

        Vector3Int prev = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        FacingPosition--;

        if ((int)FacingPosition <= 0)
        {
            FacingPosition = ObjectRotation.BACK_INVERTED;
        }

        UpdateRotation(FacingPosition);
        Vector3Int post = gridController.GetPathFindingGridFromWorldPosition(GetActionTile());
        gridController.SwapCoords(prev.x, prev.y, post.x, post.y);
        UpdateCoords();
    }

    private bool IsValidRotation(int side)
    {
        ObjectRotation tmp = FacingPosition;
        if (side == 0)
        {
            tmp--;
        }
        else
        {
            tmp++;
        }

        if ((int)tmp >= 5)
        {
            tmp = ObjectRotation.FRONT;
        }
        else if ((int)tmp <= 0)
        {
            tmp = ObjectRotation.BACK_INVERTED;
        }

        // we flip the object temporaly to check the new action tile position
        objectWithSprite.transform.localScale = GetRotationVector(tmp);
        Vector3Int rotatedPosition = gridController.GetPathFindingGridFromWorldPosition(objectTransform.position);
        Vector3Int rotatedActionTile = gridController.GetPathFindingGridFromWorldPosition(actionTiles[GetRotationActionTile(tmp)].transform.position);
        // we flip the object back 
        objectWithSprite.transform.localScale = GetRotationVector(FacingPosition);
        if (gridController.IsFreeBussCoord(rotatedPosition) || gridController.IsFreeBussCoord(rotatedActionTile))
        {
            return true;
        }

        return false;
    }

    public void UpdateRotation(ObjectRotation newPosition)
    {
        switch (newPosition)
        {
            case ObjectRotation.FRONT:
                spriteResolver.SetCategoryAndLabel(StoreGameObject.SpriteLibCategory, StoreGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT);
                actionTile = GetRotationActionTile(ObjectRotation.FRONT);
                tiles[1].color = Util.LightAvailable;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.FRONT_INVERTED:
                spriteResolver.SetCategoryAndLabel(StoreGameObject.SpriteLibCategory, StoreGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT_INVERTED);
                actionTile = GetRotationActionTile(ObjectRotation.FRONT_INVERTED);
                tiles[1].color = Util.LightAvailable;
                tiles[2].color = Util.Hidden;
                return;
            case ObjectRotation.BACK:
                spriteResolver.SetCategoryAndLabel(StoreGameObject.SpriteLibCategory + "-Inverted", StoreGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BACK);
                actionTile = GetRotationActionTile(ObjectRotation.BACK);
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.LightAvailable;
                return;
            case ObjectRotation.BACK_INVERTED:
                spriteResolver.SetCategoryAndLabel(StoreGameObject.SpriteLibCategory + "-Inverted", StoreGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BACK_INVERTED);
                actionTile = GetRotationActionTile(ObjectRotation.BACK_INVERTED);
                tiles[1].color = Util.Hidden;
                tiles[2].color = Util.LightAvailable;
                return;
        }
    }

    private Vector3 GetRotationVector(ObjectRotation objectRotation)
    {
        switch (objectRotation)
        {
            case ObjectRotation.FRONT:
                return new Vector3(1, 1, 1);
            case ObjectRotation.BACK:
                return new Vector3(1, 1, 1);
            case ObjectRotation.FRONT_INVERTED:
                return new Vector3(-1, 1, 1);
            case ObjectRotation.BACK_INVERTED:
                return new Vector3(-1, 1, 1);
        }

        return Vector3.positiveInfinity;
    }

    private int GetRotationActionTile(ObjectRotation objectRotation)
    {
        switch (objectRotation)
        {
            case ObjectRotation.FRONT:
                return 0;
            case ObjectRotation.BACK:
                return 1;
            case ObjectRotation.FRONT_INVERTED:
                return 0;
            case ObjectRotation.BACK_INVERTED:
                return 1;
        }

        return int.MaxValue;
    }

    public void HideUnderTiles()
    {
        tiles[0].color = Util.Hidden;
        tiles[actionTile + 1].color = Util.Hidden;
    }

    public void LightOccupiedUnderTiles()
    {
        tiles[0].color = Util.LightOccupied;

        if (StoreGameObject.HasActionPoint)
        {
            tiles[actionTile + 1].color = Util.LightOccupied;
        }
    }

    public void LightAvailableUnderTiles()
    {
        tiles[0].color = Util.LightAvailable;
        if (StoreGameObject.HasActionPoint)
        {
            tiles[actionTile + 1].color = Util.LightAvailable;
        }
    }
}
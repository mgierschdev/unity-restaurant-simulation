using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class GameGridObject : GameObjectBase
{
    private List<GameObject> actionTiles;
    private List<SpriteRenderer> tiles;
    private GridController Grid;
    private GameObject objectWithSprite;
    private SpriteResolver spriteResolver;
    private Transform objectTransform;
    private int actionTile;
    private StoreGameObject storeGameObject;
    private SpriteRenderer spriteRenderer;
    private ObjectRotation facingPosition; // Facing position
    private bool busy; //Being used by an NPC
    private NPCController usedBy;
    private EmployeeController attendedBy;
    private GameObject editMenu;
    private bool isObjectBeingDragged;


    public GameGridObject(Transform transform, GridController gridController, ObjectRotation position, StoreGameObject storeGameObject)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = gridController.GetPathFindingGridFromWorldPosition(transform.position); // Grid position, first position = 0, 0
        LocalGridPosition = gridController.GetLocalGridFromWorldPosition(transform.position);
        Type = storeGameObject.Type;
        this.storeGameObject = storeGameObject;
        objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
        spriteRenderer = objectWithSprite.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();
        SortingLayer.sortingOrder = Util.GetSorting(GridPosition);
        this.Grid = gridController;
        facingPosition = position;

        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

        spriteResolver = objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();//TEST TODO
        spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);

        //Edit Panel Disable
        editMenu = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        editMenu.SetActive(false);

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

    private void SetID()
    {
        string id = Grid.GetObjectCount() + 1 + "-" + Time.frameCount;
        objectTransform.name = storeGameObject.Type + "." + id;
        Name = objectTransform.name;
    }

    public void Init()
    {
        SetID();
        Hide();
    }

    private void SetEditPanelClickListeners()
    {
        GameObject saveObj = editMenu.transform.Find(Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObj.GetComponent<Button>();
        save.onClick.AddListener(StoreInInventory);

        GameObject rotateObjLeft = editMenu.transform.Find(Settings.ConstEditStoreMenuRotateLeft).gameObject;
        Button rotateLeft = rotateObjLeft.GetComponent<Button>();
        rotateLeft.onClick.AddListener(RotateObjectLeft);

        GameObject rotateObjRight = editMenu.transform.Find(Settings.ConstEditStoreMenuRotateRight).gameObject;
        Button rotateRight = rotateObjRight.GetComponent<Button>();
        rotateRight.onClick.AddListener(RotateObjectRight);
    }

    //Store Item in inventory
    private void StoreInInventory()
    {
        //TODO: Show POPUP confirming action
        //GameLog.Log("TODO: Storing item in Inventory " + Name);
        Grid.PlayerData.StoreItem(this);
        Grid.ClearCurrentClickedActiveGameObject(); // Clear the Item from the current seledted in the grid 
        Grid.FreeObject(this);
        if (Type == ObjectType.NPC_SINGLE_TABLE || Type == ObjectType.NPC_COUNTER)
        {
            Grid.RemoveBussTable(this);
        }
        Grid.ReCalculateNpcStates(this);
        Object.Destroy(objectTransform.gameObject);
    }

    public bool IsLastPositionEqual(Vector3 actionGridPosition)
    {
        return Util.IsAtDistanceWithObject(GetActionTile(), actionGridPosition);
    }

    public void UpdateCoords()
    {
        GridPosition = Grid.GetPathFindingGridFromWorldPosition(objectTransform.position);
        LocalGridPosition = Grid.GetLocalGridFromWorldPosition(objectTransform.position);
        WorldPosition = objectTransform.position;
        Grid.UpdateObjectPosition(this);
    }

    public void Hide()
    {
        HideUnderTiles();
        editMenu.SetActive(false);
    }

    public void Show()
    {
        spriteRenderer.color = Util.Available;
        editMenu.SetActive(true);
    }

    public void SetUsed(NPCController npc)
    {
        usedBy = npc;
        busy = true;
    }

    public void FreeObject()
    {
        usedBy = null;
        attendedBy = null;
        busy = false;
        Grid.RemoveBusyBusinessSpots(this);
    }

    public Vector3 GetActionTile()
    {
        // if doesnt have action point returns the object actual position
        if (!storeGameObject.HasActionPoint)
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

        Vector3Int prev = Grid.GetPathFindingGridFromWorldPosition(GetActionTile());
        facingPosition++;

        if ((int)facingPosition >= 5)
        {
            facingPosition = ObjectRotation.FRONT;
        }
        UpdateRotation(facingPosition);
        Vector3Int post = Grid.GetPathFindingGridFromWorldPosition(GetActionTile());
        Grid.SwapCoords(prev.x, prev.y, post.x, post.y);
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

        Vector3Int prev = Grid.GetPathFindingGridFromWorldPosition(GetActionTile());
        facingPosition--;

        if ((int)facingPosition <= 0)
        {
            facingPosition = ObjectRotation.BACK_INVERTED;
        }

        UpdateRotation(facingPosition);
        Vector3Int post = Grid.GetPathFindingGridFromWorldPosition(GetActionTile());
        Grid.SwapCoords(prev.x, prev.y, post.x, post.y);
        UpdateCoords();
    }

    private bool IsValidRotation(int side)
    {
        if (!storeGameObject.HasActionPoint)
        {
            return true;
        }

        ObjectRotation tmp = facingPosition;
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
        Vector3Int rotatedPosition = Grid.GetPathFindingGridFromWorldPosition(objectTransform.position);
        Vector3Int rotatedActionTile = Grid.GetPathFindingGridFromWorldPosition(actionTiles[GetRotationActionTile(tmp)].transform.position);
        // we flip the object back 
        objectWithSprite.transform.localScale = GetRotationVector(facingPosition);
        if (Grid.IsFreeBussCoord(rotatedPosition) || Grid.IsFreeBussCoord(rotatedActionTile))
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
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT);
                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.FRONT);
                    tiles[1].color = Util.LightAvailable;
                    tiles[2].color = Util.Hidden;
                }
                return;
            case ObjectRotation.FRONT_INVERTED:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT_INVERTED);

                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.FRONT_INVERTED);
                    tiles[1].color = Util.LightAvailable;
                    tiles[2].color = Util.Hidden;
                }
                return;
            case ObjectRotation.BACK:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory + "-Inverted", storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BACK);
                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.BACK);
                    tiles[1].color = Util.Hidden;
                    tiles[2].color = Util.LightAvailable;
                }
                return;
            case ObjectRotation.BACK_INVERTED:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory + "-Inverted", storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BACK_INVERTED);
                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.BACK_INVERTED);
                    tiles[1].color = Util.Hidden;
                    tiles[2].color = Util.LightAvailable;

                }
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

        if (storeGameObject.HasActionPoint)
        {
            tiles[actionTile + 1].color = Util.LightOccupied;
        }
    }

    public void LightAvailableUnderTiles()
    {
        tiles[0].color = Util.LightAvailable;
        if (storeGameObject.HasActionPoint)
        {
            tiles[actionTile + 1].color = Util.LightAvailable;
        }
    }

    public StoreGameObject GetStoreGameObject()
    {
        return storeGameObject;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public bool GetBusy()
    {
        return busy;
    }

    public void SetBusy(bool val)
    {
        busy = val;
    }

    public void SetUsedBy(NPCController controller)
    {
        usedBy = controller;
    }

    public NPCController GetUsedBy()
    {
        return usedBy;
    }

    public void SetAttendedBy(EmployeeController controller)
    {
        attendedBy = controller;
    }

    public EmployeeController GetAttendedBy()
    {
        return attendedBy;
    }

    public void SetIsObjectBeingDragged(bool val)
    {
        isObjectBeingDragged = val;
    }

    public bool GetIsObjectBeingDragged()
    {
        return isObjectBeingDragged;
    }
}
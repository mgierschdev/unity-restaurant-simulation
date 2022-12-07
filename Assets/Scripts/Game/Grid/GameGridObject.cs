using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class GameGridObject : GameObjectBase, IEquatable<GameGridObject>, IComparable<GameGridObject>
{
    private Transform objectTransform;
    private DataGameObject dataGameObject;
    private BaseObjectController baseObjectController;
    private List<GameObject> actionTiles;
    private List<SpriteRenderer> tiles;
    private SpriteResolver spriteResolver;
    private int actionTile;
    private StoreGameObject storeGameObject;
    private SpriteRenderer spriteRenderer;
    private ObjectRotation facingPosition; // Facing position
    private NPCController usedBy;
    private EmployeeController attendedBy, assignedTo;
    // Buttons/ Sprites and edit menus
    private GameObject saveObjButton, rotateObjLeftButton, cancelButton, acceptButton, editMenu, objectWithSprite;
    private InfoPopUpController infoPopUpController;
    private LoadSliderController loadSlider, loadSliderMove;
    // Store - To be bought Item, Is Item active, before purchase, (isItemReady, isItemLoading) item on top of the objects, (isObjectSelected) current under preview
    private bool isItemBought, active, isItemReady, isObjectSelected, isObjectBeingDragged;
    public GameGridObject(Transform transform)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(transform.position); // Grid position, first position = 0, 0
        LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(transform.position);
        objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
        spriteRenderer = objectWithSprite.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();
        SortingLayer.sortingOrder = Util.GetSorting(GridPosition);
        isObjectSelected = false;
        isItemBought = true;

        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

        // Setting base controller
        baseObjectController = objectTransform.GetComponent<BaseObjectController>();

        //Edit Panel Disable
        editMenu = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        editMenu.SetActive(false);

        // On top Move slider
        GameObject moveObjectSlider = transform.Find("Slider/LoadSliderMove").gameObject;
        Util.IsNull(moveObjectSlider, "MoveSlider is null");
        loadSliderMove = moveObjectSlider.GetComponent<LoadSliderController>();
        loadSliderMove.SetSliderFillMethod(Image.FillMethod.Vertical);
        loadSliderMove.SetDefaultFillTime(1f);
        loadSliderMove.SetInactive();

        //On top load slider
        GameObject loadObjectSlider = transform.Find("Slider/LoadSlider").gameObject;
        loadSlider = loadObjectSlider.GetComponent<LoadSliderController>();
        loadSlider.SetDefaultFillTime(1f);
        loadSlider.SetInactive();

        //On top Info popup
        GameObject infoPopUpGameobject = transform.Find("Slider/" + Settings.TopPopUpObject).gameObject;
        infoPopUpController = infoPopUpGameobject.GetComponent<InfoPopUpController>();

        actionTiles = new List<GameObject>() { objectActionTile.gameObject, objectSecondActionTile.gameObject };
        tiles = new List<SpriteRenderer>() { tileUnder, actionTileSpriteRenderer, secondActionTileSprite };

        active = baseObjectController.GetInitIsActive();
        // Object rotation
        facingPosition = baseObjectController.GetInitialRotation();
        SetEditPanelButtonClickListeners();
    }

    // For GamegridObject init
    public void SetStoreGameObject(StoreGameObject storeGameObject)
    {
        dataGameObject = baseObjectController.GetDataGameObject();
        // For setting the object sprite
        this.storeGameObject = storeGameObject;
        Type = storeGameObject.Type;
        spriteResolver = objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();
        spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
        infoPopUpController.SetSprite(storeGameObject.Identifier);
        if (!storeGameObject.HasActionPoint)
        {
            loadSlider.SetSliderSprite(storeGameObject.Identifier);
        }
        UpdateInitRotation(baseObjectController.GetInitialRotation());
        Init(); // StoreGameObject.Type required
    }

    private void SetID()
    {
        string id = BussGrid.GetObjectCount() + 1 + "-" + Time.frameCount;
        objectTransform.name = storeGameObject.Type + "." + id;
        Name = objectTransform.name;
    }

    public void Init()
    {
        SetID();
        HideEditMenu();
    }

    private void SetEditPanelButtonClickListeners()
    {
        saveObjButton = editMenu.transform.Find("" + Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObjButton.GetComponent<Button>();
        save.onClick.AddListener(StoreInInventory);

        rotateObjLeftButton = editMenu.transform.Find(Settings.ConstEditStoreMenuRotateLeft).gameObject;
        Button rotateLeft = rotateObjLeftButton.GetComponent<Button>();
        rotateLeft.onClick.AddListener(RotateObjectLeft);

        acceptButton = editMenu.transform.Find(Settings.ConstEditStoreMenuButtonAccept).gameObject;
        Button accept = acceptButton.GetComponent<Button>();
        accept.onClick.AddListener(AcceptPurchase);

        cancelButton = editMenu.transform.Find(Settings.ConstEditStoreMenuButtonCancel).gameObject;
        Button cancel = cancelButton.GetComponent<Button>();
        cancel.onClick.AddListener(CancelPurchase);

        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    // Store Item in inventory, store object
    public void StoreInInventory()
    {
        try
        {
            GameLog.Log("TODO: UI message / notification banner: Storing item in Inventory " + Name);
            dataGameObject.IS_STORED = true;
            PlayerData.StoreItem(this);

            // Clear the Item from the current selected in the grid 
            ObjectDraggingHandler.ClearCurrentClickedActiveGameObject();

            // we clean the table from the employer
            ClearTableEmployee();

            // we clear the table from the client
            ClearTableClient();

            BussGrid.GameController.ReCalculateNpcStates(this);
            BussGrid.GetGameGridObjectsDictionary().TryRemove(Name, out GameGridObject tmp);
            ObjectDraggingHandler.SetIsDraggingEnable(false);// it can be clicked independent 

            if (Type == ObjectType.NPC_SINGLE_TABLE)
            {
                TableHandler.GetBussQueueMap().TryRemove(this, out byte tmp2);
            }

            DisableIfCounter();
            Object.Destroy(objectTransform.gameObject);
        }
        catch (Exception e)
        {
            GameLog.LogError("Exception thrown, likely missing reference (StoreInInventory GameGridObject): " + e);
        }
    }

    private void ClearTableClient()
    {
        if (usedBy != null)
        {
            usedBy.SetTableMoved();
            usedBy = null;
        }
    }

    private void ClearTableEmployee()
    {
        if (attendedBy != null)
        {
            attendedBy.SetTableToAttend(null);
            attendedBy.SetTableMoved();
            attendedBy = null;
        }
    }

    public void UpdateObjectCoords()
    {
        GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(objectTransform.position);
        LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(objectTransform.position);
        WorldPosition = objectTransform.position;
    }

    // This is call everytime the object changes position
    public void UpdateCoordsAndSetObstacle()
    {
        GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(objectTransform.position);
        LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(objectTransform.position);
        WorldPosition = objectTransform.position;
        BussGrid.UpdateObjectPosition(this);

        // it could be a preview object
        if (dataGameObject != null)
        {
            dataGameObject.POSITION = new int[] { GridPosition.x, GridPosition.y };
        }
    }

    // Changes the sprite renderer of the selected object
    public void HideEditMenu()
    {
        SortingLayer.sortingOrder = Util.GetSorting(GridPosition);
        HideUnderTiles();
        spriteRenderer.color = Util.Free;
        editMenu.SetActive(false);
    }

    // Changes the sprite renderer of the selected object
    public void ShowEditMenu()
    {
        SortingLayer.sortingOrder = Util.highlightObjectSortingPosition;
        spriteRenderer.color = Util.LightAvailable;
        editMenu.SetActive(true);
    }

    public void FreeObject()
    {
        usedBy = null;
        attendedBy = null;
    }

    public Vector3 GetActionTile()
    {
        if (objectTransform == null)
        {
            return Vector3.negativeInfinity;
        }

        // if doesnt have action point returns the object actual position
        if (!storeGameObject.HasActionPoint)
        {
            return tiles[0].transform.position;
        }

        return actionTiles != null ? actionTiles[actionTile].transform.position : Vector3.negativeInfinity;
    }

    public Vector3Int GetActionTileInGridPosition()
    {
        if (objectTransform == null)
        {
            return Util.GetVector3IntNegativeInfinity();
        }

        // Gets the orientation and then + 1 ...
        if (facingPosition == ObjectRotation.FRONT)
        {
            return GridPosition + new Vector3Int(0, 1);
        }
        else if (facingPosition == ObjectRotation.FRONT_INVERTED)
        {
            return GridPosition + new Vector3Int(1, 0);
        }
        else if (facingPosition == ObjectRotation.BACK)
        {
            return GridPosition + new Vector3Int(0, -1);
        }
        else
        {
            return GridPosition + new Vector3Int(-1, 0);
        }
    }

    public void RotateObjectLeft()
    {
        // If there is any NPC we send it to the final state
        ResetNPCStates();
        FreeObject();
        facingPosition--;

        if ((int)facingPosition <= 0)
        {
            facingPosition = ObjectRotation.BACK_INVERTED;
        }

        UpdateRotation(facingPosition);
    }

    // If we rotate the table no one can attend the table or go to the table
    private void ResetNPCStates()
    {
        if (attendedBy != null)
        {
            attendedBy.SetTableToAttend(null);
            attendedBy = null;
        }

        if (usedBy != null)
        {
            usedBy.SetTable(null);
            usedBy.SetTableMoved();
            usedBy = null;
        }
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
        Vector3Int rotatedPosition = BussGrid.GetPathFindingGridFromWorldPosition(objectTransform.position);
        Vector3Int rotatedActionTile = BussGrid.GetPathFindingGridFromWorldPosition(actionTiles[GetRotationActionTile(tmp)].transform.position);
        // we flip the object back 
        objectWithSprite.transform.localScale = GetRotationVector(facingPosition);
        if (BussGrid.IsFreeBussCoord(rotatedPosition) || BussGrid.IsFreeBussCoord(rotatedActionTile))
        {
            return true;
        }

        return false;
    }

    // Update the initial object roation at the time of object creation
    public void UpdateInitRotation(ObjectRotation rotation)
    {
        // We dont check if the rotation is valid since we assume that the data is valid already
        ResetNPCStates();
        UpdateRotation(rotation);
    }

    public void UpdateRotation(ObjectRotation newPosition)
    {
        // it could be a preview object
        if (dataGameObject != null)
        {
            dataGameObject.ROTATION = (int)newPosition;
        }

        switch (newPosition)
        {
            case ObjectRotation.FRONT:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT);
                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.FRONT);
                    // tiles[1].color = Util.LightAvailable;
                    tiles[2].color = Util.Hidden;
                }
                return;
            case ObjectRotation.FRONT_INVERTED:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FRONT_INVERTED);

                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.FRONT_INVERTED);
                    // tiles[1].color = Util.LightAvailable;
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
                    // tiles[2].color = Util.LightAvailable;
                }
                return;
            case ObjectRotation.BACK_INVERTED:
                spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory + "-Inverted", storeGameObject.Identifier);
                objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BACK_INVERTED);
                if (storeGameObject.HasActionPoint)
                {
                    actionTile = GetRotationActionTile(ObjectRotation.BACK_INVERTED);
                    tiles[1].color = Util.Hidden;
                    // tiles[2].color = Util.LightAvailable;

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
        return Vector3.negativeInfinity;
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

    // public void ShowOccupiedUnderTiles()
    // {
    //     //tiles[0].color = Util.LightOccupied;

    //     // if (storeGameObject.HasActionPoint)
    //     // {
    //     //     tiles[actionTile + 1].color = Util.LightOccupied;
    //     // }
    // }

    // public void ShowAvailableUnderTiles()
    // {
    //     //tiles[0].color = Util.LightAvailable;
    //     // if (storeGameObject.HasActionPoint)
    //     // {
    //     //     tiles[actionTile + 1].color = Util.LightAvailable;
    //     // }
    // }

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
        return usedBy != null;
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

    public void SetAssignedTo(EmployeeController controller)
    {
        assignedTo = controller;
    }

    public bool IsItemAssignedTo()
    {
        return assignedTo != null;
    }

    public bool GetIsObjectBeingDragged()
    {
        return isObjectBeingDragged;
    }

    public bool IsFree()
    {
        //Debug.Log(Name + " UsedBy " + (usedBy == null) + " " + (attendedBy == null));
        return usedBy == null && attendedBy == null;
    }
    public bool HasClient()
    {
        return usedBy != null;
    }

    public bool HasAttendedBy()
    {
        return attendedBy != null;
    }

    public void UpdateMoveSlider()
    {
        if (isObjectSelected)
        {
            return;
        }

        if (loadSliderMove.IsFinished())
        {
            SetObjectSelected();
        }

        DiableTopInfoObject();
        loadSliderMove.SetActive();
    }

    // This loads the top Item
    public void UpdateLoadItemSlider()
    {
        if (isObjectSelected)
        {
            return;
        }

        if (loadSlider.IsFinished())
        {
            loadSlider.RestartState();
            SetItemsReady();
        }
    }

    public void SetItemsReady()
    {
        isItemReady = true;
        infoPopUpController.Enable();
    }

    public bool GetIsItemReady()
    {
        return isItemReady;
    }

    private void SetObjectSelected()
    {
        // Disables dragging for a second 
        baseObjectController.DisableDisableDraggingTemp();
        isObjectSelected = true;
        loadSliderMove.SetInactive();
        ObjectDraggingHandler.SetActiveGameGridObject(this);
        ClearTableClient();
        ClearTableEmployee();
        DisableIfCounter();
        //We clean grid position
        BussGrid.FreeObject(this);
        objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, Util.SelectedObjectZPosition);
        // We clean in case the item is loaded on top
        if (storeGameObject.HasActionPoint && isItemReady)
        {
            DiableTopInfoObject();
        }
    }

    public void DiableTopInfoObject()
    {
        isItemReady = false;
        infoPopUpController.Disable();
        loadSlider.SetInactive();
    }

    public void SetInactive()
    {
        isObjectSelected = false;
        objectTransform.position = new Vector3(objectTransform.position.x, objectTransform.position.y, Util.ObjectZPosition);
        ObjectDraggingHandler.SetIsDraggingEnable(false);
        ObjectDraggingHandler.HideHighlightedGridBussFloor();
    }

    public bool GetIsObjectSelected()
    {
        return isObjectSelected;
    }

    public void SetNewObjectActiveButtons()
    {
        acceptButton.SetActive(true);
        cancelButton.SetActive(true);
        rotateObjLeftButton.SetActive(true);
        saveObjButton.SetActive(false);
    }

    public void SetStoreObject()
    {
        // We show accept, cancel buttons and select the object
        isObjectSelected = true;
        isItemBought = false;
        ObjectDraggingHandler.SetActiveGameGridObject(this);
        SetNewObjectActiveButtons();
    }

    // Used in the case the player cancels or clicks outside the object
    public bool GetIsItemBought()
    {
        return isItemBought;
    }

    public void AcceptPurchase()
    {
        if (!baseObjectController.GetIscurrentValidPos())
        {
            return;
        }

        isItemBought = true;
        baseObjectController.SetNewItem(false, baseObjectController.GetIsStorageItem());
        baseObjectController.SetIsNewItemSetted(true);

        // We set the new state for the edit panel buttons
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
        rotateObjLeftButton.SetActive(true);
        saveObjButton.SetActive(true);
        HideEditMenu();

        // We dont substract if the item is comming from the storage
        if (!baseObjectController.GetIsStorageItem() && dataGameObject == null)
        {
            PlayerData.Subtract(storeGameObject.Cost, storeGameObject.Type);
            // We set a new firebase object
            PlayerData.AddDataGameObject(this);
        }
        else
        {
            PlayerData.SubtractFromStorage(this);
            baseObjectController.SetStorage(false);
            isItemBought = true;
            dataGameObject.IS_STORED = false;
        }

        //SetAsCounter(); //If it is a counter we set if in the grid
        BussGrid.SetObjectObstacle(this);
        UpdateCoordsAndSetObstacle();
        SetInactive();
        HideUnderTiles();
        // Now it can be used by NPCs
        active = true;
    }

    private void DisableIfCounter()
    {
        if (Type == ObjectType.NPC_COUNTER && assignedTo != null)
        {
            assignedTo.SetCounter(null);
        }
    }

    public void CancelPurchase()
    {
        BussGrid.GetGameGridObjectsDictionary().Remove(Name, out GameGridObject tmp);
        if (tmp == null)
        {
            GameLog.Log("CancelPurchase() could not remove " + Name);
        }

        PlayerData.RemoveFromInventory(this);
        Object.Destroy(objectTransform.gameObject);
        DisableIfCounter();
        SetInactive();
        BussGrid.RecalculateBussGrid();
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetDataGameObject(DataGameObject obj)
    {
        dataGameObject = obj;
    }

    public ObjectRotation GetFacingPosition()
    {
        return facingPosition;
    }

    public LoadSliderController GetLoadItemSlider()
    {
        return loadSlider;
    }

    public LoadSliderController GetMoveSlider()
    {
        return loadSliderMove;
    }

    public override string ToString()
    {
        return "isItemBought: " + isItemBought + " active: " + active + " isItemReady: " + isItemReady + " isObjectSelected: " +
        isObjectSelected + " isObjectBeingDragged: " + isObjectBeingDragged;
    }

    public int GetSortingOrder()
    {
        return SortingLayer.sortingOrder;
    }

    public int CompareTo(GameGridObject obj2)
    {
        if (obj2 == null)
        {
            return 1;
        }
        else
        {
            return obj2.GetSortingOrder() - SortingLayer.sortingOrder;
        }
    }

    public bool Equals(GameGridObject obj2)
    {
        if (obj2 == null || SortingLayer == null) { return false; }
        return SortingLayer.sortingOrder == obj2.GetSortingOrder();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class GameGridObject : GameObjectBase
{
    private List<GameObject> actionTiles;
    private List<SpriteRenderer> tiles;
    private GameObject objectWithSprite;
    private SpriteResolver spriteResolver;
    private Transform objectTransform;
    private int actionTile;
    private StoreGameObject storeGameObject;
    private SpriteRenderer spriteRenderer;
    // private TopItemController topItemController;
    private ObjectRotation facingPosition; // Facing position
    private NPCController usedBy;
    private EmployeeController attendedBy;
    private GameObject editMenu;
    private bool isObjectBeingDragged;
    private bool hasNPCAssigned;
    private GameObject saveObjButton;
    private GameObject rotateObjLeftButton;
    private GameObject cancelButton;
    private GameObject acceptButton;

    // Controllers
    private BaseObjectController baseObjectController;

    // Sliders
    // Move Slider on top of the object
    private GameObject moveObjectSlider;
    private Slider moveSlider;
    // Load item slider
    private GameObject loadObjectSlider;
    private Slider loadSlider;
    // On top info popup
    private GameObject topInfoObject;

    // Load slider attributess
    private float moveSliderMultiplayer = Settings.ObjectMoveSliderMultiplayer;
    private float currentMoveSliderValue;
    private bool isObjectSelected;

    // Move slider attributess
    private float loadSliderMultiplayer = Settings.ItemLoadSliderMultiplayer;
    private float currentLoadSliderValue;
    private bool isItemReady;// item on top of the objects

    //Firebase obj
    private FirebaseGameObject firebaseGameObject;

    // Store - To be bought Item
    private bool isItemBought;

    // Is Item active, before purchase
    private bool active;

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
        hasNPCAssigned = false;
        isObjectSelected = false;
        isItemBought = true;

        // GameObject topObject = objectWithSprite.transform.Find(Settings.BaseObjectTopObject).gameObject;
        // topItemController = topObject.GetComponent<TopItemController>();
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
        moveObjectSlider = transform.Find("Slider/Slider").gameObject;
        moveSlider = moveObjectSlider.GetComponent<Slider>();
        moveObjectSlider.SetActive(false);

        //On top load slider
        loadObjectSlider = transform.Find("Slider/LoadItemSlider").gameObject;
        loadSlider = loadObjectSlider.GetComponent<Slider>();
        loadObjectSlider.SetActive(false);

        //On top Info popup
        topInfoObject = transform.Find("Slider/InfoPopUp").gameObject;
        topInfoObject.SetActive(false);

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
        firebaseGameObject = baseObjectController.GetFirebaseGameObject();
        // For setting the object sprite
        this.storeGameObject = storeGameObject;
        Type = storeGameObject.Type;
        spriteResolver = objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();
        spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);
        UpdateInitRotation(baseObjectController.GetInitialRotation());
        Init(); // StoreGameObject.Type requiredD
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

    // Store Item in inventory
    public void StoreInInventory()
    {
        try
        {
            GameLog.Log("TODO: UI message: Storing item in Inventory " + Name);
            firebaseGameObject.IS_STORED = true;
            PlayerData.StoreItem(this);
            // Clear the Item from the current selected in the grid 
            BussGrid.ClearCurrentClickedActiveGameObject();

            // we clean the table from the employer
            if (attendedBy != null)
            {
                attendedBy.SetTableToBeAttended(null);
            }

            // we clean the table from the client
            if (usedBy != null)
            {
                usedBy.GoToFinalState();
                usedBy = null;
            }

            BussGrid.GameController.ReCalculateNpcStates(this);
            BussGrid.CameraController.SetIsPerspectiveHandTempDisabled(false);
            BussGrid.SetDraggingObject(false);
            BussGrid.GetBusinessObjects().TryRemove(Name, out GameGridObject tmp);

            if (Type == ObjectType.NPC_SINGLE_TABLE)
            {
                BussGrid.GetBussQueueMap().TryRemove(this, out byte tmp2);
            }
            else if (Type == ObjectType.NPC_COUNTER)
            {
                BussGrid.SetCounter(null);
            }

            Object.Destroy(objectTransform.gameObject);
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (StoreInInventory GameGridObject): " + e);
        }
    }

    public bool IsLastPositionEqual(Vector3 actionGridPosition)
    {
        return Util.IsAtDistanceWithObject(GetActionTile(), actionGridPosition);
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
        if (firebaseGameObject != null)
        {
            firebaseGameObject.POSITION = new int[] { GridPosition.x, GridPosition.y };
        }
    }

    public void HideEditMenu()
    {
        HideUnderTiles();
        editMenu.SetActive(false);
    }

    public void ShowEditMenu()
    {
        spriteRenderer.color = Util.Available;
        editMenu.SetActive(true);
    }

    public void FreeObject()
    {
        usedBy = null;
        attendedBy = null;
        hasNPCAssigned = false;
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

        return new Vector3Int();
    }

    public void RotateObjectLeft()
    {
        // If there is any NPC we send it to the final state
        ResetNPCStates();
        FreeObject();

        //Vector3Int prev = GetActionTileInGridPosition();
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
            attendedBy.SetTableToBeAttended(null);
            attendedBy.RestartState();
            attendedBy = null;
        }

        if (usedBy != null)
        {
            usedBy.SetTable(null);
            usedBy.GoToFinalState();
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
        if (firebaseGameObject != null)
        {
            firebaseGameObject.ROTATION = (int)newPosition;
        }

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

    public SpriteResolver GetSpriteResolver()
    {
        return spriteResolver;
    }

    public List<GameObject> GetActionTileList()
    {
        return actionTiles;
    }

    public List<SpriteRenderer> GetTileList()
    {
        return tiles;
    }

    public bool IsFree()
    {
        return usedBy == null && attendedBy == null;
    }
    public bool HasClient()
    {
        return usedBy != null;
    }

    public bool HasNPCAssigned()
    {
        return hasNPCAssigned;

    }
    public void SetHashNPCAssigned(bool val)
    {
        hasNPCAssigned = val;
    }

    private void SetActiveMoveSlider(bool var)
    {
        moveObjectSlider.SetActive(var);
        moveSlider.value = 0;
    }

    public void UpdateSlider()
    {
        if (isObjectSelected)
        {
            return;
        }

        if (!moveObjectSlider.activeSelf)
        {
            SetActiveMoveSlider(true);
        }

        // EnergyBar controller, only if it is active
        if (moveObjectSlider.activeSelf)
        {
            if (currentMoveSliderValue <= 1)
            {
                currentMoveSliderValue += Time.fixedDeltaTime * moveSliderMultiplayer;
                moveSlider.value = currentMoveSliderValue;
            }
            else
            {
                SetObjectSelected();
            }
        }
    }

    // This loads the top Item
    public void UpdateLoadItemSlider()
    {
        if (!loadObjectSlider.activeSelf)
        {
            loadObjectSlider.SetActive(true);
            loadSlider.value = 0;
        }

        // EnergyBar controller, only if it is active
        if (loadObjectSlider.activeSelf)
        {
            if (currentLoadSliderValue <= 1)
            {
                currentLoadSliderValue += Time.fixedDeltaTime * moveSliderMultiplayer;
                loadSlider.value = currentLoadSliderValue;
            }
            else
            {
                SetItemsReady();
            }
        }
    }

    public void SetItemsReady()
    {
        isItemReady = true;
        topInfoObject.SetActive(true);
        loadObjectSlider.SetActive(false);
    }

    public bool GetIsItemReady()
    {
        return isItemReady;
    }

    public void DisableSlider()
    {
        currentMoveSliderValue = 0;
        moveSlider.value = 0;
        moveObjectSlider.SetActive(false);
    }

    private void SetObjectSelected()
    {
        // Disables dragging for a second 
        baseObjectController.DisableDisableDraggingTemp();
        isObjectSelected = true;
        SetActiveMoveSlider(false);
        BussGrid.SetActiveGameGridObject(this);
        baseObjectController.RestartTableNPC();
        //We clean grid position
        BussGrid.FreeObject(this);
    }

    public void SetInactive()
    {
        isObjectSelected = false;
        BussGrid.SetDraggingObject(false);
        BussGrid.SetIsDraggingEnable(false);
        BussGrid.HideHighlightedGridBussFloor();
    }

    public float GetCurrentMoveSliderValue()
    {
        return currentMoveSliderValue;
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
        BussGrid.SetActiveGameGridObject(this);
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
        if (!baseObjectController.GetIsStorageItem() && firebaseGameObject == null)
        {
            PlayerData.Subtract(storeGameObject.Cost);
            // We set a new firebase object
            PlayerData.AddFirebaseGameObject(this);
        }
        else
        {
            PlayerData.SubtractFromStorage(this);
            baseObjectController.SetStorage(false);
            isItemBought = true;
            firebaseGameObject.IS_STORED = false;
        }

        BussGrid.SetObjectObstacle(this);
        UpdateCoordsAndSetObstacle();
        SetInactive();
        HideUnderTiles();
        // Now it can be used by NPCs
        active = true;
    }

    public void CancelPurchase()
    {
        BussGrid.BusinessObjects.Remove(Name, out GameGridObject tmp);
        PlayerData.RemoveFromInventory(this);
        Object.Destroy(objectTransform.gameObject);
        // Disables the perspective hand for a second
        BussGrid.SetDisablePerspectiveHand();
        SetInactive();
        BussGrid.RecalculateBussGrid();
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetFirebaseGameObject(FirebaseGameObject obj)
    {
        firebaseGameObject = obj;
    }

    public FirebaseGameObject GetFirebaseGameObject()
    {
        return firebaseGameObject;
    }

    public ObjectRotation GetFacingPosition()
    {
        return facingPosition;
    }
}
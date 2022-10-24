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

    //Controllers
    private BaseObjectController baseObjectController;

    //Slider on top of the object
    private GameObject objectSlider;
    private Slider slider;
    private float sliderMultiplayer = 0.5f;
    private float currentSliderValue;
    private bool isObjectSelected;

    //Firebase obj
    private FirebaseGameObject firebaseGameObject;

    //Store - To be bought Item
    private bool isItemBought;

    //Is Item active, before purchase
    private bool active;


    public GameGridObject(Transform transform, StoreGameObject storeGameObject)
    {
        objectTransform = transform;
        Name = transform.name;
        WorldPosition = transform.position; // World position on Unity coords
        GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(transform.position); // Grid position, first position = 0, 0
        LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(transform.position);
        Type = storeGameObject.Type;
        this.storeGameObject = storeGameObject;
        objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
        spriteRenderer = objectWithSprite.GetComponent<SpriteRenderer>();
        SortingLayer = transform.GetComponent<SortingGroup>();
        SortingLayer.sortingOrder = Util.GetSorting(GridPosition);
        facingPosition = ObjectRotation.FRONT;
        hasNPCAssigned = false;
        isObjectSelected = false;
        isItemBought = true;

        GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
        Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
        Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
        SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
        SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

        spriteResolver = objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();
        spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);

        //Edit Panel Disable
        editMenu = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
        editMenu.SetActive(false);

        // On top slider
        objectSlider = transform.Find("Slider/Slider").gameObject;
        slider = objectSlider.GetComponent<Slider>();
        objectSlider.SetActive(false);

        // Set the table controller
        if (Type == ObjectType.NPC_SINGLE_TABLE)
        {
            baseObjectController = transform.GetComponent<TableController>();
        }
        else if (Type == ObjectType.NPC_COUNTER)
        {
            baseObjectController = transform.GetComponent<CounterController>();

        }
        else if (Type == ObjectType.SINGLE_CONTAINER)
        {
            baseObjectController = transform.GetComponent<BaseContainerController>();
        }

        actionTiles = new List<GameObject>(){
            objectActionTile.gameObject,
            objectSecondActionTile.gameObject
            };

        tiles = new List<SpriteRenderer>(){
            tileUnder,
            actionTileSpriteRenderer,
            secondActionTileSprite
            };

        firebaseGameObject = baseObjectController.GetFirebaseGameObject();
        active = baseObjectController.GetInitIsActive();
        UpdateInitRotation(baseObjectController.GetInitialRotation());
        SetEditPanelButtonClickListeners();
        Init();
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
        Hide();
    }

    private void SetEditPanelButtonClickListeners()
    {
        saveObjButton = editMenu.transform.Find(Settings.ConstEditStoreMenuSave).gameObject;
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

    //Store Item in inventory
    private void StoreInInventory()
    {
        try
        {
            GameLog.Log("TODO: UI message: Storing item in Inventory " + Name);
            firebaseGameObject.IS_STORED = true;
            PlayerData.StoreItem(this);
            BussGrid.ClearCurrentClickedActiveGameObject(); // Clear the Item from the current selected in the grid 
            BussGrid.FreeObject(this);

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

    //This is call everytime the object changes position
    public void UpdateCoords()
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

    public void FreeObject()
    {
        usedBy = null;
        attendedBy = null;
        hasNPCAssigned = false;
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

    public Vector3Int GetActionTileInGridPosition()
    {
        //Gets the orientation and then + 1 ...
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

    // public void RotateObjectRight()
    // {
    //     if (!IsValidRotation(1) && !GetStoreGameObject().HasActionPoint) //right
    //     {
    //         return;
    //     }

    //     ResetNPCStates();

    //     Vector3Int prev = GetActionTileInGridPosition();//BussGrid.GetPathFindingGridFromWorldPosition(GetActionTile());
    //     facingPosition++;

    //     if ((int)facingPosition >= 5)
    //     {
    //         facingPosition = ObjectRotation.FRONT;
    //     }

    //     UpdateRotation(facingPosition);

    //     if (GetStoreGameObject().HasActionPoint)
    //     {
    //         Vector3Int post = GetActionTileInGridPosition();//BussGrid.GetPathFindingGridFromWorldPosition(GetActionTile());
    //         BussGrid.SwapCoords(prev.x, prev.y, post.x, post.y);
    //     }

    //     UpdateCoords();
    // }

    public void RotateObjectLeft()
    {
        if (!IsValidRotation(0) && storeGameObject.HasActionPoint) //left
        {
            GameLog.Log("Rotation is invalid");
            return;
        }

        ResetNPCStates();

        Vector3Int prev = GetActionTileInGridPosition();
        facingPosition--;

        if ((int)facingPosition <= 0)
        {
            facingPosition = ObjectRotation.BACK_INVERTED;
        }

        UpdateRotation(facingPosition);

        if (storeGameObject.HasActionPoint)
        {
            Vector3Int post = GetActionTileInGridPosition();
            BussGrid.SwapCoords(prev.x, prev.y, post.x, post.y);
        }

        UpdateCoords();
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

        Vector3Int prev = GetActionTileInGridPosition();
        UpdateRotation(rotation);

        if (storeGameObject.HasActionPoint)
        {
            Vector3Int post = GetActionTileInGridPosition();
            BussGrid.SwapCoords(prev.x, prev.y, post.x, post.y);
        }
        UpdateCoords();
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

    private void SetActiveSlider(bool var)
    {
        objectSlider.SetActive(var);
        slider.value = 0;
    }

    public void UpdateSlider()
    {
        if (isObjectSelected)
        {
            return;
        }

        if (!objectSlider.activeSelf)
        {
            SetActiveSlider(true);
        }

        // EnergyBar controller, only if it is active
        if (objectSlider.activeSelf)
        {
            if (currentSliderValue <= 1)
            {
                currentSliderValue += Time.fixedDeltaTime * sliderMultiplayer;
                slider.value = currentSliderValue;
            }
            else
            {
                SetActive();
            }
        }
    }

    public void DisableSlider()
    {
        currentSliderValue = 0;
        slider.value = 0;
        objectSlider.SetActive(false);
    }

    private void SetActive()
    {
        isObjectSelected = true;
        SetActiveSlider(false);
        BussGrid.SetActiveGameGridObject(this);
        baseObjectController.RestartTableNPC();
    }

    public void SetInactive()
    {
        isObjectSelected = false;
        BussGrid.SetDraggingObject(false);
        BussGrid.SetIsDraggingEnable(false);
        BussGrid.HideHighlightedGridBussFloor();
    }

    public float GetCurrentSliderValue()
    {
        return currentSliderValue;
    }

    public bool GetIsObjectSelected()
    {
        return isObjectSelected;
    }

    public void SetNewObjectActive()
    {
        acceptButton.SetActive(true);
        cancelButton.SetActive(true);
        rotateObjLeftButton.SetActive(true);
        saveObjButton.SetActive(false);
    }
    public void SetStoreObject()
    {
        //We show accept, cancel buttons and select the object
        isObjectSelected = true;
        isItemBought = false;
        BussGrid.SetActiveGameGridObject(this);
        SetNewObjectActive();
    }

    // Used in the case the player cancels or clicks outside the object
    public bool GetIsItemBought()
    {
        return isItemBought;
    }

    private void AcceptPurchase()
    {
        isItemBought = true;
        PlayerData.Subtract(storeGameObject.Cost);
        active = true; // now it can be used by NPCs
        //we set a new firebase object
        PlayerData.AddFirebaseGameObject(this);
        SetInactive();
    }

    public void CancelPurchase()
    {
        BussGrid.BusinessObjects.Remove(Name, out GameGridObject tmp);
        PlayerData.RemoveFromInventory(this);
        Object.Destroy(objectTransform.gameObject);
        BussGrid.SetDisablePerspectiveHand(); // disables the perspective hand for a second
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

    public ObjectRotation GetFacingPosition()
    {
        return facingPosition;
    }
}
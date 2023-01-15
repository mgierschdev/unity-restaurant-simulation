using System;
using System.Collections;
using UnityEngine;

// Here we control the object drag and drop and the state of the NPCs during the drag
public class BaseObjectController : MonoBehaviour
{
    [SerializeField]
    private Vector3 currentPos; // Current position of the object including while dragging
    protected GameGridObject gameGridObject;
    protected MenuHandlerController Menu { get; set; }
    //Long click controller
    private float timeClicking;
    //Firebase obj reference and initial rotation
    private DataGameObject dataGameObject;
    private ObjectRotation initialRotation;
    private StoreGameObject storeGameObject;
    private bool isCurrentValidPos, isNewItem, isStorageItem, isNewItemSetted, isSpriteSetted, isDraggDisabled, isClicking;
    private ClickController clickController;

    private void Awake()
    {
        isNewItem = false;
        isNewItemSetted = false;
        isSpriteSetted = false;
        isCurrentValidPos = true;
        isDraggDisabled = false;
        isClicking = false;
        timeClicking = 0;
        initialRotation = ObjectRotation.FRONT;
        currentPos = transform.position;
        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
        clickController = cController.GetComponent<ClickController>();
    }

    private void Start()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu);
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        gameGridObject = new GameGridObject(transform);
    }

    private void Update()
    {
        try
        {
            if (gameGridObject == null && storeGameObject != null)
            {
                return;
            }

            UpdateInit(); // Init constructor
            UpdateFirstTimeSetupStoreItem(); // Constructor for bought items
            UpdateMoveSelectionSlider(); // Checks for long pressed over the object and updates the slider
            UpdateTopItemStoreSlider(); // Loads the item 
            UpdateIsValidPosition(); // Checks if the current position is a valid one 
            UpdateOnMouseDown();// OnMouseDown implementation which takes into consideration the layer ordering
        }
        catch (Exception e)
        {
            GameLog.LogError("Exception thrown, BaseObjectController/Update(): " + e);
        }
    }

    private void UpdateOnMouseDown()
    {
        // UPGRADE: UPGRADE_AUTO_LOAD
        if (IsItemLoadEnableAndValid() && gameGridObject.GetStoreGameObject().GetIdentifierNumber() <= PlayerData.GetUgrade(UpgradeType.UPGRADE_AUTO_LOAD))
        {
            SetLoadSliderActive();
        }

        if (Menu.IsMenuOpen())
        {
            return;
        }

        // Loads the item once is clicked
        if (Input.GetMouseButtonDown(0) && clickController.GetGameGridClickedObject() == gameGridObject)
        {
            isClicking = true;

            if (IsItemLoadEnableAndValid())
            {
                SetLoadSliderActive();
            }
        }

        if (isClicking)
        {
            timeClicking += Time.unscaledDeltaTime;
        }

        // On release, the mouse
        if (Input.GetMouseButtonUp(0))
        {
            timeClicking = 0;
            isClicking = false;
        }
    }

    private bool IsItemLoadEnableAndValid()
    {
        return
            gameGridObject != null &&
            gameGridObject.GetStoreGameObject().Type == ObjectType.STORE_ITEM &&
            !gameGridObject.GetMoveSlider().IsActive() && // move slider should not be active
            !gameGridObject.GetIsItemReady() &&
            !gameGridObject.GetIsObjectSelected() &&
            !ObjectDraggingHandler.GetIsDraggingEnabled() &&
            !gameGridObject.GetLoadItemSlider().IsActive();
    }

    private void UpdateInit()
    {
        if (storeGameObject != null && !isSpriteSetted)
        {
            gameGridObject.SetStoreGameObject(storeGameObject);
            isSpriteSetted = true;
            PlayerData.AddItemToInventory(gameGridObject);
            gameGridObject.UpdateObjectCoords();

            // We set the coords on the Buss grid only if it is not an item from the store/storage
            if (!isNewItem && !isStorageItem)
            {
                BussGrid.SetObjectObstacle(gameGridObject);
            }
        }
    }

    private void UpdateTopItemStoreSlider()
    {
        if (gameGridObject.GetLoadItemSlider() &&
        !gameGridObject.GetIsObjectSelected()
        )
        {
            gameGridObject.UpdateLoadItemSlider();
        }
    }

    private void UpdateMoveSelectionSlider()
    {
        if (Menu.IsMenuOpen())
        {
            return;
        }

        // Selecting the item while pressing over it
        if (timeClicking > Settings.TimeBeforeTheSliderIsEnabled &&
        !gameGridObject.GetIsObjectSelected() &&
        !ObjectDraggingHandler.GetIsDraggingEnabled())
        {
            gameGridObject.UpdateMoveSlider();

            if (gameGridObject.GetLoadItemSlider().IsActive() || gameGridObject.GetIsItemReady())
            {
                gameGridObject.DiableTopInfoObject();
            }
        }
        else
        {
            if (gameGridObject.GetMoveSlider().IsActive())
            {
                gameGridObject.GetMoveSlider().SetInactive();
            }
        }

        // Unselecting the item
        // If the item is selected and the user is clicking outside we unselect the item
        if (Input.GetMouseButtonDown(0) &&
        gameGridObject.GetIsObjectSelected() &&
        !IsClickingSelf() &&
        !IsClickingButton())
        {
            gameGridObject.SetInactive();
            ObjectDraggingHandler.SetIsDraggingEnable(false);

            // If it is a store item not bought we erase it 
            if (!gameGridObject.GetIsItemBought() && !PlayerData.IsItemStored(gameGridObject.Name))
            {
                gameGridObject.CancelPurchase();
            }
            else if (!isCurrentValidPos) // If the item is bought and not valid position
            {
                gameGridObject.StoreInInventory();
            }
            else // If the item is bought and the current position is valid 
            {
                gameGridObject.AcceptPurchase();
            }
            // recalculates Goto when the object is placed
            BussGrid.GameController.ReCalculateNpcStates(gameGridObject);
        }
    }

    private void UpdateFirstTimeSetupStoreItem()
    {
        //First time setup for a store item
        if (!isNewItemSetted && isNewItem)
        {
            SetNewGameGridObject();
        }
    }

    private void UpdateIsValidPosition()
    {
        if (!gameGridObject.GetIsObjectSelected())
        {
            return;
        }

        if (BussGrid.IsValidBussPosition(gameGridObject))
        {
            isCurrentValidPos = true;
            gameGridObject.GetSpriteRenderer().color = Util.AvailableColor;
        }
        else
        {
            isCurrentValidPos = false;
            gameGridObject.GetSpriteRenderer().color = Util.OccupiedColor;
        }
    }

    private void SetLoadSliderActive()
    {
        IEnumerator coroutine = EnableSlider();
        StartCoroutine(coroutine);
    }

    private IEnumerator EnableSlider()
    {
        yield return new WaitForSeconds(Settings.TimeBeforeTheSliderIsEnabled);
        gameGridObject.GetLoadItemSlider().SetActive();
    }

    // Called when dragging the object
    private void OnMouseDrag()
    {
        if (!Menu || gameGridObject == null ||
        !gameGridObject.GetIsObjectSelected() ||
        IsClickingButton() ||
        isDraggDisabled)
        {
            return;
        }

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        currentPos = BussGrid.GetMouseOnGameGridWorldPosition();
        transform.position = new Vector3(currentPos.x, currentPos.y, Util.SelectedObjectZPosition);
        gameGridObject.UpdateObjectCoords();
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        if (!Menu || !ObjectDraggingHandler.IsDraggingEnabled(gameGridObject))
        {
            return;
        }

        gameGridObject.UpdateObjectCoords();

        // Re-evaluate all the objects currently in the grid in case of the Unity OnMouseUp failling to update
        // or updating in an inconsistent way
        BussGrid.RecalculateBussGrid();
    }

    // If overlaps with any UI button
    private bool IsClickingButton()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(Settings.Button))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsClickingSelf()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(name))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsDraggable()
    {
        if (!Menu || gameGridObject == null || !ObjectDraggingHandler.IsDraggingEnabled(gameGridObject))
        {
            return false;
        }
        // If overlaps with any UI button 
        return gameGridObject.Type != ObjectType.UNDEFINED && !IsClickingButton();
    }

    public void SetNewGameGridObject()
    {
        isNewItemSetted = true;
        gameGridObject.SetStoreObject();
    }

    public void SetNewItem(bool val, bool storage)
    {
        isNewItem = val;
        isStorageItem = storage; // is the item comming from storage
    }

    // returns if the item is comming from the storage
    public bool GetIsStorageItem()
    {
        return isStorageItem;
    }

    public void SetStorage(bool val)
    {
        isStorageItem = val;
    }

    public void SetIsNewItemSetted(bool val)
    {
        isNewItemSetted = val;
    }

    public void SetDataGameObjectAndInitRotation(DataGameObject obj)
    {
        dataGameObject = obj;
        initialRotation = (ObjectRotation)obj.ROTATION; ;
    }

    public void SetInitialObjectRotation(ObjectRotation rotation)
    {
        initialRotation = rotation;
    }

    public ObjectRotation GetInitialRotation()
    {
        return initialRotation;
    }

    public DataGameObject GetDataGameObject()
    {
        return dataGameObject;
    }

    // If it is not from the store it is an active item
    public bool GetInitIsActive()
    {
        return !isNewItem;
    }

    public void SetStoreGameObject(StoreGameObject storeGameObject)
    {
        this.storeGameObject = storeGameObject;
    }

    public void SetStoreGameObject(DataGameObject storeGameObject)
    {
        this.dataGameObject = storeGameObject;
    }

    public bool GetIscurrentValidPos()
    {
        return isCurrentValidPos;
    }

    public void DisableDisableDraggingTemp()
    {
        isDraggDisabled = true;
        IEnumerator coroutine = DisableDraggingTemp();
        StartCoroutine(coroutine);
    }

    // It disables drag for a small period of time
    private IEnumerator DisableDraggingTemp()
    {
        yield return new WaitForSeconds(0.5f);
        isDraggDisabled = false;
    }

    public GameGridObject GetGameGridObject()
    {
        return gameGridObject;
    }

    public void SetRandomColor()
    {
        gameGridObject.GetSpriteRenderer().color = Util.GetRandomColor();
    }
}
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
    private const float TIME_BEFORE_ACTIVATING_SLIDER = Settings.TimeBeforeTheSliderIsEnabled;
    //Firebase obj reference and initial rotation
    private FirebaseGameObject firebaseGameObject;
    private ObjectRotation initialRotation;
    private StoreGameObject storeGameObject;
    private bool isCurrentValidPos, isNewItem, isStorageItem, isNewItemSetted, isSpriteSetted, isDraggDisabled, isLoadingItemSlider;

    private void Awake()
    {
        isNewItem = false;
        isNewItemSetted = false;
        isSpriteSetted = false;
        isCurrentValidPos = true;
        isDraggDisabled = false;
        isLoadingItemSlider = false;
        timeClicking = 0;
        initialRotation = ObjectRotation.FRONT;
        currentPos = transform.position;
    }

    private void Start()
    {
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
        Menu = menuHandler.GetComponent<MenuHandlerController>();
        gameGridObject = new GameGridObject(transform);
    }

    private void Update()
    {
        if (gameGridObject == null && storeGameObject != null)
        {
            return;
        }

        UpdateInit(); // Init constructor
        UpdateFirstTimeSetupStoreItem(); // Constructor for bought items
        UpdateSelectionSlider(); // Checks for long pressed over the object and updates the slider
        UpdateTopItemSlider(); // Checks for long pressed over the object and updates the slider
        UpdateIsValidPosition(); // Checks if the current position is a valid one 
    }

    private void UpdateInit()
    {
        if (!isSpriteSetted)
        {
            gameGridObject.SetStoreGameObject(storeGameObject);
            isSpriteSetted = true;
            BussGrid.SetGridObject(gameGridObject);
            gameGridObject.UpdateObjectCoords();

            // We set the coords on the Buss grid only if it is not an item from the store/storage
            if (!isNewItem && !isStorageItem)
            {
                BussGrid.SetObjectObstacle(gameGridObject);
            }
        }
    }

    private void UpdateTopItemSlider()
    {
        //TODO: Check that is not a long press
        if (isLoadingItemSlider &&
        !gameGridObject.GetIsObjectSelected() &&
        timeClicking < 0.1f
        )
        {
            gameGridObject.UpdateLoadItemSlider();
        }
    }

    private void UpdateSelectionSlider()
    {
        if (timeClicking > TIME_BEFORE_ACTIVATING_SLIDER && !gameGridObject.GetIsObjectSelected())
        {
            gameGridObject.UpdateMoveSlider();
            if (gameGridObject.GetIsItemLoading() || gameGridObject.GetIsItemReady())
            {
                gameGridObject.DiableTopInfoObject();
            }
        }

        // If the item is selected and the user is clicking outside we un-select the item
        if (Input.GetMouseButtonDown(0) &&
        gameGridObject.GetIsObjectSelected() &&
        !IsClickingSelf() &&
        !IsClickingButton())
        {
            isLoadingItemSlider = false;
            gameGridObject.SetInactive();

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

            BussGrid.SetDisablePerspectiveHand(); // We disable perspective hand for a second
        }
    }

    private void UpdateFirstTimeSetupStoreItem()
    {
        //First time settup for a store item
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
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            isCurrentValidPos = false;
            gameGridObject.LightOccupiedUnderTiles();
            gameGridObject.GetSpriteRenderer().color = Util.Occupied;
        }
    }

    private void OnMouseDown()
    {
        if (gameGridObject != null &&
        !gameGridObject.GetStoreGameObject().HasActionPoint &&
        !gameGridObject.GetIsItemReady())
        {
            isLoadingItemSlider = true;
        }
    }

    // Restart the state of the npcs in case that there is any
    public void RestartTableNPC()
    {
        // If you move a table while busy the NPC will self destroy
        if (gameGridObject.GetStoreGameObject().HasActionPoint)
        {
            // Restart NPCs states
            NPCController npc = gameGridObject.GetUsedBy();
            EmployeeController employee = gameGridObject.GetAttendedBy();

            if (npc != null)
            {
                npc.SetTableMoved();
            }

            if (employee != null)
            {
                if (gameGridObject.Type == ObjectType.NPC_COUNTER)
                {
                    employee.SetUnrespawn();
                }
                else
                {
                    employee.SetTableMoved();
                }
                //employee.RecalculateState(gameGridObject);
            }

            gameGridObject.FreeObject(); // So it will be removed while dragging   
        }
    }

    private void OnMouseDrag()
    {
        timeClicking += Time.unscaledDeltaTime;

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
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);

        // So it will overlay over the rest of the items while dragging
        Vector3Int currentGridPosition = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(currentGridPosition);
        gameGridObject.UpdateObjectCoords();
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        timeClicking = 0;
        if (gameGridObject.GetCurrentMoveSliderValue() > 0)
        {
            //we disable the slider
            gameGridObject.DisableMoveSlider();
        }

        if (!Menu || !BussGrid.IsDraggingEnabled(gameGridObject))
        {
            return;
        }

        gameGridObject.UpdateObjectCoords();
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(gameGridObject.GridPosition);

        //We recalculate Paths once the object is placed
        BussGrid.GameController.ReCalculateNpcStates(gameGridObject);

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
        if (!Menu || gameGridObject == null || !BussGrid.IsDraggingEnabled(gameGridObject))
        {
            return false;
        }
        // If overlaps with any UI button 
        return gameGridObject.Type != ObjectType.UNDEFINED && !IsClickingButton();
    }

    // private bool IsOverNPC()
    // {
    //     Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
    //     foreach (Collider2D r in hits)
    //     {
    //         if (r.name.Contains(Settings.PrefabNpcClient) || r.name.Contains(Settings.PrefabNpcEmployee))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    public void SetNewGameGridObject()
    {
        isNewItemSetted = true;
        gameGridObject.SetStoreObject();
    }

    public void SetNewItem(bool val, bool storage)
    {
        isNewItem = val;
        this.isStorageItem = storage; // is the item comming from storage
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

    public void SetFirebaseGameObjectAndInitRotation(FirebaseGameObject obj)
    {
        firebaseGameObject = obj;
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

    public FirebaseGameObject GetFirebaseGameObject()
    {
        return firebaseGameObject;
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

    public void SetFirebaseGameObject(FirebaseGameObject firebaseGameObject)
    {
        this.firebaseGameObject = firebaseGameObject;
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
}
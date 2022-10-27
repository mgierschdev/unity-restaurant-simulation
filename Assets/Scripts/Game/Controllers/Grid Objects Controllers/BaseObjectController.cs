using UnityEngine;

// Here we control the object drag and drop and the state of the NPCs during the drag
public class BaseObjectController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3Int initialGridPosition;
    [SerializeField]
    private Vector3 currentPos; // Current position of the object including while dragging
    private bool iscurrentValidPos; // Is the current position valid for the object including while dragging
    // Initial object position
    [SerializeField]
    private Vector3Int initialActionTileOne;
    [SerializeField]
    protected GameGridObject gameGridObject;
    // protected ObjectRotation InitialObjectRotation;
    protected MenuHandlerController Menu { get; set; }
    //Long click controller
    private float timeClicking;
    private const float TIME_BEFORE_ACTIVATING_SLIDER = Settings.TimeBeforeTheSliderIsEnabled;
    //Firebase obj reference and initial rotation
    private FirebaseGameObject firebaseGameObject;
    private ObjectRotation initialRotation;
    private StoreGameObject storeGameObject;
    // New item (not yet bought)
    // isNewItem: New item added through the store
    private bool isNewItem;
    private bool storage;
    // isNewItemSetted: New item config setted
    private bool isNewItemSetted;
    // isSprite seted 
    private bool isSpriteSetted;

    private void Awake()
    {
        isNewItem = false;
        isNewItemSetted = false;
        isSpriteSetted = false;
        timeClicking = 0;
        initialRotation = ObjectRotation.FRONT;
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

        if (!isSpriteSetted)
        {
            gameGridObject.SetStoreGameObject(storeGameObject);
            BussGrid.SetGridObject(gameGridObject);
            isSpriteSetted = true;
        }

        if (timeClicking > TIME_BEFORE_ACTIVATING_SLIDER && !gameGridObject.GetIsObjectSelected())
        {
            gameGridObject.UpdateSlider();
        }

        // If the item is selected and the user is clicking outside we un-select the item
        if (Input.GetMouseButtonDown(0) && gameGridObject.GetIsObjectSelected() && !IsClickingSelf() && !IsClickingButton())
        {
            gameGridObject.SetInactive();

            //if it is a store item not bought we erase it 
            if (!gameGridObject.GetIsItemBought())
            {
                gameGridObject.CancelPurchase();
            }
            else
            {
                BussGrid.SetDisablePerspectiveHand(); //We disable perspective hand for a second
            }
        }

        //First time settup for a store item
        if (!isNewItemSetted && isNewItem)
        {
            SetNewGameGridObject();
        }
    }

    private void OnMouseDown()
    {
        initialActionTileOne = gameGridObject.GetActionTileInGridPosition();
        initialGridPosition = gameGridObject.GridPosition;
        initialPosition = transform.position;
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
                npc.GoToFinalState();
            }

            if (employee != null)
            {
                employee.RecalculateState(gameGridObject);
            }

            gameGridObject.FreeObject(); // So it will be removed while dragging   
        }
    }

    private void OnMouseDrag()
    {
        timeClicking += Time.unscaledDeltaTime;

        if (!Menu || !IsDraggable())
        {
            return;
        }

        // Change Overlay color depending if can place or not
        // Mark 2 tiles of the object action tile and position tile
        currentPos = BussGrid.GetGridWorldPositionMapMouseDrag(Util.GetMouseInWorldPosition());
        transform.position = new Vector3(currentPos.x, currentPos.y, 1);

        // So it will overlay over the rest of the items while dragging
        Vector3Int currentGridPosition = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(currentGridPosition);

        if (BussGrid.IsValidBussPosition(gameGridObject, currentPos) && !IsOverNPC())
        {
            iscurrentValidPos = true;
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }
        else
        {
            iscurrentValidPos = false;
            gameGridObject.LightOccupiedUnderTiles();
            gameGridObject.GetSpriteRenderer().color = Util.Occupied;
        }

        // If dragging clean previous position on the grid
        BussGrid.FreeCoordWhileDragging(initialGridPosition, initialActionTileOne, gameGridObject);
    }

    // Called when the mouse is released 
    private void OnMouseUp()
    {
        timeClicking = 0;
        if (gameGridObject.GetCurrentSliderValue() > 0)
        {
            //we disable the slider
            gameGridObject.DisableSlider();
        }

        if (!Menu || !BussGrid.IsDraggingEnabled(gameGridObject))
        {
            return;
        }

        if (iscurrentValidPos)
        {
            initialPosition = currentPos;
        }
        else
        {
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 0);
            gameGridObject.GetSpriteRenderer().color = Util.Available;
            gameGridObject.LightAvailableUnderTiles();
        }

        gameGridObject.UpdateCoords();
        gameGridObject.SortingLayer.sortingOrder = Util.GetSorting(gameGridObject.GridPosition);

        //We recalculate Paths once the object is placed
        BussGrid.GameController.ReCalculateNpcStates(gameGridObject);

        //if it was a table we re-add it to the freeBussList
        if (gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE)
        {
            gameGridObject.SetIsObjectBeingDragged(false);
        }

        // Re-evaluate all the objects currently in the grid in case of the Unity OnMouseUp failling to update
        // or updating in an inconsistent way
        BussGrid.RecalculateBussGrid();
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

    private bool IsOverNPC()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(Settings.PrefabNpcClient) || r.name.Contains(Settings.PrefabNpcEmployee))
            {
                return true;
            }
        }
        return false;
    }

    public void SetNewGameGridObject()
    {
        isNewItemSetted = true;
        gameGridObject.SetStoreObject();
    }

    public void SetNewItem(bool val, bool storage)
    {
        isNewItem = val;
        this.storage = storage; // is the item comming from storage
    }
    // returns if the item is comming from the storage
    public bool GetStorage()
    {
        return storage;
    }
    
    public void SetIsNewItemSetted(bool val)
    {
        isNewItemSetted = val;
    }

    public void SetFirebaseGameObject(FirebaseGameObject obj)
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
}
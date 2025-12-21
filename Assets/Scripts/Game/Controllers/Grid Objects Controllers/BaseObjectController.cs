using System;
using System.Collections;
using Game.Controllers.Menu_Controllers;
using Game.Controllers.Other_Controllers;
using Game.Grid;
using Game.Players;
using Game.Players.Model;
using UnityEngine;
using Util;

// Here we control the object drag and drop and the state of the NPCs during the drag
namespace Game.Controllers.Grid_Objects_Controllers
{
    /**
     * Problem: Control dragging, selection, and placement of grid objects.
     * Goal: Handle user interactions and synchronize grid state.
     * Approach: Track click/drag state and update GameGridObject behavior.
     * Time: O(1) per frame plus grid checks.
     * Space: O(1).
     */
    public class BaseObjectController : MonoBehaviour
    {
        [SerializeField] private Vector3 currentPos; // Current position of the object including while dragging
        private GameGridObject _gameGridObject;
        private MenuHandlerController _menu;
        private float _timeClicking; //Long click controller
        private DataGameObject _dataGameObject;
        private ObjectRotation _initialRotation;
        private StoreGameObject _storeGameObject;

        private bool _isCurrentValidPos,
            _isNewItem,
            _isStorageItem,
            _isNewItemSet,
            _isSpriteSet,
            _isDragDisabled,
            _isClicking;

        private ClickController _clickController;

        private void Awake()
        {
            _isNewItem = false;
            _isNewItemSet = false;
            _isSpriteSet = false;
            _isCurrentValidPos = true;
            _isDragDisabled = false;
            _isClicking = false;
            _timeClicking = 0;
            _initialRotation = ObjectRotation.Front;
            currentPos = transform.position;
            // Click controller
            GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
            _clickController = cController.GetComponent<ClickController>();
        }

        private void Start()
        {
            GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu);
            Util.Util.IsNull(menuHandler, "BaseObjectController/MenuHandlerController null");
            _menu = menuHandler.GetComponent<MenuHandlerController>();
            _gameGridObject = new GameGridObject(transform);
        }

        private void Update()
        {
            try
            {
                if (_gameGridObject == null && _storeGameObject != null)
                {
                    return;
                }

                UpdateInit(); // Init constructor
                UpdateFirstTimeSetupStoreItem(); // Constructor for bought items
                UpdateMoveSelectionSlider(); // Checks for long pressed over the object and updates the slider
                UpdateTopItemStoreSlider(); // Loads the item 
                UpdateIsValidPosition(); // Checks if the current position is a valid one 
                UpdateOnMouseDown(); // OnMouseDown implementation which takes into consideration the layer ordering
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, BaseObjectController/Update(): " + e);
            }
        }

        private void UpdateOnMouseDown()
        {
            // UPGRADE: UPGRADE_AUTO_LOAD
            if (IsItemLoadEnableAndValid() && _gameGridObject.GetStoreGameObject().GetIdentifierNumber() <=
                PlayerData.GetUgrade(UpgradeType.UpgradeAutoLoad))
            {
                SetLoadSliderActive();
            }

            if (_menu.IsMenuOpen())
            {
                return;
            }

            // Loads the item once is clicked
            if (Input.GetMouseButtonDown(0) && _clickController.GetGameGridClickedObject() == _gameGridObject)
            {
                _isClicking = true;

                if (IsItemLoadEnableAndValid())
                {
                    SetLoadSliderActive();
                }
            }

            if (_isClicking)
            {
                _timeClicking += Time.unscaledDeltaTime;
            }

            // On release, the mouse
            if (Input.GetMouseButtonUp(0))
            {
                _timeClicking = 0;
                _isClicking = false;
            }
        }

        private bool IsItemLoadEnableAndValid()
        {
            return
                _gameGridObject != null &&
                _gameGridObject.GetStoreGameObject().Type == ObjectType.StoreItem &&
                !_gameGridObject.GetMoveSlider().IsActive() && // move slider should not be active
                !_gameGridObject.GetIsItemReady() &&
                !_gameGridObject.GetIsObjectSelected() &&
                !ObjectDraggingHandler.GetIsDraggingEnabled() &&
                !_gameGridObject.GetLoadItemSlider().IsActive();
        }

        private void UpdateInit()
        {
            if (_storeGameObject != null && !_isSpriteSet)
            {
                _gameGridObject.SetStoreGameObject(_storeGameObject);
                _isSpriteSet = true;
                PlayerData.AddItemToInventory(_gameGridObject);
                _gameGridObject.UpdateObjectCoords();

                // We set the coords on the Buss grid only if it is not an item from the store/storage
                if (!_isNewItem && !_isStorageItem)
                {
                    BussGrid.SetObjectObstacle(_gameGridObject);
                }
            }
        }

        private void UpdateTopItemStoreSlider()
        {
            if (_gameGridObject.GetLoadItemSlider() &&
                !_gameGridObject.GetIsObjectSelected()
               )
            {
                _gameGridObject.UpdateLoadItemSlider();
            }
        }

        private void UpdateMoveSelectionSlider()
        {
            if (_menu.IsMenuOpen())
            {
                return;
            }

            // Selecting the item while pressing over it
            if (_timeClicking > Settings.TimeBeforeTheSliderIsEnabled &&
                !_gameGridObject.GetIsObjectSelected() &&
                !ObjectDraggingHandler.GetIsDraggingEnabled())
            {
                _gameGridObject.UpdateMoveSlider();

                if (_gameGridObject.GetLoadItemSlider().IsActive() || _gameGridObject.GetIsItemReady())
                {
                    _gameGridObject.DiableTopInfoObject();
                }
            }
            else
            {
                if (_gameGridObject.GetMoveSlider().IsActive())
                {
                    _gameGridObject.GetMoveSlider().SetInactive();
                }
            }

            // Unselecting the item
            // If the item is selected and the user is clicking outside we unselect the item
            if (Input.GetMouseButtonDown(0) &&
                _gameGridObject.GetIsObjectSelected() &&
                !IsClickingSelf() &&
                !IsClickingButton())
            {
                _gameGridObject.SetInactive();
                ObjectDraggingHandler.SetIsDraggingEnable(false);

                // If it is a store item not bought we erase it 
                if (!_gameGridObject.GetIsItemBought() && !PlayerData.IsItemStored(_gameGridObject.Name))
                {
                    _gameGridObject.CancelPurchase();
                }
                else if (!_isCurrentValidPos) // If the item is bought and not valid position
                {
                    _gameGridObject.StoreInInventory();
                }
                else // If the item is bought and the current position is valid 
                {
                    _gameGridObject.AcceptPurchase();
                }

                // recalculates Goto when the object is placed
                BussGrid.GameController.ReCalculateNpcStates(_gameGridObject);
            }
        }

        private void UpdateFirstTimeSetupStoreItem()
        {
            //First time setup for a store item
            if (!_isNewItemSet && _isNewItem)
            {
                SetNewGameGridObject();
            }
        }

        private void UpdateIsValidPosition()
        {
            if (!_gameGridObject.GetIsObjectSelected())
            {
                return;
            }

            if (BussGrid.IsValidBussPosition(_gameGridObject))
            {
                _isCurrentValidPos = true;
                _gameGridObject.GetSpriteRenderer().color = Util.Util.AvailableColor;
            }
            else
            {
                _isCurrentValidPos = false;
                _gameGridObject.GetSpriteRenderer().color = Util.Util.OccupiedColor;
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
            _gameGridObject.GetLoadItemSlider().SetActive();
        }

        // Called when dragging the object
        private void OnMouseDrag()
        {
            if (!_menu || _gameGridObject == null ||
                !_gameGridObject.GetIsObjectSelected() ||
                IsClickingButton() ||
                _isDragDisabled)
            {
                return;
            }

            // Change Overlay color depending if can place or not
            // Mark 2 tiles of the object action tile and position tile
            currentPos = BussGrid.GetMouseOnGameGridWorldPosition();
            transform.position = new Vector3(currentPos.x, currentPos.y, Util.Util.SelectedObjectZPosition);
            _gameGridObject.UpdateObjectCoords();
        }

        // Called when the mouse is released 
        private void OnMouseUp()
        {
            if (!_menu || !ObjectDraggingHandler.IsDraggingEnabled(_gameGridObject))
            {
                return;
            }

            _gameGridObject.UpdateObjectCoords();

            // Re-evaluate all the objects currently in the grid in case of the Unity OnMouseUp failling to update
            // or updating in an inconsistent way
            BussGrid.RecalculateBussGrid();
        }

        // If overlaps with any UI button
        private static bool IsClickingButton()
        {
            var hits = Physics2D.OverlapPointAll(Util.Util.GetMouseInWorldPosition());
            foreach (var r in hits)
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
            var hits = Physics2D.OverlapPointAll(Util.Util.GetMouseInWorldPosition());

            if (hits == null)
            {
                throw new ArgumentException(nameof(hits));
            }
            
            foreach (var r in hits)
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
            if (!_menu || _gameGridObject == null || !ObjectDraggingHandler.IsDraggingEnabled(_gameGridObject))
            {
                return false;
            }

            // If overlaps with any UI button 
            return _gameGridObject.Type != ObjectType.Undefined && !IsClickingButton();
        }

        public void SetNewGameGridObject()
        {
            _isNewItemSet = true;
            _gameGridObject.SetStoreObject();
        }

        public void SetNewItem(bool val, bool storage)
        {
            _isNewItem = val;
            _isStorageItem = storage; // is the item comming from storage
        }

        // returns if the item is comming from the storage
        public bool GetIsStorageItem()
        {
            return _isStorageItem;
        }

        public void SetStorage(bool val)
        {
            _isStorageItem = val;
        }

        public void SetIsNewItemSetted(bool val)
        {
            _isNewItemSet = val;
        }

        public void SetDataGameObjectAndInitRotation(DataGameObject obj)
        {
            _dataGameObject = obj;
            _initialRotation = (ObjectRotation)obj.rotation;
        }

        public void SetInitialObjectRotation(ObjectRotation rotation)
        {
            _initialRotation = rotation;
        }

        public ObjectRotation GetInitialRotation()
        {
            return _initialRotation;
        }

        public DataGameObject GetDataGameObject()
        {
            return _dataGameObject;
        }

        // If it is not from the store it is an active item
        public bool GetInitIsActive()
        {
            return !_isNewItem;
        }

        public void SetStoreGameObject(StoreGameObject storeGameObject)
        {
            this._storeGameObject = storeGameObject;
        }

        public void SetStoreGameObject(DataGameObject storeGameObject)
        {
            this._dataGameObject = storeGameObject;
        }

        public bool GetIscurrentValidPos()
        {
            return _isCurrentValidPos;
        }

        public void DisableDisableDraggingTemp()
        {
            _isDragDisabled = true;
            IEnumerator coroutine = DisableDraggingTemp();
            StartCoroutine(coroutine);
        }

        // It disables drag for a small period of time
        private IEnumerator DisableDraggingTemp()
        {
            yield return new WaitForSeconds(0.5f);
            _isDragDisabled = false;
        }

        public GameGridObject GetGameGridObject()
        {
            return _gameGridObject;
        }

        public void SetRandomColor()
        {
            _gameGridObject.GetSpriteRenderer().color = Util.Util.GetRandomColor();
        }
    }
}

using System;
using System.Collections.Generic;
using Game.Controllers.Grid_Objects_Controllers;
using Game.Controllers.Menu_Controllers;
using Game.Controllers.NPC_Controllers;
using Game.Controllers.Other_Controllers;
using Game.Players;
using Game.Players.Model;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Util;
using Object = UnityEngine.Object;

namespace Game.Grid
{
    /**
     * Problem: Represent an interactive object placed on the game grid.
     * Goal: Manage object state, interactions, and UI for grid items.
     * Approach: Wrap Unity components and game data with behavior methods.
     * Time: O(1) per interaction plus grid operations.
     * Space: O(1) per object instance.
     */
    public class GameGridObject : GameObjectBase, IEquatable<GameGridObject>, IComparable<GameGridObject>
    {
        private readonly Transform _objectTransform;
        private DataGameObject _dataGameObject;
        private readonly BaseObjectController _baseObjectController;
        private readonly List<GameObject> _actionTiles;
        private List<SpriteRenderer> _tiles;
        private SpriteResolver _spriteResolver;
        private int _actionTile;
        private StoreGameObject _storeGameObject;
        private SpriteRenderer _spriteRenderer;
        private ObjectRotation _facingPosition; // Facing position
        private ClientController _usedBy;

        private EmployeeController _attendedBy, _assignedTo;

        // Buttons/ Sprites and edit menus
        private GameObject _saveObjButton,
            _rotateObjLeftButton,
            _cancelButton,
            _acceptButton,
            _editMenu,
            _objectWithSprite;

        private InfoPopUpController _infoPopUpController;

        private LoadSliderController _loadSlider, _loadSliderMove;

        // Store - To be bought Item, Is Item active, before purchase, (isItemReady, isItemLoading) item on top of the objects, (isObjectSelected) current under preview
        private bool _isItemBought, _active, _isItemReady, _isObjectSelected, _isObjectBeingDragged;

        public GameGridObject(Transform transform)
        {
            _objectTransform = transform;
            Name = transform.name;
            WorldPosition = transform.position; // World position on Unity coords
            GridPosition =
                BussGrid.GetPathFindingGridFromWorldPosition(_objectTransform
                    .position); // Grid position, first position = 0, 0
            LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(WorldPosition);
            _objectWithSprite = transform.Find(Settings.BaseObjectSpriteRenderer).gameObject;
            _spriteRenderer = _objectWithSprite.GetComponent<SpriteRenderer>();
            SortingLayer = transform.GetComponent<SortingGroup>();
            SortingLayer.sortingOrder = Util.Util.GetSorting(GridPosition);
            _isObjectSelected = false;
            _isItemBought = true;

            GameObject objectTileUnder = transform.Find(Settings.BaseObjectUnderTile).gameObject;
            Transform objectActionTile = transform.Find(Settings.BaseObjectActionTile);
            Transform objectSecondActionTile = transform.Find(Settings.BaseObjectActionTile2);
            SpriteRenderer tileUnder = objectTileUnder.GetComponent<SpriteRenderer>();
            SpriteRenderer actionTileSpriteRenderer = objectActionTile.GetComponent<SpriteRenderer>();
            SpriteRenderer secondActionTileSprite = objectSecondActionTile.GetComponent<SpriteRenderer>();

            // Setting base controller
            _baseObjectController = _objectTransform.GetComponent<BaseObjectController>();

            //Edit Panel Disable
            _editMenu = transform.Find(Settings.ConstEditItemMenuPanel).gameObject;
            _editMenu.SetActive(false);

            // On top Move slider
            GameObject moveObjectSlider = transform.Find("Slider/LoadSliderMove").gameObject;
            Util.Util.IsNull(moveObjectSlider, "MoveSlider is null");
            _loadSliderMove = moveObjectSlider.GetComponent<LoadSliderController>();
            _loadSliderMove.SetSliderFillMethod(Image.FillMethod.Vertical);
            _loadSliderMove.SetDefaultFillTime(Settings.SpeedToMoveObjects);
            _loadSliderMove.SetInactive();

            //On top load slider
            GameObject loadObjectSlider = transform.Find("Slider/LoadSlider").gameObject;
            _loadSlider = loadObjectSlider.GetComponent<LoadSliderController>();
            _loadSlider.SetDefaultFillTime(Settings.DefaultItemLoadSpeed -
                                           (PlayerData.GetUgrade(UpgradeType.UpgradeLoadSpeed) *
                                            PlayerData.GetUgrade(UpgradeType.WaiterSpeed) *
                                            Settings.ItemLoadSliderMultiplayer));
            _loadSlider.SetInactive();

            //On top Info popup
            GameObject infoPopUpGameobject = transform.Find("Slider/" + Settings.TopPopUpObject).gameObject;
            _infoPopUpController = infoPopUpGameobject.GetComponent<InfoPopUpController>();

            _actionTiles = new List<GameObject>() { objectActionTile.gameObject, objectSecondActionTile.gameObject };
            _tiles = new List<SpriteRenderer>() { tileUnder, actionTileSpriteRenderer, secondActionTileSprite };

            _active = _baseObjectController.GetInitIsActive();
            // Object rotation
            _facingPosition = _baseObjectController.GetInitialRotation();
            SetEditPanelButtonClickListeners();
        }

        // For GamegridObject init
        public void SetStoreGameObject(StoreGameObject storeGameObject)
        {
            _dataGameObject = _baseObjectController.GetDataGameObject();
            // For setting the object sprite
            this._storeGameObject = storeGameObject;
            Type = storeGameObject.Type;
            _spriteResolver = _objectTransform.Find(Settings.BaseObjectSpriteRenderer).GetComponent<SpriteResolver>();
            _spriteResolver.SetCategoryAndLabel(storeGameObject.SpriteLibCategory, storeGameObject.Identifier);


            // Counter/Chair or store
            if (storeGameObject.Type == ObjectType.StoreItem)
            {
                _infoPopUpController.SetSprite(storeGameObject.GetCurrentSelectedObject().Identifier);
                _loadSlider.SetSliderSprite(storeGameObject.GetCurrentSelectedObject().Identifier);
            }
            else
            {
                _infoPopUpController.SetSprite(storeGameObject.Identifier);
                _loadSlider.SetSliderSprite(storeGameObject.Identifier);
            }

            UpdateInitRotation(_baseObjectController.GetInitialRotation());
            Init(); // StoreGameObject.Type required
        }

        public void DecreaseFillTime()
        {
            _loadSlider.SetDefaultFillTime(Settings.DefaultItemLoadSpeed -
                                           PlayerData.GetUgrade(UpgradeType.WaiterSpeed) *
                                           Settings.ItemLoadSliderMultiplayer);
        }

        public void Init()
        {
            _objectTransform.name = BussGrid.GetObjectID(_storeGameObject.Type);
            Name = _objectTransform.name;
            HideEditMenu();
        }

        private void SetEditPanelButtonClickListeners()
        {
            _saveObjButton = _editMenu.transform.Find("" + Settings.ConstEditStoreMenuSave).gameObject;
            Button save = _saveObjButton.GetComponent<Button>();
            save.onClick.AddListener(StoreInInventory);

            _rotateObjLeftButton = _editMenu.transform.Find(Settings.ConstEditStoreMenuRotateLeft).gameObject;
            Button rotateLeft = _rotateObjLeftButton.GetComponent<Button>();
            rotateLeft.onClick.AddListener(RotateObjectLeft);

            _acceptButton = _editMenu.transform.Find(Settings.ConstEditStoreMenuButtonAccept).gameObject;
            Button accept = _acceptButton.GetComponent<Button>();
            accept.onClick.AddListener(AcceptPurchase);

            _cancelButton = _editMenu.transform.Find(Settings.ConstEditStoreMenuButtonCancel).gameObject;
            Button cancel = _cancelButton.GetComponent<Button>();
            cancel.onClick.AddListener(CancelPurchase);

            _acceptButton.SetActive(false);
            _cancelButton.SetActive(false);
        }

        // Store Item in inventory, store object
        public void StoreInInventory()
        {
            try
            {
                ShowInventoryStoredMessage();
                _dataGameObject.isStored = true;
                PlayerData.StoreItem(this);

                // Clear the Item from the current selected in the grid 
                ObjectDraggingHandler.ClearCurrentClickedActiveGameObject();

                // we clean the table from the employer
                ClearTableEmployee();

                // we clear the table from the client
                ClearTableClient();

                BussGrid.GameController.ReCalculateNpcStates(this);
                BussGrid.GetGameGridObjectsDictionary().TryRemove(Name, out GameGridObject tmp);
                ObjectDraggingHandler.SetIsDraggingEnable(false); // it can be clicked independent 

                if (Type == ObjectType.NpcSingleTable)
                {
                    TableHandler.GetBussQueueMap().TryRemove(this, out byte tmp2);
                }

                DisableIfCounter();
                Object.Destroy(_objectTransform.gameObject);
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, likely missing reference (StoreInInventory GameGridObject): " + e);
            }
        }

        private void ClearTableClient()
        {
            if (_usedBy != null)
            {
                _usedBy.SetTableMoved();
                _usedBy = null;
            }
        }

        private void ClearTableEmployee()
        {
            if (_attendedBy != null)
            {
                _attendedBy.SetTableToAttend(null);
                _attendedBy.SetTableMoved();
                _attendedBy = null;
            }
        }

        private void ShowInventoryStoredMessage()
        {
            // Try to surface feedback in-scene; fall back to logs if UI is not present.
            var messageController = Object.FindObjectOfType<MessageController>(true);

            if (messageController != null)
            {
                messageController.SetTextMessage("Stored item in inventory: " + Name);
                messageController.Enable();
            }

            GameLog.Log("Stored item in inventory: " + Name);
        }

        public void UpdateObjectCoords()
        {
            var position = _objectTransform.transform.position;
            GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(position);
            LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(position);
            WorldPosition = position;
        }

        // This is call everytime the object changes position
        public void UpdateCoordsAndSetObstacle()
        {
            var position = _objectTransform.transform.position;
            GridPosition = BussGrid.GetPathFindingGridFromWorldPosition(position);
            LocalGridPosition = BussGrid.GetLocalGridFromWorldPosition(position);
            WorldPosition = position;
            BussGrid.UpdateObjectPosition(this);

            // it could be a preview object
            if (_dataGameObject != null)
            {
                _dataGameObject.position = new[] { GridPosition.x, GridPosition.y };
            }
        }

        // Changes the sprite renderer of the selected object
        public void HideEditMenu()
        {
            SortingLayer.sortingOrder = Util.Util.GetSorting(GridPosition);
            HideUnderTiles();
            _spriteRenderer.color = Util.Util.FreeColor;
            _editMenu.SetActive(false);
        }

        // Changes the sprite renderer of the selected object
        public void ShowEditMenu()
        {
            SortingLayer.sortingOrder = Util.Util.HighlightObjectSortingPosition;
            _spriteRenderer.color = Util.Util.AvailableColor;
            _editMenu.SetActive(true);
        }

        public void FreeObject()
        {
            _usedBy = null;
            _attendedBy = null;
        }

        public Vector3 GetActionTile()
        {
            if (_objectTransform == null)
            {
                return Vector3.negativeInfinity;
            }

            // if doesnt have action point returns the object actual position
            if (!_storeGameObject.HasActionPoint)
            {
                return _tiles[0].transform.position;
            }

            return _actionTiles != null ? _actionTiles[_actionTile].transform.position : Vector3.negativeInfinity;
        }

        public Vector3Int GetActionTileInGridPosition()
        {
            if (_objectTransform == null)
            {
                return Util.Util.GetVector3IntNegativeInfinity();
            }

            // Gets the orientation and then + 1 ...
            if (_facingPosition == ObjectRotation.Back)
            {
                return GridPosition + new Vector3Int(0, 1);
            }
            else if (_facingPosition == ObjectRotation.BackInverted)
            {
                return GridPosition + new Vector3Int(1, 0);
            }
            else if (_facingPosition == ObjectRotation.Front)
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
            ResetNpcStates();
            FreeObject();
            _facingPosition--;

            if ((int)_facingPosition <= 0)
            {
                _facingPosition = ObjectRotation.FrontInverted;
            }

            UpdateRotation(_facingPosition);
        }

        // If we rotate the table no one can attend the table or go to the table
        private void ResetNpcStates()
        {
            if (_attendedBy != null)
            {
                _attendedBy.SetTableToAttend(null);
                _attendedBy = null;
            }

            if (_usedBy != null)
            {
                _usedBy.SetTable(null);
                _usedBy.SetTableMoved();
                _usedBy = null;
            }
        }

        private bool IsValidRotation(int side)
        {
            if (!_storeGameObject.HasActionPoint)
            {
                return true;
            }

            ObjectRotation tmp = _facingPosition;
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
                tmp = ObjectRotation.Back;
            }
            else if ((int)tmp <= 0)
            {
                tmp = ObjectRotation.FrontInverted;
            }

            // we flip the object temporaly to check the new action tile position
            var rotatedPosition = BussGrid.GetPathFindingGridFromWorldPosition(_objectTransform.position);
            var rotatedActionTile =
                BussGrid.GetPathFindingGridFromWorldPosition(
                    _actionTiles[GetRotationActionTile(tmp)].transform.position);
            // we flip the object back 
            var localScale = GetRotationVector(_facingPosition);
            _objectWithSprite.transform.localScale = localScale;
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
            ResetNpcStates();
            UpdateRotation(rotation);
        }

        public void UpdateRotation(ObjectRotation newPosition)
        {
            // it could be a preview object
            if (_dataGameObject != null)
            {
                _dataGameObject.rotation = (int)newPosition;
            }

            switch (newPosition)
            {
                case ObjectRotation.Back:
                    _spriteResolver.SetCategoryAndLabel(_storeGameObject.SpriteLibCategory + "-Inverted",
                        _storeGameObject.Identifier);
                    _objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.Back);
                    if (_storeGameObject.HasActionPoint)
                    {
                        _actionTile = GetRotationActionTile(ObjectRotation.Back);
                        _tiles[0].color = Util.Util.UnderTilesColor;
                        _tiles[1].color = Util.Util.UnderTilesColor;
                        _tiles[2].color = Util.Util.HiddenColor;
                    }

                    return;
                case ObjectRotation.BackInverted:
                    _spriteResolver.SetCategoryAndLabel(_storeGameObject.SpriteLibCategory + "-Inverted",
                        _storeGameObject.Identifier);
                    _objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.BackInverted);

                    if (_storeGameObject.HasActionPoint)
                    {
                        _actionTile = GetRotationActionTile(ObjectRotation.BackInverted);
                        _tiles[0].color = Util.Util.UnderTilesColor;
                        _tiles[1].color = Util.Util.UnderTilesColor;
                        _tiles[2].color = Util.Util.HiddenColor;
                    }

                    return;
                case ObjectRotation.Front:
                    _spriteResolver.SetCategoryAndLabel(_storeGameObject.SpriteLibCategory,
                        _storeGameObject.Identifier);
                    _objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.Front);
                    if (_storeGameObject.HasActionPoint)
                    {
                        _actionTile = GetRotationActionTile(ObjectRotation.Front);
                        _tiles[0].color = Util.Util.UnderTilesColor;
                        _tiles[1].color = Util.Util.HiddenColor;
                        _tiles[2].color = Util.Util.UnderTilesColor;
                    }

                    return;
                case ObjectRotation.FrontInverted:
                    _spriteResolver.SetCategoryAndLabel(_storeGameObject.SpriteLibCategory,
                        _storeGameObject.Identifier);
                    _objectWithSprite.transform.localScale = GetRotationVector(ObjectRotation.FrontInverted);
                    if (_storeGameObject.HasActionPoint)
                    {
                        _actionTile = GetRotationActionTile(ObjectRotation.FrontInverted);
                        _tiles[0].color = Util.Util.UnderTilesColor;
                        _tiles[1].color = Util.Util.HiddenColor;
                        _tiles[2].color = Util.Util.UnderTilesColor;
                    }

                    return;
            }
        }

        private Vector3 GetRotationVector(ObjectRotation objectRotation)
        {
            switch (objectRotation)
            {
                case ObjectRotation.Back:
                    return new Vector3(1, 1, 1);
                case ObjectRotation.Front:
                    return new Vector3(1, 1, 1);
                case ObjectRotation.BackInverted:
                    return new Vector3(-1, 1, 1);
                case ObjectRotation.FrontInverted:
                    return new Vector3(-1, 1, 1);
            }

            return Vector3.negativeInfinity;
        }

        private int GetRotationActionTile(ObjectRotation objectRotation)
        {
            switch (objectRotation)
            {
                case ObjectRotation.Back:
                    return 0;
                case ObjectRotation.Front:
                    return 1;
                case ObjectRotation.BackInverted:
                    return 0;
                case ObjectRotation.FrontInverted:
                    return 1;
            }

            return int.MaxValue;
        }

        public void HideUnderTiles()
        {
            _tiles[0].color = Util.Util.HiddenColor;
            _tiles[_actionTile + 1].color = Util.Util.HiddenColor;
        }

        public void ShowOccupiedUnderTiles()
        {
            _tiles[0].color = Util.Util.UnderTilesColor;

            if (_storeGameObject.HasActionPoint)
            {
                _tiles[_actionTile + 1].color = Util.Util.UnderTilesColor;
            }
        }

        public void ShowAvailableUnderTiles()
        {
            _tiles[0].color = Util.Util.UnderTilesColor;
            if (_storeGameObject.HasActionPoint)
            {
                _tiles[_actionTile + 1].color = Util.Util.UnderTilesColor;
            }
        }

        public StoreGameObject GetStoreGameObject()
        {
            return _storeGameObject;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return _spriteRenderer;
        }

        public bool GetBusy()
        {
            return _usedBy != null;
        }

        public void SetUsedBy(ClientController controller)
        {
            _usedBy = controller;
        }

        public ClientController GetUsedBy()
        {
            return _usedBy;
        }

        public void SetAttendedBy(EmployeeController controller)
        {
            _attendedBy = controller;
        }

        public void SetAssignedTo(EmployeeController controller)
        {
            _assignedTo = controller;
        }

        public bool IsItemAssignedTo()
        {
            return _assignedTo != null;
        }

        public bool GetIsObjectBeingDragged()
        {
            return _isObjectBeingDragged;
        }

        public bool IsFree()
        {
            return _usedBy == null && _attendedBy == null;
        }

        public bool HasClient()
        {
            return _usedBy != null;
        }

        public bool HasAttendedBy()
        {
            return _attendedBy != null;
        }

        public void UpdateMoveSlider()
        {
            if (_isObjectSelected)
            {
                return;
            }

            if (_loadSliderMove.IsFinished())
            {
                SetObjectSelected();
            }

            DiableTopInfoObject();
            _loadSliderMove.SetActive();
        }

        // This loads the top Item
        public void UpdateLoadItemSlider()
        {
            if (_isObjectSelected)
            {
                return;
            }

            if (_loadSlider.IsFinished())
            {
                _loadSlider.RestartState();
                SetItemsReady();
            }
        }

        public void SetItemsReady()
        {
            _isItemReady = true;
            _infoPopUpController.Enable();
        }

        public bool GetIsItemReady()
        {
            return _isItemReady;
        }

        private void SetObjectSelected()
        {
            // Disables dragging for a second 
            _baseObjectController.DisableDisableDraggingTemp();
            _isObjectSelected = true;
            _loadSliderMove.SetInactive();
            ObjectDraggingHandler.SetActiveGameGridObject(this);
            ClearTableClient();
            ClearTableEmployee();
            DisableIfCounter();
            //We clean grid position
            BussGrid.FreeObject(this);
            var position = _objectTransform.position;
            position = new Vector3(position.x, position.y,
                Util.Util.SelectedObjectZPosition);
            _objectTransform.position = position;
            // We clean in case the item is loaded on top
            if (_storeGameObject.HasActionPoint && _isItemReady)
            {
                DiableTopInfoObject();
            }
        }

        public void DiableTopInfoObject()
        {
            _isItemReady = false;
            _infoPopUpController.Disable();
            _loadSlider.SetInactive();
        }

        public void SetInactive()
        {
            _isObjectSelected = false;
            var objectTransform = _objectTransform.position;
            _objectTransform.position =
                new Vector3(objectTransform.x, objectTransform.y, Util.Util.ObjectZPosition);
            ObjectDraggingHandler.SetIsDraggingEnable(false);
            ObjectDraggingHandler.HideHighlightedGridBussFloor();
        }

        public bool GetIsObjectSelected()
        {
            return _isObjectSelected;
        }

        public void SetNewObjectActiveButtons()
        {
            _acceptButton.SetActive(true);
            _cancelButton.SetActive(true);
            _rotateObjLeftButton.SetActive(true);
            _saveObjButton.SetActive(false);
        }

        public void SetStoreObject()
        {
            // We show accept, cancel buttons and select the object
            _isObjectSelected = true;
            _isItemBought = false;
            ObjectDraggingHandler.SetActiveGameGridObject(this);
            SetNewObjectActiveButtons();
        }

        // Used in the case the player cancels or clicks outside the object
        public bool GetIsItemBought()
        {
            return _isItemBought;
        }

        public void AcceptPurchase()
        {
            if (!_baseObjectController.GetIscurrentValidPos())
            {
                return;
            }

            _isItemBought = true;
            _baseObjectController.SetNewItem(false, _baseObjectController.GetIsStorageItem());
            _baseObjectController.SetIsNewItemSetted(true);

            // We set the new state for the edit panel buttons
            _acceptButton.SetActive(false);
            _cancelButton.SetActive(false);
            _rotateObjLeftButton.SetActive(true);
            _saveObjButton.SetActive(true);
            HideEditMenu();

            // We dont substract if the item is comming from the storage
            if (!_baseObjectController.GetIsStorageItem() && _dataGameObject == null)
            {
                PlayerData.Subtract(_storeGameObject.Cost, _storeGameObject.Type);
                // We set a new firebase object
                PlayerData.AddDataGameObject(this);
            }
            else
            {
                PlayerData.SubtractFromStorage(this);
                _baseObjectController.SetStorage(false);
                _isItemBought = true;
                _dataGameObject.isStored = false;
            }

            //SetAsCounter(); //If it is a counter we set if in the grid
            BussGrid.SetObjectObstacle(this);
            UpdateCoordsAndSetObstacle();
            SetInactive();
            HideUnderTiles();
            // Now it can be used by NPCs
            _active = true;
        }

        private void DisableIfCounter()
        {
            if (Type == ObjectType.NpcCounter && _assignedTo != null)
            {
                _assignedTo.SetCounter(null);
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
            Object.Destroy(_objectTransform.gameObject);
            DisableIfCounter();
            SetInactive();
            BussGrid.RecalculateBussGrid();
        }

        public bool GetActive()
        {
            return _active;
        }

        public void SetDataGameObject(DataGameObject obj)
        {
            _dataGameObject = obj;
        }

        public ObjectRotation GetFacingPosition()
        {
            return _facingPosition;
        }

        public LoadSliderController GetLoadItemSlider()
        {
            return _loadSlider;
        }

        public LoadSliderController GetMoveSlider()
        {
            return _loadSliderMove;
        }

        public override string ToString()
        {
            return "isItemBought: " + _isItemBought + " active: " + _active + " isItemReady: " + _isItemReady +
                   " isObjectSelected: " +
                   _isObjectSelected + " isObjectBeingDragged: " + _isObjectBeingDragged;
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
            if (obj2 == null || SortingLayer == null)
            {
                return false;
            }

            return SortingLayer.sortingOrder == obj2.GetSortingOrder();
        }
    }
}

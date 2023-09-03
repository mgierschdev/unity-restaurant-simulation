using System;
using System.Collections;
using System.Collections.Generic;
using Game.Controllers.Grid_Objects_Controllers;
using Game.Grid;
using Game.Players;
using Game.Players.Model;
using Game.UI.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Util.Collections;

// This will be only element attached in the UI
// All the bottom calls will be handled by this class.
namespace Game.Controllers.Menu_Controllers
{
    public class MenuHandlerController : MonoBehaviour
    {
        //center panel scrollview
        private GameObject _centerPanel,
            _scrollView,
            _scrollViewContent,
            _centerPanelSideMenu,
            _leftDownPanel,
            _storeButton,
            _centerPanelMenuTitleObject;

        //saves the latest reference to the npc if the menu was opened
        private MenuItem _centerTabMenu;

        // Min amount of time the the menu has to be open before activating -> closing on click outside
        private TextMeshProUGUI _menuPanelTitle;
        private List<RectTransform> _visibleRects;
        private MenuBackgroundController _menuBackgroundController;

        private GridLayoutGroup _gridLayoutGroup;

        // Scroll view content
        private List<UpgradeItemController> _upgradeItemControllerList;
        private List<InventoryItemController> _storeInventoryItemControllerList;

        private SettingsPanelController _settingsPanelController;

        // Current menu tab open
        private MenuTab _currentTab;

        //Dictionary tabs and buttons, for main menus
        private Dictionary<MenuTab, Button> _centerMenuTabMap;

        // MenuHandlerController Attached to CanvasMenu Parent of all Menus
        private void Awake()
        {
            // Setting up Top level UI
            GameObject topResourcePanelMoney = GameObject.Find(Settings.ConstTopMenuDisplayMoney);
            TextMeshProUGUI moneyText = topResourcePanelMoney.GetComponent<TextMeshProUGUI>();
            GameObject topResourcePanelLevel = GameObject.Find(Settings.ConstTopMenuLevel);
            TextMeshProUGUI levelText = topResourcePanelLevel.GetComponent<TextMeshProUGUI>();
            GameObject topExpSlider = GameObject.Find(Settings.ConstTopMenuExpSlider);
            Slider expSlider = topExpSlider.GetComponent<Slider>();

            PlayerData.SetPlayerData(moneyText, levelText, expSlider);

            // Left down panel and Edit store panel
            _leftDownPanel = GameObject.Find(Settings.ConstLeftDownPanel).gameObject;

            // Scrollview content 
            _upgradeItemControllerList = new List<UpgradeItemController>();
            _storeInventoryItemControllerList = new List<InventoryItemController>();

            // Menu Background Controller 
            _menuBackgroundController =
                GameObject.Find(Settings.MenuContainer).GetComponent<MenuBackgroundController>();
            if (_menuBackgroundController == null)
            {
                GameLog.LogWarning("MenuHandlerController.cs/menuBackgroundController is null");
            }

            // Menu Body
            _centerPanel = GameObject.Find(Settings.ConstCenterTabMenu);
            _scrollView = _centerPanel.transform.Find(Settings.ConstCenterScrollView).gameObject;
            _scrollViewContent = _centerPanel.transform.Find(Settings.ConstCenterScrollContent).gameObject;
            _gridLayoutGroup = _scrollViewContent.GetComponent<GridLayoutGroup>();
            _centerPanelSideMenu = _centerPanel.transform.Find(Settings.ConstButtonMenuPanel).gameObject;

            // Menu title 
            _centerPanelMenuTitleObject = _centerPanel.transform.Find(Settings.ConstCenterMenuPanelTitle).gameObject;
            _menuPanelTitle = _centerPanelMenuTitleObject.GetComponent<TextMeshProUGUI>();

            var centerPanelViewPanel = _centerPanel.transform.Find("ViewPanel").gameObject;
            _visibleRects = new List<RectTransform>
            {
                _centerPanelSideMenu.GetComponent<RectTransform>(),
                centerPanelViewPanel.GetComponent<RectTransform>()
            };

            if (!_centerPanel || !_leftDownPanel)
            {
                GameLog.LogWarning("UIHandler Menu null ");
                GameLog.LogWarning("tabMenu " + _centerPanel);
                GameLog.LogWarning("leftDownPanel " + _leftDownPanel);
            }

            //Center tab menus tabs
            _centerTabMenu = new MenuItem(MenuTab.StoreItems, MenuType.TabMenu, Settings.ConstCenterTabMenu);
            _centerMenuTabMap = new Dictionary<MenuTab, Button>();
            _currentTab = MenuTab.StoreItems;

            // Loading left side buttons from the center panel
            LoadCenterPanelSideMenu();
            // Setting Click Listeners to Left Down Panel
            SetLeftDownPanelClickListeners();
            CloseCenterPanel();
            StartCoroutine(UpdateUI());
        }

        private IEnumerator UpdateUI()
        {
            for (;;)
            {
                if (!IsMenuOpen() || (_storeInventoryItemControllerList.Count == 0 &&
                                      _upgradeItemControllerList.Count == 0))
                {
                    yield return new WaitForSeconds(2);
                }

                if (_currentTab == MenuTab.StoreItems)
                {
                    UpdateStoreItemsScrollView();
                }
                else if (_currentTab == MenuTab.Upgrade)
                {
                    UpdateUpgradeScrollView();
                }
                else if (_currentTab == MenuTab.SettingsTab)
                {
                    UpdateSettingsPanel();
                }

                yield return new WaitForSeconds(2);
            }
        }

        private void UpdateUpgradeScrollView()
        {
            if (_upgradeItemControllerList.Count == 0)
            {
                return;
            }

            foreach (UpgradeItemController upgradeItemController in _upgradeItemControllerList)
            {
                Button button = upgradeItemController.GetButton();
                button.onClick.RemoveAllListeners();

                if (upgradeItemController.GetStoreGameObject().Cost <= PlayerData.GetMoneyDouble())
                {
                    button.onClick.AddListener(() =>
                        UpgradeItem(upgradeItemController.GetStoreGameObject(), upgradeItemController));
                    upgradeItemController.SetAvailable();
                }
                else
                {
                    upgradeItemController.SetUnavailable();
                }
            }
        }

        private void UpdateStoreItemsScrollView()
        {
            if (_storeInventoryItemControllerList.Count == 0)
            {
                return;
            }

            foreach (InventoryItemController inventoryItemController in _storeInventoryItemControllerList)
            {
                Button button = inventoryItemController.GetButton();
                button.onClick.RemoveAllListeners();

                Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>
                {
                    Key = inventoryItemController.GetStoreGameObject()
                };

                if (CanObjectBeBought(inventoryItemController.GetStoreGameObject()))
                {
                    button.onClick.AddListener(() => OpenStoreEditPanel(pair, false));
                    inventoryItemController.SetAvailable();
                }
                else
                {
                    button.onClick.AddListener(() => EmptyClickListener());
                    inventoryItemController.SetUnavailable();
                }
            }
        }

        private void UpdateSettingsPanel()
        {
            if (_settingsPanelController == null)
            {
                return;
            }

            _settingsPanelController.SetStatsText();
        }

        private void LoadCenterPanelSideMenu()
        {
            // Clear the dev button view
            foreach (Transform child in _centerPanelSideMenu.transform)
            {
                Destroy(child.gameObject);
            }

            // Set Menu buttons ButtonMenuPanel
            foreach (MenuTab tab in Enum.GetValues(typeof(MenuTab)))
            {
                var button = Instantiate(Resources.Load(Settings.SideMenuButton, typeof(GameObject)), Vector3.zero,
                    Quaternion.identity) as GameObject;

                if (button == null)
                {
                    return;
                }

                var controller = button.GetComponent<CenterTabMenuBottonController>();
                controller.SetText(GameObjectList.GetButtonLabel(tab));
                button.transform.SetParent(_centerPanelSideMenu.transform);
                var bStore = controller.GetButton();
                var current = new MenuItem(tab, MenuType.TabMenu, tab.ToString());

                _centerMenuTabMap.TryAdd(tab, bStore);

                bStore.onClick.AddListener(() => AddMenuItemsToScrollView(current));
            }
        }

        public void SetMainMenuTitle(string title)
        {
            _menuPanelTitle.text = title;
        }

        public void CloseMenu()
        {
            // We disable the image menu backgrounds
            if (_menuBackgroundController.IsActive())
            {
                _menuBackgroundController.Disable();
            }

            if (_centerPanel.activeSelf)
            {
                CloseCenterPanel();
            }

            _upgradeItemControllerList.Clear();
            _storeInventoryItemControllerList.Clear();
        }

        private void OpenMenu(MenuItem menu)
        {
            // Open all menus except settings and InGameStore
            AddMenuItemsToScrollView(menu);
            OpenCenterPanel();

            // If there is a selected object on the UI we un-unselect the object
            if (ObjectDraggingHandler.GetIsDraggingEnabled())
            {
                ObjectDraggingHandler.DisableDragging();
            }

            // we enable menu background
            _menuBackgroundController.Enable();
        }

        private bool IsClickOutside()
        {
            foreach (var rect in _visibleRects)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
                {
                    return false;
                }
            }

            return true;
        }

        private void AddMenuItemsToScrollView(MenuItem menu)
        {
            if (!_scrollViewContent)
            {
                return;
            }

            // Clear ScrollView
            foreach (Transform child in _scrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }

            //Set top panel title
            SetMainMenuTitle(GameObjectList.GetButtonLabel(menu.GetMenuTab()));
            SetCellSize(Settings.StoreCellSizeX, Settings.StoreCellSizeY);

            //Clear scrollview content list
            _storeInventoryItemControllerList.Clear();
            _upgradeItemControllerList.Clear();
            _currentTab = menu.GetMenuTab();

            // Add items to scroll view depending on the tab
            switch (menu.GetMenuTab())
            {
                case MenuTab.StoreItems:
                    AddItemsToScrollView(menu);
                    StartCoroutine(ScrollToTop());
                    return;
                case MenuTab.Upgrade:
                    AddItemsToUpgradeScrollView(menu);
                    StartCoroutine(ScrollToTop());
                    return;
                case MenuTab.StorageTab:
                    AddStorageItemsToScrollView(menu);
                    StartCoroutine(ScrollToTop());
                    return;
                case MenuTab.SettingsTab:
                    AddSettingsItemsToScrollView();
                    StartCoroutine(ScrollToTop());
                    return;
            }
        }

        private IEnumerator ScrollToTop()
        {
            yield return new WaitForSeconds(0.001f);
            // We scroll to the top of the scroll view
            ScrollRect scroll = _scrollView.GetComponent<ScrollRect>();
            scroll.verticalNormalizedPosition = 1f;
        }

        private void AddSettingsItemsToScrollView()
        {
            var settings = Instantiate(Resources.Load(Settings.PrefabSettingsItem, typeof(GameObject)),
                Vector3.zero,
                Quaternion.identity) as GameObject;

            if (settings == null)
            {
                throw new ArgumentException(nameof(settings));
            }
            
            _settingsPanelController = settings.GetComponent<SettingsPanelController>();
            settings.transform.SetParent(_scrollViewContent.transform);
            settings.transform.localScale = new Vector3(1, 1, 1);
            SetCellSize(Settings.SettingsCellSizeX, Settings.SettingsCellSizeY);
        }

        private void AddStorageItemsToScrollView(MenuItem menu)
        {
            GameObjectList.GetItemList(menu.GetMenuTab());
            InventoryItemController inventoryItemController;
            Button button;
            var objectDic = new Dictionary<StoreGameObject, int>();
            var dicPair =
                new Dictionary<StoreGameObject, Pair<StoreGameObject, DataGameObject>>();
            var userStorage = GameObjectList.LoadCurrentUserStorage();

            foreach (DataGameObject fireObj in userStorage)
            {
                StoreGameObject obj = GameObjectList.GetStoreObject((StoreItemType)fireObj.id);

                int count = objectDic.GetValueOrDefault(obj, 0) + 1;
                if (objectDic.ContainsKey(obj))
                {
                    objectDic.Remove(obj);
                }
                else
                {
                    Pair<StoreGameObject, DataGameObject>
                        pair = new Pair<StoreGameObject, DataGameObject>(obj, fireObj);
                    dicPair.Add(obj, pair);
                }

                objectDic.Add(obj, count);
            }

            // sorting the final list
            SortedList<StoreGameObject, int> list = new SortedList<StoreGameObject, int>();

            foreach (KeyValuePair<StoreGameObject, int> entry in objectDic)
            {
                list.Add(entry.Key, entry.Value);
            }

            foreach (KeyValuePair<StoreGameObject, int> entry in list)
            {
                var item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero,
                    Quaternion.identity) as GameObject;

                if (item == null)
                {
                    throw new ArgumentException(nameof(item));
                }
                
                inventoryItemController = item.GetComponent<InventoryItemController>();
                button = inventoryItemController.GetButton();
                button.onClick.AddListener(() => OpenStoreEditPanel(dicPair[entry.Key], true));
                inventoryItemController.SetInventoryItem(entry.Key);
                inventoryItemController.SetAmmount(entry.Value.ToString());
                inventoryItemController.SetAvailable();
                item.transform.SetParent(_scrollViewContent.transform);
                item.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void AddItemsToUpgradeScrollView(MenuItem menu)
        {
            List<StoreGameObject> objects = GameObjectList.GetItemList(menu.GetMenuTab());
            GameObject item;
            UpgradeItemController upgradeItemController;
            Button button;

            // Add new Items
            foreach (StoreGameObject obj in objects)
            {
                item = Instantiate(Resources.Load(Settings.PrefabUpgradeInventoryItem, typeof(GameObject)),
                    Vector3.zero,
                    Quaternion.identity) as GameObject;

                if (item == null)
                {
                    throw new ArgumentException(nameof(item));
                }
                
                upgradeItemController = item.GetComponent<UpgradeItemController>();
                button = upgradeItemController.GetButton();

                // Adding click listener
                if (obj.Cost <= PlayerData.GetMoneyDouble())
                {
                    var controller = upgradeItemController;
                    button.onClick.AddListener(() => UpgradeItem(obj, controller));
                    upgradeItemController.SetAvailable();
                }
                else
                {
                    upgradeItemController.SetUnavailable();
                }

                upgradeItemController.SetInventoryItem(obj);
                item.transform.SetParent(_scrollViewContent.transform);
                item.transform.localScale = new Vector3(1, 1, 1);
                _upgradeItemControllerList.Add(upgradeItemController);
            }
        }

        private void UpgradeItem(StoreGameObject storeGameObject, UpgradeItemController upgradeItemController)
        {
            if (PlayerData.IncreaseUpgrade(storeGameObject))
            {
                upgradeItemController.IncreaseUpgrade();
            }
        }

        private void AddItemsToScrollView(MenuItem menu)
        {
            List<StoreGameObject> objects = GameObjectList.GetItemList(menu.GetMenuTab());
            GameObject item;
            InventoryItemController inventoryItemController;
            Button button;

            // Add new Items
            foreach (StoreGameObject obj in objects)
            {
                item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero,
                    Quaternion.identity) as GameObject;

                if (item == null)
                {
                    throw new ArgumentException(nameof(item));
                }
                
                inventoryItemController = item.GetComponent<InventoryItemController>();
                button = inventoryItemController.GetButton();
                Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>
                {
                    Key = obj
                };

                // Adding click listener
                if (CanObjectBeBought(obj))
                {
                    button.onClick.AddListener(() => OpenStoreEditPanel(pair, false));
                    inventoryItemController.SetAvailable();
                }
                else
                {
                    inventoryItemController.SetUnavailable();
                }

                inventoryItemController.SetInventoryItem(obj);
                inventoryItemController.SetPrice(obj.Cost.ToString());
                item.transform.SetParent(_scrollViewContent.transform);
                item.transform.localScale = new Vector3(1, 1, 1);
                _storeInventoryItemControllerList.Add(inventoryItemController);
            }
        }

        public bool CanObjectBeBought(StoreGameObject obj)
        {
            return obj.Cost <= PlayerData.GetMoneyDouble() &&
                   ( //UPGRADE: max number of counters
                       (obj.Type == ObjectType.NpcCounter &&
                        PlayerData.GetNumberCounters() <= PlayerData.GetUgrade(UpgradeType.NumberWaiters)) ||
                       (obj.Type != ObjectType.NpcCounter));
        }

        private void SetLeftDownPanelClickListeners()
        {
            _storeButton = _leftDownPanel.transform.Find(Settings.ConstLeftDownMenuStore).gameObject;
            Button bStore = _storeButton.GetComponent<Button>();
            bStore.onClick.AddListener(() => OpenMenu(_centerTabMenu));
        }

        //ConstLeftDownMenuStore CenterPanel open button
        private void HideMainMenuButton()
        {
            _storeButton.SetActive(false);
        }

        private void ShowMainMenuButton()
        {
            _storeButton.SetActive(true);
        }

        private void OpenStoreEditPanel(Pair<StoreGameObject, DataGameObject> pair, bool storage)
        {
            StoreGameObject obj = pair.Key;

            if (!PlayerData.CanSubtract(obj.Cost) && !storage)
            {
                // GameLog.Log("UI message: Insufficient funds " + PlayerData.GetMoney());
                return;
            }

            CloseMenu();
            GameObject newObject;

            // Auto place : placeGameObject(obj); /  PlaceSingleTileObject(obj);
            newObject = PlaceAtCameraSquare(obj);

            BaseObjectController baseObjectController = newObject.GetComponent<BaseObjectController>();
            baseObjectController.SetNewItem(true, storage);
            baseObjectController.SetStoreGameObject(obj);

            if (storage)
            {
                // we set the new rotation setted by the placeGameObject
                pair.Value.rotation = (int)baseObjectController.GetInitialRotation();
                baseObjectController.SetStoreGameObject(pair.Value);
            }

            ObjectDraggingHandler.SetPreviewItem(baseObjectController);
        }

        private IEnumerator TestPlacingObjects(StoreGameObject obj)
        {
            // Print the time of when the function is first called.
            // yield on a new YieldInstruction that waits for 5 seconds.
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.15f); // 0.15f
                PlaceGameObject(obj);
            }

            // After we have waited 5 seconds print the time again.
            yield return new WaitForSeconds(0);
        }

        public bool IsMenuOpen()
        {
            return _centerPanel.activeSelf;
        }

        private static GameObject PlaceAtCameraSquare(StoreGameObject obj)
        {
            GameObject parent = GameObject.Find(Settings.TilemapObjects);
            Vector3 spamPosition = BussGrid.GetGridWorldPositionMapMouseDrag(Util.Util.GetCameraPosition());
            GameObject newObject;

            newObject = Instantiate(Resources.Load(GameObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)),
                new Vector3(spamPosition.x, spamPosition.y, Util.Util.SelectedObjectZPosition), Quaternion.identity,
                parent.transform) as GameObject;
            return newObject;
        }

        private static GameObject PlaceSingleTileObject(StoreGameObject obj)
        {
            var parent = GameObject.Find(Settings.TilemapObjects);
            var spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextFreeTile());
            GameObject newObject;

            if (Util.Util.CompareNegativeInfinity(spamPosition))
            {
                return null;
            }
            else
            {
                newObject = Instantiate(Resources.Load(GameObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)),
                    new Vector3(spamPosition.x, spamPosition.y, Util.Util.SelectedObjectZPosition), Quaternion.identity,
                    parent.transform) as GameObject;
            }

            return newObject;
        }

        private static GameObject PlaceGameObject(StoreGameObject obj)
        {
            var parent = GameObject.Find(Settings.TilemapObjects);
            Vector3 spamPosition;

            if (BussGrid.GetGameGridObjectsDictionary().Count == 0)
            {
                spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextTileFromEmptyMap(obj));
                return spamPosition == Util.Util.GetVector3IntNegativeInfinity()
                    ? null
                    : Instantiate(Resources.Load(GameObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)),
                        new Vector3(spamPosition.x, spamPosition.y, Util.Util.SelectedObjectZPosition), Quaternion.identity,
                        parent.transform) as GameObject;
            }

            foreach (KeyValuePair<string, GameGridObject> dic in BussGrid.GetGameGridObjectsDictionary())
            {
                GameGridObject current = dic.Value;
                Vector3Int[] nextTile = BussGrid.GetNextFreeTileWithActionPoint(current);

                if (nextTile.GetLength(0) != 0)
                {
                    // We place the object 
                    spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(nextTile[0]);
                    bool inverted = !(nextTile[1] == Vector3Int.up);

                    var newObject = Instantiate(
                        Resources.Load(GameObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)),
                        new Vector3(spamPosition.x, spamPosition.y, Util.Util.SelectedObjectZPosition), Quaternion.identity,
                        parent.transform) as GameObject;

                    if (newObject == null)
                    {
                        throw new ArgumentNullException(nameof(newObject));
                    }

                    // There can be only one counter at the tinme
                    BaseObjectController baseObjectController = newObject.GetComponent<BaseObjectController>();
                    baseObjectController.SetInitialObjectRotation(inverted
                        ? ObjectRotation.BackInverted
                        : ObjectRotation.Back);
                    return newObject;
                }
            }

            return null;
        }

        private void EmptyClickListener()
        {
        }

        private void CloseCenterPanel()
        {
            _centerPanel.SetActive(false);
            ShowMainMenuButton();
        }

        private void OpenCenterPanel()
        {
            HideMainMenuButton();
            _centerPanel.SetActive(true);
            CheckStorageButton(); // check if we enable the storage button
            // Selecting the button at the same time of opening the menu
            Button tableTabButton = _centerMenuTabMap[MenuTab.StoreItems];
            tableTabButton.Select();
        }

        private void CheckStorageButton()
        {
            List<DataGameObject> userStorage = GameObjectList.LoadCurrentUserStorage();
            Button button = _centerMenuTabMap[MenuTab.StorageTab];
            // if we dont have storage we disable the button
            if (userStorage.Count == 0)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }

        private void SetCellSize(float sizeX, float sizeY)
        {
            _gridLayoutGroup.cellSize = new Vector2(sizeX, sizeY);
        }
    }
}
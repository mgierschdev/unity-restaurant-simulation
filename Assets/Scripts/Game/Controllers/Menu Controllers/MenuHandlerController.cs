using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This will be only element attached in the UI
// All the bottom calls will be handled by this class.
public class MenuHandlerController : MonoBehaviour
{
    //center panel scrollview
    private GameObject centerPanel, scrollView, scrollViewContent, centerPanelSideMenu, leftDownPanel, storeButton, centerPanelMenuTitleObject;
    //saves the latest reference to the npc if the menu was opened
    private MenuItem centerTabMenu;
    // Min amount of time the the menu has to be open before activating -> closing on click outside
    private float MIN_OPENED_TIME = 0.5f, openedTime;
    private TextMeshProUGUI moneyText, menuPanelTitle;
    private List<RectTransform> visibleRects;
    private MenuBackgroundController menuBackgroundController;
    private Button tableTabButton;
    private GridLayoutGroup gridLayoutGroup;
    // Scroll view content
    private List<UpgradeItemController> upgradeItemControllerList;
    private List<InventoryItemController> storeInventoryItemControllerList;
    // Current menu tab open
    private MenuItem currentTab;

    // MenuHandlerController Attached to CanvasMenu Parent of all Menus
    private void Awake()
    {
        // Setting up Top level UI
        GameObject topResourcePanelMoney = GameObject.Find(Settings.ConstTopMenuDisplayMoney);
        TextMeshProUGUI moneyText = topResourcePanelMoney.GetComponent<TextMeshProUGUI>();
        GameObject topResourcePanelLevel = GameObject.Find(Settings.ConstTopMenuLevel);
        TextMeshProUGUI levelText = topResourcePanelLevel.GetComponent<TextMeshProUGUI>();
        GameObject topResourcePanelGems = GameObject.Find(Settings.ConstTopMenuDisplayGems);
        TextMeshProUGUI gemsText = topResourcePanelGems.GetComponent<TextMeshProUGUI>();
        GameObject topExpSlider = GameObject.Find(Settings.ConstTopMenuExpSlider);
        Slider expSlider = topExpSlider.GetComponent<Slider>();

        PlayerData.SetPlayerData(moneyText, levelText, gemsText, expSlider);

        // Left down panel and Edit store panel
        leftDownPanel = GameObject.Find(Settings.ConstLeftDownPanel).gameObject;

        // Scrollview content 
        upgradeItemControllerList = new List<UpgradeItemController>();
        storeInventoryItemControllerList = new List<InventoryItemController>();

        // Menu Background Controller 
        menuBackgroundController = GameObject.Find(Settings.MenuContainer).GetComponent<MenuBackgroundController>();
        if (menuBackgroundController == null)
        {
            GameLog.LogWarning("MenuHandlerController.cs/menuBackgroundController is null");
        }

        // Menu Body
        centerPanel = GameObject.Find(Settings.ConstCenterTabMenu);
        scrollView = centerPanel.transform.Find(Settings.ConstCenterScrollView).gameObject;
        scrollViewContent = centerPanel.transform.Find(Settings.ConstCenterScrollContent).gameObject;
        gridLayoutGroup = scrollViewContent.GetComponent<GridLayoutGroup>();
        centerPanelSideMenu = centerPanel.transform.Find(Settings.ConstButtonMenuPanel).gameObject;

        // Menu title 
        centerPanelMenuTitleObject = centerPanel.transform.Find(Settings.ConstCenterMenuPanelTitle).gameObject;
        menuPanelTitle = centerPanelMenuTitleObject.GetComponent<TextMeshProUGUI>();

        GameObject CenterPanelViewPanel = centerPanel.transform.Find("ViewPanel").gameObject;
        visibleRects = new List<RectTransform>{
            centerPanelSideMenu.GetComponent<RectTransform>(),
            CenterPanelViewPanel.GetComponent<RectTransform>()
        };

        if (!centerPanel || !leftDownPanel)
        {
            GameLog.LogWarning("UIHandler Menu null ");
            GameLog.LogWarning("tabMenu " + centerPanel);
            GameLog.LogWarning("leftDownPanel " + leftDownPanel);
        }

        //Center tab menus tabs
        centerTabMenu = new MenuItem(MenuTab.STORE_ITEMS, MenuType.TAB_MENU, Settings.ConstCenterTabMenu);
        currentTab = centerTabMenu;

        LoadCenterPanelSideMenu();
        // Setting Click Listeners to Left Down Panel
        SetLeftDownPanelClickListeners();
        LoadCenterPanelSideMenu();
        CloseCenterPanel();
        StartCoroutine(UpdateUI());
        openedTime = 0;
    }

    private IEnumerator UpdateUI()
    {
        for (; ; )
        {
            if (!IsMenuOpen() || (storeInventoryItemControllerList.Count == 0 && upgradeItemControllerList.Count == 0))
            {
                yield return new WaitForSeconds(2);
            }

            if (currentTab.GetMenuTab() == MenuTab.STORE_ITEMS)
            {
                UpdateStoreItemsScrollView();
            }else if(currentTab.GetMenuTab() == MenuTab.UPGRADE){
                //TODO: UpdateUpgradeItemsTab
            }else if(currentTab.GetMenuTab() == MenuTab.SETTINGS_TAB){
                //TODO: Update stats page
            }

            yield return new WaitForSeconds(2);
        }
    }

    private void UpdateStoreItemsScrollView()
    {
        if (storeInventoryItemControllerList.Count == 0)
        {
            return;
        }

        foreach (InventoryItemController inventoryItemController in storeInventoryItemControllerList)
        {
            Button button = inventoryItemController.GetButton();
            button.onClick.RemoveAllListeners();
            
            Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>
            {
                Key = inventoryItemController.GetStoreGameObject()
            };

            if (inventoryItemController.GetStoreGameObject().Cost <= PlayerData.GetMoneyDouble())
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

    private void LoadCenterPanelSideMenu()
    {
        // Clear the dev button view
        foreach (Transform child in centerPanelSideMenu.transform)
        {
            Destroy(child.gameObject);
        }
        // Set Menu buttons ButtonMenuPanel
        foreach (MenuTab tab in MenuTab.GetValues(typeof(MenuTab)))
        {
            GameObject button = Instantiate(Resources.Load(Settings.SideMenuButton, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            CenterTabMenuBottonController controller = button.GetComponent<CenterTabMenuBottonController>();
            controller.SetText(MenuObjectList.GetButtonLabel(tab));
            button.transform.SetParent(centerPanelSideMenu.transform);
            Button bStore = controller.GetButton();
            MenuItem current = new MenuItem(tab, MenuType.TAB_MENU, tab.ToString());
            bStore.onClick.AddListener(() => AddMenuItemsToScrollView(current));

            // We save the tables tab button, to select it as soon as we open the menu
            if (MenuTab.STORE_ITEMS == tab)
            {
                tableTabButton = bStore;
            }
        }
    }

    public void SetMainMenuTitle(string title)
    {
        menuPanelTitle.text = title;
    }

    public void CloseMenu()
    {
        // We disable the image menu backgrounds
        if (menuBackgroundController.IsActive())
        {
            menuBackgroundController.Disable();
        }

        if (centerPanel.activeSelf)
        {
            CloseCenterPanel();
        }

        upgradeItemControllerList.Clear();
        storeInventoryItemControllerList.Clear();
    }

    private void OpenMenu(MenuItem menu)
    {
        // Open all menus except settings and InGameStore
        AddMenuItemsToScrollView(menu);
        OpenCenterPanel();
        currentTab = menu;

        // If there is a selected object on the UI we un-unselect the object
        if (ObjectDraggingHandler.GetIsDraggingEnabled())
        {
            ObjectDraggingHandler.DisableDragging();
        }

        // we enable menu background
        menuBackgroundController.Enable();
    }

    private bool IsClickOutside()
    {
        foreach (RectTransform rect in visibleRects)
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
        if (!scrollViewContent)
        {
            return;
        }

        // Clear ScrollView
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        //Set top panel title
        SetMainMenuTitle(MenuObjectList.GetButtonLabel(menu.GetMenuTab()));
        SetCellSize(400, 400);

        //Clear scrollview content list
        storeInventoryItemControllerList.Clear();
        upgradeItemControllerList.Clear();

        // Add items to scroll view depending on the tab
        switch (menu.GetMenuTab())
        {
            case MenuTab.STORE_ITEMS: AddItemsToScrollView(menu); StartCoroutine(ScrollToTop()); return;
            case MenuTab.UPGRADE: AddItemsToUpgradeScrollView(menu); StartCoroutine(ScrollToTop()); return;
            // case MenuTab.IN_GAME_STORE_TAB: /*TODO*/ return;
            case MenuTab.STORAGE_TAB: AddStorageItemsToScrollView(); StartCoroutine(ScrollToTop()); return;
            case MenuTab.SETTINGS_TAB: AddSettingsItemsToScrollView(); StartCoroutine(ScrollToTop()); return;
        }
    }

    private IEnumerator ScrollToTop()
    {
        yield return new WaitForSeconds(0.001f);
        // We scroll to the top of the scroll view
        ScrollRect scroll = scrollView.GetComponent<ScrollRect>();
        scroll.verticalNormalizedPosition = 1f;
    }

    private void AddSettingsItemsToScrollView()
    {
        GameObject settings = Instantiate(Resources.Load(Settings.PrefabSettingsItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
        settings.transform.SetParent(scrollViewContent.transform);
        settings.transform.localScale = new Vector3(1, 1, 1);
        SetCellSize(1000, 1200);
    }

    private void AddStorageItemsToScrollView()
    {
        List<StoreGameObject> objects = MenuObjectList.GetItemList(MenuTab.STORAGE_TAB);
        GameObject item;
        InventoryItemController inventoryItemController;
        Button button;
        Dictionary<StoreGameObject, int> objectDic = new Dictionary<StoreGameObject, int>();
        Dictionary<StoreGameObject, Pair<StoreGameObject, DataGameObject>> dicPair = new Dictionary<StoreGameObject, Pair<StoreGameObject, DataGameObject>>();
        List<DataGameObject> userStorage = MenuObjectList.LoadCurrentUserStorage();

        foreach (DataGameObject fireObj in userStorage)
        {
            StoreGameObject obj = MenuObjectList.GetStoreObject((StoreItemType)fireObj.ID);

            int count = objectDic.GetValueOrDefault(obj, 0) + 1;
            if (objectDic.ContainsKey(obj))
            {
                objectDic.Remove(obj);
            }
            else
            {
                Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>(obj, fireObj);
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
            item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            inventoryItemController = item.GetComponent<InventoryItemController>();
            button = inventoryItemController.GetButton();
            //TODO: max employees we can define the number of counters here
            button.onClick.AddListener(() => OpenStoreEditPanel(dicPair[entry.Key], true));

            inventoryItemController.SetInventoryItem(entry.Key.MenuItemSprite, entry.Value.ToString(), entry.Key);
            item.transform.SetParent(scrollViewContent.transform);
            item.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void AddItemsToUpgradeScrollView(MenuItem menu)
    {
        List<StoreGameObject> objects = MenuObjectList.GetItemList(menu.GetMenuTab());
        GameObject item;
        UpgradeItemController upgradeItemController;
        Button button;

        // Add new Items
        foreach (StoreGameObject obj in objects)
        {
            item = Instantiate(Resources.Load(Settings.PrefabUpgradeInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            upgradeItemController = item.GetComponent<UpgradeItemController>();
            button = upgradeItemController.GetButton();
            Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>
            {
                Key = obj
            };

            // Adding click listener
            // TODO: max employees we can define the number of counters here
            if (obj.Cost <= PlayerData.GetMoneyDouble())
            {
                //button.onClick.AddListener(() => OpenStoreEditPanel(pair, false));
            }

            upgradeItemController.SetInventoryItem(obj.MenuItemSprite, obj.Cost.ToString(), obj.StoreItemType);
            item.transform.SetParent(scrollViewContent.transform);
            item.transform.localScale = new Vector3(1, 1, 1);
            upgradeItemControllerList.Add(upgradeItemController);
        }
    }

    private void AddItemsToScrollView(MenuItem menu)
    {
        List<StoreGameObject> objects = MenuObjectList.GetItemList(menu.GetMenuTab());
        GameObject item;
        InventoryItemController inventoryItemController;
        Button button;

        // Add new Items
        foreach (StoreGameObject obj in objects)
        {
            item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            inventoryItemController = item.GetComponent<InventoryItemController>();
            button = inventoryItemController.GetButton();
            Pair<StoreGameObject, DataGameObject> pair = new Pair<StoreGameObject, DataGameObject>
            {
                Key = obj
            };

            // Adding click listener
            // TODO: max employees we can define the number of counters here
            if (obj.Cost <= PlayerData.GetMoneyDouble())
            {
                button.onClick.AddListener(() => OpenStoreEditPanel(pair, false));
            }
            else
            {
                inventoryItemController.SetUnavailable();
            }

            inventoryItemController.SetInventoryItem(obj.MenuItemSprite, obj.Cost.ToString(), obj);
            item.transform.SetParent(scrollViewContent.transform);
            item.transform.localScale = new Vector3(1, 1, 1);
            storeInventoryItemControllerList.Add(inventoryItemController);
        }
    }

    private void SetLeftDownPanelClickListeners()
    {
        storeButton = leftDownPanel.transform.Find(Settings.ConstLeftDownMenuStore).gameObject;
        Button bStore = storeButton.GetComponent<Button>();
        bStore.onClick.AddListener(() => OpenMenu(centerTabMenu));
    }

    //ConstLeftDownMenuStore CenterPanel open button
    private void HideMainMenuButton()
    {
        storeButton.SetActive(false);
    }

    private void ShowMainMenuButton()
    {
        storeButton.SetActive(true);
    }

    private void OpenStoreEditPanel(Pair<StoreGameObject, DataGameObject> pair, bool storage)
    {
        StoreGameObject obj = pair.Key;

        if (!PlayerData.CanSubtract(obj.Cost) && !storage)
        {
            GameLog.Log("TODO: UI message: Insufficient funds " + PlayerData.GetMoney());
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
            pair.Value.ROTATION = (int)baseObjectController.GetInitialRotation();
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
            placeGameObject(obj);
        }
        // After we have waited 5 seconds print the time again.
        yield return new WaitForSeconds(0);
    }

    public bool IsMenuOpen()
    {
        return centerPanel.activeSelf;
    }

    private static GameObject PlaceAtCameraSquare(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        Vector3 spamPosition = BussGrid.GetGridWorldPositionMapMouseDrag(Util.GetCameraPoisiton());
        GameObject newObject;

        newObject = Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, Util.SelectedObjectZPosition), Quaternion.identity, parent.transform) as GameObject;
        return newObject;
    }

    private static GameObject PlaceSingleTileObject(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        Vector3 spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextFreeTile());
        GameObject newObject;

        if (Util.CompareNegativeInifinity(spamPosition))
        {
            return null;
        }
        else
        {
            newObject = Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, Util.SelectedObjectZPosition), Quaternion.identity, parent.transform) as GameObject;
        }

        return newObject;
    }

    private static GameObject placeGameObject(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        GameObject newObject;
        Vector3 spamPosition;

        if (BussGrid.GetGameGridObjectsDictionary().Count == 0)
        {
            spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextTileFromEmptyMap(obj));
            return spamPosition == Util.GetVector3IntNegativeInfinity() ? null : Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, Util.SelectedObjectZPosition), Quaternion.identity, parent.transform) as GameObject;
        }

        foreach (KeyValuePair<string, GameGridObject> dic in BussGrid.GetGameGridObjectsDictionary())
        {
            GameGridObject current = dic.Value;
            Vector3Int[] nextTile = BussGrid.GetNextFreeTileWithActionPoint(current);

            if (nextTile.GetLength(0) != 0)
            {
                // We place the object 
                spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(nextTile[0]);
                bool inverted = true;

                if (nextTile[1] == Vector3Int.up)// Vector3Int.up == front
                {
                    inverted = false;
                }

                newObject = Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, Util.SelectedObjectZPosition), Quaternion.identity, parent.transform) as GameObject;
                // There can be only one counter at the tinme
                BaseObjectController baseObjectController = newObject.GetComponent<BaseObjectController>();
                baseObjectController.SetInitialObjectRotation(inverted ? ObjectRotation.FRONT_INVERTED : ObjectRotation.FRONT);
                return newObject;
            }
        }

        return null;
    }

    private void EmptyClickListener()
    {
        return;
    }

    private void CloseCenterPanel()
    {
        centerPanel.SetActive(false);
        ShowMainMenuButton();
    }

    private void OpenCenterPanel()
    {
        HideMainMenuButton();
        centerPanel.SetActive(true);
        // Selecting the button at the same time of opening the menu
        tableTabButton.Select();
    }

    private void SetCellSize(float sizeX, float sizeY)
    {
        gridLayoutGroup.cellSize = new Vector2(sizeX, sizeY);
    }
}
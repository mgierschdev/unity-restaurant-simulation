using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This will be only element attached in the UI
// All the bottom calls will be handled by this class.
public class MenuHandlerController : MonoBehaviour
{
    private GameObject centerPanel;
    private GameObject centerPanelSideMenu;
    //saves the latest reference to the npc if the menu was opened
    private NPCController npc;
    private EmployeeController employee;
    private MenuItem centerTabMenu;
    // Click controller
    private ClickController clickController;
    // Min amount of time the the menu has to be open before activating -> closing on click outside
    private const float MIN_OPENED_TIME = 0.5f;
    private float openedTime;
    // Menu realtime refresh rate
    private const float MENU_REFRESH_RATE = 3f;
    private GameObject leftDownPanel;
    private TextMeshProUGUI moneyText;
    private List<RectTransform> visibleRects;
    private MenuBackgroundController menuBackgroundController;

    // MenuHandlerController Attached to CanvasMenu Parent of all Menus
    private void Awake()
    {
        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
        clickController = cController.GetComponent<ClickController>();

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

        // Menu Background Controller 
        menuBackgroundController = GameObject.Find(Settings.MenuContainer).GetComponent<MenuBackgroundController>();
        if (menuBackgroundController == null)
        {
            GameLog.LogWarning("MenuHandlerController.cs/menuBackgroundController is null");
        }

        // Menu Body
        centerPanel = GameObject.Find(Settings.ConstCenterTabMenu);
        centerPanelSideMenu = centerPanel.transform.Find("ButtonMenuPanel").gameObject;
        GameObject CenterPanelViewPanel = centerPanel.transform.Find("ViewPanel").gameObject;
        visibleRects = new List<RectTransform>{
            centerPanelSideMenu.GetComponent<RectTransform>(),
            CenterPanelViewPanel.GetComponent<RectTransform>()
        };

        if (!centerPanel || !leftDownPanel || !cController)
        {
            GameLog.LogWarning("UIHandler Menu null ");
            GameLog.LogWarning("tabMenu " + centerPanel);
            GameLog.LogWarning("leftDownPanel " + leftDownPanel);
            GameLog.LogWarning("cController " + cController);
        }

        //Center tab menus tabs
        centerTabMenu = new MenuItem(MenuTab.TABLES_TAB, MenuType.TAB_MENU, Settings.ConstCenterTabMenu);

        LoadCenterPanelSideMenu();
        // Setting Click Listeners to Left Down Panel
        SetLeftDownPanelClickListeners();
        LoadCenterPanelSideMenu();

        CloseCenterPanel();
        openedTime = 0;
    }

    private void LoadCenterPanelSideMenu()
    {
        // Clear the dev button view
        foreach (Transform child in centerPanelSideMenu.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (MenuTab tab in MenuTab.GetValues(typeof(MenuTab)))
        {
            GameObject button = Instantiate(Resources.Load(Settings.SideMenuButton, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            CenterTabMenuBottonController controller = button.GetComponent<CenterTabMenuBottonController>();
            controller.SetText(MenuObjectList.GetButtonLabel(tab));
            button.transform.SetParent(centerPanelSideMenu.transform);
            Button bStore = controller.GetButton();
            MenuItem current = new MenuItem(tab, MenuType.TAB_MENU, tab.ToString());
            bStore.onClick.AddListener(() => AddMenuItemsToScrollView(current));
        }
    }

    private bool CanCloseOnClickOutside()
    {
        return openedTime > MIN_OPENED_TIME;
    }

    private void CheckCLickControl()
    {
        if (clickController == null || !clickController.GetClickedObject())
        {
            return;
        }

        Util.GetObjectType(clickController.GetClickedObject());

        if (clickController.GetClickedObject().name.Contains(Settings.PrefabNpcEmployee))
        {
            return;
        }
        // We reset the clicked object after the action
        clickController.SetClickedObject(null);
        if (clickController.GetClickedGameTile() != null)
        {
            clickController.SetClickedGameTile(null);
        }
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
    }

    private void OpenMenu(MenuItem menu)
    {
        // Open all menus except settings and InGameStore
        AddMenuItemsToScrollView(menu);
        OpenCenterPanel();

        // If there is a selected object on the UI we un-unselect the object
        if (BussGrid.GetIsDraggingEnabled())
        {
            BussGrid.DisableDragging();
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
        GameObject scrollView = centerPanel.transform.Find(Settings.ConstCenterScrollContent).gameObject;

        if (!scrollView)
        {
            return;
        }

        // Clear ScrollView
        foreach (Transform child in scrollView.transform)
        {
            Destroy(child.gameObject);
        }

        List<StoreGameObject> objects = MenuObjectList.GetItemList(menu.GetMenuTab());

        // Add new Items
        foreach (StoreGameObject obj in objects)
        {
            GameObject item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            InventoryItemController inventoryItemController = item.GetComponent<InventoryItemController>();
            Button button = inventoryItemController.GetButton();

            // Adding click listener
            if (obj.Cost <= PlayerData.GetMoneyDouble())
            {
                button.onClick.AddListener(() => OpenStoreEditPanel(obj));
            }
            else
            {
                inventoryItemController.SetBackground(Util.Unavailable);
            }

            inventoryItemController.SetInventoryItem(obj.MenuItemSprite, obj.Cost.ToString());
            item.transform.SetParent(scrollView.transform);
            item.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void SetLeftDownPanelClickListeners()
    {
        GameObject store = leftDownPanel.transform.Find(Settings.ConstLeftDownMenuStore).gameObject;
        Button bStore = store.GetComponent<Button>();
        bStore.onClick.AddListener(() => OpenMenu(centerTabMenu));
    }

    private void OpenStoreEditPanel(StoreGameObject obj)
    {
        if (!PlayerData.CanSubtract(obj.Cost))
        {
            GameLog.Log("TODO: UI message: Insufficient funds " + PlayerData.GetMoney());
            return;
        }

        CloseMenu();
        GameObject newObject;

        if (obj.HasActionPoint)
        {
            newObject = placeGameObject(obj);
        }
        else
        {
            newObject = PlaceSingleTileObject(obj);
        }



        if (newObject != null)
        {
            BaseObjectController controller = null;

            if (obj.Type == ObjectType.NPC_SINGLE_TABLE)
            {
                controller = newObject.GetComponent<TableController>();
            }
            else if (obj.Type == ObjectType.NPC_COUNTER)
            {
                controller = newObject.GetComponent<CounterController>();
            }
            else if (obj.Type == ObjectType.BASE_CONTAINER)
            {
                controller = newObject.GetComponent<BaseContainerController>();
            }

            controller.SetNewItem(true);
        }
        else
        {
            GameLog.Log("TODO: UI message: Place not found");
        }
    }
    IEnumerator TestPlacingObjects(StoreGameObject obj)
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

    private static GameObject PlaceSingleTileObject(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        Vector3 spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextFreeTile());
        return spamPosition == Util.GetVector3IntPositiveInfinity() ? null : Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, 1), Quaternion.identity, parent.transform) as GameObject;

    }

    private static GameObject placeGameObject(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        GameObject newObject;
        Vector3 spamPosition;

        if (BussGrid.BusinessObjects.Count == 0)
        {
            spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextTileFromEmptyMap(obj));
            return spamPosition == Util.GetVector3IntPositiveInfinity() ? null : Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, 1), Quaternion.identity, parent.transform) as GameObject;
        }

        foreach (KeyValuePair<string, GameGridObject> dic in BussGrid.BusinessObjects)
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

                newObject = Instantiate(Resources.Load(MenuObjectList.GetPrefab(obj.StoreItemType), typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, 1), Quaternion.identity, parent.transform) as GameObject;
                BaseObjectController baseObjectController;

                // There can be only one counter at the tinme
                if (obj.Type == ObjectType.NPC_COUNTER && BussGrid.GetCounter() == null)
                {
                    baseObjectController = newObject.GetComponent<CounterController>();
                }
                else
                {
                    baseObjectController = newObject.GetComponent<TableController>();
                }

                baseObjectController.SetInitialObjectRotation(inverted ? ObjectRotation.FRONT_INVERTED : ObjectRotation.FRONT);
                return newObject;
            }
        }

        return null;
    }

    private void CloseCenterPanel()
    {
        centerPanel.SetActive(false);
    }

    private void OpenCenterPanel()
    {
        centerPanel.SetActive(true);
    }
}
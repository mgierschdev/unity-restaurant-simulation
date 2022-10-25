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
    private NPCController npc; //saves the latest reference to the npc if the menu was opened
    private EmployeeController employee;
    private MenuItem centerTabMenu;
    private Stack<MenuItem> menuStack;
    private HashSet<string> openMenus;
    // private bool isGamePaused;
    // Click controller
    private ClickController clickController;
    // Min amount of time the the menu has to be open before activating -> closing on click outside
    private const float MIN_OPENED_TIME = 0.5f;
    private float openedTime;
    // Menu realtime refresh rate
    private const float MENU_REFRESH_RATE = 3f;
    private GameObject leftDownPanel;
    //  private GameObject editStoreMenuPanel;
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
        GameObject CenterPanelSideMenu = centerPanel.transform.Find("ButtonMenuPanel").gameObject;
        GameObject CenterPanelViewPanel = centerPanel.transform.Find("ViewPanel").gameObject;
        visibleRects = new List<RectTransform>{
            CenterPanelSideMenu.GetComponent<RectTransform>(),
            CenterPanelViewPanel.GetComponent<RectTransform>()
        };

        if (!centerPanel || !leftDownPanel || !cController)
        {
            GameLog.LogWarning("UIHandler Menu null ");
            GameLog.LogWarning("tabMenu " + centerPanel);
            GameLog.LogWarning("leftDownPanel " + leftDownPanel);
            GameLog.LogWarning("cController " + cController);
        }

        menuStack = new Stack<MenuItem>();
        openMenus = new HashSet<string>();
        centerTabMenu = new MenuItem(Menu.TABLES_TAB, MenuType.TAB_MENU, Settings.ConstCenterTabMenu, centerPanel);

        // Setting Click Listeners to Left Down Panel
        SetLeftDownPanelClickListeners();

        centerTabMenu.Close();
        openedTime = 0;
    }


    // private void TimeControl()
    // {
    //     if (menuStack == null)
    //     {
    //         return;
    //     }

    //     // Handles for how long before activating CloseOnCLickOutside
    //     if (menuStack.Count > 0)
    //     {
    //         openedTime += Time.unscaledDeltaTime;

    //         // Handles UI refresh rate 
    //         MenuItem menu = menuStack.Peek();

    //         if (menu.Menu == Menu.NPC_PROFILE && openedTime > MENU_REFRESH_RATE)
    //         {
    //             openedTime = 0;
    //         }
    //     }
    //     else
    //     {
    //         openedTime = 0.0f;
    //     }
    // }

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

    private void CloseAllMenus()
    {
        while (menuStack.Count > 0)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        // We disable the image menu backgrounds
        if (menuBackgroundController.IsActive())
        {
            menuBackgroundController.Disable();
        }

        if (menuStack.Count > 0)
        {
            MenuItem menu = menuStack.Pop();
            menu.Close();
            openMenus.Remove(menu.Name);
            // if (menu.PauseGame)
            // {
            //     HandleTimeScale();
            // }
        }
    }

    private void OpenMenu(MenuItem menu)
    {
        if (openMenus.Contains(menu.Name))
        {
            return;
        }

        if (menu.Type == MenuType.TAB_MENU)
        {
            AddMenuItemsToScrollView(centerTabMenu);
        }

        menu.UnityObject.SetActive(true);
        menuStack.Push(menu);
        openMenus.Add(menu.Name);

        // If there is a selected object on the UI we un-unselect the object
        if (BussGrid.GetIsDraggingEnabled())
        {
            BussGrid.DisableDragging();
        }

        // we enable menu background
        menuBackgroundController.Enable();
    }

    // private void HandleTimeScale()
    // {
    //     if (menuStack.Count > 0)
    //     {
    //         PauseGame();
    //     }
    //     else
    //     {
    //         ResumeGame();
    //     }
    // }

    // private void PauseGame()
    // {
    //     Time.timeScale = 0;
    //     isGamePaused = true;
    // }

    // private void ResumeGame()
    // {
    //     Time.timeScale = 1;
    //     isGamePaused = false;
    // }

    private bool IsClickOutside()
    {
        if (menuStack.Count <= 0)
        {
            return false;
        }

        MenuItem menu = menuStack.Peek();

        if (menu.Type == MenuType.TAB_MENU)
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

        return !RectTransformUtility.RectangleContainsScreenPoint(menu.UnityObject.GetComponent<RectTransform>(), Input.mousePosition);
    }

    private void AddMenuItemsToScrollView(MenuItem menu)
    {
        GameObject scrollView = menu.UnityObject.transform.Find(Settings.ConstCenterScrollContent).gameObject;

        if (!scrollView)
        {
            return;
        }

        // Clear ScrollView
        foreach (Transform child in scrollView.transform)
        {
            Destroy(child.gameObject);
        }

        // Add new Items
        foreach (StoreGameObject obj in MenuObjectList.AllStoreItems)
        {
            GameObject item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            Button button = item.GetComponent<Button>();
            GameObject img = item.transform.Find(Settings.PrefabInventoryItemImage).gameObject;
            Image image = img.GetComponent<Image>();
            // Adding click listener
            if (obj.Cost <= PlayerData.GetMoneyDouble())
            {
                button.onClick.AddListener(() => OpenStoreEditPanel(obj));
            }
            else
            {
                image.color = Util.Unavailable;
            }

            GameObject text = item.transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
            TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
            textMesh.text = obj.Cost.ToString();

            Image imgComponent = img.GetComponent<Image>();
            Sprite sp = Resources.Load<Sprite>(obj.MenuItemSprite);

            imgComponent.sprite = sp;
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

        CloseAllMenus();

        // Load testing debug
        // StartCoroutine(TestPlacingObjects(obj));
        // Load testing debug

        GameObject newObject = placeGameObject(obj);

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
        return menuStack.Count > 0;
    }

    private static GameObject placeGameObject(StoreGameObject obj)
    {
        GameObject parent = GameObject.Find(Settings.TilemapObjects);
        GameObject newObject;
        Vector3 spamPosition;

        if (BussGrid.BusinessObjects.Count == 0)
        {
            spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(BussGrid.GetNextTileFromEmptyMap(obj));
            return spamPosition == null ? null : Instantiate(Resources.Load(Settings.PrefabSingleTable, typeof(GameObject)), new Vector3(spamPosition.x, spamPosition.y, 1), Quaternion.identity, parent.transform) as GameObject;
        }

        foreach (KeyValuePair<string, GameGridObject> dic in BussGrid.BusinessObjects)
        {
            GameGridObject current = dic.Value;
            Vector3Int[] nextTile = BussGrid.GetNextTileWithActionPoint(current);

            if (nextTile.GetLength(0) != 0)
            {
                // We place the object 
                spamPosition = BussGrid.GetWorldFromPathFindingGridPosition(nextTile[0]);
                bool inverted = true;

                if (nextTile[1] == Vector3Int.up)// Vector3Int.up //front
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
}
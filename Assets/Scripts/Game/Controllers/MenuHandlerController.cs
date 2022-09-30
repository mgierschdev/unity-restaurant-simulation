using System.Collections;
using System.Collections.Generic;
using Game.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This will be only element attached in the UI
// All the bottom calls will be handled by this class.
public class MenuHandlerController : MonoBehaviour
{
    private GameObject centerPanel;
    private MenuItem npcProfileMenu;
    private NPCController npc; //saves the latest reference to the npc if the menu was opened
    private EmployeeController employee;
    private MenuItem centerTabMenu;
    private Stack<MenuItem> menuStack;
    private HashSet<string> openMenus;
    private bool isGamePaused;
    // Click controller
    private ClickController clickController;
    //Min amount of time the the menu has to be open before activating -> closing on click outside
    private const float MIN_OPENED_TIME = 0.5f;
    private float openedTime;
    //Menu realtime refresh rate
    private const float MENU_REFRESH_RATE = 3f;
    private GameObject leftDownPanel;
    private GameObject editStoreMenuPanel;
    private GridController gridController;
    private PlayerData playerData;
    private TextMeshProUGUI moneyText;
    private List<RectTransform> visibleRects;
    private MenuBackgroundController menuBackgroundController;

    // MenuHandlerController Attached to CanvasMenu Parent of all Menus
    private void Awake()
    {
        // Grid Controller
        GameObject gridObj = GameObject.Find(Settings.GameGrid).gameObject;
        gridController = gridObj.GetComponent<GridController>();

        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
        clickController = cController.GetComponent<ClickController>();

        //Left down panel and Edit store panel
        leftDownPanel = GameObject.Find(Settings.ConstLeftDownPanel).gameObject;
        editStoreMenuPanel = GameObject.Find(Settings.ConstEditStoreMenuPanel).gameObject;
        GameObject npcProfileGameObject = transform.Find(Settings.ConstNpcProfileMenu).gameObject;

        //Menu Background Controller 
        menuBackgroundController = GameObject.Find(Settings.MenuContainer).GetComponent<MenuBackgroundController>();
        if (menuBackgroundController == null)
        {
            Debug.Log("MenuHandlerController.cs/menuBackgroundController is null");
        }

        // Menu Body
        // TODO: repeated code 
        centerPanel = GameObject.Find(Settings.ConstCenterTabMenu);
        GameObject CenterPanelSideMenu = centerPanel.transform.Find("ButtonMenuPanel").gameObject;
        GameObject CenterPanelViewPanel = centerPanel.transform.Find("ViewPanel").gameObject;
        visibleRects = new List<RectTransform>{
            CenterPanelSideMenu.GetComponent<RectTransform>(),
            CenterPanelViewPanel.GetComponent<RectTransform>()
        };

        if (!centerPanel || !leftDownPanel || !cController || !gridController)
        {
            GameLog.LogWarning("UIHandler Menu null ");
            GameLog.LogWarning("tabMenu " + centerPanel);
            GameLog.LogWarning("leftDownPanel " + leftDownPanel);
            GameLog.LogWarning("cController " + cController);
            GameLog.LogWarning("gridController " + cController);
        }

        menuStack = new Stack<MenuItem>();
        openMenus = new HashSet<string>();

        centerTabMenu = new MenuItem(Menu.CENTER_TAB_MENU, MenuType.TAB_MENU, Settings.ConstCenterTabMenu, centerPanel, true);
        npcProfileMenu = new MenuItem(Menu.NPC_PROFILE, MenuType.DIALOG, Settings.ConstNpcProfileMenu, npcProfileGameObject, false);

        //Setting Click Listeners to Left Down Panel
        SetLeftDownPanelClickListeners();
        SetEditStorePanelClickListeners();

        editStoreMenuPanel.SetActive(false);
        centerTabMenu.Close();
        npcProfileMenu.Close();
        openedTime = 0;
    }


    private void TimeControl()
    {
        if (menuStack == null)
        {
            return;
        }

        //Handles for how long before activating CloseOnCLickOutside
        if (menuStack.Count > 0)
        {
            openedTime += Time.unscaledDeltaTime;

            //Handles UI refresh rate 
            MenuItem menu = menuStack.Peek();

            if (menu.Menu == Menu.NPC_PROFILE && openedTime > MENU_REFRESH_RATE)
            {
                RefreshNpcProfile();
                openedTime = 0;
            }
        }
        else
        {
            openedTime = 0.0f;
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

        ObjectType type = Util.GetObjectType(clickController.GetClickedObject());

        if (clickController.GetClickedObject().name.Contains(Settings.PrefabNpcEmployee))
        {
            return;
        }

        if (type == ObjectType.NPC || type == ObjectType.EMPLOYEE)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            if (type == ObjectType.NPC)
            {
                npc = clickController.GetClickedObject().GetComponent<NPCController>();
                map.Add("Name", npc.Name);
                map.Add("Debug", npc.GetDebugInfo());
            }

            if (type == ObjectType.EMPLOYEE)
            {
                employee = clickController.GetClickedObject().GetComponent<EmployeeController>();
                map.Add("Name", employee.Name);
                map.Add("Debug", employee.GetDebugInfo());
            }

            OpenMenu(npcProfileMenu);
            npcProfileMenu.SetFields(map);
        }

        // We reset the clicked object after the action
        clickController.SetClickedObject(null);
        if (clickController.GetClickedGameTile() != null)
        {
            clickController.SetClickedGameTile(null);
        }
    }

    private void RefreshNpcProfile()
    {
        // The NPC may not longer exist
        if (!npc)
        {
            return;
        }

        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "Name", npc.Name },
            { "Debug", npc.GetDebugInfo() }
        };
        OpenMenu(npcProfileMenu);
        npcProfileMenu.SetFields(map);
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
        //We disable the image menu backgrounds
        if(menuBackgroundController.IsActive()){
            menuBackgroundController.Disable();
        }

        if (menuStack.Count > 0)
        {
            MenuItem menu = menuStack.Pop();
            menu.Close();
            openMenus.Remove(menu.Name);
            if (menu.PauseGameGame)
            {
                HandleTimeScale();
            }
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
        if (menu.PauseGameGame)
        {
            HandleTimeScale();
        }

        // we enable menu background
        menuBackgroundController.Enable();
    }

    private void HandleTimeScale()
    {
        if (menuStack.Count > 0)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        isGamePaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        isGamePaused = false;
    }

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

        //Clear ScrollView
        foreach (Transform child in scrollView.transform)
        {
            Destroy(child.gameObject);
        }

        //Add new Items
        foreach (StoreGameObject obj in gridController.GetObjectListConfiguration().AllStoreItems)
        {
            GameObject item = Instantiate(Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            Button button = item.GetComponent<Button>();
            GameObject img = item.transform.Find(Settings.PrefabInventoryItemImage).gameObject;
            Image image = img.GetComponent<Image>();
            //Adding click listener
            if (obj.Cost <= gridController.playerData.GetMoneyDouble())
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

        GameObject inventory = leftDownPanel.transform.Find(Settings.ConstLeftDownMenuInventory).gameObject;
        Button bInventory = inventory.GetComponent<Button>();
        bInventory.onClick.AddListener(OpenEditPanel);
    }

    private void SetEditStorePanelClickListeners()
    {
        GameObject cancel = editStoreMenuPanel.transform.Find(Settings.ConstEditStoreMenuCancel).gameObject;
        Button bCancel = cancel.GetComponent<Button>();
        bCancel.onClick.AddListener(CloseEditPanel);
    }

    private void OpenStoreEditPanel(StoreGameObject obj)
    {
        if (!gridController.playerData.CanSubtract(obj.Cost))
        {
            GameLog.Log("TODO: Insufficient funds " + gridController.playerData.GetMoney());
            return;
        }

        // GameLog.Log("Object to find a Place for " + obj.Name);
        // we fix the camera in case the player is zoomed
        CloseAllMenus();
        gridController.HighlightGridBussFloor();

        // Load test debug
        // StartCoroutine(TestPlacingObjects(obj));
        // Load test debug

        if (gridController.PlaceGameObject(obj))
        {
            gridController.playerData.Subtract(obj.Cost);
            // GameLog.Log("TODO: Object Placed discounting " + (-obj.Cost));
        }
        else
        {
            // GameLog.Log("TODO: Place not found");
        }

        //Disable Left down panel
        PauseGame();
        leftDownPanel.SetActive(false);
        editStoreMenuPanel.SetActive(true);
        //enabling background image
        menuBackgroundController.Disable();
    }
    IEnumerator TestPlacingObjects(StoreGameObject obj)
    {
        //Print the time of when the function is first called.
        // Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.15f); // 0.15f
            // Debug.Log("Placing object");
            gridController.PlaceGameObject(obj);
        }
        //After we have waited 5 seconds print the time again.
        // Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(0);
    }
    
    private void OpenEditPanel()
    {
        // we fix the camera in case the player is zoomed
        CloseAllMenus();
        gridController.HighlightGridBussFloor();
        PauseGame();
        leftDownPanel.SetActive(false);
        editStoreMenuPanel.SetActive(true);
        // We disable 
        menuBackgroundController.Disable();
    }

    private void OpenEmployeePanel()
    {
        GameLog.Log("Opening Employee panel"); // TODO
    }

    // Closes the edit panel without changes 
    private void CloseEditPanel()
    {
        editStoreMenuPanel.SetActive(false);
        leftDownPanel.SetActive(true);
        gridController.HideGridBussFloor();
        ResumeGame();
    }

    public bool IsEditPanelOpen()
    {
        return editStoreMenuPanel.activeSelf;
    }

    private void ItemClicked()
    {
        //GameLog.Log("Clicking inventory/bEmployees");
    }

    public void InventoryItemClicked(GameGridObject obj)
    {
        GameLog.Log("Button Clicked " + obj.Name);
    }

    public bool IsMenuOpen()
    {
        return menuStack.Count > 0;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This will be only element attached in the UI
// All the buttom calls will be handled by this class.
public class MenuHandlerController : MonoBehaviour
{
    private GameObject tabMenu;
    private MenuItem npcProfileMenu;
    NPCController npc; //saves the latest reference to the npc if the menu was openned
    EmployeeController employee;
    private MenuItem centerTabMenu;
    private Stack<MenuItem> menuStack;
    private HashSet<string> openMenus;
    private bool isGamePaused;

    // Click controller
    private ClickController clickController;

    //Min  ammount of time the the menu has to be open before activating -> closing on click outisde
    private float minOpenedTime = 0.5f;
    private float openedTime;
    //Menu realtime refreshrate
    private float menuRefreshRate = 3f;
    private MenuObjectList storeList;
    private GameObject leftDownPanel;
    private GameObject editStoreMenuPanel;
    private GridController gridController;

    // MenuHandlerController Attached to CanvasMenu Parent of all Menus
    private void Start()
    {
        // Grid Controller
        GameObject gridObj = GameObject.Find(Settings.GAME_GRID).gameObject;
        gridController = gridObj.GetComponent<GridController>();

        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.CONST_PARENT_GAME_OBJECT);
        clickController = cController.GetComponent<ClickController>();

        //Left down panel and Edit store panel
        leftDownPanel = GameObject.Find(Settings.CONST_LEFT_DOWN_PANEL).gameObject;
        editStoreMenuPanel = GameObject.Find(Settings.CONST_EDIT_STORE_MENU_PANEL).gameObject;

        //Containing all Inventory Items
        storeList = new MenuObjectList();

        tabMenu = transform.Find(Settings.CONST_CENTER_TAB_MENU).gameObject;
        GameObject npcProfileGameObject = transform.Find(Settings.CONST_NPC_PROFILE_MENU).gameObject;

        if (tabMenu == null || leftDownPanel == null || cController == null || gridController == null)
        {
            Debug.LogWarning("MenuHandlerController Menu null ");
            Debug.LogWarning("tabMenu " + tabMenu);
            Debug.LogWarning("leftDownPanel " + leftDownPanel);
            Debug.LogWarning("cController " + cController);
            Debug.LogWarning("gridController " + cController);
        }

        menuStack = new Stack<MenuItem>();
        openMenus = new HashSet<string>();

        centerTabMenu = new MenuItem(Menu.CENTER_TAB_MENU, MenuType.TAB_MENU, Settings.CONST_CENTER_TAB_MENU, tabMenu, true);
        npcProfileMenu = new MenuItem(Menu.NPC_PROFILE, MenuType.DIALOG, Settings.CONST_NPC_PROFILE_MENU, npcProfileGameObject, false);

        //Adding inventory Items Tables
        SetClickListeners(npcProfileMenu);

        //Adding Scroll content
        AddMenuItemsToScrollView(centerTabMenu);

        //Setting Click Listeners to Left Down Panel
        SetLeftDownPanelClickListeners();
        SetEditStorePanelClickListeners();

        editStoreMenuPanel.SetActive(false);
        centerTabMenu.Close();
        npcProfileMenu.Close();

        openedTime = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanCloseOnClickOutside() && IsClickOutside() && !clickController.IsLongClick)
        {
            CloseMenu();
        }

        //Min  ammount of time the the menu has to be open before activating -> closing on click outisde
        TimeControl();

        // Checks for clicks to the objects in the UI
        CheckCLickControl();
    }

    private void TimeControl()
    {
        if (menuStack == null)
        {
            return;
        }

        //Handles for how long abefore activating CloseOnCLickOutside
        if (menuStack.Count > 0)
        {
            openedTime += Time.unscaledDeltaTime;

            //Handles UI refresh rate 
            MenuItem menu = menuStack.Peek();

            if (menu.Menu == Menu.NPC_PROFILE && openedTime > menuRefreshRate)
            {
                RefresNPCProfile();
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
        return openedTime > minOpenedTime;
    }

    private void CheckCLickControl()
    {
        if (clickController.ClickedObject != null)
        {
            ObjectType type = Util.GetObjectType(clickController.ClickedObject);

            if (type == ObjectType.NPC || type == ObjectType.EMPLOYEE)
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                if (type == ObjectType.NPC)
                {
                    npc = clickController.ClickedObject.GetComponent<NPCController>();
                    map.Add("Name", npc.Name);
                    map.Add("Debug", npc.GetDebugInfo());
                }

                if (type == ObjectType.EMPLOYEE)
                {
                    employee = clickController.ClickedObject.GetComponent<EmployeeController>();
                    map.Add("Name", employee.Name);
                    map.Add("Debug", employee.GetDebugInfo());
                }

                OpenMenu(npcProfileMenu);
                npcProfileMenu.SetFields(map);
            }

            // We reset the clicked object after the action
            clickController.ClickedObject = null;
        }

        if (clickController.ClickedGameTile != null)
        {
            clickController.ClickedGameTile = null;
        }
    }

    private void RefresNPCProfile()
    {
        // The NPC may not longer exist
        if (npc == null)
        {
            return;
        }

        Dictionary<string, string> map = new Dictionary<string, string>
           {
           {"Name", npc.Name},
           {"Debug", npc.GetDebugInfo()}
           };
        OpenMenu(npcProfileMenu);
        npcProfileMenu.SetFields(map);
    }

    private void SetClickListeners(MenuItem menu)
    {
        GameObject menuGameObject = menu.UnityObject;

        foreach (string buttonName in menu.Buttons)
        {
            GameObject currentComponent = GameObject.Find(buttonName);

            if (currentComponent != null)
            {
                Button buttonListener = currentComponent.GetComponent<Button>();

                if (buttonName == Settings.CONST_UI_EXIT_BUTTON)
                {
                    buttonListener.onClick.AddListener(() => CloseMenu());
                }

                if (buttonName == Settings.CONST_UI_INVENTORY_BUTTON)
                {
                    buttonListener.onClick.AddListener(() => OpenMenu(centerTabMenu));
                }
            }
        }
    }

    private void CloseAllMenus()
    {
        while (menuStack.Count > 0)
        {
            CloseMenu();
        }
    }

    private void CloseMenu()
    {
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

        menu.UnityObject.SetActive(true);
        menuStack.Push(menu);
        openMenus.Add(menu.Name);
        if (menu.PauseGameGame)
        {
            HandleTimeScale();
        }
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
        if (menuStack.Count > 0)
        {
            MenuItem menu = menuStack.Peek();
            if (menu.Type == MenuType.TAB_MENU)
            {
                GameObject menuBody = GameObject.Find(Settings.CONST_CENTER_TAB_MENU_BODY);
                return !(RectTransformUtility.RectangleContainsScreenPoint(tabMenu.GetComponent<RectTransform>(), Input.mousePosition) ||
                RectTransformUtility.RectangleContainsScreenPoint(menuBody.GetComponent<RectTransform>(), Input.mousePosition));
            }
            else
            {
                return !RectTransformUtility.RectangleContainsScreenPoint(menu.UnityObject.GetComponent<RectTransform>(), Input.mousePosition);
            }
        }
        else
        {
            return false;
        }
    }
    private void AddMenuItemsToScrollView(MenuItem menu)
    {
        GameObject scrollView = menu.UnityObject.transform.Find(Settings.CONST_CENTER_SCROLL_CONTENT).gameObject;

        if (scrollView == null)
        {
            return;
        }

        //Clear ScrollView
        foreach (Transform child in scrollView.transform)
        {
            Destroy(child.gameObject);
        }

        //Add new Items
        foreach (GameGridObject obj in storeList.Tables)
        {
            GameObject item = Instantiate(Resources.Load(Settings.PREFAB_INVENTORY_ITEM, typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            Button button = item.GetComponent<Button>();
            //Adding click listener
            button.onClick.AddListener(() => OpenStoreEditPanel(obj));
            GameObject img = item.transform.Find(Settings.PREFAB_INVENTORY_ITEM_IMAGE).gameObject;
            GameObject text = item.transform.Find(Settings.PREFAB_INVENTORY_ITEM_TEXT_PRICE).gameObject;
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
        GameObject store = leftDownPanel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_STORE).gameObject;
        Button bStore = store.GetComponent<Button>();
        bStore.onClick.AddListener(() => OpenMenu(centerTabMenu));

        GameObject inventory = leftDownPanel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_INVENTORY).gameObject;
        Button bInventory = inventory.GetComponent<Button>();
        bInventory.onClick.AddListener(() => ItemClicked());

        GameObject employees = leftDownPanel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_EMPLOYEES).gameObject;
        Button bEmployees = employees.GetComponent<Button>();
        bEmployees.onClick.AddListener(() => ItemClicked());
    }

    private void SetEditStorePanelClickListeners()
    {
        GameObject accept = editStoreMenuPanel.transform.Find(Settings.CONST_EDIT_STORE_MENU_ACCEPT).gameObject;
        Button bAccept = accept.GetComponent<Button>();
        bAccept.onClick.AddListener(() => ItemClicked());

        GameObject cancel = editStoreMenuPanel.transform.Find(Settings.CONST_EDIT_STORE_MENU_CANCEL).gameObject;
        Button bCancel = cancel.GetComponent<Button>();
        bCancel.onClick.AddListener(() =>  CloseEditPanel());

        GameObject rotate = editStoreMenuPanel.transform.Find(Settings.CONST_EDIT_STORE_MENU_ROTATE).gameObject;
        Button bRotate = rotate.GetComponent<Button>();
        bRotate.onClick.AddListener(() => ItemClicked());
    }

    private void OpenStoreEditPanel(GameGridObject obj)
    {
        CloseAllMenus();
        gridController.HighlightGridBussFloor();
        //Disable Lefdown panel
        PauseGame();
        leftDownPanel.SetActive(false);
        editStoreMenuPanel.SetActive(true);
    }

    // Closes the edit panel without changes 
    private void CloseEditPanel(){
        editStoreMenuPanel.SetActive(false);
        leftDownPanel.SetActive(true);
        gridController.HideGridBussFloor();
        ResumeGame();
    }

    public bool IsEditPanelOpen(){
        return editStoreMenuPanel.activeSelf;
    }

    private void ItemClicked()
    {
        //Debug.Log("Clicking inventory/bEmployees");
    }
    public void InventoryItemClicked(GameGridObject obj)
    {
        Debug.Log("Button Clicked " + obj.Name);
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
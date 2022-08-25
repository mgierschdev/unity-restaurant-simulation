using System.Collections.Generic;
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
    private MenuItem topGameMenu;
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


    // MenuHandlerController Attached to CanvasMenu Parent of all Menus
    private void Start()
    {
        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.CONST_PARENT_GAME_OBJECT);
        clickController = cController.GetComponent<ClickController>();

        tabMenu = transform.Find(Settings.CONST_CENTER_TAB_MENU).gameObject;
        GameObject gameMenu = transform.Find(Settings.CONST_TOP_GAME_MENU).gameObject;
        GameObject npcProfileGameObject = transform.Find(Settings.CONST_NPC_PROFILE_MENU).gameObject;

        menuStack = new Stack<MenuItem>();
        openMenus = new HashSet<string>();

        topGameMenu = new MenuItem(Menu.TOP_MENU, MenuType.ON_SCREEN, Settings.CONST_TOP_GAME_MENU, gameMenu, true);
        centerTabMenu = new MenuItem(Menu.CENTER_TAB_MENU, MenuType.TAB_MENU, Settings.CONST_CENTER_TAB_MENU, tabMenu, true);
        npcProfileMenu = new MenuItem(Menu.NPC_PROFILE, MenuType.DIALOG, Settings.CONST_NPC_PROFILE_MENU, npcProfileGameObject, false);

        topGameMenu.Buttons.Add(Settings.CONST_UI_INVENTORY_BUTTON);
        centerTabMenu.Buttons.Add(Settings.CONST_UI_EXIT_BUTTON);

        SetClickListeners(centerTabMenu);
        SetClickListeners(topGameMenu);
        SetClickListeners(npcProfileMenu);

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
                    map.Add("Debug", npc.NPCDebug);
                }

                if (type == ObjectType.EMPLOYEE)
                {
                    employee = clickController.ClickedObject.GetComponent<EmployeeController>();
                    map.Add("Name", employee.Name);
                    map.Add("Debug", employee.NPCDebug);
                }

                OpenMenu(npcProfileMenu);
                npcProfileMenu.SetFields(map);
            }

            // We reset the clicked object after the action
            clickController.ClickedObject = null;
        }

        if (clickController.ClickedGameTile != null)
        {
            // Debug.Log("GameTile Clicked "+clickController.ClickedGameTile.Name);
            clickController.ClickedGameTile = null;
        }
    }

    private void RefresNPCProfile()
    {
        Dictionary<string, string> map = new Dictionary<string, string>
           {
           {"Name", npc.Name},
           {"Debug", npc.NPCDebug}
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
        if (!openMenus.Contains(menu.Name))
        {
            menu.UnityObject.SetActive(true);
            menuStack.Push(menu);
            openMenus.Add(menu.Name);
            if (menu.PauseGameGame)
            {
                HandleTimeScale();
            }
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
                GameObject MenuBody = GameObject.Find(Settings.CONST_CENTER_TAB_MENU_BODY);
                return !(RectTransformUtility.RectangleContainsScreenPoint(tabMenu.GetComponent<RectTransform>(), Input.mousePosition) ||
                RectTransformUtility.RectangleContainsScreenPoint(MenuBody.GetComponent<RectTransform>(), Input.mousePosition));
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

    public bool IsMenuOpen()
    {
        return menuStack.Count > 0;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
}
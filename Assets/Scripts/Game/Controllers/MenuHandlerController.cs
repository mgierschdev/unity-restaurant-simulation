using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// This will be only element attached in the UI
// All the buttom calls will be handled by this class.
public class MenuHandlerController : MonoBehaviour
{
    private GameObject tabMenu;
    private MenuItem centerTabMenu;
    private MenuItem topGameMenu;
    private Stack<MenuItem> menuStack;
    private HashSet<string> openMenus;


    private void Start()
    {
        tabMenu = transform.Find(Settings.CONST_CENTER_TAB_MENU).gameObject;
        GameObject gameMenu = transform.Find(Settings.CONST_TOP_GAME_MENU).gameObject;

        menuStack = new Stack<MenuItem>();
        openMenus = new HashSet<string>();

        topGameMenu = new MenuItem(MenuType.ON_SCREEN, Settings.CONST_TOP_GAME_MENU, gameMenu);
        centerTabMenu = new MenuItem(MenuType.TAB_MENU, Settings.CONST_CENTER_TAB_MENU, tabMenu);

        topGameMenu.Buttons.Add(Settings.CONST_UI_INVENTORY_BUTTON);
        centerTabMenu.Buttons.Add(Settings.CONST_UI_EXIT_BUTTON);

        SetClickListeners(centerTabMenu);
        SetClickListeners(topGameMenu);

        centerTabMenu.Close();
    }

    private void Update()
    {
        if (menuStack.Count > 0 && Input.GetMouseButton(0) && IsClickOutside())
        {
            CloseMenu();
        }
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
            HandleTimeScale();
        }
    }

    private void OpenMenu(MenuItem menu)
    {
        if (!openMenus.Contains(menu.Name))
        {
            menu.UnityObject.SetActive(true);
            menuStack.Push(menu);
            openMenus.Add(menu.Name);
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
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
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
}
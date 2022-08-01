using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// This will be only element attached in the UI
// All the buttom calls will be handled by this class.
public class MenuHandler : MonoBehaviour
{
    private MenuItem centerTabMenu;
    private MenuItem topGameMenu;
    private Stack<MenuItem> menuStack;
    private HashSet<string> openMenus;


    private void Start()
    {
        GameObject tabMenu = transform.Find(Settings.CONST_CENTER_TAB_MENU).gameObject;
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
        Debug.Log(menuStack.Count);
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
}
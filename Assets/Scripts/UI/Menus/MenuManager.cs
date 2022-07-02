using UnityEngine;

// This class handles the current displayed menu
public static class MenuManager
{
    public static bool IsInitialized { get; private set; }
    public static GameObject storeMenu, configMenu, topGameMenu;

    public static void Init()
    {
        GameObject canvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
        storeMenu = canvas.transform.Find(Settings.CONST_STORE_MENU).gameObject;
        configMenu = canvas.transform.Find(Settings.CONST_CONFIG_MENU).gameObject;
        topGameMenu = canvas.transform.Find(Settings.CONST_TOP_GAME_MENU).gameObject;
        IsInitialized = true;
    }

    public static void OpenMenu(Settings.Menu menu, GameObject callingMenu)
    {
        if (!IsInitialized)
        {
            Init();
        }

        switch (menu)
        {
            case Settings.Menu.CONFIG_MENU:
                configMenu.SetActive(true);
                break;
            case Settings.Menu.STORE_MENU:
                storeMenu.SetActive(true);
                break;
        }

        callingMenu.SetActive(false);
    }

    public static void OpenMenu(Settings.Menu menu)
    {
        switch (menu)
        {
            case Settings.Menu.CONFIG_MENU:
                configMenu.SetActive(true);
                break;
            case Settings.Menu.STORE_MENU:
                storeMenu.SetActive(true);
                break;
            case Settings.Menu.TOP_GAME_MENU:
                topGameMenu.SetActive(true);
                break;
        }
    }

    public static void CloseMenu(GameObject callingMenu)
    {
        callingMenu.SetActive(false);
    }
}
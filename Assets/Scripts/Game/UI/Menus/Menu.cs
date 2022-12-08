using UnityEngine.UI;

public class MenuItem
{
    private MenuType type;
    private MenuTab menuTab;
    private Button tabButton;
    public string name;

    public MenuItem(MenuTab menuTab, MenuType type, string name)
    {
        this.menuTab = menuTab;
        this.name = name;
        this.type = type;
    }

    public MenuItem(MenuTab menuTab, MenuType type, string name, Button tabButton)
    {
        this.tabButton = tabButton;
        this.menuTab = menuTab;
        this.name = name;
        this.type = type;
    }

    public MenuType GetMenuType()
    {
        return type;
    }

    public MenuTab GetMenuTab()
    {
        return menuTab;
    }

    public Button GetTabButton()
    {
        return tabButton;
    }
}
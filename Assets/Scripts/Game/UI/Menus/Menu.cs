public class MenuItem
{
    private MenuType type;
    private MenuTab menuTab;
    public string name;

    public MenuItem(MenuTab menuTab, MenuType type, string name)
    {
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
}
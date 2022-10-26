using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItem
{
    private MenuType type;
    private MenuTab menuTab;
    public string name;

    public MenuItem(MenuTab menu, MenuType type, string name)
    {
        this.menuTab = menu;
        this.name = name;
        this.type = type;
    }

    public MenuType GetType()
    {
        return type;
    }

    public MenuTab GetMenuTab()
    {
        return menuTab;
    }
}
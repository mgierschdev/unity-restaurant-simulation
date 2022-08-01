using System.Collections.Generic;
using UnityEngine;

public class MenuItem
{
    public MenuType Type { get; set; }
    public string Name { get; set; }
    public GameObject UnityObject { get; set; }
    public List<string> Buttons { get; set; }

    public MenuItem(MenuType type, string name, GameObject gobj)
    {
        this.UnityObject = gobj;
        this.Name = name;
        this.Type = type;
        Buttons = new List<string>();
    }

    public void Close()
    {
        UnityObject.SetActive(false);
    }

    public void Open()
    {
        UnityObject.SetActive(true);
    }
}
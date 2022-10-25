using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItem
{
    private MenuType type;
    private MenuTab menuTab;
    public string name;
    // public GameObject UnityObject { get; }

    public MenuItem(MenuTab menu, MenuType type, string name)
    {
        this.menuTab = menu;
        // UnityObject = gameObj;
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

    // public void SetFields(Dictionary<string, string> fields)
    // {
    //     foreach (KeyValuePair<string, string> kvp in fields)
    //     {
    //         GameObject go = GameObject.Find(kvp.Key);

    //         if (go)
    //         {
    //             TextMeshProUGUI textMesh = go.GetComponent<TextMeshProUGUI>();

    //             if (textMesh)
    //             {
    //                 textMesh.text = kvp.Value;
    //             }
    //             else
    //             {
    //                 GameLog.LogWarning("MenuItem/SetFields TextMesh Null");
    //             }
    //         }
    //         else
    //         {
    //             GameLog.LogWarning("MenuItem/SetFields Object " + kvp.Key + " null");
    //         }
    //     }
    // }

    // public void Close()
    // {
    //     UnityObject.SetActive(false);
    // }

    // public void Open()
    // {
    //     UnityObject.SetActive(true);
    // }
}
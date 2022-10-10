namespace UI;

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItem
{
    public MenuType Type { get; }
    public Menu Menu { get; }
    public string Name { get; }
    public GameObject UnityObject { get; }
    public bool PauseGame { get; }

    public MenuItem(Menu menu, MenuType type, string name, GameObject gameObj, bool pauseGame)
    {
        Menu = menu;
        PauseGame = pauseGame;
        UnityObject = gameObj;
        Name = name;
        Type = type;
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        foreach (KeyValuePair<string, string> kvp in fields)
        {
            GameObject go = GameObject.Find(kvp.Key);

            if (go)
            {
                TextMeshProUGUI textMesh = go.GetComponent<TextMeshProUGUI>();

                if (textMesh)
                {
                    textMesh.text = kvp.Value;
                }
                else
                {
                    GameLog.LogWarning("MenuItem/SetFields TextMesh Null");
                }
            }
            else
            {
                GameLog.LogWarning("MenuItem/SetFields Object " + kvp.Key + " null");
            }
        }
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
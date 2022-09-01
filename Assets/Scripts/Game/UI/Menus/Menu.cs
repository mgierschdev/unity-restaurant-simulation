using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItem
{
    public MenuType Type { get; set; }
    public Menu Menu { get; set; }
    public string Name { get; set; }
    public GameObject UnityObject { get; set; }
    public List<string> Buttons { get; set; }
    public Dictionary<string, string> Fields { get; set; }
    public bool PauseGameGame { get; set; }
    public GameObject scrollView;

    public MenuItem(Menu menu, MenuType type, string name, GameObject gobj, bool pauseGame)
    {
        this.Menu = menu;
        this.PauseGameGame = pauseGame;
        this.UnityObject = gobj;
        this.Name = name;
        this.Type = type;
        Buttons = new List<string>();
        Fields = new Dictionary<string, string>();
    }

    public void SetFields(Dictionary<string, string> fields)
    {
        this.Fields = fields;
        foreach (KeyValuePair<string, string> kvp in fields)
        {
            GameObject go = GameObject.Find(kvp.Key);

            if (go != null)
            {
                TextMeshProUGUI textMesh = go.GetComponent<TextMeshProUGUI>();

                if (textMesh != null)
                {
                    textMesh.text = kvp.Value;
                }
                else
                {
                    Debug.LogWarning("MenuItem/SetFields TextMesh Null");
                }
            }
            else
            {
                Debug.LogWarning("MenuItem/SetFields Object " + kvp.Key + " null");
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
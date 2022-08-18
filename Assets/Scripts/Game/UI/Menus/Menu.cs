using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem
{
    public MenuType Type { get; set; }
    public string Name { get; set; }
    public GameObject UnityObject { get; set; }
    public List<string> Buttons { get; set; }
    public Dictionary<string, string> Fields { get; set; }

    public MenuItem(MenuType type, string name, GameObject gobj)
    {
        this.UnityObject = gobj;
        this.Name = name;
        this.Type = type;
        Buttons = new List<string>();
        Fields = new Dictionary<string, string>();
    }

    public void SetFields(Dictionary<string, string> Fields)
    {
        this.Fields = Fields;
        
        foreach (KeyValuePair<string, string> kvp in Fields)
        {
            Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Transform go = UnityObject.transform.Find(kvp.Key);
            TextMesh textMesh = go.GetComponent<TextMesh>();

            if(textMesh != null){
                textMesh.text = kvp.Value;
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
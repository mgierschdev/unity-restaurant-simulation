using NUnit.Framework;
using UnityEngine;

public class TestUI
{
    [Test]
    public void TestLeftDownPanel()
    {
        GameObject panel = GameObject.Find(Settings.CONST_LEFT_DOWN_PANEL).gameObject;
        Transform inventory = panel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_INVENTORY);
        Transform store = panel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_STORE);
        Transform employees = panel.transform.Find(Settings.CONST_LEFT_DOWN_MENU_EMPLOYEES);
        Assert.NotNull(panel);
        Assert.NotNull(store);
        Assert.NotNull(employees);
        Assert.NotNull(inventory);
    }

    [Test]
    public void TestEditStoreMenu()
    {
        GameObject panel = GameObject.Find(Settings.CONST_EDIT_STORE_MENU_PANEL).gameObject;
        Transform cancel = panel.transform.Find(Settings.CONST_EDIT_STORE_MENU_CANCEL);
        Transform accept = panel.transform.Find(Settings.CONST_EDIT_STORE_MENU_ACCEPT);
        Transform rotate = panel.transform.Find(Settings.CONST_EDIT_STORE_MENU_ROTATE);
        Assert.NotNull(panel);
        Assert.NotNull(cancel);
        Assert.NotNull(accept);
        Assert.NotNull(rotate);
    }
}
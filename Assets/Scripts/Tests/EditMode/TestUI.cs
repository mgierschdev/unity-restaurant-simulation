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
}
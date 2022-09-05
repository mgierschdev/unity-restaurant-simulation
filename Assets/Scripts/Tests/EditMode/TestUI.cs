using NUnit.Framework;
using UnityEngine;

public class TestUI
{
    [Test]
    public void TestLeftDownPanel()
    {
        GameObject panel = GameObject.Find(Settings.ConstLeftDownPanel).gameObject;
        Transform inventory = panel.transform.Find(Settings.ConstLeftDownMenuInventory);
        Transform store = panel.transform.Find(Settings.ConstLeftDownMenuStore);
        Transform employees = panel.transform.Find(Settings.ConstLeftDownMenuEmployees);
        Assert.NotNull(panel);
        Assert.NotNull(store);
        Assert.NotNull(employees);
        Assert.NotNull(inventory);
    }

    [Test]
    public void TestEditStoreMenu()
    {
        GameObject panel = GameObject.Find(Settings.ConstEditStoreMenuPanel).gameObject;
        Transform cancel = panel.transform.Find(Settings.ConstEditStoreMenuCancel);
        Transform accept = panel.transform.Find(Settings.ConstEditStoreMenuAccept);
        Transform rotate = panel.transform.Find(Settings.ConstEditStoreMenuRotate);
        Assert.NotNull(panel);
        Assert.NotNull(cancel);
        Assert.NotNull(accept);
        Assert.NotNull(rotate);
    }
}
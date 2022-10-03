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
        Assert.NotNull(panel);
        Assert.NotNull(store);
        Assert.NotNull(inventory);
    }

    [Test]
    public void TestEditStoreMenu()
    {
        GameObject panel = GameObject.Find(Settings.ConstEditStoreMenuPanel).gameObject;
        Transform cancel = panel.transform.Find(Settings.ConstEditStoreMenuCancel);
        Assert.NotNull(panel);
        Assert.NotNull(cancel);
    }
}
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
        Transform accept = panel.transform.Find(Settings.ConstEditStoreMenuAccept);
        Transform rotateLeft = panel.transform.Find(Settings.ConstEditStoreMenuRotateLeft);
        Transform rotateRight = panel.transform.Find(Settings.ConstEditStoreMenuRotateRight);
        Assert.NotNull(panel);
        Assert.NotNull(cancel);
        Assert.NotNull(accept);
        Assert.NotNull(rotateLeft);
        Assert.NotNull(rotateRight);
    }
}
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
}
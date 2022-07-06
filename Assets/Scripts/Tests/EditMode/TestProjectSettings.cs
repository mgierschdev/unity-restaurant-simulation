using NUnit.Framework;
using UnityEngine;

public class TestProjectSettings
{
    [Test]
    public void TestgameBackground()
    {
        GameObject gameBackground = GameObject.Find(Settings.CONST_GAME_BACKGROUND);
        Assert.NotNull(gameBackground);
    }

    [Test]
    public void TestparentCanvas()
    {
        GameObject parentCanvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
        Assert.NotNull(parentCanvas);
    }

    [Test]
    public void TestconfigMenu()
    {
        GameObject configMenu = GameObject.Find(Settings.CONST_CONFIG_MENU);
        Assert.NotNull(configMenu);
    }

    [Test]
    public void TestTopGameMenu()
    {
        GameObject topGameMenu = GameObject.Find(Settings.CONST_TOP_GAME_MENU);
        Assert.NotNull(topGameMenu);
    }

    [Test]
    public void TeststoreMenu()
    {
        GameObject storeMenu = GameObject.Find(Settings.CONST_STORE_MENU);
        Assert.NotNull(storeMenu);
    }

    [Test]
    public void TestNPCEnergyBar()
    {
        if (Settings.NPC_ENERGY_ENABLED)
        {
            GameObject NPCEnergyBar = GameObject.FindWithTag(Settings.NPC_ENERGY_BAR);
            Assert.NotNull(NPCEnergyBar);
        }
    }

    [Test]
    public void TestGameGrid()
    {
        GameObject gameGrid = GameObject.Find(Settings.CONST_GAME_GRID);
        Assert.NotNull(gameGrid);
    }
}

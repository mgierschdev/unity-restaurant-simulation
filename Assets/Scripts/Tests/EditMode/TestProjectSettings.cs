using NUnit.Framework;
using UnityEngine;

public class TestProjectSettings
{
    [Test]
    public void TestgameBackground()
    {
        GameObject gameBackground = GameObject.Find(Settings.CONST_GAME_BACKGROUND_DEFAULT);
        Assert.NotNull(gameBackground);
    }

    // [Test]
    // public void TestparentCanvas()
    // {
    //     GameObject parentCanvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
    //     Assert.NotNull(parentCanvas);
    // }

    [Test]
    public void TesttabMenu()
    {
        GameObject tabMenu = GameObject.Find(Settings.CONST_CENTER_TAB_MENU);
        Assert.NotNull(tabMenu);
    }

    [Test]
    public void TestTopGameMenu()
    {
        GameObject topGameMenu = GameObject.Find(Settings.CONST_TOP_GAME_MENU);
        Assert.NotNull(topGameMenu);
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
        GameObject gameGrid = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);
        Assert.NotNull(gameGrid);
    }
}

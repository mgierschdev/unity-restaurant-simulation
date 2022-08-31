using NUnit.Framework;
using UnityEngine;

public class TestProjectSettings
{
    [Test]
    public void TestparentCanvas()
    {
        GameObject parentCanvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
        Assert.NotNull(parentCanvas);
    }


    [Test]
    public void TestNPCProfile()
    {
        GameObject npcProfile = GameObject.Find(Settings.CONST_NPC_PROFILE_MENU);
        Assert.NotNull(npcProfile);
    }

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
    public void TestGameGrid()
    {
        GameObject gameGrid = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);
        Assert.NotNull(gameGrid);
    }

    [Test]
    public void TestTileFloor0()
    {
        GameObject tilemap = GameObject.Find(Settings.TILEMAP_FLOOR_0);
        Assert.NotNull(tilemap);
    }

    [Test]
    public void TestTileColliders()
    {
        GameObject tilemap = GameObject.Find(Settings.TILEMAP_COLLIDERS);
        Assert.NotNull(tilemap);
    }

    [Test]
    public void TestTileObjects()
    {
        GameObject tilemap = GameObject.Find(Settings.TILEMAP_OBJECTS);
        Assert.NotNull(tilemap);
    }

    [Test]
    public void TestPathFindingObjects()
    {
        GameObject tilemap = GameObject.Find(Settings.PATH_FINDING_GRID);
        Assert.NotNull(tilemap);
    }

    [Test]
    public void TestPrefabLoadGridTile()
    {
        Object obj = Resources.Load(Settings.PREFAB_GRID_TILE, typeof(GameObject));
        Assert.NotNull(obj);
    }

    [Test]
    public void TestPrefabLoadInventoryItem()
    {
        Object obj = Resources.Load(Settings.PREFAB_INVENTORY_ITEM, typeof(GameObject));
        Assert.NotNull(obj);
    }

    [Test]
    public void TestPrefabLoadIsometricNPC()
    {
        Object obj = Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject));
        Assert.NotNull(obj);
    }

    [Test]
    public void TestPrefabLoadIsometricPlayer()
    {
        Object obj = Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject));
        Assert.NotNull(obj);
    }

    [Test]
    public void TestPrefabLoadSandAndSea()
    {
        Object obj = Resources.Load(Settings.PREFAB_SAND_SEA, typeof(GameObject));
        Assert.NotNull(obj);
    }
}

using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestGameTile
{
    [Test]
    public void TestGameTileFloor()
    {
        Vector3 worldPosition = Vector3.zero;
        GameTile tile = new GameTile(worldPosition, TileType.FLOOR_1);
        Assert.AreEqual(tile.Type, TileType.FLOOR_1);
    }

    [Test]
    public void TestGameTileWorldPosition()
    {
        Vector3 worldPosition = Vector3.zero;
        GameTile tile = new GameTile(worldPosition, TileType.FLOOR_1);
        Assert.AreEqual(tile.Type, TileType.FLOOR_1);
        Assert.AreEqual(tile.WorldPosition, Vector3.zero);
    }

    [Test]
    public void TestGameTileGridPosition()
    {
        Vector3 worldPosition = Vector3.zero;
        GameTile tile = new GameTile(worldPosition, TileType.FLOOR_1);
        Assert.AreEqual(tile.Type, TileType.FLOOR_1);
        Vector2Int target = Util.GetXYInGameMap(Vector3.zero);
        Assert.AreEqual(tile.GridPosition, new Vector3(target.x, target.y, 1));
    }
}
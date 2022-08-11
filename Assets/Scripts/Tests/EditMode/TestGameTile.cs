using NUnit.Framework;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using UnityEngine.Tilemaps;

public class TestGameTile
{
    [Test]
    public void TestGameTileFloor()
    {
        Vector3 worldPosition = Vector3.zero;
        TileBase baseTile = null;
        GameTile tile = new GameTile(worldPosition, new Vector2Int(0, 0), TileType.FLOOR_1, ObjectType.FLOOR, baseTile);
        Assert.AreEqual(tile.Type, ObjectType.FLOOR);
    }

    [Test]
    public void TestGameTileWorldPosition()
    {
        Vector3 worldPosition = Vector3.zero;
        TileBase baseTile = null;
        GameTile tile = new GameTile(worldPosition, new Vector2Int(0, 0), TileType.FLOOR_1, ObjectType.FLOOR, baseTile);
        Assert.AreEqual(tile.Type, ObjectType.FLOOR);
        Assert.AreEqual(tile.WorldPosition, Vector3.zero);
    }

    [Test]
    public void TestGameTileGridPosition()
    {
        Vector3 worldPosition = Vector3.zero;
        TileBase baseTile = null;
        GameTile tile = new GameTile(worldPosition, new Vector2Int(0, 0), TileType.FLOOR_1, ObjectType.FLOOR, baseTile);
        Assert.AreEqual(tile.Type, ObjectType.FLOOR);
        Vector2Int target = Util.GetXYInGameMap(Vector3.zero);
        Assert.AreEqual(tile.GridPosition, new Vector3(target.x, target.y, 1));
    }
}
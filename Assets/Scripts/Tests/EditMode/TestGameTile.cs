using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestGameTile
{
    [Test]
    public void TestGameTileFloor()
    {
        Vector3 worldPosition = Vector3.zero;
        TileBase baseTile = null;
        GameTile tile = new GameTile(worldPosition, new Vector3Int(0, 0), new Vector3Int(0, 0), TileType.FLOOR_3, ObjectType.FLOOR, baseTile);
        Assert.AreEqual(tile.Type, ObjectType.FLOOR);
    }

    [Test]
    public void TestGameTileWorldPosition()
    {
        Vector3 worldPosition = Vector3.zero;
        TileBase baseTile = null;
        GameTile tile = new GameTile(worldPosition, new Vector3Int(0, 0), new Vector3Int(0, 0), TileType.FLOOR_3, ObjectType.FLOOR, baseTile);
        Assert.AreEqual(tile.Type, ObjectType.FLOOR);
        Assert.AreEqual(tile.WorldPosition, Vector3.zero);
    }
}
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

    [Test]
    public void TestBaseTileGrid(){
        //Base tile used to build the floor of the build, initally positioned in -12/28
        Tilemap tilemapPathFinding = GameObject.Find(Settings.PATH_FINDING_GRID).GetComponent<Tilemap>();
        TileBase gridTile = tilemapPathFinding.GetTile(new Vector3Int(-12, 28));
        Assert.NotNull(gridTile);
    }

    [Test]
    public void TestLoadTileMap(){
        TileBase gridTile = Resources.Load<Tile>(Settings.GRID_TILES_SIMPLE);
        Assert.NotNull(gridTile);
    }
}
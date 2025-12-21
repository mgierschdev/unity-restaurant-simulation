using Game.Grid;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;

namespace Tests.EditMode
{
    /**
     * Problem: Validate basic GameTile construction and resources.
     * Goal: Ensure GameTile properties and tile assets load correctly.
     * Approach: Instantiate GameTile and assert expected values.
     * Time: O(1) per assertion.
     * Space: O(1).
     */
    public class TestGameTile
    {
        [Test]
        public void TestGameTileFloor()
        {
            var worldPosition = Vector3.zero;

            var tile = new GameTile(worldPosition, new Vector3Int(0, 0), new Vector3Int(0, 0), TileType.Floor3,
                ObjectType.Floor, null);

            Assert.AreEqual(tile.Type, ObjectType.Floor);
        }

        [Test]
        public void TestGameTileWorldPosition()
        {
            Vector3 worldPosition = Vector3.zero;
            GameTile tile = new GameTile(worldPosition, new Vector3Int(0, 0), new Vector3Int(0, 0), TileType.Floor3,
                ObjectType.Floor, null);
            Assert.AreEqual(tile.Type, ObjectType.Floor);
            Assert.AreEqual(tile.WorldPosition, Vector3.zero);
        }

        [Test]
        public void TestLoadSimpleTileMap()
        {
            TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesSimple);
            Assert.NotNull(gridTile);
        }
    }
}

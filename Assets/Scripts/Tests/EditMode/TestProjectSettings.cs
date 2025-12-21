using NUnit.Framework;
using UnityEngine;
using Util;

namespace Tests.EditMode
{
    /**
     * Problem: Validate required Unity objects and resources exist.
     * Goal: Ensure scenes and prefabs can be found by Settings paths.
     * Approach: Use NUnit to assert GameObject/Resource lookups.
     * Time: O(1) per lookup.
     * Space: O(1).
     */
    public class TestProjectSettings
    {
        [Test]
        public void TestparentCanvas()
        {
            GameObject parentCanvas = GameObject.Find(Settings.ConstCanvasParentMenu);
            Assert.NotNull(parentCanvas);
        }

        [Test]
        public void TesttabMenu()
        {
            GameObject tabMenu = GameObject.Find(Settings.ConstCenterTabMenu);
            GameLog.Log(tabMenu.transform.GetChild(0).name);
            GameObject scrollViewPort = tabMenu.transform.Find(Settings.ConstCenterScrollContent).gameObject;
            Assert.NotNull(tabMenu);
            Assert.NotNull(scrollViewPort);
        }

        [Test]
        public void TestTileFloor0()
        {
            GameObject tilemap = GameObject.Find(Settings.TilemapSpamFloor);
            Assert.NotNull(tilemap);
        }

        [Test]
        public void TestTileColliders()
        {
            GameObject tilemap = GameObject.Find(Settings.TilemapColliders);
            Assert.NotNull(tilemap);
        }

        [Test]
        public void TestTileObjects()
        {
            GameObject tilemap = GameObject.Find(Settings.TilemapObjects);
            Assert.NotNull(tilemap);
        }

        [Test]
        public void TestPathFindingObjects()
        {
            GameObject tilemap = GameObject.Find(Settings.PathFindingGrid);
            Assert.NotNull(tilemap);
        }

        [Test]
        public void TestPrefabLoadInventoryItem()
        {
            Object obj = Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject));
            Assert.NotNull(obj);
        }

        [Test]
        public void TestPrefabLoadIsometricNpc()
        {
            Object obj = Resources.Load(Settings.PrefabNpcClient, typeof(GameObject));
            Assert.NotNull(obj);
        }

        [Test]
        public void TestPrefabLoadIsometricPlayer()
        {
            Object obj = Resources.Load(Settings.PrefabPlayer, typeof(GameObject));
            Assert.NotNull(obj);
        }

        [Test]
        public void TestLoadingInventoryMenuItem()
        {
            GameObject item = (GameObject)Resources.Load(Settings.PrefabInventoryItem, typeof(GameObject));
            Transform image = item.transform.Find(Settings.PrefabInventoryItemImage);
            Transform price = item.transform.Find(Settings.PrefabInventoryItemTextPrice);
            Assert.NotNull(item);
            Assert.NotNull(image);
            Assert.NotNull(price);
        }
    }
}

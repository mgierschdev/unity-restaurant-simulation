using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestPlayerMovementPathFinding
{
    private GameObject playerObject;
    private PlayerController playerController;
    private GameObject gridObject;
    private GameGridController gameGridController; // 18x18 Default size

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<GameGridController>();
        // First NPC
        playerObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject))) as GameObject;
        playerObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject)), gameGridController.GetCellPosition(new Vector3(1, 1, Settings.DEFAULT_GAME_OBJECTS_Z)), Quaternion.identity) as GameObject;
        playerObject.transform.SetParent(gridObject.transform);
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.SetTestGameGridController(gameGridController);
        playerController.SetSpeed(100);
    }

    [UnityTest]
    public IEnumerator TestSimplePath()
    {
        int[] endPosition = new int[] { 10, 10 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        playerController.AddPath(path);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 16, 16 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        gameGridController.SetTestGridObstacles(5, 1, 15);
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        playerController.AddPath(path);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(playerObject);
        Object.Destroy(playerController);
        Object.Destroy(gridObject);
        Object.Destroy(gameGridController);
    }
}
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
        playerObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject)), gameGridController.GetCellPosition(1, 1, 1), Quaternion.identity) as GameObject;
        playerObject.transform.SetParent(gridObject.transform);
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.SetTestGameGridController(gameGridController);
    }

    [UnityTest]
    public IEnumerator TestSimplePath()
    {
        int[] endPosition = new int[] { 4, 4 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        playerController.AddPath(path);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 14, 14 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        gameGridController.SetTestGridObstacles(5, 1, 16);
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        playerController.AddPath(path);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
    }
}
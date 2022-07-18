using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestNPCMovementPathFinding
{
    private GameObject firstNPCObject;
    private GameObject secondNPCObject;
    private NPCController firstNPCController;
    private NPCController secondNPCController;
    private GameObject gridObject;
    private GameGridController gameGridController; // 18x18 Default size

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<GameGridController>();
        // First NPC
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gameGridController.GetCellPosition(1, 1, 1), Quaternion.identity) as GameObject;
        firstNPCObject.transform.SetParent(gridObject.transform);
        firstNPCController = firstNPCObject.GetComponent<NPCController>();
        firstNPCController.SetTestGameGridController(gameGridController);
        firstNPCController.SetSpeed(100);
        // Second NPC
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gameGridController.GetCellPosition(1, 1, 1), Quaternion.identity) as GameObject;
        secondNPCObject.transform.SetParent(gridObject.transform);
        secondNPCController = secondNPCObject.GetComponent<NPCController>();
        secondNPCController.SetTestGameGridController(gameGridController);
        secondNPCController.SetSpeed(100);
    }


    [UnityTest]
    public IEnumerator TestSimpleMoving()
    {
        int[] endPosition = new int[] { 4, 4 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 14, 14 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        gameGridController.SetTestGridObstacles(5, 1, 15);
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }

    [UnityTest]
    // For now they can overlap
    public IEnumerator TestMultipleNPC()
    {
        int[] endPosition = new int[] { 14, 14 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        gameGridController.SetTestGridObstacles(5, 1, 15);
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        secondNPCController.AddPath(path);
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[1], endPosition[1]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }
}
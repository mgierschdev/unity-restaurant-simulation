using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestNPCMovementPathFinding
{
    private GameObject firstNPCObject;
    private GameObject secondNPCObject;
    private IsometricNPCController firstNPCController;
    private IsometricNPCController secondNPCController;
    private GameObject gridObject;
    private IsometricGridController gameGridController;
    private Vector3 initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<IsometricGridController>();
        // First NPC
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject))) as GameObject;
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject)), Util.GetCellPosition(new Vector3(1, 1, Settings.DEFAULT_GAME_OBJECTS_Z)), Quaternion.identity) as GameObject;
        firstNPCObject.transform.SetParent(gridObject.transform);
        firstNPCController = firstNPCObject.GetComponent<IsometricNPCController>();
        firstNPCController.GameGrid = gameGridController;
        // Second NPC
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject))) as GameObject;
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject)), Util.GetCellPosition(new Vector3(1, 1, Settings.DEFAULT_GAME_OBJECTS_Z)), Quaternion.identity) as GameObject;
        secondNPCObject.transform.SetParent(gridObject.transform);
        secondNPCController = secondNPCObject.GetComponent<IsometricNPCController>();
        secondNPCController.GameGrid = gameGridController;

        initialTestingPosition = new Vector3(1, 1, Settings.DEFAULT_GAME_OBJECTS_Z);
    }


    [UnityTest]
    public IEnumerator TestSimpleMoving()
    {
        int[] endPosition = new int[] { 4, 4 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        firstNPCController.Position = initialTestingPosition;
        firstNPCController.Speed = 100;
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(1f);
        Debug.Log(firstNPCController.Position);
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
        firstNPCController.Position = initialTestingPosition;
        firstNPCController.Speed = 100;
        Util.PrintPath(path);
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }

    [UnityTest]
    //They can overlap
    public IEnumerator TestMultipleNPC()
    {
        int[] endPosition = new int[] { 14, 14 };
        int[] startPosition = new int[] { 1, 1 }; // Corners are outside perimeter
        gameGridController.SetTestGridObstacles(5, 1, 15);
        List<Node> path = gameGridController.GetPath(startPosition, endPosition);
        Util.PrintPath(path);
        firstNPCController.Position = initialTestingPosition;
        firstNPCController.Speed = 100;
        secondNPCController.Position = initialTestingPosition;
        secondNPCController.Speed = 100;
        secondNPCController.AddPath(path);
        firstNPCController.AddPath(path);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[1], endPosition[1]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(firstNPCObject);
        Object.Destroy(secondNPCObject);
        Object.Destroy(firstNPCController);
        Object.Destroy(secondNPCController);
        Object.Destroy(gridObject);
        Object.Destroy(gameGridController);
    }
}
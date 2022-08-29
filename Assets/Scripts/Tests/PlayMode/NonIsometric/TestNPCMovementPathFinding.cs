using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestNPCMovementPathFinding
{
    private GameObject firstNPCObject;
    private GameObject secondNPCObject;
    private NPCController firstNPCController;
    private NPCController secondNPCController;
    private GameObject gridObject;
    private GridController gameGridController;
    private Vector3Int initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<GridController>();
        // First NPC
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject))) as GameObject;
        firstNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject)), new Vector3(1, 1), Quaternion.identity) as GameObject;
        firstNPCObject.transform.SetParent(gridObject.transform);
        firstNPCController = firstNPCObject.GetComponent<NPCController>();
        firstNPCController.GameGrid = gameGridController;
        // Second NPC
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject))) as GameObject;
        secondNPCObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject)), new Vector3(1, 1), Quaternion.identity) as GameObject;
        secondNPCObject.transform.SetParent(gridObject.transform);
        secondNPCController = secondNPCObject.GetComponent<NPCController>();
        secondNPCController.GameGrid = gameGridController;

        initialTestingPosition = new Vector3Int(1, 1);
    }


    [UnityTest]
    public IEnumerator TestSimpleMoving()
    {
        int[] endPosition = new int[] { 4, 4 };
        firstNPCController.Position = initialTestingPosition;
        firstNPCController.Speed = 100;
        firstNPCController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(2f);
        Debug.Log(firstNPCController.Position);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 25, 14 };
        gameGridController.SetTestGridObstacles(21, 1, 15);
        firstNPCController.Speed = 100;
        firstNPCController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(2f);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }

    [UnityTest]
    //They can overlap
    public IEnumerator TestMultipleNPC()
    {
        int[] endPosition = new int[] { 25, 14 };
        gameGridController.SetTestGridObstacles(21, 1, 15);
        firstNPCController.Speed = 100;
        secondNPCController.Speed = 100;
        firstNPCController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        secondNPCController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(2f);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(secondNPCController.GetPositionAsArray()[1], endPosition[1]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(firstNPCController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }
}
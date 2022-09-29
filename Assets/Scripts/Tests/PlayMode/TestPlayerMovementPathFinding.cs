using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestPlayerMovementPathFinding
{
    private GameObject playerObject;
    private PlayerController playerController;
    private GridController gameGridController;
    private GameObject gridObject;
    private Vector3Int initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.GameGrid, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<GridController>();
        initialTestingPosition = new Vector3Int(1, 1);
        // Player
        playerObject = Transform.Instantiate(Resources.Load(Settings.PrefabPlayer, typeof(GameObject))) as GameObject;
        playerObject = Transform.Instantiate(Resources.Load(Settings.PrefabPlayer, typeof(GameObject)), initialTestingPosition, Quaternion.identity) as GameObject;
        playerObject.transform.SetParent(gridObject.transform);
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.Grid = gameGridController;
    }

    [UnityTest]
    public IEnumerator TestSimplePath()
    {
        int[] endPosition = new int[] { 25, 14 };
        playerController.Speed = 100;
        playerController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 25, 14 };
        gameGridController.SetTestGridObstacles(21, 1, 15);
        //playerController.Position = gameGridController.GetWorldFromPathFindingGridPosition(new Vector3Int(startPosition[0], startPosition[1]));
        playerController.Speed = 100;
        playerController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(1f);
        GameLog.Log(playerController.Position);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
        gameGridController.FreeTestGridObstacles(5, 1, 15);
    }
}
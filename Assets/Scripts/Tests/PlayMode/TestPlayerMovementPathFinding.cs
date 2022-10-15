using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestPlayerMovementPathFinding
{
    private GameObject playerObject;
    private PlayerController playerController;
    private GameObject gridObject;
    private Vector3Int initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        initialTestingPosition = new Vector3Int(1, 1);
        // Player
        playerObject = Transform.Instantiate(Resources.Load(Settings.PrefabPlayer, typeof(GameObject))) as GameObject;
        playerObject = Transform.Instantiate(Resources.Load(Settings.PrefabPlayer, typeof(GameObject)), initialTestingPosition, Quaternion.identity) as GameObject;
        playerObject.transform.SetParent(gridObject.transform);
        playerController = playerObject.GetComponent<PlayerController>();
    }

    [UnityTest]
    public IEnumerator TestSimplePath()
    {
        int[] endPosition = new int[] { 25, 14 };
        playerController.SetSpeed(100);
        playerController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
    }

    [UnityTest]
    public IEnumerator TestPathWithObstacles()
    {
        int[] endPosition = new int[] { 25, 14 };
        BussGrid.SetTestGridObstacles(21, 1, 15);
        //playerController.Position = gameGridController.GetWorldFromPathFindingGridPosition(new Vector3Int(startPosition[0], startPosition[1]));
        playerController.SetSpeed(100);
        playerController.GoTo(new Vector3Int(endPosition[0], endPosition[1]));
        yield return new WaitForSeconds(1f);
        GameLog.Log(playerController.Position);
        Assert.AreEqual(playerController.GetPositionAsArray()[0], endPosition[0]);
        Assert.AreEqual(playerController.GetPositionAsArray()[1], endPosition[1]);
        BussGrid.FreeTestGridObstacles(5, 1, 15);
    }
}
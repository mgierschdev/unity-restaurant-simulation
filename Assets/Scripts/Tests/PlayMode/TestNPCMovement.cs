using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class TestNPCMovement
{
    private GameObject npcObject;
    private NPCController npcController;
    private Vector3 target;
    private GameObject gridObject;
    private GameGridController gameGridController;
    private Vector3 initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<GameGridController>();

        // Adding NPC object
        // First NPC
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gameGridController.GetCellPosition(new Vector3(0, 0, Settings.DEFAULT_GAME_OBJECTS_Z)), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gridObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        npcController.GameGrid = gameGridController;
    }

    [UnityTest]
    public IEnumerator TestMovementDOWN()
    {
        npcController.Position = initialTestingPosition;
        npcController.Speed = 100;
        Debug.Log(npcController.Position);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWN);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Debug.Log(npcController.Position);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUP()
    {
        npcController.Position = initialTestingPosition;
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.UP);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementRIGHT()
    {
        npcController.Position = initialTestingPosition;
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.RIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementLEFT()
    {
        npcController.Position = initialTestingPosition;
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.LEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementDOWNLEFT()
    {
        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementDOWNRIGHT()
    {
        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUPLEFT()
    {
        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.UPLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUPRIGHT()
    {
        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementIDLE()
    {
        npcController.Position = initialTestingPosition;
        npcController.WanderOn = false;
        target = Util.GetVectorFromDirection(MoveDirection.IDLE);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.Position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(npcObject);
        Object.Destroy(npcController);
    }
}
using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class TestNPCMovement
{
    private GameObject npcObject;
    private IsometricNPCController npcController;
    private Vector3 target;
    private GameObject gridObject;
    private IsometricGridController gameGridController;
    private Vector3 initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Game Grid
        gridObject = Transform.Instantiate(Resources.Load(Settings.GAME_GRID, typeof(GameObject))) as GameObject;
        gameGridController = gridObject.GetComponent<IsometricGridController>();

        // Adding NPC object
        // First NPC
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject))) as GameObject;
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject)),  new Vector3Int(0, 0), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gridObject.transform);
        npcController = npcObject.GetComponent<IsometricNPCController>();
        initialTestingPosition = new Vector3(0, 0);
        npcController.GameGrid = gameGridController;
    }

    [UnityTest]
    // Calculated in Unity world coords
    public IEnumerator TestMovementDOWN()
    {
        Debug.Log("Testing Movement down");
        Debug.Log("NPC Controller: is null ? "+(npcController == null));

        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWN);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWN);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUP()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.UP);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.UP);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementRIGHT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.RIGHT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.RIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementLEFT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.LEFT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.LEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementDOWNLEFT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementDOWNRIGHT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUPLEFT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.UPLEFT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.UPLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementUPRIGHT()
    {
        npcController.Speed = 100;
        npcController.Position = initialTestingPosition;
        Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.5f);
        Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
    }

    [UnityTest]
    public IEnumerator TestMovementIDLE()
    {
        npcController.Position = initialTestingPosition;
        npcController.SetNPCState(NPCState.IDLE);
        target = Util.GetVectorFromDirection(MoveDirection.IDLE);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.6f);
        Assert.That(npcObject.transform.position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(npcObject);
        Object.Destroy(npcController);
    }
}
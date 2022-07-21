using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class TestNPCMovement
{
    private GameObject npcObject;
    private NPCController npcController;
    private Vector3 initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        //Instantiating GameGrid
        GameObject gridObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_GAME_GRID, typeof(GameObject))) as GameObject;
        GameGridController gameGridController = gridObject.GetComponent<GameGridController>();
        // Adding NPC object
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        npcObject.transform.SetParent(npcObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        npcController.SetTestGameGridController(gameGridController);
        initialTestingPosition = new Vector3(0, 0, Settings.DEFAULT_GAME_OBJECTS_Z);
        npcController.Speed = 100;
    }

    [UnityTest]
    public IEnumerator TestMovement()
    {

        npcController.Position = initialTestingPosition;
        Debug.Log(npcController.Position);
        Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWN);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.UP);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.RIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.LEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.UPLEFT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));

        npcController.Position = initialTestingPosition;
        target = Util.GetVectorFromDirection(MoveDirection.IDLE);
        npcController.AddMovement(target);
        yield return new WaitForSeconds(0.4f);
        Assert.That(npcController.transform.position, Is.EqualTo(target).Using(Vector3EqualityComparer.Instance));
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(npcObject);
        Object.Destroy(npcController);
    }
}
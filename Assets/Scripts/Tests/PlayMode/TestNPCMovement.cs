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
        initialTestingPosition = new Vector3(0, 0, 1);
        npcController.SetSpeed(1000);
    }


    [UnityTest]
    public IEnumerator TestMovement()
    {

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.DOWN);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(0, -1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.UP);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(0, 1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.RIGHT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, 0, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.LEFT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, 0, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.DOWNLEFT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, -1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.DOWNRIGHT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, -1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.UPLEFT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, 1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.AddMovement(MoveDirection.UPRIGHT);
        yield return new WaitForSeconds(0.1f);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, 1, 1)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        Vector3 before = npcController.transform.position;
        npcController.AddMovement(MoveDirection.IDLE);
        Assert.That(npcController.transform.position, Is.EqualTo(before).Using(Vector3EqualityComparer.Instance));
    }
}
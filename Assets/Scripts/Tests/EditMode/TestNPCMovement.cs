using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

public class TestNPCMovement
{
    private GameObject npcObject;
    private NPCController npcController;
    private Vector3EqualityComparer vectorComparer;
    private Vector3 initialTestingPosition;

    [SetUp]
    public void Setup()
    {
        // Adding NPC object
        npcObject = Transform.Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        npcObject.transform.SetParent(npcObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        vectorComparer = new Vector3EqualityComparer(10e-6f); // default error 0.0001f.
        initialTestingPosition = new Vector3(0, 0, 0);
    }

    [Test]
    public void TestMovement()
    {
        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.DOWN);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(0, -1, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.UP);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(0, 1, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.RIGHT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, 0, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.LEFT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, 0, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.DOWNLEFT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, -1, 0)).Using(Vector3EqualityComparer.Instance));
        
        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.DOWNRIGHT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, -1, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.UPLEFT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(-1, 1, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        npcController.Move(MoveDirection.UPRIGHT);
        Assert.That(npcController.transform.position, Is.EqualTo(new Vector3(1, 1, 0)).Using(Vector3EqualityComparer.Instance));

        npcController.SetPosition(initialTestingPosition);
        Vector3 before = npcController.transform.position;
        npcController.Move(MoveDirection.IDLE);
        Assert.That(npcController.transform.position, Is.EqualTo(before).Using(Vector3EqualityComparer.Instance));          
    }
}

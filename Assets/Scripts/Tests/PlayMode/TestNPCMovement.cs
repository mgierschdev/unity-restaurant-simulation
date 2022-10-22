using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class TestNPCMovement
{
//     private GameObject npcObject;
//     private NPCController npcController;
//     private Vector3 target;
//     private GameObject gridObject;
//     private Vector3Int initialTestingPosition;

//     [SetUp]
//     public void Setup()
//     {
//         // Adding NPC object
//         // First NPC
//         npcObject = Transform.Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject))) as GameObject;
//         npcObject = Transform.Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject)),  new Vector3Int(0, 0), Quaternion.identity) as GameObject;
//         npcObject.transform.SetParent(gridObject.transform);
//         npcController = npcObject.GetComponent<NPCController>();
//         initialTestingPosition = new Vector3Int(0, 0);
//     }

//     [UnityTest]
//     // Calculated in Unity world coords
//     public IEnumerator TestMovementDOWN()
//     {
//         GameLog.Log("Testing Movement down");
//         GameLog.Log("NPC Controller: is null ? "+(npcController == null));

//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWN);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWN);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementUP()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.UP);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.UP);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementRIGHT()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.RIGHT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.RIGHT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementLEFT()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.LEFT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.LEFT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementDOWNLEFT()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementDOWNRIGHT()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWNRIGHT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementUpLeft()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.UPLEFT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.UPLEFT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementUpRight()
//     {
//         npcController.SetSpeed(100);
//         npcController.Position = initialTestingPosition;
//         Vector3 transformPosition = npcObject.transform.position;
//         Vector3 expected =  transformPosition + Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
//         Vector3 target = Util.GetVectorFromDirection(MoveDirection.UPRIGHT);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.5f);
//         Assert.That(transformPosition, Is.EqualTo(new Vector3(expected.x, expected.y)).Using(FloatEqualityComparer.Instance));
//     }

//     [UnityTest]
//     public IEnumerator TestMovementIdle()
//     {
//         npcController.Position = initialTestingPosition;
//         target = Util.GetVectorFromDirection(MoveDirection.IDLE);
//         npcController.AddMovement(target);
//         yield return new WaitForSeconds(0.6f);
//         Assert.That(npcObject.transform.position, Is.EqualTo(target).Using(FloatEqualityComparer.Instance));
//     }

//     [TearDown]
//     public void TearDown()
//     {
//         Object.Destroy(npcObject);
//         Object.Destroy(npcController);
//     }
}
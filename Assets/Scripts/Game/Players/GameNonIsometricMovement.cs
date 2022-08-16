using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class GameNonIsometricMovement : GameObjectMovementBase
{
    //public GameGridController GameGrid { get; set; }

    // Al objects in screen should have sorting group component
    // private void Awake()
    // {
    //     body = GetComponent<Rigidbody2D>();
    //     sortingLayer = GetComponent<SortingGroup>();

    //     // Game Grid
    //     gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);

    //     if (gameGridObject != null)
    //     {
    //         GameGrid = gameGridObject.GetComponent<GameGridController>();
    //     }
    //     else
    //     {
    //         if (Settings.DEBUG_ENABLE)
    //         {
    //             Debug.LogWarning("GameGridController.cs/gameGridObject null");
    //         }
    //     }

    //     // Movement Queue
    //     nextTarget = Vector3.zero;
    //     pendingMovementQueue = new Queue();

    //     //Update Object initial position
    //     currentTargetPosition = Vector3.negativeInfinity;
    //     UpdatePosition();
    // }

    // Overriding to use GetCellPosition in an Isometric Grid
    // public override void AddMovement()
    // {

    //     if (pendingMovementQueue.Count == 0)
    //     {
    //         return;
    //     }

    //     Vector3 direction = Util.GetCellPosition((Vector3)pendingMovementQueue.Dequeue());
    //     Vector3 nextTarget = new Vector3(direction.x, direction.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    //     this.nextTarget = nextTarget;
    // }

    // public List<Node> GetPath(int[] from, int[] to)
    // {
    //     return GameGrid.GetPath(from, to);
    // }

    // public void AddPath(List<Node> path)
    // {
    //     if (path.Count == 0)
    //     {
    //         return;
    //     }

    //     if (pendingMovementQueue.Count != 0)
    //     {
    //         path = MergePath(path); // We merge Paths
    //     }

    //     pendingMovementQueue.Enqueue(path[0].GetVector3());

    //     for (int i = 1; i < path.Count; i++)
    //     {
    //         Vector3 from = Util.GetCellPosition(path[i - 1].GetVector3());
    //         Vector3 to = Util.GetCellPosition(path[i].GetVector3());

    //         if (Settings.DEBUG_ENABLE)
    //         {
    //             Debug.DrawLine(from, to, Color.yellow, 15f);
    //         }

    //         pendingMovementQueue.Enqueue(path[i].GetVector3());
    //     }

    //     if (path.Count == 0)
    //     {
    //         Debug.LogWarning("Path out of reach");
    //         return;
    //     }

    //     AddMovement(); // To set the first target
    // }

    // public override void UpdateTargetMovement()
    // {
    //     if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
    //     {
    //         nextTarget = Vector3.zero;
    //         currentTargetPosition = Vector3.zero;

    //         if (pendingMovementQueue.Count != 0)
    //         {
    //             AddMovement();
    //         }
    //         else
    //         {
    //             if (Settings.DEBUG_ENABLE)
    //             {
    //                 //Debug.Log("[Moving] Target Reached: " + transform.name + " " + Position);
    //             }
    //         }
    //     }

    //     if (nextTarget != Vector3.zero)
    //     {
    //         currentTargetPosition = nextTarget;
    //         transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, Speed * Time.deltaTime);
    //     }
    // }
}
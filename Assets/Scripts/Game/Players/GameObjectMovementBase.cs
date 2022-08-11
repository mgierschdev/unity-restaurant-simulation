using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public abstract class GameObjectMovementBase : MonoBehaviour
{

    // Getters and setters
    public int X { get; set; }
    public int Y { get; set; }
    public ObjectType Type { get; set; }
    public float Speed { get; set; }
    public Vector3 Position { get; set; } // Position in game map/ grid  GetXYInGameMap

    // Sprite level ordering
    protected SortingGroup sortingLayer;

    // Movement 
    protected Vector2 movement;
    protected Rigidbody2D body;

    //Movement Queue
    protected Queue pendingMovementQueue;
    protected Vector3 nextTarget;
    protected Vector3 currentTargetPosition;
    protected GameObject gameGridObject;

    protected void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        sortingLayer.sortingOrder = pos.y * -1;
        X = pos.x;
        Y = pos.y;
        Position = new Vector3(X, Y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    protected void ResetMovementIfMoving()
    {
        // If the player is moving, we change direction and empty the previous queue
        if (pendingMovementQueue.Count != 0)
        {
            ResetMovementQueue();
        }
    }

    protected bool IsMoving()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Resets the planned Path
    protected void ResetMovementQueue()
    {
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { Position.x, Position.y };
    }

    public List<Node> MergePath(List<Node> path)
    {
        List<Vector3> queuePath = new List<Vector3>();
        List<Node> merge = new List<Node>();

        while (pendingMovementQueue.Count > 0)
        {
            queuePath.Add((Vector3)pendingMovementQueue.Dequeue());
        }

        int index = 0;

        while (index < path.Count && path[index].GetVector3() != queuePath[0])
        {
            index++;
        }

        if (index == path.Count)
        {
            return path;
        }

        for (int i = index; i < path.Count; i++)
        {
            merge.Add(path[i]);
        }
        return merge;
    }
}
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
    protected Rigidbody2D body;

    //Movement Queue
    public Queue pendingMovementQueue;
    protected Vector3 nextTarget;
    protected Vector3 currentTargetPosition;
    protected GameObject gameGridObject;

    virtual public void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        sortingLayer.sortingOrder = pos.y * -1;
        X = pos.x;
        Y = pos.y;
        Position = new Vector3(X, Y);
    }

    virtual public void UpdateTargetMovement()
    {
        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            nextTarget = Vector3.zero;
            currentTargetPosition = Vector3.zero;

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
            else
            {
                if (Settings.DEBUG_ENABLE)
                {
                    //Debug.Log("[Moving] Target Reached: " + transform.name + " " + Position);
                }
            }
        }

        if (nextTarget != Vector3.zero)
        {
            currentTargetPosition = nextTarget;
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, Speed * Time.deltaTime);
        }
    }

    public void AddMovement(Vector3 direction)
    {
        if (direction != new Vector3(0, 0))
        {
            Vector3 newDirection = direction + transform.position;
            this.nextTarget = new Vector3(newDirection.x, newDirection.y);
        }
    }

    public virtual void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 direction = Util.GetCellPosition((Vector3)pendingMovementQueue.Dequeue());
        Vector3 nextTarget = new Vector3(direction.x, direction.y);
        this.nextTarget = nextTarget;
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
        if (pendingMovementQueue == null || pendingMovementQueue.Count == 0)
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
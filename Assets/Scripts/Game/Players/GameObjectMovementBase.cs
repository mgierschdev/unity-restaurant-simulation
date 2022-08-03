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
    public GameGridController GameGrid { get; set; }

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

    // Al objects in screen should have sorting group component
    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sortingLayer = GetComponent<SortingGroup>();

        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);

        if (gameGridObject != null)
        {
            GameGrid = gameGridObject.GetComponent<GameGridController>();
        }
        else
        {
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogWarning("NPCController.cs/gameGridObject null");
            }
        }

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

        //Update Object initial position
        currentTargetPosition = Vector3.negativeInfinity;
        UpdatePosition();
    }

    protected void UpdateTargetMovement()
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

    protected void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 direction = GameGrid.GetCellPosition((Vector3)pendingMovementQueue.Dequeue());
        Vector3 nextTarget = new Vector3(direction.x, direction.y, Settings.DEFAULT_GAME_OBJECTS_Z);
        this.nextTarget = nextTarget;
    }

    public void AddMovement(Vector3 direction)
    {
        if (direction != Util.GetVectorFromDirection(MoveDirection.IDLE))
        {
            Vector3 newDirection = direction + transform.position;
            this.nextTarget = new Vector3(newDirection.x, newDirection.y, Settings.DEFAULT_GAME_OBJECTS_Z);
        }
    }

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

    public void AddPath(List<Node> path)
    {
        if (path.Count == 0)
        {
            return;
        }

        if (pendingMovementQueue.Count != 0)
        {
            path = MergePath(path); // We merge Paths
        }

        pendingMovementQueue.Enqueue(path[0].GetVector3());

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 from = GameGrid.GetCellPosition(path[i - 1].GetVector3());
            Vector3 to = GameGrid.GetCellPosition(path[i].GetVector3());
            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(from, to, Color.magenta, 10f);
            }
            pendingMovementQueue.Enqueue(path[i].GetVector3());
        }

        if (path.Count == 0)
        {
            Debug.LogWarning("Path out of reach");
            return;
        }

        AddMovement(); // To set the first target
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
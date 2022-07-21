using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameObjectMovementBase : MonoBehaviour
{

    // Getters and setters
    public int X { get; set; }
    public int Y { get; set; }
    public ObjectType Type { get; set; }
    public float Speed { get; set; }
    public Vector3 Position { get; set; }

    // Movement 
    protected Vector2 movement;
    protected Rigidbody2D body;
    protected GameGridController gameGrid;

    //Movement Queue
    protected Queue pendingMovementQueue;
    protected Vector3 nextTarget;
    protected Vector3 currentTargetPosition;
    protected GameObject gameGridObject;


    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);

        if (gameGridObject != null)
        {
            gameGrid = gameGridObject.GetComponent<GameGridController>();
        }
        else
        {
            Debug.LogWarning("NPCController.cs/gameGridObject null");
        }

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

         //Update NPC initial position
        currentTargetPosition = transform.position;
        UpdatePosition();
    }

    protected void UpdateTargetMovement()
    {

        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            Debug.Log("UpdateTargetMovement / " + currentTargetPosition + " " + transform.position);
            nextTarget = Vector3.zero;
            currentTargetPosition = Vector3.zero;

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
            else
            {
                // Target Reached
            }
        }

        if (nextTarget != Vector3.zero)
        {
            Debug.Log("UpdateTargetMovement /nextTarget " + nextTarget);
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

        Vector3 direction = (Vector3)pendingMovementQueue.Dequeue();
        Vector3 nextTarget = new Vector3(direction.x, direction.y, Settings.DEFAULT_GAME_OBJECTS_Z);
        this.nextTarget = nextTarget;
    }

    protected void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
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

    protected void OnCollisionEnter2D(Collision2D other)
    {
        Debug.LogWarning("Colliding " + other.GetType());
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { Position.x, Position.y };
    }

    // Only for unit testing
    public void SetTestGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public void AddMovement(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Debug.Log("Adding movement " + direction);
            Vector3 newDirection = direction + transform.position;
            this.nextTarget = new Vector3(newDirection.x, newDirection.y, Settings.DEFAULT_GAME_OBJECTS_Z);
        }
    }

    public void AddPath(List<Node> list)
    {
        Util.AddPath(list, gameGrid, pendingMovementQueue);
        AddMovement(); // To set the first target
    }
}
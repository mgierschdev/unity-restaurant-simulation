using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectMovementBase : MonoBehaviour
{

    // Getters and setters
    [SerializeField]
    public ObjectType Type { get; set; }
    [SerializeField]
    public float Speed { get; set; }
    [SerializeField]
    public Vector3 Velocity { get; set; }
    [SerializeField]
    public Vector3Int Position { get; set; } //PathFindingGrid Position
    public IsometricGridController GameGrid { get; set; }
    // Sprite level ordering
    protected SortingGroup sortingLayer;
    // Movement 
    protected Rigidbody2D body;
    //Movement Queue
    [SerializeField]
    public Queue pendingMovementQueue;
    [SerializeField]
    protected Vector3 currentTargetPosition;
    protected GameObject gameGridObject;
    // ClickController for the long click duration for the player
    private ClickController clickController;

    private void Awake()
    {
        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);
        body = GetComponent<Rigidbody2D>();

        if (gameGridObject != null)
        {
            GameGrid = gameGridObject.GetComponent<IsometricGridController>();
        }
        else
        {
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogWarning("IsometricGridController.cs/gameGridObject null");
            }
        }
        // Movement Queue
        pendingMovementQueue = new Queue();

        //Update Object initial position
        currentTargetPosition = transform.position;
    }

    // Overlap spehre
    private void Start()
    {
        UpdatePosition();
        ClickUpdateController();
    }

    protected void UpdateTargetMovement()
    {
        body = GetComponent<Rigidbody2D>();
        sortingLayer = GetComponent<SortingGroup>();


        if (currentTargetPosition == transform.position)
        {
            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
            else
            {
                if (Settings.DEBUG_ENABLE)
                {
                    //("[Moving] Target Reached: " + transform.name + " " + Position);
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, Speed * Time.deltaTime);
        }
    }
    private void ClickUpdateController()
    {
        Type = ObjectType.PLAYER;
        Speed = Settings.PLAYER_MOVEMENT_SPEED;
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.CONST_PARENT_GAME_OBJECT);

        if (cController != null)
        {
            clickController = cController.GetComponent<ClickController>();
        }
        else if (Settings.DEBUG_ENABLE)
        {
            Debug.LogWarning("PlayerController/clickController null");
        }
    }
    public void AddMovement(Vector3 direction)
    {
        if (direction != new Vector3(0, 0))
        {
            Vector3 newDirection = direction + transform.position;
            currentTargetPosition = new Vector3(newDirection.x, newDirection.y);
        }
    }

    public void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 queuePosition = (Vector3)pendingMovementQueue.Dequeue();
        Vector3 direction = GameGrid.GetWorldFromPathFindingGridPosition(new Vector3Int((int)queuePosition.x, (int)queuePosition.y));
        currentTargetPosition = new Vector3(direction.x, direction.y);
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
        currentTargetPosition = Vector3.negativeInfinity;
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

    virtual public void UpdatePosition()
    {
        Vector3Int pos = GameGrid.GetPathFindingGridFromWorldPosition(transform.position);
        //  sortingLayer.sortingOrder = pos.y * -1;S
        Position = new Vector3Int(pos.x, pos.y);
    }

    public List<Node> GetPath(int[] from, int[] to)
    {
        return GameGrid.GetPath(from, to);
    }

    public void MovingOnLongtouch()
    {
        if (clickController != null && clickController.IsLongClick)
        {
            ResetMovementIfMoving();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3 delta = mousePosition - transform.position;

            float radians = Mathf.Atan2(delta.x, delta.y);
            float degrees = radians * Mathf.Rad2Deg;

            // normalizing -180-180, 0-360
            if (degrees < 0)
            {
                degrees += 360;
            }

            AddMovement(Util.GetVectorFromDirection(Util.GetDirectionFromAngles(degrees)));

            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(transform.position, mousePosition, Color.blue);
            }
        }
    }

    public void AddPath(List<Node> path)
    {
        Debug.Log("Adding path "+path.Count);
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
            Vector3 from = GameGrid.GetWorldFromPathFindingGridPosition(path[i - 1].GetVector3Int());
            Vector3 to = GameGrid.GetWorldFromPathFindingGridPosition(path[i].GetVector3Int());

            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(from, to, Color.yellow, 15f);
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

    public void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0) && clickController != null && !clickController.IsLongClick)
        {
            UpdatePosition();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3Int mouseInGridPosition = GameGrid.GetPathFindingGridFromWorldPosition(mousePosition);
            List<Node> path = GetPath(new int[] { (int)Position.x, (int)Position.y }, new int[] { mouseInGridPosition.x, mouseInGridPosition.y });
            AddPath(path);

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
        }
    }

    public void SetClickController(ClickController controller)
    {
        this.clickController = controller;
    }
}
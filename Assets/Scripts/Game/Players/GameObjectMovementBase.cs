using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
public abstract class GameObjectMovementBase : MonoBehaviour
{
    public string Name { get; set; }
    [SerializeField]
    // Getters and setters
    public ObjectType Type { get; set; }
    [SerializeField]
    public float Speed { get; set; }
    [SerializeField]
    public Vector3 Velocity { get; set; }
    [SerializeField]
    public Vector3Int Position { get; set; } //PathFindingGrid Position
    public GridController GameGrid { get; set; }
    public NPCState state;

    // Sprite level ordering
    protected SortingGroup sortingLayer;
    // Movement 
    protected Rigidbody2D body;
    //Movement Queue
    protected float minDistanceToTarget = 0.2f;
    [SerializeField]
    public Queue pendingMovementQueue;
    [SerializeField]
    protected Vector3 currentTargetPosition;
    protected Vector3Int FinalTarget { get; set; }
    protected GameObject gameGridObject;
    // ClickController for the long click duration for the player
    private ClickController clickController;

    //Energy Bar
    private EnergyBarController energyBar;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;

    // Wander properties
    [SerializeField]
    private float idleTime = 0;
    [SerializeField]
    private float idleMaxTime = 6f; //in seconds
    private float randMax = 3f;

    // Debug attributes
    [SerializeField]
    public string NPCDebug { get; set; }
    [SerializeField]
    private Queue<string> stateHistory;
    [SerializeField]
    private int stateHistoryMaxSize = 10;

    private void Awake()
    {
        // Debug parameters
        stateHistory = new Queue<string>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBarController>();
        if (!Util.IsNull(energyBar, "GameObjectMovementBase/energyBar null"))
        {
            SetEnergyBar();
        }


        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);
        body = GetComponent<Rigidbody2D>();

        if (!Util.IsNull(gameGridObject, "GameObjectMovementBase/gameGridObject null"))
        {
            GameGrid = gameGridObject.GetComponent<GridController>();
        }

        // Movement Queue
        pendingMovementQueue = new Queue();

        //Update Object initial position
        currentTargetPosition = transform.position;

        FinalTarget = Util.GetVector3IntPositiveInfinity();
    }

    // Overlap spehre
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sortingLayer = GetComponent<SortingGroup>();
        UpdatePosition();
        ClickUpdateController();
    }

    private void SetEnergyBar()
    {
        if (Settings.NPC_ENERGY_ENABLED)
        {
            energyBar.SetActive();
        }
        else
        {
            energyBar.SetInactive();
        }
    }

    protected void GoTo(Vector3Int pos)
    {
        List<Node> path = GameGrid.GetPath(new int[] { (int)Position.x, (int)Position.y }, new int[] { pos.x, pos.y });
        AddStateHistory("Time: " + Time.fixedTime + " d: " + path.Count + " t: " + pos.x + "," + pos.y);
        FinalTarget = pos;
        AddPath(path);
    }


    protected void UpdateEnergyBar()
    {
        // EnergyBar controller, only if it is active
        if (energyBar.IsActive())
        {
            if (currentEnergy > 0)
            {
                currentEnergy -= Time.deltaTime * 20f;
                energyBar.SetEnergy((int)currentEnergy);
            }
            else
            {
                currentEnergy = 100;
                energyBar.SetEnergy(Settings.NPC_DEFAULT_ENERGY);
            }
        }
    }

    private void ActivateEnergyBar()
    {
        energyBar.SetActive();
    }

    private void AddEnergyBar()
    {
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBarController>();
    }


    protected void UpdateTargetMovement()
    {
        if (IsInTargetPosition())
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

        if (!Util.IsNull(cController, "PlayerController/clickController null"))
        {
            clickController = cController.GetComponent<ClickController>();
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

    private bool IsInTargetPosition()
    {
        return Vector3.Distance(currentTargetPosition, transform.position) < minDistanceToTarget;
    }

    public bool IsInFinalTargetPosition()
    {
        return Vector3.Distance(FinalTarget, Position) < minDistanceToTarget || FinalTarget == Util.GetVector3IntPositiveInfinity();
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
        Position = GameGrid.GetPathFindingGridFromWorldPosition(transform.position);
        body.angularVelocity = 0;
        body.rotation = 0;
        Position = new Vector3Int(Position.x, Position.y);
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

    protected void Wander()
    {
        if (!IsMoving())
        {
            // we could add more random by deciding to move or not 
            idleTime += Time.deltaTime;
        }

        if (!IsMoving() && idleTime >= randMax)
        {
            List<Node> path;
            idleTime = 0;
            randMax = Random.Range(0, idleMaxTime);
            Vector3Int position = GameGrid.GetRandomWalkableGridPosition();
            // It should be mostly free, if invalid it will return an empty path
            path = GameGrid.GetPath(new int[] { (int)Position.x, (int)Position.y }, new int[] { position.x, position.y });
            AddStateHistory("Time: " + Time.fixedTime + " d: " + path.Count + " t: " + position.x + "," + position.y);
            AddPath(path);
        }
    }
    public void SetNPCState(NPCState state)
    {
        this.state = state;
    }

    private void SetDebug()
    {
        NPCDebug = "";
        for (int i = 0; i < stateHistory.Count; i++)
        {
            NPCDebug += stateHistory.ElementAt(i) + "<br>";
        }
    }

    protected void AddStateHistory(string s)
    {
        stateHistory.Enqueue(s);

        if (stateHistory.Count > stateHistoryMaxSize)
        {
            stateHistory.Dequeue();
        }
        SetDebug();
    }

    public void SetClickController(ClickController controller)
    {
        this.clickController = controller;
    }

    public bool IsWalking()
    {
        return pendingMovementQueue.Count > 0;
    }
}
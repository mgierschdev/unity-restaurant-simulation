using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectMovementBase : MonoBehaviour
{
    public string Name { get; set; }
    // Getters and setters
    protected ObjectType Type { get; set; }
    public Vector3Int Position { get; set; } //PathFindingGrid Position

    // Movement 
    private MoveDirection moveDirection;
    private bool side; // false right, true left

    //Movement Queue
    private Queue pendingMovementQueue;
    private Vector3 currentTargetPosition;
    private Vector3Int FinalTarget { get; set; }
    private GameObject gameGridObject;

    // ClickController for the long click duration for the player
    private ClickController clickController;

    //Energy Bars
    private EnergyBarController energyBar;
    protected float CurrentEnergy { get; set; }
    private float speedDecreaseEnergyBar;

    // Debug attributes
    private string npcDebug;
    private Queue<string> stateHistory;
    private const int STATE_HISTORY_MAX_SIZE = 20;
    private SortingGroup sortingLayer;
    protected Rigidbody2D rb2D;
    private float speed;
    private bool isMoving;

    private void Awake()
    {
        //Setting default init name
        Name = transform.name;

        //Sortering layer
        sortingLayer = transform.GetComponent<SortingGroup>();

        // Debug parameters
        stateHistory = new Queue<string>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NpcEnergyBar).gameObject.GetComponent<EnergyBarController>();
        if (!Util.IsNull(energyBar, "GameObjectMovementBase/energyBar null"))
        {
            energyBar.SetInactive();
        }

        // Movement Queue
        pendingMovementQueue = new Queue();

        //Update Object initial position
        currentTargetPosition = transform.position;
        FinalTarget = Util.GetVector3IntPositiveInfinity();
        side = false; // The side in which the character is facing by default = false meaning right.
        speedDecreaseEnergyBar = 20f;

        //Velocity for the 2D rigidbody
        rb2D = transform.GetComponent<Rigidbody2D>();
        speed = Settings.NpcDefaultMovementSpeed;

        isMoving = false;
    }

    // Overlap sphere
    private void Start()
    {
        UpdatePosition();
        ClickUpdateController();
    }

    protected void ActivateEnergyBar(float val)
    {
        if (!energyBar.IsActive())
        {
            energyBar.SetActive();
            CurrentEnergy = 0;
            speedDecreaseEnergyBar = val;
        }
    }

    protected void StandTowards(Vector3Int target)
    {
        MoveDirection m = GetDirectionFromPositions(Position, target);
        if (m == MoveDirection.LEFT || m == MoveDirection.DOWNLEFT || m == MoveDirection.UPLEFT)
        {
            //flip left
        }
        else if (m == MoveDirection.RIGHT || m == MoveDirection.DOWNRIGHT || m == MoveDirection.UPRIGHT)
        {
            // flip right
        }
        else
        {
            //stay in your final position when up or down
        }
    }

    protected void UpdateEnergyBar()
    {
        // EnergyBar controller, only if it is active
        if (energyBar.IsActive())
        {
            if (CurrentEnergy <= 100)
            {
                CurrentEnergy += Time.fixedDeltaTime * speedDecreaseEnergyBar;
                energyBar.SetEnergy((int)CurrentEnergy);
            }
            else
            {
                energyBar.SetInactive();
            }
        }
    }

    protected void UpdatePosition()
    {
        Position = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
        Position = new Vector3Int(Position.x, Position.y);
        sortingLayer.sortingOrder = Util.GetSorting(Position);
    }

    private void UpdateObjectDirection()
    {
        if (side && (moveDirection == MoveDirection.DOWN ||
                     moveDirection == MoveDirection.UPRIGHT ||
                     moveDirection == MoveDirection.DOWNRIGHT ||
                     moveDirection == MoveDirection.RIGHT))
        {
            side = false;
            FlipSide();
        }
        else if (!side && (moveDirection == MoveDirection.UP ||
                           moveDirection == MoveDirection.UPLEFT ||
                           moveDirection == MoveDirection.DOWNLEFT ||
                           moveDirection == MoveDirection.LEFT))
        {
            side = true;
            FlipSide();
        }
    }

    private void FlipSide()
    {
        Vector3 tmp = gameObject.transform.localScale;
        tmp.x = -tmp.x;
        transform.localScale = tmp;
    }

    // false right, true left    
    private void FlipToSide(bool side)-
    {

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
                //target reached 
                moveDirection = MoveDirection.IDLE;
                isMoving = false;
            }
        }
        else
        {
            moveDirection = GetDirectionFromPositions(transform.position, currentTargetPosition);
            UpdateObjectDirection(); // It flips the side of the pbject depending on direction
                                     //rb2D.MovePosition(Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.fixedDeltaTime));
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.fixedDeltaTime);
        }
    }

    private void ClickUpdateController()
    {
        Type = ObjectType.PLAYER;
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);

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

    private void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 queuePosition = (Vector3)pendingMovementQueue.Dequeue();
        Vector3 direction = BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(new Vector3Int((int)queuePosition.x, (int)queuePosition.y));
        currentTargetPosition = new Vector3(direction.x, direction.y);
    }

    private void ResetMovementIfMoving()
    {
        // If the player is moving, we change direction and empty the previous queue
        if (pendingMovementQueue.Count != 0)
        {
            ResetMovementQueue();
        }
    }

    protected bool IsMoving()
    {
        return isMoving;
    }

    // Resets the planned Path
    private void ResetMovementQueue()
    {
        currentTargetPosition = Vector3.negativeInfinity;
        pendingMovementQueue = new Queue();
    }

    protected void ResetMovement()
    {
        currentTargetPosition = transform.position; // we are already at target and not moving
        pendingMovementQueue = new Queue();
        if (energyBar.IsActive())
        {
            energyBar.SetInactive();
        }
    }

    private List<Node> MergePath(List<Node> path)
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

    //A to B direction
    private MoveDirection GetDirectionFromPositions(Vector3 a, Vector3 b)
    {
        Vector3 delta = b - a;
        float radians = Mathf.Atan2(delta.x, delta.y);
        float degrees = radians * Mathf.Rad2Deg;
        // normalizing -180-180, 0-360
        if (degrees < 0)
        {
            degrees += 360;
        }

        return Util.GetDirectionFromAngles(degrees);
    }

    protected void MovingOnLongTouch()
    {
        if (!clickController || !clickController.IsLongClick)
        {
            return;
        }

        ResetMovementIfMoving();
        Vector3 mousePosition = Util.GetMouseInWorldPosition();
        AddMovement(Util.GetVectorFromDirection(GetDirectionFromPositions(transform.position, mousePosition)));

        // if (Settings.DEBUG_ENABLE)
        // {
        //     GameLog.DrawLine(transform.position, mousePosition, Color.blue);
        // }
    }

    private void AddPath(List<Node> path)
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
            if (Settings.CellDebug)
            {
                Vector3 from = BussGrid.GetWorldFromPathFindingGridPosition(path[i - 1].GetVector3Int());
                Vector3 to = BussGrid.GetWorldFromPathFindingGridPosition(path[i].GetVector3Int());
                Debug.DrawLine(from, to, Util.GetRandomColor(), 15f);
            }

            pendingMovementQueue.Enqueue(path[i].GetVector3());
        }

        if (path.Count == 0)
        {
            GameLog.LogWarning("Path out of reach");
            return;
        }

        AddMovement(); // To set the first target
    }

    protected void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0) && clickController && !clickController.IsLongClick)
        {
            UpdatePosition();
            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3Int mouseInGridPosition = BussGrid.GetPathFindingGridFromWorldPosition(mousePosition);
            GoTo(mouseInGridPosition);
        }
    }

    private void SetDebug()
    {
        npcDebug = "";
        for (int i = 0; i < stateHistory.Count; i++)
        {
            npcDebug += stateHistory.ElementAt(i) + "<br>";
        }
    }

    private void AddStateHistory(string s)
    {
        stateHistory.Enqueue(s);

        if (stateHistory.Count > STATE_HISTORY_MAX_SIZE)
        {
            stateHistory.Dequeue();
        }

        SetDebug();
    }

    public bool GoTo(Vector3Int pos)
    {
        List<Node> path = BussGrid.GetPath(new[] { Position.x, Position.y }, new[] { pos.x, pos.y });

        if (path.Count == 0)
        {
            GameLog.Log("No path found " + transform.name + " From " + Position + " to " + pos);
            return false;
        }

        isMoving = true;
        AddStateHistory("Time: " + Time.fixedTime + " steps: " + path.Count + " t: " + pos.x + "," + pos.y);
        FinalTarget = pos;
        AddPath(path);
        if (pendingMovementQueue.Count != 0)
        {
            AddMovement();
        }

        AddStateHistory("Pending movingQueue Size: " + pendingMovementQueue.Count);
        return true;
    }

    public void SetClickController(ClickController controller)
    {
        this.clickController = controller;
    }

    public string GetDebugInfo()
    {
        return npcDebug;
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { Position.x, Position.y };
    }

    private bool IsInTargetPosition()
    {
        return Util.IsAtDistanceWithObject(currentTargetPosition, transform.position);
    }

    public bool IsInFinalTargetPosition()
    {
        return Util.IsAtDistanceWithObject(FinalTarget, Position) ||
               FinalTarget == Util.GetVector3IntPositiveInfinity();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectMovementBase : MonoBehaviour
{
    public string Name { get; set; }
    public Vector3Int Position { get; set; } //PathFindingGrid Position
    private MoveDirection moveDirection;
    private CharacterSide side; // false right, true left
    //Movement Queue
    private Vector3 currentTargetPosition;
    private GameObject gameGridObject;
    //Energy Bars
    private EnergyBarController energyBar;
    [SerializeField]
    protected float currentEnergy, energyBarSpeed, idleTime, stateTime, speed, timeBeforeRemovingDebugPanel = 0.1f;
    private const int STATE_HISTORY_MAX_SIZE = 20;
    private SortingGroup sortingLayer;
    [SerializeField]
    protected NpcState currentState, prevState;
    protected PlayerAnimationStateController animationController;
    protected GameController gameController;
    protected ObjectType type;
    [SerializeField]
    private Queue pendingMovementQueue;
    // Attributes for temporaly marking the path of the NPC on the grid
    // This will help to void placing objects on top of the NPC
    private HashSet<Vector3Int> positionAdded;
    private Queue<Pair<float, Vector3Int>> npcPrevPositions;

    //Final target 
    protected Vector3 currentTargetWorldPosition;
    protected Vector3Int currentTargetGridPosition;

    //State machine
    protected GameGridObject table;
    protected StateMachine stateMachine;
    protected bool[] transitionStates;
    protected bool tableMoved, waitingAtTable, attended, beingAttended, orderServed;

    private void Awake()
    {
        Name = transform.name;
        currentTargetPosition = transform.position;
        speed = Settings.NpcDefaultMovementSpeed;
        side = CharacterSide.RIGHT;
        pendingMovementQueue = new Queue();
        positionAdded = new HashSet<Vector3Int>();
        npcPrevPositions = new Queue<Pair<float, Vector3Int>>();

        GameObject gameObject = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObject.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();
        sortingLayer = transform.GetComponent<SortingGroup>();
        energyBar = transform.Find(Settings.NpcEnergyBar).GetComponent<EnergyBarController>();

        if (!Util.IsNull(energyBar, "GameObjectMovementBase/energyBar null"))
        {
            energyBar.SetInactive();
        }

        if (animationController == null || gameObject == null)
        {
            GameLog.LogWarning("NPCController/animationController-gameObj null");
        }
    }

    // Overlap sphere
    private void Start()
    {
        idleTime = 0;
        stateTime = 0;
        prevState = currentState;
        Name = transform.name;
        energyBarSpeed = 20f;
        transitionStates = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];
        tableMoved = false;
        waitingAtTable = false;
        attended = false;
        beingAttended = false;
        orderServed = false;
        UpdatePosition();
    }

    protected void ActivateEnergyBar(float val)
    {
        if (!energyBar.IsActive())
        {
            energyBar.SetActive();
            currentEnergy = 0;
            energyBarSpeed = val;
        }
    }

    protected void StandTowards(Vector3Int target)
    {
        MoveDirection m = GetDirectionFromPositions(Position, target);
        if (m == MoveDirection.LEFT || m == MoveDirection.DOWNLEFT || m == MoveDirection.UPLEFT || m == MoveDirection.UP)
        {
            FlipToSide(CharacterSide.LEFT);
        }
        else if (m == MoveDirection.RIGHT || m == MoveDirection.DOWNRIGHT || m == MoveDirection.UPRIGHT || m == MoveDirection.DOWN)
        {
            FlipToSide(CharacterSide.RIGHT);
        }
    }

    protected void UpdateEnergyBar()
    {
        if (energyBar == null)
        {
            return;
        }

        // EnergyBar controller, only if it is active
        if (energyBar.IsActive())
        {
            if (currentEnergy <= 100)
            {
                currentEnergy += Time.fixedDeltaTime * energyBarSpeed;
                energyBar.SetEnergy((int)currentEnergy);
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
        //Sorting (sprite) the layer depending on the player position
        sortingLayer.sortingOrder = Util.GetSorting(Position);
        // We mark the grid with the current NPC position
        if (!positionAdded.Contains(Position))
        {
            positionAdded.Add(Position);// we add once and we remove inside the co-routine
            Pair<float, Vector3Int> current = new Pair<float, Vector3Int>(Time.fixedTime, Position);
            npcPrevPositions.Enqueue(current);
            BussGrid.MarkNPCPosition(Position);

            if (Time.fixedTime - npcPrevPositions.Peek().Key > timeBeforeRemovingDebugPanel)
            {
                while (npcPrevPositions.Count > 0 && (Time.fixedTime - npcPrevPositions.Peek().Key > timeBeforeRemovingDebugPanel))
                {
                    positionAdded.Remove(npcPrevPositions.Peek().Value);
                    BussGrid.RemoveMarkNPCPosition(npcPrevPositions.Peek().Value);
                    npcPrevPositions.Dequeue();
                }
            }
        }
    }

    private void UpdateObjectDirection()
    {
        if (side == CharacterSide.LEFT &&
                    (moveDirection == MoveDirection.DOWN ||
                     moveDirection == MoveDirection.UPRIGHT ||
                     moveDirection == MoveDirection.DOWNRIGHT ||
                     moveDirection == MoveDirection.RIGHT))
        {
            FlipToSide(CharacterSide.RIGHT);
        }
        else if (side == CharacterSide.RIGHT &&
                          (moveDirection == MoveDirection.UP ||
                           moveDirection == MoveDirection.UPLEFT ||
                           moveDirection == MoveDirection.DOWNLEFT ||
                           moveDirection == MoveDirection.LEFT))
        {
            FlipToSide(CharacterSide.LEFT);
        }
    }

    private void FlipToSide(CharacterSide flipSide)
    {
        Vector3 tmp = gameObject.transform.localScale;

        if (flipSide == CharacterSide.LEFT)
        {
            tmp.x = tmp.x > 0 ? -tmp.x : tmp.x;
        }
        else
        {
            tmp.x = tmp.x < 0 ? -tmp.x : tmp.x;
        }

        transform.localScale = tmp;
        side = flipSide;
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
            }
        }
        else
        {
            moveDirection = GetDirectionFromPositions(transform.position, currentTargetPosition);
            UpdateObjectDirection(); // It flips the side of the object depending on direction
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.fixedDeltaTime);
        }
    }

    protected void UpdateAnimation()
    {
        // TODO: for performance reasons only animate inside camera CLAMP --> animationController.SetState(NpcState.IDLE);
        // Animates depending on the current state
        if (IsMoving())
        {
            animationController.SetState(NpcState.WALKING);
        }
        else
        {
            animationController.SetState(currentState);
        }
    }

    protected void UpdateTimeInState()
    {
        // keeps the time in the current state
        if (prevState == currentState)
        {
            //Log("Current state time "+stateTime);
            stateTime += Time.fixedDeltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = currentState;
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

    protected bool IsMoving()
    {
        return pendingMovementQueue.Count != 0;
    }

    // Resets the planned Path
    private void ResetMovementQueue()
    {
        currentTargetPosition = Vector3.negativeInfinity;
        pendingMovementQueue = new Queue();
    }

    protected void ResetMovement()
    {
        // we are already at target and not moving
        currentTargetPosition = transform.position;
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

    public bool GoTo(Vector3Int pos)
    {
        List<Node> path = BussGrid.GetPath(new[] { Position.x, Position.y }, new[] { pos.x, pos.y });

        if (path.Count == 0)
        {
            return false;
        }

        currentTargetGridPosition = pos;
        currentTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPosition(pos);

        AddPath(path);

        if (pendingMovementQueue.Count != 0)
        {
            AddMovement();
        }

        return true;
    }

    public void RecalculateGoTo()
    {
        if (!GoTo(currentTargetGridPosition))
        {
            return;
        }
    }

    private bool IsInTargetPosition()
    {
        return Util.IsAtDistanceWithObject(currentTargetPosition, transform.position);
    }

    public float GetSpeed()
    {
        return speed;
    }
}
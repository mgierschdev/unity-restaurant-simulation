using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectMovementBase : MonoBehaviour
{
    public string Name { get; set; }
    public Vector3Int Position { get; set; } //PathFindingGrid Position
    public Vector3Int PrevGridPosition { get; set; }
    private MoveDirection moveDirection;
    private CharacterSide side; // false right, true left
    private GameObject gameGridObject;
    //Energy Bars
    private EnergyBarController energyBar;
    [SerializeField]
    protected float currentEnergy, energyBarSpeed, idleTime, stateTime, speed, timeBeforeRemovingDebugPanel = 0.1f;
    private bool isMoving;//, AStar = false, BugPathFinding = true;
    private const int STATE_HISTORY_MAX_SIZE = 20;
    private SortingGroup sortingLayer;
    [SerializeField]
    private NpcState currentState, prevState;
    protected PlayerAnimationStateController animationController;
    protected GameController gameController;
    protected ObjectType type;
    private PolygonCollider2D collider; // to check for other npc

    //A*
    // Attributes for temporaly marking the path of the NPC on the grid
    // This will help to void placing objects on top of the NPC
    private Queue pendingMovementQueue;
    private Vector3 currentLocalTargetPosition; //step by step target to
    private HashSet<Vector3Int> positionAdded;
    private Queue<Pair<float, Vector3Int>> npcPrevPositions;
    //A*

    //Bug Path finding
    // private Vector3Int nextBugTargetGridPosition;
    // private Vector3 nextBugTargetWorldPosition;
    // 
    //Bug Path finding

    //Final target 
    protected Vector3 currentTargetWorldPosition;
    protected Vector3Int currentTargetGridPosition;

    //State machine
    protected GameGridObject table;
    protected StateMachine<NpcState, NpcStateTransitions> stateMachine;


    private void Awake()
    {
        currentLocalTargetPosition = transform.position;
        speed = Settings.NpcDefaultMovementSpeed;
        side = CharacterSide.RIGHT;
        pendingMovementQueue = new Queue();
        positionAdded = new HashSet<Vector3Int>();
        npcPrevPositions = new Queue<Pair<float, Vector3Int>>();

        GameObject gameObject = GameObject.Find(Settings.ConstParentGameObject);
        collider = gameObject.GetComponent<PolygonCollider2D>();
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

    private void Start()
    {
        idleTime = 0;
        stateTime = 0;
        prevState = currentState;
        energyBarSpeed = 20f;
        isMoving = false;
        currentState = NpcState.IDLE;
        // nextBugTargetGridPosition = Vector3Int.zero;
        // nextBugTargetWorldPosition = Vector3.zero;
        UpdatePosition();
    }

    protected void SetID()
    {
        string id = BussGrid.GameController.GetNpcSet().Count + 1 + "-" + Time.frameCount;
        transform.name = type.ToString() + "." + id;
        Name = transform.name;
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
        UpdatePrevPosition();
        //Sorting (sprite) the layer depending on the player position
        sortingLayer.sortingOrder = Util.GetSorting(Position);

        // TOOD: remove for UpdatePrevPosition(), Only for DEBUG
        // We mark the grid with the current NPC position
        // Performance relevant
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
        if (!isMoving)
        {
            return;
        }

        // if (AStar)
        // {
        UpdateAStarMovement();
        // }
        // else if (BugPathFinding)
        // {
        //     UpdateBugPathMovement();
        // }
    }

    // private void UpdateBugPathMovement()
    // {
    //     Debug.Log("Is at distance : " + currentTargetWorldPosition + "," + transform.position + " " +
    //     Vector3.Distance(new Vector3(currentTargetWorldPosition.x, currentTargetWorldPosition.y, 0), new Vector3(transform.position.x, transform.position.y, 0)));

    //     if (Vector3.Distance(new Vector3(currentTargetWorldPosition.x, currentTargetWorldPosition.y, 0), new Vector3(transform.position.x, transform.position.y, 0)) <= 0.4f)
    //     {
    //         //final target reached 
    //         moveDirection = MoveDirection.IDLE;
    //         isMoving = false;
    //     }
    //     else if (Util.IsAtDistanceWithObject(nextBugTargetWorldPosition, transform.position) || (nextBugTargetGridPosition.x == 0 && nextBugTargetGridPosition.y == 0))
    //     {
    //         //Get next move close to 
    //         // // Assign nextBugTargetPosition
    //         // nextBugTargetGridPosition = Move // NextPathFindingBugPosition();
    //         // nextBugTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPosition(nextBugTargetGridPosition);

    //         //nextBugTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(Position + Util.GetVector3IntNegativeInfinity);
    //         //nextBugTargetGridPosition = BussGrid.GetLocalGridFromWorldPosition(nextBugTargetWorldPosition);
    //         nextBugTargetGridPosition = GetNextBugCell();
    //         nextBugTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(nextBugTargetGridPosition);
    //         //Debug.Log("Next Bug " + nextBugTargetGridPosition + " " + nextBugTargetWorldPosition);
    //     }
    //     else
    //     {
    //         moveDirection = GetDirectionFromPositions(transform.position, BussGrid.GetWorldFromPathFindingGridPosition(nextBugTargetGridPosition));
    //         // Move arround the wall/ or colliders objects
    //         UpdateObjectDirection(); // It flips the side of the object depending on direction
    //         transform.position = Vector3.MoveTowards(transform.position, nextBugTargetWorldPosition, speed * Time.fixedDeltaTime);
    //     }
    // }

    // private Vector3Int GetNextBugCell()
    // {
    //     double min = double.MaxValue;
    //     Vector3Int sol = Vector3Int.zero;

    //     for (int i = (int)MoveDirection.UP; i <= (int)MoveDirection.RIGHT; i++)
    //     {
    //         MoveDirection direction = (MoveDirection)i;
    //         Vector3Int current = Util.GetGridPositionFromMoveDirection(direction, Position);
    //         double localDistance = Util.EuclidianDistance(current, currentTargetGridPosition);
    //         Debug.Log("Calc " + current + "," + currentTargetGridPosition + " local dist: " + localDistance + " Direction " + direction);

    //         if (localDistance < min)
    //         {
    //             sol = current;
    //             min = localDistance;
    //         }
    //     }

    //     return sol;
    // }

    // private void GotoBug(Vector3Int target)
    // {
    //     // Debug.Log("GotoBug() Setting Goto to " + target);
    //     SetGoTo(target);
    // }

    // private Vector3 Move(MoveDirection direction)
    // {
    //     Vector3 target = Util.GetVectorFromDirection(direction);

    //     return target;

    //     // if (direction == MoveDirection.DOWN)
    //     // {
    //     //     //Vector3 expected = npcObject.transform.position + Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
    //     //     //Vector3 target = Util.GetVectorFromDirection(MoveDirection.DOWNLEFT);
    //     //     //npcController.AddMovement(target);
    //     // }
    // }

    // private Vector3Int NextPathFindingBugPosition()
    // {
    //     //   collider.Distance(); min distance to other colliders
    //     //choose the next position based on the euclidian distance
    //     // and if it is not busy with another npc

    //     Vector3Int result = Position;
    //     double distance = double.MaxValue;

    //     for (int i = 0; i < Util.ArroundVectorPoints.GetLength(0); i++)
    //     {
    //         int x = Util.ArroundVectorPoints[i, 0] + currentTargetGridPosition.x;
    //         int y = Util.ArroundVectorPoints[i, 1] + currentTargetGridPosition.y;
    //         Vector3Int tmp = new Vector3Int(x, y, 0);
    //         double localMin = Util.EuclidianDistance(new int[] { Position.x, Position.y }, new int[] { x, y });

    //         if (BussGrid.IsValidWalkablePosition(tmp) && localMin < distance)
    //         {
    //             distance = localMin;
    //             result = tmp;
    //         }
    //     }
    //     return result;
    // }

    private void UpdateAStarMovement()
    {
        if (IsInTargetPosition())
        {
            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
            else
            {
                //final target reached 
                moveDirection = MoveDirection.IDLE;
                isMoving = false;
            }
        }
        else
        {
            moveDirection = GetDirectionFromPositions(transform.position, currentLocalTargetPosition);
            UpdateObjectDirection(); // It flips the side of the object depending on direction
            transform.position = Vector3.MoveTowards(transform.position, currentLocalTargetPosition, speed * Time.fixedDeltaTime);
        }
    }

    protected void UpdateAnimation()
    {
        // TODO: for performance reasons only animate inside camera CLAMP --> animationController.SetState(NpcState.IDLE);
        // another sol: it can also spam the NPC inside/near the camera 
        // Animates depending on the current state
        if (IsMoving() && prevState == currentState)
        {
            animationController.SetState(NpcState.WALKING);
        }
        else
        {
            animationController.SetState(stateMachine.Current.State);
        }
    }

    protected void UpdateTimeInState()
    {
        currentState = stateMachine.Current.State;
        // keeps the time in the current state
        if (prevState == currentState)
        {
            stateTime += Time.fixedDeltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = currentState;
        }
    }

    private void UpdatePrevPosition()
    {
        //Position changed
        if (PrevGridPosition != Position)
        {
            BussGrid.GameController.RemovePlayerPosition(PrevGridPosition);
            BussGrid.GameController.AddPlayerPositions(Position);
            GoToAStar(currentTargetGridPosition); // recalculate
            PrevGridPosition = Position;
        }
        // DEBUG
        BussGrid.GameController.PrintDebugPlayerPositionsSet();
    }

    private void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 queuePosition = (Vector3)pendingMovementQueue.Dequeue();
        Vector3 direction = BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(new Vector3Int((int)queuePosition.x, (int)queuePosition.y));

        // we can recalculate for each step
        currentLocalTargetPosition = new Vector3(direction.x, direction.y);
    }

    protected bool IsMoving()
    {
        return isMoving;
    }

    protected bool IsAtTarget()
    {
        return Util.IsAtDistanceWithObject(transform.position, currentLocalTargetPosition);
    }

    // Resets the planned Path
    private void ResetMovementQueue()
    {
        currentLocalTargetPosition = Vector3.negativeInfinity;
        pendingMovementQueue = new Queue();
    }

    protected void ResetMovement()
    {
        // we are already at target and not moving
        currentLocalTargetPosition = transform.position;
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
        // if (AStar)
        // {
        return GoToAStar(pos);
        // }
        // else
        // {
        //     GotoBug(pos);
        //     return true;
        // }
        // return false;
    }

    public bool GoToAStar(Vector3Int pos)
    {
        List<Node> path = BussGrid.GetPath(new[] { Position.x, Position.y }, new[] { pos.x, pos.y });

        if (path.Count == 0)
        {
            // Re try not found path after some time 
            Debug.Log("Not path found");
            return false;
        }

        SetGoTo(pos);
        AddPath(path);

        if (pendingMovementQueue.Count != 0)
        {
            AddMovement();
        }

        return true;
    }

    private void SetGoTo(Vector3Int target)
    {
        currentTargetGridPosition = target;
        currentTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPosition(target);
        isMoving = true;
    }

    public void RecalculateGoTo()
    {
        if (!GoTo(currentTargetGridPosition))
        {
            return;
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public bool IsEnergybarActive()
    {
        return energyBar.IsActive();
    }

    private bool IsInTargetPosition()
    {
        return Util.IsAtDistanceWithObject(currentLocalTargetPosition, transform.position);
    }

    public StateMachine<NpcState, NpcStateTransitions> GetStateMachine()
    {
        return stateMachine;
    }
}
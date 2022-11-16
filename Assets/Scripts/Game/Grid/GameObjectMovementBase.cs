
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
    //Energy Bars
    protected LoadSliderController EnergyBar;
    [SerializeField]
    protected float idleTime, stateTime, speed, timeBeforeRemovingDebugPanel = 0.1f;
    private bool isMoving;
    private SortingGroup sortingLayer;
    [SerializeField]
    private NpcState currentState, prevState;
    protected PlayerAnimationStateController animationController;
    protected GameController gameController;
    protected ObjectType type;

    //A*
    // Attributes for temporaly marking the path of the NPC on the grid
    // This will help to void placing objects on top of the NPC
    private Queue pendingMovementQueue;
    private Vector3 currentLocalTargetPosition; //step by step target to
    private int debugColorValue = 0;
    //A*

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
        gameController = gameObject.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();
        sortingLayer = transform.GetComponent<SortingGroup>();
        EnergyBar = transform.Find(Settings.LoadSlider).GetComponent<LoadSliderController>();

        if (!Util.IsNull(EnergyBar, "GameObjectMovementBase/energyBar null"))
        {
            EnergyBar.SetInactive();
        }

        if (animationController == null)
        {
            GameLog.LogWarning("NPCController/animationController-gameObj null");
        }
    }

    private void Start()
    {
        idleTime = 0;
        stateTime = 0;
        prevState = currentState;
        isMoving = false;
        currentState = NpcState.IDLE;
        UpdatePosition();
    }

    protected void SetID()
    {
        string id = BussGrid.GameController.GetNpcSet().Count + 1 + "-" + Time.frameCount;
        transform.name = type.ToString() + "." + id;
        Name = transform.name;
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

    protected void UpdatePosition()
    {
        Position = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
        Position = new Vector3Int(Position.x, Position.y);
        UpdatePrevPosition();
        //Sorting (sprite) the layer depending on the player position
        sortingLayer.sortingOrder = Util.GetSorting(Position);
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

        UpdateAStarMovement();
    }

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
                BussGrid.GameController.RemoveEmployeePlannedTarget(currentTargetGridPosition);
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
            PrevGridPosition = Position;
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

        currentLocalTargetPosition = new Vector3(direction.x, direction.y);
    }

    protected bool IsMoving()
    {
        return isMoving;
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
        if (EnergyBar.IsActive())
        {
            EnergyBar.SetInactive();
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
            //path = MergePath(path); // We merge Paths
            pendingMovementQueue.Clear();
        }

        pendingMovementQueue.Enqueue(path[0].GetVector3());

        for (int i = 1; i < path.Count; i++)
        {
            if (Settings.CellDebug)
            {
                Vector3 from = BussGrid.GetWorldFromPathFindingGridPosition(path[i - 1].GetVector3Int());
                Vector3 to = BussGrid.GetWorldFromPathFindingGridPosition(path[i].GetVector3Int());
                Debug.DrawLine(from, to, Util.GetRandomColor(debugColorValue % 10), 15f);
            }

            pendingMovementQueue.Enqueue(path[i].GetVector3());
        }

        debugColorValue++;

        if (path.Count == 0)
        {
            GameLog.LogWarning("Path out of reach");
            return;
        }

        AddMovement(); // To set the first target
    }

    public bool GoTo(Vector3Int position)
    {
        if (GoToAStar(position))
        {
            SetGoTo(position);
            return true;
        }
        return false;
    }

    public bool GoToAStar(Vector3Int pos)
    {
        List<Node> path = BussGrid.GetPath(new[] { Position.x, Position.y }, new[] { pos.x, pos.y });

        if (path.Count == 0)
        {
            // TODO: Retry not found path after some time add new state
            //GameLog.Log("Not path found");
            return false;
        }

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
        BussGrid.GameController.AddEmployeePlannedTarget(target);
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

    private bool IsInTargetPosition()
    {
        return Util.IsAtDistanceWithObject(currentLocalTargetPosition, transform.position);
    }

    public StateMachine<NpcState, NpcStateTransitions> GetStateMachine()
    {
        return stateMachine;
    }
}
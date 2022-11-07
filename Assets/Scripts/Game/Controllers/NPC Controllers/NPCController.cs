using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    private GameGridObject table;
    private StateMachine stateMachine;
    private bool[] transitionStates;
    private bool tableMoved, waitingAtTable, attended, beingAttended, orderServed;
    [SerializeField]
    private const float MAX_STATE_TIME = 15;

    private void Start()
    {
        type = ObjectType.NPC;
        currentState = NpcState.WANDER;
        // MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine();
        transitionStates = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];
        tableMoved = false;
        waitingAtTable = false;
        attended = false;
        beingAttended = false;
        orderServed = false;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();
        UpdateTimeInState();

        try
        {
            UpdateTransitionStates();
            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
            tableMoved = true; // we unrespawn
        }
    }

    public void UpdateTransitionStates()
    {
        if (IsMoving())
        {
            return;
        }
        else
        {
            CheckIfAtTarget();
        }

        currentState = stateMachine.Current.State;

        transitionStates[0] = CheckIfTableHasBeenAssigned(); // TABLE_AVAILABLE = 0,
        transitionStates[1] = tableMoved; // TABLE_MOVED = 1,
        transitionStates[2] = Unrespawn(); // WALK_TO_UNRESPAWN = 2,
        transitionStates[3] = waitingAtTable; // WAITING_AT_TABLE_TIME = 3,
        transitionStates[4] = false; // UNDEFINED_4 = 4,
        transitionStates[5] = orderServed; // ORDER_SERVED = 5,
        transitionStates[6] = false; // ORDER_FINISHED = 6,
        transitionStates[7] = false; // ENERGY_BAR_VALUE = 7,
        transitionStates[8] = false; // COUNTER_MOVED = 8,
        transitionStates[9] = Wander(); // WANDER = 9,
        transitionStates[10] = false; // UNDEFINED_10 = 10,
        transitionStates[11] = attended; // ATTENDED = 11,
        transitionStates[12] = beingAttended; // BEING_ATTENDED = 12,
        transitionStates[13] = false; // STATE_TIME = 13
        transitionStates[14] = false; // UNDEFINED_14 = 14

        stateMachine.CheckTransition(transitionStates);
        MoveNPC();// Move /or not, depending on the state
    }

    private void MoveNPC()
    {
        if (currentState == NpcState.WALKING_UNRESPAWN && prevState != NpcState.WALKING_UNRESPAWN)
        {
            GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        }
        else if (currentState == NpcState.WANDER)
        {
            GoTo(BussGrid.GetRandomWalkablePosition(Position));
        }
        else if (currentState == NpcState.WALKING_TO_TABLE)
        {
            if (table == null) { return; }
            GoTo(table.GetActionTileInGridPosition());
        }
    }

    private bool Unrespawn()
    {
        if ((!tableMoved && stateTime < MAX_STATE_TIME && !attended) || currentState == NpcState.WALKING_UNRESPAWN)
        {
            return false;
        }
        // it is required since it most match all operator to pass to the next stage
        tableMoved = true;
        // we clean the table pointer if assigned
        if (table != null)
        {
            table.FreeObject();
            table = null;
        }
        return true;
    }

    private bool Wander()
    {
        float randT = Random.Range(0, 1000);//TODO
        if (currentState != NpcState.IDLE || randT > 2)
        {
            return false;
        }
        return true;
    }

    private bool CheckIfTableHasBeenAssigned()
    {
        if (currentState != NpcState.IDLE)
        {
            return false;
        }
        return table != null;
    }

    private void CheckIfAtTarget()
    {
        if (!(currentTargetGridPosition.x == Position.x && currentTargetGridPosition.y == Position.y))
        {
            return;
        }

        if (currentState == NpcState.WALKING_UNRESPAWN)
        {
            gameController.RemoveNpc(this);
            Destroy(gameObject);
        }

        if (currentState == NpcState.WALKING_TO_TABLE)
        {
            waitingAtTable = true;
        }
    }

    private void UpdateTimeInState()
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

    public NpcState GetNpcState()
    {
        return currentState;
    }

    public float GetNpcStateTime()
    {
        return Mathf.Floor(stateTime);
    }

    public GameGridObject GetTable()
    {
        return table;
    }

    public void SetTable(GameGridObject obj)
    {
        table = obj;
    }

    public void FlipTowards(Vector3Int direction)
    {
        StandTowards(direction);
    }

    public bool HasTable()
    {
        return table != null;
    }

    public void SetTableMoved()
    {
        tableMoved = true;
    }

    public void SetAttended()
    {
        attended = true;
        orderServed = true;
    }

    public void SetBeingAttended()
    {
        beingAttended = true;
    }
}
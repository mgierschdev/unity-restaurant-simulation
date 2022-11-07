using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    [SerializeField]
    private const float MAX_STATE_TIME = 15;

    private void Start()
    {
        type = ObjectType.NPC;
        // MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine();
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
            stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        }
    }

    public void UpdateTransitionStates()
    {
        Debug.Log("Name " + Name +" "+stateMachine.ToString());
        
        if (IsMoving())
        {
            return;
        }
        else
        {
            CheckIfAtTarget();
        }

        currentState = stateMachine.Current.State;

        CheckUnrespawn();
        CheckIfTableHasBeenAssigned();
        Wander();

        stateMachine.CheckTransition();
        MoveNPC();
    }

    private void MoveNPC()
    {
        if (currentState == NpcState.WALKING_UNRESPAWN)
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

    private void CheckUnrespawn()
    {
        if (!stateMachine.GetTransitionState(NpcStateTransitions.WALK_TO_UNRESPAWN))
        {
            Debug.Log("Checking CheckUnrespawn " + stateTime + " " + stateMachine.GetTransitionState(NpcStateTransitions.TABLE_MOVED) + " " + stateMachine.GetTransitionState(NpcStateTransitions.ATTENDED));
            if (stateTime >= MAX_STATE_TIME ||
            stateMachine.GetTransitionState(NpcStateTransitions.TABLE_MOVED) ||
            stateMachine.GetTransitionState(NpcStateTransitions.ATTENDED))
            {
                stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
                stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
            }
        }
    }

    private void Wander()
    {
        float randT = Random.Range(0, 1000);//TODO
        if (currentState != NpcState.IDLE || randT > 2)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.WANDER);
            return;
        }
        stateMachine.SetTransition(NpcStateTransitions.WANDER);
    }

    private void CheckIfTableHasBeenAssigned()
    {
        if (currentState != NpcState.IDLE)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.TABLE_AVAILABLE);
            return;
        }
        if (table != null)
        {
            stateMachine.SetTransition(NpcStateTransitions.TABLE_AVAILABLE);
        }
    }

    private void CheckIfAtTarget()
    {
        if (!(currentTargetGridPosition.x == Position.x && currentTargetGridPosition.y == Position.y))
        {
            return;
        }
        else if (currentState == NpcState.WALKING_UNRESPAWN)
        {
            gameController.RemoveNpc(this);
            if (table != null)
            {
                table.FreeObject();
                table = null;
            }
            Destroy(gameObject);
        }
        else if (currentState == NpcState.WALKING_TO_TABLE)
        {
            stateMachine.SetTransition(NpcStateTransitions.WAITING_AT_TABLE);
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
        stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
    }

    public void SetAttended()
    {
        stateMachine.SetTransition(NpcStateTransitions.ATTENDED);
        stateMachine.SetTransition(NpcStateTransitions.ORDER_SERVED);
    }

    public void SetBeingAttended()
    {
        stateMachine.SetTransition(NpcStateTransitions.BEING_ATTENDED);
    }
}
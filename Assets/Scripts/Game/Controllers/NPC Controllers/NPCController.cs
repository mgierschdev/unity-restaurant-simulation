using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    [SerializeField]
    private const float MaxStateTime = 120; // 2min
    [SerializeField]
    private NpcState state;//TODO: for debug

    private void Start()
    {
        type = ObjectType.NPC;
        SetID();
        //TODO: MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine(Name);
        StartCoroutine(UpdateTransitionStates());
    }

    private void FixedUpdate()
    {
        try
        {
            UpdatePosition();
            UpdateTimeInState();
            UpdateTargetMovement();
            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
            stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        }
    }

    public IEnumerator UpdateTransitionStates()
    {
        for (; ; )
        {
            if (!IsMoving())
            {
                CheckIfAtTarget();
                CheckUnrespawn();
                CheckIfTableHasBeenAssigned();
                Wander();
                CheckIfTableMoved();
                state = stateMachine.Current.State;
                stateMachine.CheckTransition();
                MoveNPC();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void MoveNPC()
    {
        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN && !stateMachine.GetTransitionState(NpcStateTransitions.MOVING_TO_UNSRESPAWN))
        {
            stateMachine.SetTransition(NpcStateTransitions.MOVING_TO_UNSRESPAWN);
            GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        }
        else if (stateMachine.Current.State == NpcState.WANDER)
        {
            GoTo(BussGrid.GetRandomWalkablePosition(Position));
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            if (table == null) { return; }
            GoTo(table.GetActionTileInGridPosition());
        }
    }

    private void CheckIfTableMoved()
    {
        if (stateMachine.Current.State == NpcState.WAITING_TO_BE_ATTENDED && table == null)
        {
            stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        }
    }

    private void CheckUnrespawn()
    {
        if (!stateMachine.GetTransitionState(NpcStateTransitions.WALK_TO_UNRESPAWN))
        {
            if (stateTime >= MaxStateTime ||
                stateMachine.GetTransitionState(NpcStateTransitions.TABLE_MOVED) ||
                stateMachine.GetTransitionState(NpcStateTransitions.ATTENDED))
            {
                stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
                stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
            }
        }
    }

    private void CheckWanderToIdle()
    {

    }

    private void Wander()
    {
        // Chance to no wander
        float randT = Random.Range(0, 8);

        if (stateMachine.Current.State != NpcState.IDLE || randT > 2)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.WANDER);

            if (stateMachine.Current.State == NpcState.WANDER)
            {
                stateMachine.SetTransition(NpcStateTransitions.WANDER_TO_IDLE);
            }
        }
        else
        {
            stateMachine.UnSetTransition(NpcStateTransitions.WANDER_TO_IDLE);
            stateMachine.SetTransition(NpcStateTransitions.WANDER);
        }
    }

    private void CheckIfTableHasBeenAssigned()
    {
        if (table != null)
        {
            stateMachine.SetTransition(NpcStateTransitions.TABLE_AVAILABLE);
        }
        else
        {
            stateMachine.UnSetTransition(NpcStateTransitions.TABLE_AVAILABLE);
        }
    }

    private void CheckIfAtTarget()
    {
        if (!(currentTargetGridPosition.x == Position.x && currentTargetGridPosition.y == Position.y))
        {
            return;
        }

        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN)
        {
            BussGrid.GameController.RemoveNpc(this);

            if (table != null)
            {
                table.FreeObject();
                table = null;
            }
            Destroy(gameObject);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            stateMachine.SetTransition(NpcStateTransitions.WAITING_AT_TABLE);
        }
    }

    public NpcState GetNpcState()
    {
        return stateMachine.Current.State;
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
        table = null;
        stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
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
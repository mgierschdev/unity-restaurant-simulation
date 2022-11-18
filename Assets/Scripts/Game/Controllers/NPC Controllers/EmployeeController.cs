using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
using Random = UnityEngine.Random;

public class EmployeeController : GameObjectMovementBase
{
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float MAX_TIME_IN_STATE = Settings.NPCMaxTimeInState,
    MAX_TABLE_WAITING_TIME = Settings.NPCMaxWaitingTime,
    TIME_IDLE_BEFORE_TAKING_ORDER = 2f,
    SPEED_TIME_TO_REGISTER_IN_CASH = 150f,
    SPEED_TIME_TAKING_ORDER = 1.5f;
    private GameGridObject counter;
    private bool counterAssigned;

    private void Start()
    {
        type = ObjectType.EMPLOYEE;
        counterAssigned = false;
        SetID();
        stateMachine = NPCStateMachineFactory.GetEmployeeStateMachine(Name);
        StartCoroutine(UpdateTransitionStates());
    }

    private void FixedUpdate()
    {
        if (!counterAssigned)
        {
            return;
        }

        try
        {
            UpdatePosition();
            UpdateTimeInState();
            UpdateTargetMovement();
            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate EmployeeController): " + e);
        }
    }

    public IEnumerator UpdateTransitionStates()
    {
        for (; ; )
        {
            if (IsMoving() || EnergyBar.IsActive())
            {

            }
            else
            {
                CheckIfAtTarget();
                TableWithCustomer();
                Unrespawn();
                CheckCounter();
                CheckAtCounter();
                CheckTableMoved();
                stateMachine.CheckTransition();
                MoveNPC();// Move/or not, depending on the state
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // if Idle and table moved we unset it since it is not longer attending the table
    private void CheckTableMoved()
    {
        if (stateMachine.Current.State == NpcState.IDLE && stateMachine.GetTransitionState(NpcStateTransitions.TABLE_MOVED))
        {
            stateMachine.UnSetTransition(NpcStateTransitions.TABLE_MOVED);
        }
    }

    private void CheckAtCounter()
    {
        // we check if at counter we set the bit
        if (counter != null && Position.x == counter.GetActionTileInGridPosition().x && Position.y == counter.GetActionTileInGridPosition().y)
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER);
        }
    }

    private void CheckCounter()
    {
        if (counter == null)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.COUNTER_AVAILABLE);
        }
        else
        {
            stateMachine.SetTransition(NpcStateTransitions.COUNTER_AVAILABLE);
        }
    }

    private void TableWithCustomer()
    {
        if (stateMachine.Current.State != NpcState.AT_COUNTER)
        {
            return;
        }

        if (BussGrid.GetTableWithClient(out table))
        {
            table.SetAttendedBy(this);
            stateMachine.SetTransition(NpcStateTransitions.TABLE_AVAILABLE);
            return;
        }
        stateMachine.UnSetTransition(NpcStateTransitions.TABLE_AVAILABLE);
    }

    private void Unrespawn()
    {
        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN || counter != null)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
            return;
        }

        // we clean the table pointer if assigned
        if (table != null)
        {
            table.FreeObject();
            table = null;
        }
        stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
    }

    private void CheckIfAtTarget()
    {
        if (!(currentTargetGridPosition.x == Position.x && currentTargetGridPosition.y == Position.y))
        {
            return;
        }

        if (stateMachine.Current.State == NpcState.WALKING_TO_COUNTER && !stateMachine.GetTransitionState(NpcStateTransitions.AT_COUNTER))
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER);
            stateMachine.UnSetTransition(NpcStateTransitions.CASH_REGISTERED);
            StandTowards(counter.GridPosition);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_TABLE);
            stateMachine.UnSetTransition(NpcStateTransitions.AT_COUNTER);
            if (table == null) { return; }
            NPCController controller = table.GetUsedBy();
            if (controller == null) { return; }
            StandTowards(table.GetUsedBy().Position);//We flip the Employee -> CLient
            table.GetUsedBy().FlipTowards(Position); // We flip client -> employee
        }
        else if (stateMachine.Current.State == NpcState.TAKING_ORDER && EnergyBar.IsFinished() && !stateMachine.GetTransitionState(NpcStateTransitions.ORDER_SERVED))
        {
            stateMachine.SetTransition(NpcStateTransitions.ORDER_SERVED);
            if (table == null) { return; }
            NPCController controller = table.GetUsedBy();
            if (controller == null) { return; }
            controller.SetAttended();
        }

        else if (stateMachine.Current.State == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER);
            stateMachine.SetTransition(NpcStateTransitions.REGISTERING_CASH);
            stateMachine.UnSetTransition(NpcStateTransitions.AT_TABLE);
        }
        else if (stateMachine.Current.State == NpcState.REGISTERING_CASH && EnergyBar.IsFinished())
        {
            stateMachine.SetTransition(NpcStateTransitions.CASH_REGISTERED);
            // EnergyBar.ResetCurrentEnergy();
            //TODO: register cost depending on the NPC order
            double orderCost = Random.Range(5, 10);
            PlayerData.AddMoney(orderCost);
        }
        else if (stateMachine.Current.State == NpcState.AT_COUNTER_FINAL)
        {
            stateMachine.UnSetAll();
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER_FINAL);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN)
        {
            BussGrid.GameController.RemoveEmployee();
            Destroy(gameObject);
        }
    }

    private void MoveNPC()
    {
        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN && !stateMachine.GetTransitionState(NpcStateTransitions.MOVING_TO_UNSRESPAWN))
        {
            stateMachine.SetTransition(NpcStateTransitions.MOVING_TO_UNSRESPAWN);
            GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_COUNTER)
        {
            if (counter == null) { return; }
            GoTo(counter.GetActionTileInGridPosition());
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            if (table == null) { return; }
            //TODO: improve, method to standup on any non-busy cell
            GoTo(BussGrid.GetClosestPathGridPoint(Position, table.GetActionTileInGridPosition()));
        }
        else if (stateMachine.Current.State == NpcState.TAKING_ORDER && !stateMachine.GetTransitionState(NpcStateTransitions.ORDER_SERVED))
        {
            EnergyBar.SetActive(SPEED_TIME_TAKING_ORDER);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
        {
            if (counter == null) { return; }
            GoTo(counter.GetActionTileInGridPosition());
        }
        else if (stateMachine.Current.State == NpcState.REGISTERING_CASH && !stateMachine.GetTransitionState(NpcStateTransitions.CASH_REGISTERED))
        {
            EnergyBar.SetActive(SPEED_TIME_TAKING_ORDER);
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

    public void SetTableToBeAttended(GameGridObject table)
    {
        this.table = table;
    }

    public Vector3Int GetCoordOfTableToBeAttended()
    {
        return table.GridPosition;
    }

    public void SetUnrespawn()
    {
        stateMachine.SetTransition(NpcStateTransitions.MOVING_TO_UNSRESPAWN);
    }

    public void SetTableMoved()
    {
        stateMachine.UnSetAll();
        stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
    }

    public void SetCounter(GameGridObject counter)
    {
        counterAssigned = true;
        this.counter = counter;
    }
}
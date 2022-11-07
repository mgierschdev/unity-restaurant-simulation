using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmployeeController : GameObjectMovementBase
{
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float MAX_TIME_IN_STATE = Settings.NPCMaxTimeInState,
    MAX_TABLE_WAITING_TIME = Settings.NPCMaxWaitingTime,
    TIME_IDLE_BEFORE_TAKING_ORDER = 2f,
    SPEED_TIME_TO_REGISTER_IN_CASH = 150f,
    SPEED_TIME_TO_TAKING_ORDER = 80f;
    private GameGridObject counter;

    private void Start()
    {
        type = ObjectType.EMPLOYEE;
        counter = BussGrid.GetCounter();
        stateMachine = NPCStateMachineFactory.GetEmployeeStateMachine();
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
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate EmployeeController): " + e);
        }
    }

    public void UpdateTransitionStates()
    {
        if (IsMoving() && IsEnergybarActive())
        {
            return;
        }
        else
        {
            CheckIfAtTarget();
        }

        currentState = stateMachine.Current.State;
        TableWithCustomer();
        Unrespawn();
        CheckCounter();
        stateMachine.CheckTransition();
        MoveNPC();// Move /or not, depending on the state
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
        if (currentState != NpcState.AT_COUNTER)
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
        if (currentState == NpcState.WALKING_UNRESPAWN || BussGrid.GetCounter() != null)
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

        if (currentState == NpcState.WALKING_TO_COUNTER && !stateMachine.GetTransitionState(NpcStateTransitions.AT_COUNTER))
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER);
            StandTowards(BussGrid.GetCounter().GridPosition);
        }
        else if (currentState == NpcState.WALKING_TO_TABLE)
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_TABLE);
            stateMachine.UnSetTransition(NpcStateTransitions.AT_COUNTER);
        }
        else if (currentState == NpcState.TAKING_ORDER && currentEnergy >= 100 && !stateMachine.GetTransitionState(NpcStateTransitions.ORDER_SERVED))
        {
            stateMachine.SetTransition(NpcStateTransitions.ORDER_SERVED);
            table.GetUsedBy().SetAttended();
        }
        else if (currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
        {
            stateMachine.SetTransition(NpcStateTransitions.AT_COUNTER);
            stateMachine.SetTransition(NpcStateTransitions.REGISTERING_CASH);
            stateMachine.UnSetTransition(NpcStateTransitions.AT_TABLE);
        }
        else if (currentState == NpcState.REGISTERING_CASH && currentEnergy >= 100)
        {
            stateMachine.SetTransition(NpcStateTransitions.CASH_REGISTERED);
            stateMachine.UnSetTransition(NpcStateTransitions.ORDER_SERVED);
            //TODO: register cost depending on the NPC order
            double orderCost = Random.Range(5, 10);
            PlayerData.AddMoney(orderCost);
        }
    }

    private void MoveNPC()
    {
        if (currentState == NpcState.WALKING_UNRESPAWN && !stateMachine.GetTransitionState(NpcStateTransitions.MOVING_TO_UNSRESPAWN))
        {
            stateMachine.SetTransition(NpcStateTransitions.MOVING_TO_UNSRESPAWN);
            GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        }
        else if (currentState == NpcState.WALKING_TO_COUNTER)
        {
            if (counter == null) { return; }
            GoTo(counter.GetActionTileInGridPosition());
        }
        else if (currentState == NpcState.WALKING_TO_TABLE)
        {
            if (table == null) { return; }
            //TODO: improve, method to standup on any non-busy cell
            GoTo(BussGrid.GetClosestPathGridPoint(Position, table.GetActionTileInGridPosition()));
        }
        else if (currentState == NpcState.TAKING_ORDER && !stateMachine.GetTransitionState(NpcStateTransitions.ORDER_SERVED))
        {
            ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
            StandTowards(table.GetUsedBy().Position);//We flip the Employee -> CLient
            table.GetUsedBy().FlipTowards(Position); // We flip client -> employee
        }
        else if (currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
        {
            if (counter == null) { return; }
            GoTo(counter.GetActionTileInGridPosition());
        }
        else if (currentState == NpcState.REGISTERING_CASH && !stateMachine.GetTransitionState(NpcStateTransitions.CASH_REGISTERED))
        {
            ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
        }
    }


    // This updates checks in case the table is not longer available or any other state in which
    // the npc has to restart
    // private void UpdateRestartStates()
    // {
    //     if (Util.CompareNegativeInifinity(coordOfTableToBeAttended))
    //     {
    //         table = null;
    //     }

    //     if (BussGrid.GetCounter() != null && BussGrid.GetCounter().GetIsObjectSelected() && currentState != NpcState.WALKING_UNRESPAWN)
    //     {
    //         UpdateGoToUnrespawn_1();
    //     }
    //     else if (BussGrid.GetCounter() == null && currentState != NpcState.WALKING_UNRESPAWN)
    //     {
    //         UpdateGoToUnrespawn_1();
    //     }
    //     else if ((currentState == NpcState.WALKING_TO_COUNTER ||
    //         currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER ||
    //         currentState == NpcState.AT_COUNTER ||
    //         currentState == NpcState.REGISTERING_CASH ||
    //         currentState == NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH) &&
    //         BussGrid.GetCounter() == null)
    //     {
    //         UpdateGoToUnrespawn_1();
    //     }
    //     else if ((currentState == NpcState.WALKING_TO_TABLE ||
    //         currentState == NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER ||
    //         currentState == NpcState.TAKING_ORDER) &&
    //         (table == null || (table != null && !table.GetBusy())))
    //     {
    //         table = null;
    //         currentState = NpcState.WALKING_TO_COUNTER;
    //         GoToCounter();
    //     }
    // }

    // private void UpdateTimeInState()
    // {
    //     // keeps the time in the current state
    //     if (prevState == currentState)
    //     {
    //         stateTime += Time.fixedDeltaTime;
    //     }
    //     else
    //     {
    //         stateTime = 0;
    //         prevState = currentState;
    //     }

    //     idleTime += Time.fixedDeltaTime;

    //     if (stateTime > MAX_TIME_IN_STATE)
    //     {
    //         // if we are already at the counter and the time passed the max time 
    //         // there is no cufstomers we stay at the counter, no need to RecalculateState()
    //         if (currentState == NpcState.AT_COUNTER || BussGrid.GetCounter() == null)
    //         {
    //             return;
    //         }

    //         ResetMovement();
    //         RecalculateState(tableToBeAttended);
    //     }
    // }
    //First State
    // private void UpdateGoToUnrespawn_1()
    // {
    //     currentState = NpcState.WALKING_UNRESPAWN;
    //     unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
    //     target = unRespawnTile.GridPosition;
    //     if (!GoTo(target))
    //     {
    //         GameLog.LogWarning("Retrying: We could not find a path - UpdateGoToUnrespawn_0()");
    //         currentState = NpcState.IDLE;
    //     }
    // }

    // private void UpdateIsAtUnrespawn_2()
    // {
    //     if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
    //     {
    //         gameController.RemoveEmployee();
    //         Destroy(gameObject);
    //     }
    // }
    // private void UpdateGoNextToCounter_3()
    // {
    //     currentState = NpcState.WALKING_TO_COUNTER;
    //     target = BussGrid.GetCounter().GetActionTileInGridPosition();

    //     if (!GoTo(target))
    //     {
    //         //Go to counter if it is already at the counter we change the state
    //         if (IsAtTargetPosition(target))
    //         {
    //             currentState = NpcState.AT_COUNTER;
    //         }
    //         else
    //         {
    //             GameLog.LogWarning("Retrying: We could not find a path - UpdateGoNextToCounter_1() ");
    //             currentState = NpcState.IDLE;
    //         }
    //     }
    // }

    // private bool IsAtTargetPosition(Vector3 target)
    // {
    //     if (Vector3.Distance(Position, target) < Settings.MinDistanceToTarget)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // private void UpdateIsAtCounter_4()
    // {
    //     if (!Util.IsAtDistanceWithObject(transform.position, BussGrid.GetCounter().GetActionTile()))
    //     {
    //         return;
    //     }
    //     SetStateAtCounter(); //localState = NpcState.AT_COUNTER;
    //     idleTime = 0;
    // }

    // private void SetStateAtCounter()
    // {

    //     currentState = NpcState.AT_COUNTER;
    // }

    // private void UpdateAttendTable_5()
    // {
    //     // we idle and not attend the table. "Waiting..."
    //     float idleProbability = Random.Range(0, 100);
    //     if (idleProbability < RANDOM_PROBABILITY_TO_WAIT)
    //     {
    //         idleTime = 0;
    //         return;
    //     }
    //     currentState = NpcState.WALKING_TO_TABLE;
    //     GoToTableToBeAttended();
    // }

    // private void UpdateIsTakingOrder_6()
    // {
    //     if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(table.GridPosition), transform))
    //     {
    //         return;
    //     }

    //     currentState = NpcState.TAKING_ORDER;

    //     //the NPC left and the pointer to the table is null, we go to the counter
    //     if (table == null || table.GetUsedBy() == null)
    //     {
    //         if (!RestartState())
    //         {
    //             currentState = NpcState.IDLE;
    //         }
    //         return;
    //     }

    //     StandTowards(table.GetUsedBy().Position);//We flip the Employee -> CLient
    //     table.GetUsedBy().FlipTowards(Position); // We flip client -> employee
    //     table.GetUsedBy().SetBeingAttended();
    // }

    // private void UpdateTakeOrder_7()
    // {
    //     currentState = NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER;
    //     ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
    // }

    // The client was attended we return the free table and Add money to the wallet
    // The client leaves the table once the table is set as free
    // private void UpdateOrderAttended_8()
    // {
    //     currentState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
    //     table.GetUsedBy().SetAttended();
    //     table.SetUsedBy(null);
    //     table = null;

    //     if (!GoToCounter())
    //     {
    //         GameLog.LogWarning("Retryng: could not go to counter UpdateOrderAttended_6()");
    //         if (IsAtTargetPosition(target))
    //         {
    //             SetStateAtCounter();
    //         }
    //     }
    // }

    // private void UpdateIsAtCounterAfterOrder_9()
    // {
    //     if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetCounter().GetActionTile(), transform))
    //     {
    //         return;
    //     }
    //     currentState = NpcState.REGISTERING_CASH;
    //     StandTowards(BussGrid.GetCounter().GridPosition);
    // }

    // private void UpdateRegisterCash_10()
    // {
    //     currentState = NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH;
    //     ActivateEnergyBar(SPEED_TIME_TO_REGISTER_IN_CASH);
    // }

    // private void UpdateFinishRegistering_11()
    // {
    //     SetStateAtCounter();
    //     double orderCost = Random.Range(5, 10);
    //     //TODO: cost depending on the NPC order
    //     PlayerData.AddMoney(orderCost);
    // }

    public void RecalculateState(GameGridObject obj)
    {

        if (obj == table)
        {
            if (!RestartState())
            {
                currentState = NpcState.IDLE;
            }
        }
        else if (currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER || currentState == NpcState.WALKING_TO_COUNTER)
        {
            // if (!GoToCounter())
            // {
            //     if (IsAtTargetPosition(currentTargetWorldPosition))
            //     {
            //         SetStateAtCounter();
            //     }
            //     else
            //     {
            //         GameLog.LogWarning("Could not go to the counter. RecalculateState()");
            //     }
            // }
        }
    }

    // private bool GoToCounter()
    // {
    //     if (BussGrid.GetCounter() == null)
    //     {
    //         return false;
    //     }

    //     //target = BussGrid.GetPathFindingGridFromWorldPosition(BussGrid.GetCounter().GetActionTile());
    //     //return GoTo(target);
    // }

    // private void GoToTableToBeAttended()
    // {
    //     if (table == null || table.GetIsObjectBeingDragged())
    //     {
    //         if (BussGrid.GetTableWithClient(out table))
    //         {
    //             table.SetAttendedBy(this);
    //         }
    //         else
    //         {
    //             // If there is not tables with client
    //             currentState = NpcState.IDLE;
    //             return;
    //         }
    //     }

    //     Vector3Int localTarget = BussGrid.GetPathFindingGridFromWorldPosition(table.GetActionTile());
    //     Vector3Int coordOfTableToBeAttended = BussGrid.GetClosestPathGridPoint(Position, localTarget);

    //     // Meaning we did not find a correct spot to standup, we return
    //     // and enqueue de table to the list again 
    //     // if (localTarget == CoordOfTableToBeAttended)
    //     // {
    //     //     GameLog.Log("TODO: Popup We could not find a proper place to standup - GoToTableToBeAttended()");
    //     //     return;
    //     // }

    //     // target = coordOfTableToBeAttended;

    //     // // we can attend the table from the position we are currently at the moment     
    //     // // In case the table is placed next to the counter 
    //     // if (isAlreadyAtTarget(localTarget))
    //     // {
    //     //     target = Position;
    //     //     coordOfTableToBeAttended = Position;
    //     // }

    //     // if (!GoTo(target))
    //     // {
    //     //     return;
    //     // }
    // }

    public bool RestartState()
    {
        // ResetMovement();
        // target = BussGrid.GetCounter().GetActionTileInGridPosition();

        // if (!GoTo(target))
        // {
        //     if (IsAtTargetPosition(target))
        //     {
        //         currentState = NpcState.AT_COUNTER;
        //         return true;
        //     }
        //     else
        //     {
        //         // ("Retrying: We could not find a path - RestartState()");
        //     }
        //     return false;
        // }

        // currentState = NpcState.WALKING_TO_COUNTER;
        // return true;
        return false;
    }

    public NpcState GetNpcState()
    {
        return currentState;
    }

    public float GetNpcStateTime()
    {
        return Mathf.Floor(stateTime);
    }

    public void SetTableToBeAttended(GameGridObject table)
    {
        this.table = table;
    }

    //In case the table is placed next to the counter there is no need to calculate the path
    public bool isAlreadyAtTarget(Vector3Int target)
    {
        for (int i = 0; i < Util.ArroundVectorPoints.GetLength(0); i++)
        {
            Vector3Int current = new Vector3Int(Position.x + Util.ArroundVectorPoints[i, 0], Position.y + Util.ArroundVectorPoints[i, 1]);

            if (current == target)
            {
                return true;
            }
        }
        return false;
    }

    public Vector3Int GetCoordOfTableToBeAttended()
    {
        return table.GridPosition;
    }
}
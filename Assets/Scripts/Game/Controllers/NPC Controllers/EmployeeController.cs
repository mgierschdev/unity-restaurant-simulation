using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmployeeController : GameObjectMovementBase
{
    // This attributes stay here, since there will be different employees with different attributes
    private const float SPEED_TIME_TO_TAKING_ORDER = 80f; //Decrease per second  100/15
    private const float SPEED_TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10
    private const float TIME_IDLE_BEFORE_TAKING_ORDER = 2f;
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float MAX_TIME_IN_STATE = Settings.NPCMaxTimeInState;
    private const float MAX_TABLE_WAITING_TIME = Settings.NPCMaxWaitingTime;
    private GameGridObject tableToBeAttended;
    private GameTile unRespawnTile;
    private Vector3Int target, coordOfTableToBeAttended;

    private void Start()
    {
        type = ObjectType.EMPLOYEE;
        currentState = NpcState.IDLE;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();
        UpdateTimeInState();

        try
        {
            UpdateRestartStates();

            switch (currentState)
            {
                case NpcState.WALKING_UNRESPAWN: UpdateIsAtUnrespawn_2(); break;
                case NpcState.IDLE: UpdateGoNextToCounter_3(); break;
                case NpcState.WALKING_TO_COUNTER: UpdateIsAtCounter_4(); break;
                case NpcState.AT_COUNTER when idleTime > TIME_IDLE_BEFORE_TAKING_ORDER: UpdateAttendTable_5(); break;
                case NpcState.WALKING_TO_TABLE: UpdateIsTakingOrder_6(); break;
                case NpcState.TAKING_ORDER: UpdateTakeOrder_7(); break;
                case NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER when currentEnergy >= 100: UpdateOrderAttended_8(); break;
                case NpcState.WALKING_TO_COUNTER_AFTER_ORDER: UpdateIsAtCounterAfterOrder_9(); break;
                case NpcState.REGISTERING_CASH: UpdateRegisterCash_10(); break;
                case NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH when currentEnergy >= 100: UpdateFinishRegistering_11(); break;
            }

            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate EmployeeController): " + e);
        }        
    }

    // This updates checks in case the table is not longer available or any other state in which
    // the npc has to restart
    private void UpdateRestartStates()
    {
        if (Util.CompareNegativeInifinity(coordOfTableToBeAttended))
        {
            tableToBeAttended = null;
        }

        if (BussGrid.GetCounter() != null && BussGrid.GetCounter().GetIsObjectSelected() && currentState != NpcState.WALKING_UNRESPAWN)
        {
            UpdateGoToUnrespawn_1();
        }
        else if (BussGrid.GetCounter() == null && currentState != NpcState.WALKING_UNRESPAWN)
        {
            UpdateGoToUnrespawn_1();
        }
        else if ((currentState == NpcState.WALKING_TO_COUNTER ||
            currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER ||
            currentState == NpcState.AT_COUNTER ||
            currentState == NpcState.REGISTERING_CASH ||
            currentState == NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH) &&
            BussGrid.GetCounter() == null)
        {
            UpdateGoToUnrespawn_1();
        }
        else if ((currentState == NpcState.WALKING_TO_TABLE ||
            currentState == NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER ||
            currentState == NpcState.TAKING_ORDER) &&
            (tableToBeAttended == null || (tableToBeAttended != null && !tableToBeAttended.GetBusy())))
        {
            tableToBeAttended = null;
            currentState = NpcState.WALKING_TO_COUNTER;
            GoToCounter();
        }
    }

    private void UpdateTimeInState()
    {
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

        idleTime += Time.fixedDeltaTime;

        if (stateTime > MAX_TIME_IN_STATE)
        {
            // if we are already at the counter and the time passed the max time 
            // there is no cufstomers we stay at the counter, no need to RecalculateState()
            if (currentState == NpcState.AT_COUNTER || BussGrid.GetCounter() == null)
            {
                return;
            }

            ResetMovement();
            RecalculateState(tableToBeAttended);
        }
    }
    //First State
    private void UpdateGoToUnrespawn_1()
    {
        currentState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            GameLog.LogWarning("Retrying: We could not find a path - UpdateGoToUnrespawn_0()");
            currentState = NpcState.IDLE;
        }
    }

    private void UpdateIsAtUnrespawn_2()
    {
        if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveEmployee();
            Destroy(gameObject);
        }
    }
    private void UpdateGoNextToCounter_3()
    {
        currentState = NpcState.WALKING_TO_COUNTER;
        target = BussGrid.GetCounter().GetActionTileInGridPosition();

        if (!GoTo(target))
        {
            //Go to counter if it is already at the counter we change the state
            if (IsAtTargetPosition(target))
            {
                currentState = NpcState.AT_COUNTER;
            }
            else
            {
                GameLog.LogWarning("Retrying: We could not find a path - UpdateGoNextToCounter_1() ");
                currentState = NpcState.IDLE;
            }
        }
    }

    private bool IsAtTargetPosition(Vector3 target)
    {
        if (Vector3.Distance(Position, target) < Settings.MinDistanceToTarget)
        {
            return true;
        }
        return false;
    }

    private void UpdateIsAtCounter_4()
    {
        if (!Util.IsAtDistanceWithObject(transform.position, BussGrid.GetCounter().GetActionTile()))
        {
            return;
        }
        SetStateAtCounter(); //localState = NpcState.AT_COUNTER;
        idleTime = 0;
    }

    private void SetStateAtCounter()
    {
        StandTowards(BussGrid.GetCounter().GridPosition);
        currentState = NpcState.AT_COUNTER;
    }

    private void UpdateAttendTable_5()
    {
        // we idle and not attend the table. "Waiting..."
        float idleProbability = Random.Range(0, 100);
        if (idleProbability < RANDOM_PROBABILITY_TO_WAIT)
        {
            idleTime = 0;
            return;
        }
        currentState = NpcState.WALKING_TO_TABLE;
        GoToTableToBeAttended();
    }

    private void UpdateIsTakingOrder_6()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(coordOfTableToBeAttended), transform))
        {
            return;
        }

        currentState = NpcState.TAKING_ORDER;

        //the NPC left and the pointer to the table is null, we go to the counter
        if (tableToBeAttended == null || tableToBeAttended.GetUsedBy() == null)
        {
            if (!RestartState())
            {
                currentState = NpcState.IDLE;
            }
            return;
        }

        StandTowards(tableToBeAttended.GetUsedBy().Position);//We flip the Employee -> CLient
        tableToBeAttended.GetUsedBy().FlipTowards(Position); // We flip client -> employee
        tableToBeAttended.GetUsedBy().SetBeingAttended();
    }

    private void UpdateTakeOrder_7()
    {
        currentState = NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER;
        ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
    }

    // The client was attended we return the free table and Add money to the wallet
    // The client leaves the table once the table is set as free
    private void UpdateOrderAttended_8()
    {
        currentState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
        tableToBeAttended.GetUsedBy().SetAttended();
        tableToBeAttended.SetUsedBy(null);
        tableToBeAttended = null;

        if (!GoToCounter())
        {
            GameLog.LogWarning("Retryng: could not go to counter UpdateOrderAttended_6()");
            if (IsAtTargetPosition(target))
            {
                SetStateAtCounter();
            }
        }
    }

    private void UpdateIsAtCounterAfterOrder_9()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetCounter().GetActionTile(), transform))
        {
            return;
        }
        currentState = NpcState.REGISTERING_CASH;
        StandTowards(BussGrid.GetCounter().GridPosition);
    }

    private void UpdateRegisterCash_10()
    {
        currentState = NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH;
        ActivateEnergyBar(SPEED_TIME_TO_REGISTER_IN_CASH);
    }

    private void UpdateFinishRegistering_11()
    {
        SetStateAtCounter();
        double orderCost = Random.Range(5, 10);
        //TODO: cost depending on the NPC order
        PlayerData.AddMoney(orderCost);
    }

    public void RecalculateState(GameGridObject obj)
    {

        if (obj == tableToBeAttended)
        {
            if (!RestartState())
            {
                currentState = NpcState.IDLE;
            }
        }
        else if (currentState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER || currentState == NpcState.WALKING_TO_COUNTER)
        {
            if (!GoToCounter())
            {
                if (IsAtTargetPosition(target))
                {
                    SetStateAtCounter();
                }
                else
                {
                    GameLog.LogWarning("Could not go to the counter. RecalculateState()");
                }
            }
        }
    }

    private bool GoToCounter()
    {
        if (BussGrid.GetCounter() == null)
        {
            return false;
        }

        target = BussGrid.GetPathFindingGridFromWorldPosition(BussGrid.GetCounter().GetActionTile());
        return GoTo(target);
    }

    private void GoToTableToBeAttended()
    {
        if (tableToBeAttended == null || tableToBeAttended.GetIsObjectBeingDragged())
        {
            if (BussGrid.GetTableWithClient(out tableToBeAttended))
            {
                tableToBeAttended.SetAttendedBy(this);
            }
            else
            {
                // If there is not tables with client
                currentState = NpcState.IDLE;
                return;
            }
        }

        Vector3Int localTarget = BussGrid.GetPathFindingGridFromWorldPosition(tableToBeAttended.GetActionTile());
        coordOfTableToBeAttended = BussGrid.GetClosestPathGridPoint(Position, localTarget);

        // Meaning we did not find a correct spot to standup, we return
        // and enqueue de table to the list again 
        // if (localTarget == CoordOfTableToBeAttended)
        // {
        //     GameLog.Log("TODO: Popup We could not find a proper place to standup - GoToTableToBeAttended()");
        //     return;
        // }

        target = coordOfTableToBeAttended;

        // we can attend the table from the position we are currently at the moment     
        // In case the table is placed next to the counter 
        if (isAlreadyAtTarget(localTarget))
        {
            target = Position;
            coordOfTableToBeAttended = Position;
        }

        if (!GoTo(target))
        {
            return;
        }
    }

    public bool RestartState()
    {
        ResetMovement();
        target = BussGrid.GetCounter().GetActionTileInGridPosition();

        if (!GoTo(target))
        {
            if (IsAtTargetPosition(target))
            {
                currentState = NpcState.AT_COUNTER;
                return true;
            }
            else
            {
                // ("Retrying: We could not find a path - RestartState()");
            }
            return false;
        }

        currentState = NpcState.WALKING_TO_COUNTER;
        return true;
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
        tableToBeAttended = table;
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
        return coordOfTableToBeAttended;
    }
}
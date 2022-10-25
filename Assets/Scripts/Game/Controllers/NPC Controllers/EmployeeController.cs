using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmployeeController : GameObjectMovementBase
{
    private GameGridObject tableToBeAttended;
    [SerializeField]
    private NpcState localState;
    private PlayerAnimationStateController animationController;
    private GameController gameController;
    private GameTile unRespawnTile;
    // This attributes stay here, since there will be different employees with different attributes
    private const float SPEED_TIME_TO_TAKING_ORDER = 80f; //Decrease per second  100/15
    private const float SPEED_TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10
    private const float TIME_IDLE_BEFORE_TAKING_ORDER = 2f;
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float MAX_TIME_IN_STATE = Settings.NPCMaxTimeInState;
    public Vector3Int CoordOfTableToBeAttended { get; set; }
    private float idleTime;
    //Time in the current state
    private float stateTime;
    private NpcState prevState;
    //Current Goto Target
    private Vector3Int target;
    private const float MAX_TABLE_WAITING_TIME = Settings.NPCMaxWaitingTime;

    private void Start()
    {
        // Animation controller
        animationController = GetComponent<PlayerAnimationStateController>();

        // Game Controller
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObj.GetComponent<GameController>();

        // keeps the time in the current state
        idleTime = 0;
        stateTime = 0;
        prevState = localState;

        //Setting initial vars
        Type = ObjectType.EMPLOYEE;
        localState = NpcState.IDLE;
        Name = transform.name;

        //Checking for null
        if (animationController == null || gameObj == null)
        {
            GameLog.LogWarning("NPCController/animationController-gameObj null");
        }
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        // To Handle States
        // If the player removes the counter the employee goes away
        try
        {
            if (BussGrid.GetCounter() == null && localState != NpcState.WALKING_UNRESPAWN)
            {
                AddStateHistory("State: 1 \n");
                UpdateGoToUnrespawn_1();
            }
            else if (localState == NpcState.WALKING_UNRESPAWN)
            {
                AddStateHistory("State: 2 \n");
                UpdaetIsAtUnrespawn_2();
            }
            else if (localState == NpcState.IDLE)
            {
                AddStateHistory("State: 3 \n");
                UpdateGoNextToCounter_3();
            }
            else if (localState == NpcState.WALKING_TO_COUNTER)
            {
                AddStateHistory("State: 4 \n");
                UpdateIsAtCounter_4();
            }
            else if (localState == NpcState.AT_COUNTER && idleTime > TIME_IDLE_BEFORE_TAKING_ORDER)
            {
                AddStateHistory("State: 5 \n");
                UpdateAttendTable_5();
            }
            else if (localState == NpcState.WALKING_TO_TABLE)
            {
                AddStateHistory("State: 6 \n");
                UpdateIsTakingOrder_6();
            }
            else if (localState == NpcState.TAKING_ORDER)
            {
                AddStateHistory("State: 7 \n");
                UpdateTakeOrder_7();
            }
            else if (localState == NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER && CurrentEnergy >= 100)
            {
                AddStateHistory("State: 8 \n");
                UpdateOrderAttended_8();
            }
            else if (localState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
            {
                AddStateHistory("State: 9 \n");
                UpdateIsAtCounterAfterOrder_9();
            }
            else if (localState == NpcState.REGISTERING_CASH)
            {
                AddStateHistory("State: 10 \n");
                UpdateRegisterCash_10();
            }
            else if (localState == NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH && CurrentEnergy >= 100)
            {
                AddStateHistory("State: 11 \n");
                UpdateFinishRegistering_11();
            }
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate EmployeeController): " + e);
        }
        // Intended to be at the end
        UpdateAnimation();
        UpdateTimeInState();
    }

    private void UpdateAnimation()
    {
        if (IsMoving())
        {
            animationController.SetState(NpcState.WALKING);
        }
        else
        {
            animationController.SetState(localState);
        }
    }

    private void UpdateTimeInState()
    {
        // keeps the time in the current state
        if (prevState == localState)
        {
            stateTime += Time.fixedDeltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = localState;
        }

        idleTime += Time.fixedDeltaTime;

        if (stateTime > MAX_TIME_IN_STATE)
        {
            // if we are already at the counter and the time passed the max time 
            // there is no customers we stay at the counter, no need to RecalculateState()
            if (localState == NpcState.AT_COUNTER || BussGrid.GetCounter() == null)
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
        localState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            GameLog.LogWarning("Retrying: We could not find a path - UpdateGoToUnrespawn_0()");
            localState = NpcState.IDLE;
        }
    }

    private void UpdaetIsAtUnrespawn_2()
    {
        if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveEmployee();
            Destroy(gameObject);
        }
    }
    private void UpdateGoNextToCounter_3()
    {
        localState = NpcState.WALKING_TO_COUNTER;
        target = BussGrid.GetCounter().GetActionTileInGridPosition();

        if (!GoTo(target))
        {
            //Go to counter if it is already at the counter we change the state
            if (IsAtTargetPosition(target))
            {
                localState = NpcState.AT_COUNTER;
            }
            else
            {
                GameLog.LogWarning("Retrying: We could not find a path - UpdateGoNextToCounter_1() ");
                localState = NpcState.IDLE;
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
        localState = NpcState.AT_COUNTER;
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
        localState = NpcState.WALKING_TO_TABLE;
        GoToTableToBeAttended();
    }

    private void UpdateIsTakingOrder_6()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(CoordOfTableToBeAttended), transform))
        {
            return;
        }

        localState = NpcState.TAKING_ORDER;

        //the NPC left and the pointer to the table is null, we go to the counter
        if (tableToBeAttended == null || tableToBeAttended.GetUsedBy() == null)
        {
            if (!RestartState())
            {
                localState = NpcState.IDLE;
            }
            return;
        }

        StandTowards(tableToBeAttended.GetUsedBy().Position);//We flip the Employee -> CLient
        tableToBeAttended.GetUsedBy().FlipTowards(Position); // We flip client -> employee
        tableToBeAttended.GetUsedBy().SetBeingAttended();
    }

    private void UpdateTakeOrder_7()
    {
        localState = NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER;
        ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
    }

    // The client was attended we return the free table and Add money to the wallet
    // The client leaves the table once the table is set as free
    private void UpdateOrderAttended_8()
    {
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
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
        localState = NpcState.REGISTERING_CASH;
        StandTowards(BussGrid.GetCounter().GridPosition);
    }

    private void UpdateRegisterCash_10()
    {
        localState = NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH;
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
                localState = NpcState.IDLE;
            }
        }
        else if (localState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER || localState == NpcState.WALKING_TO_COUNTER)
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
                localState = NpcState.IDLE;
                return;
            }
        }

        Vector3Int localTarget = BussGrid.GetPathFindingGridFromWorldPosition(tableToBeAttended.GetActionTile());
        CoordOfTableToBeAttended = BussGrid.GetClosestPathGridPoint(Position, localTarget);

        // Meaning we did not find a correct spot to standup, we return
        // and enqueue de table to the list again 
        if (localTarget == CoordOfTableToBeAttended)
        {
            //("We could not find a proper place to standup - GoToTableToBeAttended()");
            return;
        }

        target = CoordOfTableToBeAttended;

        // we can attend the table from the position we are currently at the moment     
        // In case the table is placed next to the counter 
        if (isAlreadyAtTarget(localTarget))
        {
            target = Position;
            CoordOfTableToBeAttended = Position;
        }

        if (!GoTo(target))
        {
            //("Retrying: We could not find a path - GoToTableToBeAttended()");
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
                localState = NpcState.AT_COUNTER;
                return true;
            }
            else
            {
                // ("Retrying: We could not find a path - RestartState()");
            }
            return false;
        }

        localState = NpcState.WALKING_TO_COUNTER;
        return true;
    }

    public NpcState GetNpcState()
    {
        return localState;
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
}
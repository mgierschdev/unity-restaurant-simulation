using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    private GameGridObject tableToBeAttended;
    [SerializeField]
    private NpcState localState;
    private PlayerAnimationStateController animationController;
    private const float TIME_TO_TAKING_ORDER = 80f; //Decrease per second  100/15
    private const float TIME_IDLE_BEFORE_TAKING_ORDER = 2f;
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10
    public Vector3Int CoordOfTableToBeAttended { get; set; }
    private float idleTime;
    //Time in the current state
    private float stateTime;
    private NpcState prevState;
    //Current Goto Target
    private Vector3Int target;

    private const float MAX_TABLE_WAITING_TIME = 10f;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        localState = NpcState.IDLE_0;
        Name = transform.name;
        animationController = GetComponent<PlayerAnimationStateController>();
        idleTime = 0;
        // keeps the time in the current state
        stateTime = 0;
        prevState = localState;

        if (animationController == null)
        {
            GameLog.LogWarning("NPCController/animationController null");
        }
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        // To Handle States
        UpdateGoNextToCounter();
        UpdateIsAtCounter();
        UpdateAttendTable();
        UpdateIsTakingOrder();
        UpdateTakeOrder();
        UpdateOrderAttended();
        UpdateIsAtCounterAfterOrder();
        UpdateRegisterCash();
        UpdateFinishRegistering();

        // keeps the time in the current state
        if (prevState == localState)
        {
            // GameLog.Log("Current state time " + stateTime);
            stateTime += Time.deltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = localState;
        }


        //Defult time to restart
        if (localState == NpcState.WALKING_TO_TABLE_1 && stateTime >= 3f)
        {
            //GameLog.Log("NPC stuck restarting");
            ResetState();
        }

        animationController.SetState(localState);
        idleTime += Time.deltaTime;
    }

    private void ResetState()
    {
        ResetMovement(); // we stop the player from moving
        RestartState(); // we reset the state
    }

    private void UpdateFinishRegistering()
    {
        if (localState == NpcState.REGISTERING_CASH_10 && CurrentEnergy >= 100)
        {
            localState = NpcState.AT_COUNTER_4; // we are at counter 
            double orderCost = Random.Range(5, 10);
            //GameLog.Log("TODO: +" + orderCost);
            Grid.playerData.AddMoney(orderCost);
        }
    }

    private void UpdateRegisterCash()
    {
        if (localState == NpcState.REGISTERING_CASH_10)
        {
            ActivateEnergyBar(TIME_TO_REGISTER_IN_CASH);
        }
    }

    private void UpdateIsAtCounterAfterOrder()
    {
        if (localState != NpcState.WALKING_TO_COUNTER_AFTER_ORDER_9 || Grid.GetCounter() == null || !Util.IsAtDistanceWithObjectTraslate(transform.position, Grid.GetCounter().GetActionTile(), transform))
        {
            return;
        }
        localState = NpcState.REGISTERING_CASH_10;
    }

    // The client was attended we return the free table and Add money to the wallet
    private void UpdateOrderAttended()
    {
        if (localState != NpcState.TAKING_ORDER_6 || CurrentEnergy < 100)
        {
            return;
        }
        RestartStateAfterAttendingTable();
    }

    private void UpdateTakeOrder()
    {
        if (localState != NpcState.TAKING_ORDER_6)
        {
            return;
        }

        ActivateEnergyBar(TIME_TO_TAKING_ORDER);
    }

    private void UpdateAttendTable()
    {
        if (localState != NpcState.AT_COUNTER_4 || !Grid.IsThereCustomer() || idleTime < TIME_IDLE_BEFORE_TAKING_ORDER)
        {
            return;
        }

        // We can we idle and not attend the table
        float idleProbability = Random.Range(0, 100);
        if (idleProbability < RANDOM_PROBABILITY_TO_WAIT)
        {
            GameLog.Log("Waiting...");
            idleTime = 0;
            return;
        }

        localState = NpcState.WALKING_TO_TABLE_1;
        GoToTableToBeAttended();
    }

    public void RecalculateState(GameGridObject obj)
    {

        if (obj == tableToBeAttended)
        {
            RestartState();
        }
        else if (localState == NpcState.WALKING_TO_TABLE_1)
        {
            GoToTableToBeAttended();
        }
        else if (localState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER_9 || localState == NpcState.WALKING_TO_COUNTER_3)
        {
            GoToCounter();
        }
    }

    private void GoToCounter()
    {
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());
        GoTo(target);
    }
    private void GoToTableToBeAttended()
    {
        if (tableToBeAttended == null)
        {
            tableToBeAttended = Grid.GetTableWithClient();
        }
        Vector3Int localTarget = Grid.GetPathFindingGridFromWorldPosition(tableToBeAttended.GetActionTile());
        CoordOfTableToBeAttended = Grid.GetClosestPathGridPoint(localTarget);

        // Meaning we did not find a correct spot to standup, we return
        // and enqueue de table to the list again 
        if (localTarget == CoordOfTableToBeAttended)
        {
            GameLog.Log("We could not find a proper place to standup");
            Grid.AddClientToTable(tableToBeAttended);
            return;
        }
        target = CoordOfTableToBeAttended;
        if (!GoTo(target))
        {
            GameLog.Log("We could not find a path");
            Grid.AddClientToTable(tableToBeAttended);
            return;

        }
    }

    private void UpdateIsTakingOrder()
    {
        if (localState != NpcState.WALKING_TO_TABLE_1 || !Util.IsAtDistanceWithObjectTraslate(transform.position, Grid.GetWorldFromPathFindingGridPositionWithOffSet(CoordOfTableToBeAttended), transform))
        {
            return;
        }
        localState = NpcState.TAKING_ORDER_6;
    }

    //First State
    private void UpdateGoNextToCounter()
    {
        if (localState != NpcState.IDLE_0 || Grid.GetCounter() == null)
        {
            return;
        }
        localState = NpcState.WALKING_TO_COUNTER_3;
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());
        if (!GoTo(target))
        {
            localState = NpcState.IDLE_0;
        }
    }

    private void UpdateIsAtCounter()
    {
        if (localState != NpcState.WALKING_TO_COUNTER_3 || !Util.IsAtDistanceWithObject(transform.position, Grid.GetCounter().GetActionTile()) || Grid.GetCounter() == null)
        {
            return;
        }
        localState = NpcState.AT_COUNTER_4;
        idleTime = 0;
    }

    private void RestartStateAfterAttendingTable()
    {
        GoToCounter();
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER_9;
        Grid.AddFreeBusinessSpots(tableToBeAttended);
        tableToBeAttended.SetBusy(false);
        tableToBeAttended = null;
    }

    private void RestartState()
    {
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());
        GoTo(target);
        localState = NpcState.WALKING_TO_COUNTER_3;
        if (tableToBeAttended != null)
        {
            Grid.AddFreeBusinessSpots(tableToBeAttended);
            tableToBeAttended.SetBusy(false);
            tableToBeAttended = null;
        }
    }

    public NpcState GetNpcState()
    {
        return localState;
    }
}
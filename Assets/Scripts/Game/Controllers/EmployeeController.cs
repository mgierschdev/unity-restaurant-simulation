using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    private GameGridObject tableToBeAttended;
    [SerializeField]
    private NpcState localState;
    private PlayerAnimationStateController animationController;
    private GameController gameController;
    private GameTile unRespawnTile;
    private const float SPEED_TIME_TO_TAKING_ORDER = 80f; //Decrease per second  100/15
    private const float SPEED_TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10
    private const float TIME_IDLE_BEFORE_TAKING_ORDER = 2f;
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
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

        // To Handle Statess
        if (Grid.GetCounter() == null) //If the player removes the counter the employee goes away
        {
            UpdateGoToUnrespawn_0();
        }
        else if (localState == NpcState.WALKING_UNRESPAWN)
        {
            UpdaetIsAtUnrespawn_0();
        }
        else if (localState == NpcState.IDLE && Grid.GetCounter() != null)
        {
            UpdateGoNextToCounter_1();
        }
        else if (localState == NpcState.WALKING_TO_COUNTER)
        {
            UpdateIsAtCounter_2();
        }
        else if (localState == NpcState.AT_COUNTER && Grid.IsThereCustomer() && idleTime > TIME_IDLE_BEFORE_TAKING_ORDER)
        {
            UpdateAttendTable_3();
        }
        else if (localState == NpcState.WALKING_TO_TABLE)
        {
            UpdateIsTakingOrder_4();
        }
        else if (localState == NpcState.TAKING_ORDER)
        {
            UpdateTakeOrder_5();
        }
        else if (localState == NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER && CurrentEnergy >= 100)
        {
            UpdateOrderAttended_6();
        }
        else if (localState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER)
        {
            UpdateIsAtCounterAfterOrder_7();
        }
        else if (localState == NpcState.REGISTERING_CASH)
        {
            UpdateRegisterCash_8();
        }
        else if (localState == NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH && CurrentEnergy >= 100)
        {
            UpdateFinishRegistering_9();
        }

        // keeps the time in the current state
        if (prevState == localState)
        {
            stateTime += Time.deltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = localState;
        }

        animationController.SetState(localState);
        idleTime += Time.deltaTime;
    }

    //First State
    private void UpdateGoToUnrespawn_0()
    {
        localState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = Grid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            //GameLog.Log("We could not find path to unrespawn");
            localState = NpcState.IDLE;
        }
    }

    private void UpdaetIsAtUnrespawn_0()
    {
        if (Util.IsAtDistanceWithObject(transform.position, Grid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveEmployee();
            Destroy(gameObject);
        }
    }
    private void UpdateGoNextToCounter_1()
    {
        localState = NpcState.WALKING_TO_COUNTER;
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());

        //Go to counter if it is already at the counter we change the state
        if (Position == target)
        {
            localState = NpcState.AT_COUNTER;
            return;
        }

        if (!GoTo(target))
        {
            localState = NpcState.IDLE;
        }
    }

    private void UpdateIsAtCounter_2()
    {
        if (!Util.IsAtDistanceWithObject(transform.position, Grid.GetCounter().GetActionTile()))
        {
            return;
        }
        localState = NpcState.AT_COUNTER;
        idleTime = 0;
    }

    private void UpdateAttendTable_3()
    {
        // We can we idle and not attend the table
        float idleProbability = Random.Range(0, 100);
        if (idleProbability < RANDOM_PROBABILITY_TO_WAIT)
        {
            GameLog.Log("Waiting...");
            idleTime = 0;
            return;
        }
        localState = NpcState.WALKING_TO_TABLE;
        GoToTableToBeAttended();
    }

    private void UpdateIsTakingOrder_4()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, Grid.GetWorldFromPathFindingGridPositionWithOffSet(CoordOfTableToBeAttended), transform))
        {
            return;
        }
        localState = NpcState.TAKING_ORDER;
    }

    private void UpdateTakeOrder_5()
    {
        ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
        localState = NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER;
    }

    // The client was attended we return the free table and Add money to the wallet
    private void UpdateOrderAttended_6()
    {
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
        tableToBeAttended.SetBusy(false);
        tableToBeAttended = null;
        if (!GoToCounter())
        {
            while (!GoToCounter() && Grid.GetCounter() != null) { }
        }
    }

    private void UpdateIsAtCounterAfterOrder_7()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, Grid.GetCounter().GetActionTile(), transform))
        {
            return;
        }
        localState = NpcState.REGISTERING_CASH;
    }

    private void UpdateRegisterCash_8()
    {
        ActivateEnergyBar(SPEED_TIME_TO_REGISTER_IN_CASH);
        localState = NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH;
    }

    private void UpdateFinishRegistering_9()
    {
        localState = NpcState.AT_COUNTER;
        double orderCost = Random.Range(5, 10);
        //GameLog.Log("TODO: +" + orderCost);
        Grid.playerData.AddMoney(orderCost);
    }

    // private void ResetState()
    // {
    //     ResetMovement(); // we stop the player from moving
    //     RestartState(); // we reset the state
    // }

    public void RecalculateState(GameGridObject obj)
    {

        if (obj == tableToBeAttended)
        {
            if (!RestartState())
            {
                localState = NpcState.IDLE;
            }
        }
        else if (localState == NpcState.WALKING_TO_TABLE)
        {
            GoToTableToBeAttended();
        }
        else if (localState == NpcState.WALKING_TO_COUNTER_AFTER_ORDER || localState == NpcState.WALKING_TO_COUNTER)
        {
            if (!GoToCounter())
            {
                localState = NpcState.IDLE;
            }
        }
    }

    private bool GoToCounter()
    {
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());
        return GoTo(target);
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
            GameLog.Log("We could not find a proper place to standup - GoToTableToBeAttended()");
            return;
        }
        target = CoordOfTableToBeAttended;
        if (!GoTo(target))
        {
            GameLog.Log("We could not find a path - GoToTableToBeAttended()");
            return;
        }
    }

    private bool RestartState()
    {
        target = Grid.GetPathFindingGridFromWorldPosition(Grid.GetCounter().GetActionTile());

        if (!GoTo(target))
        {
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
}
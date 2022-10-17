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
    private const float MAX_TIME_IN_STATE = 10F;
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

        // To Handle States
        if (BussGrid.GetCounter() == null && localState != NpcState.WALKING_UNRESPAWN) //If the player removes the counter the employee goes away
        {
            UpdateGoToUnrespawn_0();
        }
        else if (localState == NpcState.WALKING_UNRESPAWN)
        {
            UpdaetIsAtUnrespawn_0();
        }
        else if (localState == NpcState.IDLE)
        {
            UpdateGoNextToCounter_1();
        }
        else if (localState == NpcState.WALKING_TO_COUNTER)
        {
            UpdateIsAtCounter_2();
        }
        else if (localState == NpcState.AT_COUNTER && BussGrid.IsThereCustomer() && idleTime > TIME_IDLE_BEFORE_TAKING_ORDER)
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
            if (localState == NpcState.AT_COUNTER)
            {
                return;
            }

            ResetMovement();
            RecalculateState(tableToBeAttended);
        }
    }
    //First State
    private void UpdateGoToUnrespawn_0()
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

    private void UpdaetIsAtUnrespawn_0()
    {
        if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveEmployee();
            Destroy(gameObject);
        }
    }
    private void UpdateGoNextToCounter_1()
    {
        localState = NpcState.WALKING_TO_COUNTER;
        target = BussGrid.GetPathFindingGridFromWorldPosition(BussGrid.GetCounter().GetActionTile());

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

    private void UpdateIsAtCounter_2()
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
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(CoordOfTableToBeAttended), transform))
        {
            return;
        }

        StandTowards(tableToBeAttended.GetUsedBy().Position);//We flip the Employee -> CLient
        tableToBeAttended.GetUsedBy().FlipTowards(Position); // We flip client -> employee
        tableToBeAttended.GetUsedBy().SetAttended();
        localState = NpcState.TAKING_ORDER;
    }

    private void UpdateTakeOrder_5()
    {
        ActivateEnergyBar(SPEED_TIME_TO_TAKING_ORDER);
        localState = NpcState.WAITING_FOR_ENERGY_BAR_TAKING_ORDER;
    }

    // The client was attended we return the free table and Add money to the wallet
    // The client leaves the table onece the table is set as free
    private void UpdateOrderAttended_6()
    {
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
        tableToBeAttended.SetBusy(false);
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

    private void UpdateIsAtCounterAfterOrder_7()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, BussGrid.GetCounter().GetActionTile(), transform))
        {
            return;
        }
        StandTowards(BussGrid.GetCounter().GridPosition);
        localState = NpcState.REGISTERING_CASH;
    }

    private void UpdateRegisterCash_8()
    {
        ActivateEnergyBar(SPEED_TIME_TO_REGISTER_IN_CASH);
        localState = NpcState.WAITING_FOR_ENERGY_BAR_REGISTERING_CASH;
    }

    private void UpdateFinishRegistering_9()
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
        else if (localState == NpcState.WALKING_TO_TABLE)
        {
            GoToTableToBeAttended();
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
        if (tableToBeAttended == null)
        {
            //GameLog.Log("Getting table with client: GoToTableToBeAttended()");
            tableToBeAttended = BussGrid.GetTableWithClient();
            tableToBeAttended.SetAttendedBy(this);

            if (tableToBeAttended == null)
            {
                GameLog.Log("There is no table to attend: GoToTableToBeAttended()");
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
            GameLog.Log("We could not find a proper place to standup - GoToTableToBeAttended()");
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
            GameLog.LogWarning("Retrying: We could not find a path - GoToTableToBeAttended()");
            return;
        }
    }

    public bool RestartState()
    {
        ResetMovement();
        target = BussGrid.GetPathFindingGridFromWorldPosition(BussGrid.GetCounter().GetActionTile());

        if (!GoTo(target))
        {
            if (IsAtTargetPosition(target))
            {
                localState = NpcState.AT_COUNTER;
                return true;
            }
            else
            {
                GameLog.LogWarning("Retrying: We could not find a path - RestartState()");
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
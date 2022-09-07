using UnityEngine;
using UnityEngine.Serialization;

public class EmployeeController : GameObjectMovementBase
{
    private GameGridObject counter;
    private GameGridObject tableToBeAttended;
    [SerializeField]
    private NpcState localState;
    private PlayerAnimationStateController animationController;
    private const float TIME_TO_TAKING_ORDER = 80f; //Decrease per second  100/15
    private const float TIME_IDLE_BEFORE_TAKING_ORDER = 2f;
    private const int RANDOM_PROBABILITY_TO_WAIT = 0;
    private const float TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10
    private Vector3Int coordOfTableToBeAttended;
    private float idleTime;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        localState = NpcState.IDLE;
        Name = transform.name;
        counter = GameGrid.Counter;
        animationController = GetComponent<PlayerAnimationStateController>();
        idleTime = 0;

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

        // Client left
        if (tableToBeAttended != null && GameGrid.IsTableInFreeBussSpot(tableToBeAttended))
        {
            GameLog.Log("Table moved employee");
            RestartState();
        }

        animationController.SetState(localState);
        idleTime += Time.deltaTime;
    }

    private void UpdateFinishRegistering()
    {
        if (localState == NpcState.REGISTERING_CASH && CurrentEnergy >= 100)
        {
            localState = NpcState.AT_COUNTER; // we are at counter 
        }
    }

    private void UpdateRegisterCash()
    {
        if (localState == NpcState.REGISTERING_CASH)
        {
            ActivateEnergyBar(TIME_TO_REGISTER_IN_CASH);
        }
    }

    private void UpdateIsAtCounterAfterOrder()
    {
        if (localState != NpcState.WALKING_TO_COUNTER_AFTER_ORDER || counter == null || !IsAtGameGridObject(counter.ActionGridPosition))
        {
            return;
        }
        localState = NpcState.REGISTERING_CASH;
    }

    // The client was attended we return the free table and Add money to the wallet
    private void UpdateOrderAttended()
    {
        if (localState != NpcState.TAKING_ORDER || CurrentEnergy < 100)
        {
            return;
        }
        GameGrid.PlayerData.AddMoney(Random.Range(5, 10));
        RestartState();
    }

    private void UpdateTakeOrder()
    {
        if (localState != NpcState.TAKING_ORDER)
        {
            return;
        }

        ActivateEnergyBar(TIME_TO_TAKING_ORDER);
    }

    private void UpdateAttendTable()
    {
        if (localState != NpcState.AT_COUNTER || !GameGrid.IsThereCustomer() || idleTime < TIME_IDLE_BEFORE_TAKING_ORDER)
        {
            return;
        }

        // We can we idle and not attend the table
        float idleProbability  = Random.Range(0, 100);
        if(idleProbability < RANDOM_PROBABILITY_TO_WAIT){
            GameLog.Log("Waiting...");
            idleTime = 0;
            return;
        }

        tableToBeAttended = GameGrid.GetTableWithClient();
        localState = NpcState.WALKING_TO_TABLE;
        coordOfTableToBeAttended = GameGrid.GetClosestPathGridPoint(counter.ActionGridPosition, tableToBeAttended.ActionGridPosition);
        GoTo(coordOfTableToBeAttended);
    }

    private void UpdateIsTakingOrder()
    {
        if (localState != NpcState.WALKING_TO_TABLE || !IsAtGameGridObject(coordOfTableToBeAttended))
        {
            return;
        }
        localState = NpcState.TAKING_ORDER;
    }

    //First State
    private void UpdateGoNextToCounter()
    {
        if (localState != NpcState.IDLE || counter == null)
        {
            return;
        }
        counter = GameGrid.Counter;
        localState = NpcState.WALKING_TO_COUNTER;
        GoTo(counter.ActionGridPosition);
    }

    private void UpdateIsAtCounter()
    {
        if (localState != NpcState.WALKING_TO_COUNTER || !IsAtGameGridObject(counter.ActionGridPosition) || counter == null)
        {
            return;
        }
        localState = NpcState.AT_COUNTER;
        idleTime = 0;
    }

    private bool IsAtGameGridObject(Vector3Int target)
    {
        return Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(target)) < Settings.MinDistanceToTarget;
    }

    private void RestartState()
    {
        GoTo(counter.ActionGridPosition);
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
        GameGrid.AddFreeBusinessSpots(tableToBeAttended);
        tableToBeAttended.Busy = false;
        tableToBeAttended = null;
    }
}
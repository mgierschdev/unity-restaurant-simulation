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
    private Vector3Int coordOfTableToBeAttended;
    private float idleTime;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        localState = NpcState.IDLE;
        Name = transform.name;
        animationController = GetComponent<PlayerAnimationStateController>();
        idleTime = 0;

        if (animationController == null)
        {
            GameLog.LogWarning("NPCController/animationController null");
        }
    }

    private void FixedUpdate()
    {

        // Client left or table was moved of position
        if (tableToBeAttended != null && GameGrid.IsTableInFreeBussSpot(tableToBeAttended))
        {
            GameLog.Log("Object being used moved " + tableToBeAttended.Name);
            ResetMovement(); // we stop the player from moving
            RestartState(); // we reset the state
        }

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

        animationController.SetState(localState);
        idleTime += Time.deltaTime;
    }

    private void UpdateFinishRegistering()
    {
        if (localState == NpcState.REGISTERING_CASH && CurrentEnergy >= 100)
        {
            localState = NpcState.AT_COUNTER; // we are at counter 
            double orderCost = Random.Range(5, 10);
            GameLog.Log("Added to the wallet: " + orderCost);
            GameGrid.PlayerData.AddMoney(orderCost);
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
        if (localState != NpcState.WALKING_TO_COUNTER_AFTER_ORDER || GameGrid.Counter == null || !Util.IsAtDistanceWithObject(transform.position, GameGrid.Counter.GetActionTile()))
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
        RestartStateAfterAttendingTable();
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
        float idleProbability = Random.Range(0, 100);
        if (idleProbability < RANDOM_PROBABILITY_TO_WAIT)
        {
            GameLog.Log("Waiting...");
            idleTime = 0;
            return;
        }

        tableToBeAttended = GameGrid.GetTableWithClient();  
        localState = NpcState.WALKING_TO_TABLE;
        coordOfTableToBeAttended = GameGrid.GetClosestPathGridPoint(GameGrid.GetPathFindingGridFromWorldPosition(GameGrid.Counter.GetActionTile()), GameGrid.GetPathFindingGridFromWorldPosition(tableToBeAttended.GetActionTile()));
        GoTo(coordOfTableToBeAttended);
    }

    private void UpdateIsTakingOrder()
    {
        if (localState != NpcState.WALKING_TO_TABLE || !Util.IsAtDistanceWithObject(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(coordOfTableToBeAttended)))
        {
            return;
        }
        localState = NpcState.TAKING_ORDER;
    }

    //First State
    private void UpdateGoNextToCounter()
    {
        if (localState != NpcState.IDLE || GameGrid.Counter == null)
        {
            return;
        }
        localState = NpcState.WALKING_TO_COUNTER;
        GoTo(GameGrid.GetPathFindingGridFromWorldPosition(GameGrid.Counter.GetActionTile()));
    }

    private void UpdateIsAtCounter()
    {
        if (localState != NpcState.WALKING_TO_COUNTER || !Util.IsAtDistanceWithObject(transform.position, GameGrid.Counter.GetActionTile()) || GameGrid.Counter == null)
        {
            return;
        }
        localState = NpcState.AT_COUNTER;
        idleTime = 0;
    }

    private void RestartStateAfterAttendingTable()
    {
        GoTo(GameGrid.GetPathFindingGridFromWorldPosition(GameGrid.Counter.GetActionTile()));
        localState = NpcState.WALKING_TO_COUNTER_AFTER_ORDER;
        GameGrid.AddFreeBusinessSpots(tableToBeAttended);
        tableToBeAttended.Busy = false;
        tableToBeAttended = null;
    }

    private void RestartState()
    {
        GoTo(GameGrid.GetPathFindingGridFromWorldPosition(GameGrid.Counter.GetActionTile()));
        localState = NpcState.WALKING_TO_COUNTER;
        tableToBeAttended.Busy = false;
        tableToBeAttended = null;
    }
}
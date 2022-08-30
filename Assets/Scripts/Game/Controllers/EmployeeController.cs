using PlasticPipe.PlasticProtocol.Client.Proxies;
using UnityEngine;
using UnityEngine.Rendering;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    GameGridObject tableToBeAttended;
    [SerializeField]
    NPCState state;

    float timeToTakeOrder = 15f; //seconds to serve the order
    float timeToRegisterInCash = 15f; //seconds to serve the order

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        state = NPCState.IDLE;
        Name = transform.name;
        counter = GameGrid.Counter;
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

    }

    private void UpdateFinishRegistering(){
        if(state == NPCState.REGISTERING_CASH && CurrentEnergy < 0){
            state = NPCState.AT_COUNTER; // we are at counter 
        }
    }

    private void UpdateRegisterCash()
    {
        if (state == NPCState.REGISTERING_CASH)
        {
            ActivateEnergyBar(timeToRegisterInCash);
        }
    }

    private void UpdateIsAtCounterAfterOrder()
    {
        if (state == NPCState.WALKING_TO_COUNTER_AFTER_ORDER && counter != null)
        {
            if (IsAtGameGridObject(counter))
            {
                state = NPCState.REGISTERING_CASH;
            }
        }
    }

    private void UpdateOrderAttended()
    {
        if (state == NPCState.TAKING_ORDER && CurrentEnergy <= 0)
        {
            GoTo(counter.ActionGridPosition);
            state = NPCState.WALKING_TO_COUNTER_AFTER_ORDER;
            GameGrid.AddFreeBusinessSpots(tableToBeAttended);
            tableToBeAttended.Busy = false;
            tableToBeAttended = null;
        }
    }

    private void UpdateTakeOrder()
    {
        if (state == NPCState.TAKING_ORDER)
        {
            ActivateEnergyBar(timeToTakeOrder);
        }
    }

    private void UpdateAttendTable()
    {
        if (state == NPCState.AT_COUNTER && GameGrid.IsThereCustomer())
        {
            tableToBeAttended = GameGrid.GetTableWithClient();
            if (tableToBeAttended != null)
            {
                state = NPCState.WALKING_TO_TABLE;
                GoTo(tableToBeAttended.ActionGridPosition);
            }
        }
    }

    private void UpdateIsTakingOrder()
    {
        if (state == NPCState.WALKING_TO_TABLE)
        {
            if (IsAtGameGridObject(tableToBeAttended))
            {
                state = NPCState.TAKING_ORDER;
            }
        }
    }
    //First State
    private void UpdateGoNextToCounter()
    {
        if (state == NPCState.IDLE)
        {
            counter = GameGrid.Counter;
            if (counter != null)
            {
                state = NPCState.WALKING_TO_COUNTER;
                GoTo(counter.ActionGridPosition);
            }
        }
    }

    private void UpdateIsAtCounter()
    {
        if (state == NPCState.WALKING_TO_COUNTER && counter != null)
        {
            if (IsAtGameGridObject(counter))
            {
                state = NPCState.AT_COUNTER;
            }
        }
    }

    private bool IsAtGameGridObject(GameGridObject obj)
    {
        return Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(obj.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET;
    }
}
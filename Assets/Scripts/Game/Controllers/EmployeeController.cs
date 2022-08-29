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
    }

    private void UpdateOrderAttended()
    {
        if(state ==  NPCState.TAKING_ORDER && CurrentEnergy <= 0){
            GoTo(counter.ActionGridPosition);
            state = NPCState.WALKING_TO_COUNTER;
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
        if (GameGrid.IsThereCustomer() && state == NPCState.AT_COUNTER)
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
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(tableToBeAttended.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                state = NPCState.TAKING_ORDER;
            }
        }
    }
    private void UpdateGoNextToCounter()
    {
        if (state != NPCState.WALKING_TO_COUNTER && state != NPCState.AT_COUNTER && state != NPCState.WALKING_TO_TABLE && state != NPCState.TAKING_ORDER)
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
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(counter.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                state = NPCState.AT_COUNTER;
            }
        }
    }
}
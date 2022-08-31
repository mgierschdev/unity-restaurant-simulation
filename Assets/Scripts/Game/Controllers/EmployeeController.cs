using PlasticPipe.PlasticProtocol.Client.Proxies;
using UnityEngine;
using UnityEngine.Rendering;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    GameGridObject tableToBeAttended;
    [SerializeField]
    NPCState LocalState;
    private PlayerAnimationStateController animationController;

    float timeToTakeOrder = 80f; //Decrease per second  100/15
    float timeToRegisterInCash = 150f; //Decrease per second  100/30 10

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        LocalState = NPCState.IDLE;
        Name = transform.name;
        counter = GameGrid.Counter;
        animationController = GetComponent<PlayerAnimationStateController>();

        if (animationController == null)
        {
            Debug.LogWarning("NPCController/animationController null");
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

        animationController.SetState(LocalState);
    }

    private void UpdateFinishRegistering()
    {
        if (LocalState == NPCState.REGISTERING_CASH && CurrentEnergy >= 100)
        {
            LocalState = NPCState.AT_COUNTER; // we are at counter 
        }
    }

    private void UpdateRegisterCash()
    {
        if (LocalState == NPCState.REGISTERING_CASH)
        {
            ActivateEnergyBar(timeToRegisterInCash);
        }
    }

    private void UpdateIsAtCounterAfterOrder()
    {
        if (LocalState == NPCState.WALKING_TO_COUNTER_AFTER_ORDER && counter != null)
        {
            if (IsAtGameGridObject(counter))
            {
                LocalState = NPCState.REGISTERING_CASH;
            }
        }
    }

    private void UpdateOrderAttended()
    {
        if (LocalState == NPCState.TAKING_ORDER && CurrentEnergy >= 100)
        {
            GoTo(counter.ActionGridPosition);
            LocalState = NPCState.WALKING_TO_COUNTER_AFTER_ORDER;
            GameGrid.AddFreeBusinessSpots(tableToBeAttended);
            tableToBeAttended.Busy = false;
            tableToBeAttended = null;
        }
    }

    private void UpdateTakeOrder()
    {
        if (LocalState == NPCState.TAKING_ORDER)
        {
            ActivateEnergyBar(timeToTakeOrder);
        }
    }

    private void UpdateAttendTable()
    {
        if (LocalState == NPCState.AT_COUNTER && GameGrid.IsThereCustomer())
        {
            tableToBeAttended = GameGrid.GetTableWithClient();
            if (tableToBeAttended != null)
            {
                LocalState = NPCState.WALKING_TO_TABLE;
                GoTo(tableToBeAttended.ActionGridPosition);
            }
        }
    }

    private void UpdateIsTakingOrder()
    {
        if (LocalState == NPCState.WALKING_TO_TABLE)
        {
            if (IsAtGameGridObject(tableToBeAttended))
            {
                LocalState = NPCState.TAKING_ORDER;
            }
        }
    }
    //First State
    private void UpdateGoNextToCounter()
    {
        if (LocalState == NPCState.IDLE)
        {
            counter = GameGrid.Counter;
            if (counter != null)
            {
                LocalState = NPCState.WALKING_TO_COUNTER;
                GoTo(counter.ActionGridPosition);
            }
        }
    }

    private void UpdateIsAtCounter()
    {
        if (LocalState == NPCState.WALKING_TO_COUNTER && counter != null)
        {
            if (IsAtGameGridObject(counter))
            {
                LocalState = NPCState.AT_COUNTER;
            }
        }
    }

    private bool IsAtGameGridObject(GameGridObject obj)
    {
        return Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(obj.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET;
    }
}
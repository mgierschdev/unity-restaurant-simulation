using PlasticPipe.PlasticProtocol.Client.Proxies;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class EmployeeController : GameObjectMovementBase
{
    private GameGridObject counter;
    private GameGridObject tableToBeAttended;
    [FormerlySerializedAs("LocalState")] [SerializeField]
    private NPCState localState;
    private PlayerAnimationStateController animationController;
    private const float TIME_TO_TAKE_ORDER = 80f; //Decrease per second  100/15
    private const float TIME_TO_REGISTER_IN_CASH = 150f; //Decrease per second  100/30 10

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        localState = NPCState.IDLE;
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

        // Client left
        if (tableToBeAttended != null && GameGrid.IsTableInFreeBussSpot(tableToBeAttended))
        {
            Debug.Log("Table moved employee");
            RestartState();
        }

        animationController.SetState(localState);
    }

    private void UpdateFinishRegistering()
    {
        if (localState == NPCState.REGISTERING_CASH && CurrentEnergy >= 100)
        {
            localState = NPCState.AT_COUNTER; // we are at counter 
        }
    }

    private void UpdateRegisterCash()
    {
        if (localState == NPCState.REGISTERING_CASH)
        {
            ActivateEnergyBar(TIME_TO_REGISTER_IN_CASH);
        }
    }

    private void UpdateIsAtCounterAfterOrder()
    {
        if (localState != NPCState.WALKING_TO_COUNTER_AFTER_ORDER || counter == null || !IsAtGameGridObject(counter))
        {
            return;
        }
        
        localState = NPCState.REGISTERING_CASH;
    }

    // The client was attended we return the free table
    private void UpdateOrderAttended()
    {
        if (localState == NPCState.TAKING_ORDER && CurrentEnergy >= 100)
        {
            RestartState();
        }
    }

    private void UpdateTakeOrder()
    {
        if (localState == NPCState.TAKING_ORDER)
        {
            ActivateEnergyBar(TIME_TO_TAKE_ORDER);
        }
    }

    private void UpdateAttendTable()
    {
        if (localState != NPCState.AT_COUNTER || !GameGrid.IsThereCustomer())
        {
            return;
        }
        tableToBeAttended = GameGrid.GetTableWithClient();
        localState = NPCState.WALKING_TO_TABLE;
        GoTo(tableToBeAttended.ActionGridPosition);
    }

    private void UpdateIsTakingOrder()
    {
        if (localState != NPCState.WALKING_TO_TABLE || !IsAtGameGridObject(tableToBeAttended))
        {
            return;
        } 
        
        localState = NPCState.TAKING_ORDER;
    }
    
    //First State
    private void UpdateGoNextToCounter()
    {
        if (localState != NPCState.IDLE || counter == null)
        {
            return;
        }
        
        counter = GameGrid.Counter;
        localState = NPCState.WALKING_TO_COUNTER;
        GoTo(counter.ActionGridPosition);
    }

    private void UpdateIsAtCounter()
    {
        if (localState != NPCState.WALKING_TO_COUNTER || !IsAtGameGridObject(counter) || counter == null)
        {
            return;
        }

        localState = NPCState.AT_COUNTER;
    }

    private bool IsAtGameGridObject(GameGridObject obj)
    {
        return Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(obj.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET;
    }

    private void RestartState()
    {
        GoTo(counter.ActionGridPosition);
        localState = NPCState.WALKING_TO_COUNTER_AFTER_ORDER;
        GameGrid.AddFreeBusinessSpots(tableToBeAttended);
        tableToBeAttended.Busy = false;
        tableToBeAttended = null;
    }
}
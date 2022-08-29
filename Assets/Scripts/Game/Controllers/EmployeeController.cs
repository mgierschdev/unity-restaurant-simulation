using PlasticPipe.PlasticProtocol.Client.Proxies;
using UnityEngine;
using UnityEngine.Rendering;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    GameGridObject tableToBeAttended;
    [SerializeField]
    NPCState localState;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        localState = NPCState.IDLE;
        Name = transform.name;
        counter = GameGrid.Counter;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        if (!IsWalking())
        {
            GoNextToCounter();

            // if (IsAtCounter())
            // {
            //     Debug.Log("At counter");
            //     localState = NPCState.WAITING;
            // }

            // if(state == NPCState.WAITING){
            //     //Check for customers to take order
            //     if(IsThereCustomer()){
            //         ServeTable();
            //         state = NPCState.SERVING;
            //     }
            // }
        }
    }
    private bool GoNextToCounter()
    {
        counter = GameGrid.Counter;
        if (counter != null)
        {
            GoTo(counter.GetGridPositionWithOffset());// arrive one spot infront
            return true;
        }
        return false;
    }
    private bool IsAtCounter()
    {
        if (counter != null)
        {
            return Vector3.Distance(transform.position, counter.GetWorldPositionWithOffset()) < Settings.MIN_DISTANCE_TO_TARGET;
        }
        else
        {
            return false;
        }

    }
    private void ServeTable()
    {
        GoTo(tableToBeAttended.GetGridPositionWithOffset());
    }
    private bool IsThereCustomer()
    {
        tableToBeAttended = GameGrid.GetTableWithClient();
        return tableToBeAttended != null;
    }
}
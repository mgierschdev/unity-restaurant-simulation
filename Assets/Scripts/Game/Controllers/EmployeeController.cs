using PlasticPipe.PlasticProtocol.Client.Proxies;
using UnityEngine;
using UnityEngine.Rendering;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    GameGridObject tableToBeAttended;
    [SerializeField]
    NPCState state;

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

        if (state == NPCState.AT_COUNTER)
        {
            //Working
        }
    }
    private void UpdateGoNextToCounter()
    {
        if (state != NPCState.WALKING_TO_COUNTER && state != NPCState.AT_COUNTER)
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
        if (state != NPCState.AT_COUNTER && counter != null)
        {
            Debug.Log(Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(counter.ActionGridPosition)) + " < " + Settings.MIN_DISTANCE_TO_TARGET);

            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(counter.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                state = NPCState.AT_COUNTER;
            }
        }
    }
    private void ServeTable()
    {
        GoTo(tableToBeAttended.ActionGridPosition);
    }
    private bool IsThereCustomer()
    {
        tableToBeAttended = GameGrid.GetTableWithClient();
        return tableToBeAttended != null;
    }
}
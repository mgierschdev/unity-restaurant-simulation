using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    private bool busy = false;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;
        Name = transform.name;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        if(!busy){
            GoNextToCounter();
        }
    }

    private bool GoNextToCounter()
    {
        counter = GameGrid.Counter;

        if (counter != null)
        {
            busy = true;
            GoTo(counter.GridPosition - new Vector3Int(2, 2, 0));// arrive one spot infront
            return true;
        }
        return false;
    }
}
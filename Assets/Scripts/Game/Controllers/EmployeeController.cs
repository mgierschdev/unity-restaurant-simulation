using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    private bool busy = false;

    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = NPCState.IDLE;
        Name = transform.name;
        counter = GameGrid.Counter;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        if (!IsMoving() && !busy)
        {
            GoNextToCounter();
        }
    }

    private bool GoNextToCounter()
    {
        counter = GameGrid.Counter;
        if (counter != null)
        {
            busy = true;
            GoTo(counter.GridPosition + new Vector3Int(0, 1, 0));// arrive one spot infront
            return true;
        }
        return false;
    }
}
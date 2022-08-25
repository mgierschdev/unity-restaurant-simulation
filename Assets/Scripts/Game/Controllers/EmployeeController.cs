using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    public string Name { get; set; }
    GameGridObject counter;
    private bool busy = false;

    private void Start()
    {
        Type = ObjectType.NPC;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;
        Name = transform.name;
    }

    private void FixedUpdate()
    {

        body.angularVelocity = 0;
        body.rotation = 0;

        UpdateTargetMovement();
        // Updating position in the Grid
        UpdatePosition();

        UpdateEnergyBar();
    }

    private bool GoToNextToCounter()
    {
        counter = GameGrid.Counter;

        if (counter != null)
        {
            busy = true;
            GoTo(counter.GridPosition + new Vector3Int(2, 2, 0));// arrive one spot infront
            return true;
        }
        return false;
    }
}
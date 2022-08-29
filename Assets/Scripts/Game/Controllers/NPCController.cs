using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activitiy properties
    private bool busy = false;
    GameGridObject table;

    private void Start()
    {
        Type = ObjectType.NPC;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;
        Name = transform.name;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();
        
        //Go and wander if not busy
        if (state == NPCState.WANDER && !busy)
        {
            FindPlace();
            Wander();
        }
    }

    private bool FindPlace()
    {
        table = GameGrid.GetFreeTable();

        if (table != null)
        {
            busy = true;
            GoTo(table.GridPosition + new Vector3Int(0, 1, 0));// arrive one spot infront
            return true;
        }
        return false;
    }
}
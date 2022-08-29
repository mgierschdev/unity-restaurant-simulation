using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activitiy properties
    GameGridObject table;
    [SerializeField]
    private NPCState state;

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        state = NPCState.IDLE;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        //Handle NPC States
        UpdateFindPlace();
        UpdateIsAtTable();

        if (state == NPCState.AT_TABLE)
        {
            //Working
        }

        if(!GameGrid.IsThereFreeTables() && state != NPCState.AT_TABLE && state != NPCState.WALKING_TO_TABLE){
            Wander();
        }
    }

    private void UpdateIsAtTable()
    {
        if (state != NPCState.WANDER && state != NPCState.AT_TABLE && table != null)
        {
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(table.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                state = NPCState.AT_TABLE;
            }
        }
    }

    private void UpdateFindPlace()
    {
        if (state != NPCState.WALKING_TO_TABLE && state != NPCState.AT_TABLE)
        {
            table = GameGrid.GetFreeTable();
            if (table != null)
            {
                state = NPCState.WALKING_TO_TABLE;
                GoTo(table.ActionGridPosition);// arrive one spot infront
            }
        }
    }
}
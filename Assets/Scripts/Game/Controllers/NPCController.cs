using Codice.Client.Common.GameUI;
using UnityEditor;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activitiy properties
    GameGridObject table;
    private NPCState localState;

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        localState = NPCState.IDLE;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        if (!IsWalking())
        {


            //Go and wander if not busy
            if (!IsWalking() && !IsAtTable())
            {
                FindPlace();
            }

            if (IsAtTable())
            {
                Debug.Log("At table " + Name);
            }

            // if (IsAtTable() && state != NPCState.WAITING)
            // {
            //     GameGrid.AddClientToTable(table);
            //     state = NPCState.WAITING;
            // }
        }
    }

    private bool IsAtTable()
    {
        if (table != null)
        {
            return Vector3.Distance(transform.position, table.GetWorldPositionWithOffset()) < Settings.MIN_DISTANCE_TO_TARGET;
        }
        else
        {
            return false;
        }
    }

    private bool FindPlace()
    {
        table = GameGrid.GetFreeTable();

        if (table != null)
        {
            GoTo(table.GridPosition + new Vector3Int(0, 1, 0));// arrive one spot infront
            return true;
        }
        return false;
    }
}
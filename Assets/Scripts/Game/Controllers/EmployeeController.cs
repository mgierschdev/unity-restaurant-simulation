using System.Collections.Generic;
using UnityEngine;

public class EmployeeController : GameObjectMovementBase
{
    GameGridObject counter;
    Vector3Int targetPosition;

    //Test instructions queue
    List<Vector3Int> list = new List<Vector3Int>();
    private int index = 0;


    private void Start()
    {
        Type = ObjectType.EMPLOYEE;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = NPCState.IDLE;
        Name = transform.name;
        targetPosition = Position;

        list.Add(new Vector3Int(26, 32));
        list.Add(new Vector3Int(27, 33));
        list.Add(new Vector3Int(27, 34));
        list.Add(new Vector3Int(26, 34));
        list.Add(new Vector3Int(25, 33));

    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        // if (state == NPCState.IDLE && !InCounterPosition())
        // {
        //     GoNextToCounter();
        //     state = NPCState.BUSY;
        // }

        if (!IsWalking() && IsInFinalTargetPosition())
        {
            //Debug.Log("Going to "+(new Vector3Int(26, 32)));
          //  Debug.Log("Going to "+list[index % list.Count]+" From "+Position);
            GoTo(list[index % list.Count]);
            index++;

        }
    }

    private bool GoNextToCounter()
    {
        counter = GameGrid.Counter;

        if (counter != null)
        {
            state = NPCState.BUSY;
            targetPosition = counter.GridPosition - new Vector3Int(2, 2, 0);
            GoTo(targetPosition);// arrive one spot infront
            return true;
        }

        return false;
    }

    private bool InCounterPosition()
    {
        return Position == counter.GridPosition - new Vector3Int(2, 2, 0);
    }
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    private GameGridObject table;
    private StateMachine stateMachine;
    private bool[] transitionStates;
    private bool tableAvailable;
    private bool tableMoved;
    [SerializeField]
    private const float MAX_STATE_TIME = 10;

    private void Start()
    {
        type = ObjectType.NPC;
        currentState = NpcState.WANDER;
        // MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine();
        transitionStates = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];
        tableAvailable = false;
        tableMoved = false;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();
        UpdateTimeInState();

        try
        {
            // UpdateTableAvailability();
            //Handle NPC States
            // switch (localState)
            // {
            //     case NpcState.WANDER: Wander_0(); break;
            //     case NpcState.IDLE: UpdateFindPlace_1(); break;
            //     case NpcState.WALKING_TO_TABLE: UpdateIsAtTable_2(); break;
            //     case NpcState.AT_TABLE: UpdateWaitToBeAttended_3(); break;
            //     case NpcState.ATTENDED: GoToFinalState_4(); break;
            //     case NpcState.WALKING_UNRESPAWN: UpdateIsAtRespawn_5(); break;
            // }

            // Test statemachine
            UpdateTransitionStates();

            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
        }
    }

    public void UpdateTransitionStates()
    {
        if (IsMoving())
        {
            return;
        }
        else
        {
            CheckIfAtTarget();
        }

        currentState = stateMachine.Current.State;
        Debug.Log("Current state " + currentState + " " + Name);

        transitionStates[0] = tableAvailable; // TABLE_AVAILABLE = 0,
        transitionStates[1] = tableMoved; // TABLE_MOVED = 1,
        transitionStates[2] = Unrespawn(); // WALK_TO_UNRESPAWN = 2,
        transitionStates[3] = false; // WAITING_AT_TABLE_TIME = 3,
        transitionStates[4] = false; // IDLE_TIME = 4,
        transitionStates[5] = false; // ORDER_SERVED = 5,
        transitionStates[6] = false; // ORDER_FINISHED = 6,
        transitionStates[7] = false; // ENERGY_BAR_VALUE = 7,
        transitionStates[8] = false; // COUNTER_MOVED = 8,
        transitionStates[9] = Wander(); // WANDER = 9,
        transitionStates[10] = !IsMoving(); // NPC_IS_NOT_MOVING = 10,
        transitionStates[11] = false; // ATTENDED = 11,
        transitionStates[12] = false; // BEING_ATTENDED = 12,
        transitionStates[13] = false; // STATE_TIME = 13
        transitionStates[14] = IsMoving(); // NPC_IS_MOVING = 14

        stateMachine.CheckTransition(transitionStates);
    }

    private bool Unrespawn()
    {
        if (stateTime < MAX_STATE_TIME || currentState == NpcState.WALKING_UNRESPAWN)
        {
            return false;
        }
        GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        return true;
    }

    private bool Wander()
    {
        float randT = Random.Range(0, 1000);
        if (currentState != NpcState.IDLE || randT > 2)
        {
            return false;
        }
        GoTo(GetRandomWalkablePosition());
        return true;
    }

    private void CheckIfAtTarget()
    {
        if (!Util.IsAtDistanceWithObjectTraslate(transform.position, currentTargetWorldPosition, transform))
        {
            return;
        }

    }




    // public void UpdateTableAvailability()
    // {
    //     if (currentState != NpcState.WALKING_TO_TABLE || currentState != NpcState.AT_TABLE)
    //     {
    //         return;
    //     }

    //     if (table == null || table.GetIsObjectSelected())
    //     {
    //         GoToFinalState_4();
    //     }
    // }

    private void UpdateTimeInState()
    {
        // keeps the time in the current state
        if (prevState == currentState)
        {
            //Log("Current state time "+stateTime);
            stateTime += Time.fixedDeltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = currentState;
        }
    }

    // private void UpdateFindPlace_1()
    // {
    //     if (timeWandering < MIN_TIME_TO_FIND_TABLE)
    //     {
    //         currentState = NpcState.WANDER;
    //         return;
    //     }

    //     if (BussGrid.GetFreeTable(out table))
    //     {
    //         timeWandering = 0;
    //         currentState = NpcState.WALKING_TO_TABLE;
    //         table.SetHashNPCAssigned(true);
    //         GoToWalkingToTable_6();
    //         return;
    //     }
    //     // we go wandering and retry after certain amount of time
    //     currentState = NpcState.WANDER;
    // }

    // private void UpdateIsAtTable_2()
    // {
    //     if (Util.IsAtDistanceWithObjectTraslate(transform.position, targetInWorldPosition, transform))
    //     {
    //         currentState = NpcState.AT_TABLE;
    //         // The table was stored at the same time the NPC was moving towards the table
    //         if (table == null || PlayerData.IsItemStored(table.Name))
    //         {
    //             GoToFinalState_4();
    //             return;
    //         }

    //         table.SetUsedBy(this);
    //     }
    // }

    // private void UpdateWaitToBeAttended_3()
    // {
    //     currentState = NpcState.WAITING_TO_BE_ATTENDED;
    // }

    // private void GoToFinalState_4()
    // {

    //     if (table == null || (currentState == NpcState.BEING_ATTENDED && stateTime < MAX_TABLE_WAITING_TIME))
    //     {
    //         return;
    //     }

    //     if (table != null)
    //     {
    //         table.FreeObject();
    //         table = null;
    //     }

    //     currentState = NpcState.WALKING_UNRESPAWN;
    //     ResetMovement();
    //     unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
    //     target = unRespawnTile.GridPosition;
    //     if (!GoTo(target))
    //     {
    //         //Log("Could not find a path GoToFinalState_4() ");
    //         return;
    //     }
    // }

    // public void GoToFinalState()
    // {
    //     currentState = NpcState.WALKING_UNRESPAWN;
    //     table = null;
    //     ResetMovement();
    //     unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
    //     target = unRespawnTile.GridPosition;
    //     if (!GoTo(target))
    //     {
    //         //Log("Could not find a path GoToFinalState ");
    //         return;
    //     }
    // }

    // private void UpdateIsAtRespawn_5()
    // {
    //     if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
    //     {
    //         gameController.RemoveNpc(this);
    //         Destroy(gameObject);
    //     }

    //     if (!IsMoving())
    //     {
    //         GoToFinalState_4();
    //     }
    // }

    //Calculates the path to the current table
    // private void GoToWalkingToTable_6()
    // {
    //     try
    //     {
    //         targetInWorldPosition = table.GetActionTile();

    //         target = table.GetActionTileInGridPosition();

    //         //If we are already at the table
    //         if (target == Position)
    //         {
    //             currentState = NpcState.AT_TABLE;
    //             return;
    //         }

    //         if (!GoTo(target))
    //         {
    //             //Log("Could not find a path GoToWalkingToTable_6() ");
    //             return;
    //         }
    //     }
    //     catch (SystemException e)
    //     {
    //         GameLog.Log(e.ToString());
    //     }
    // }

    // public void RecalculateGoTo()
    // {
    //     if (currentState == NpcState.WALKING_TO_TABLE)
    //     {
    //         GoToWalkingToTable_6();
    //     }
    //     else
    //     {
    //         if (!GoTo(target))
    //         {
    //             //Log("Could not find a path RecalculateGoTo() ");
    //             return;
    //         }
    //     }
    // }

    // private void Wander_0()
    // {
    //     timeWandering += Time.fixedDeltaTime;

    //     if (IsMoving())
    //     {
    //         return;
    //     }

    //     // we could add more random by deciding to move or not 
    //     idleTime += Time.fixedDeltaTime;

    //     if (idleTime < randMax)
    //     {
    //         currentState = NpcState.IDLE;
    //         return;
    //     }

    //     currentState = NpcState.WANDER;
    //     idleTime = 0;
    //     randMax = Random.Range(0, IDLE_MAX_TIME);
    //     target = GetRandomWalkablePosition();

    //     if (!GoTo(target))
    //     {
    //         currentState = NpcState.IDLE;
    //         return;
    //     }
    // }


    private Vector3Int GetRandomWalkablePosition()
    {
        Vector3Int wanderPos = Position;
        // There is a small chance that the NPC will go to the same place 
        while (wanderPos == Position)
        {
            wanderPos = BussGrid.GetRandomWalkableGridPosition();
        }
        return wanderPos;
    }

    public NpcState GetNpcState()
    {
        return currentState;
    }

    public float GetNpcStateTime()
    {
        return Mathf.Floor(stateTime);
    }

    public GameGridObject GetTable()
    {
        return table;
    }

    public void SetTable(GameGridObject obj)
    {
        table = obj;
    }

    public void FlipTowards(Vector3Int direction)
    {
        StandTowards(direction);
    }

    public void SetAttended()
    {
        currentState = NpcState.ATTENDED;
    }

    public void SetBeingAttended()
    {
        currentState = NpcState.BEING_ATTENDED;
    }

    public bool HasTable()
    {
        return table != null;
    }

    public void SetTableMoved()
    {
        tableMoved = true;
    }
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    private const float IDLE_MAX_TIME = 3f; //in seconds
    private const float MAX_TABLE_WAITING_TIME = 20f;
    private float MIN_TIME_TO_FIND_TABLE, randMax = 3f;// Defined as random 
    //Doing a different activity properties
    private GameGridObject table;
    private GameTile unRespawnTile;
    // Wander properties
    private Vector3Int target; // walking to target
    private Vector3 targetInWorldPosition;
    private float timeWandering;
    private StateMachine stateMachine;


    //StateMachine transition attribures
    private float smTimWandering;
    private bool smTableAvailable;
    private bool smTableMoved;
    [SerializeField]
    private NpcState smState;
    // TABLE_AVAILABLE = 0,
    // TABLE_MOVED = 1,
    // WANDER_TIME = 2,
    // WAITING_AT_TABLE_TIME = 3,
    // IDLE_TIME = 4,
    // ORDER_SERVED = 5,
    // ORDER_FINISHED = 6,
    // ENERGY_BAR_VALUE = 7,
    // COUNTER_MOVED = 8,
    // WANDER = 9,
    // NPC_IS_NOT_MOVING = 10,
    // ATTENDED = 11,
    // BEING_ATTENDED = 12


    private void Start()
    {
        timeWandering = 0;
        type = ObjectType.NPC;
        localState = NpcState.WANDER;
        MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine();

        //StateMachine transition attribures
        smTimWandering = 0f;
        smTableAvailable = false;
        smTableMoved = false;
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();
        UpdateTimeInState();

        try
        {
            //UpdateTableAvailability();
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
        smState = stateMachine.Current.State;

        if (stateMachine.Current.State == NpcState.WANDER)
        {
            smTimWandering += Time.fixedDeltaTime;
        }
    }

    public void UpdateTableAvailability()
    {
        if (localState != NpcState.WALKING_TO_TABLE || localState != NpcState.AT_TABLE)
        {
            return;
        }

        if (table == null || table.GetIsObjectSelected())
        {
            GoToFinalState_4();
        }
    }

    private void UpdateTimeInState()
    {
        // keeps the time in the current state
        if (prevState == localState)
        {
            //Log("Current state time "+stateTime);
            stateTime += Time.fixedDeltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = localState;
        }

        if (stateTime > MAX_TABLE_WAITING_TIME)
        {
            GoToFinalState_4();
        }
    }

    private void UpdateFindPlace_1()
    {
        if (timeWandering < MIN_TIME_TO_FIND_TABLE)
        {
            localState = NpcState.WANDER;
            return;
        }

        if (BussGrid.GetFreeTable(out table))
        {
            timeWandering = 0;
            localState = NpcState.WALKING_TO_TABLE;
            table.SetHashNPCAssigned(true);
            GoToWalkingToTable_6();
            return;
        }
        // we go wandering and retry after certain amount of time
        localState = NpcState.WANDER;
    }

    private void UpdateIsAtTable_2()
    {
        if (Util.IsAtDistanceWithObjectTraslate(transform.position, targetInWorldPosition, transform))
        {
            localState = NpcState.AT_TABLE;
            // The table was stored at the same time the NPC was moving towards the table
            if (table == null || PlayerData.IsItemStored(table.Name))
            {
                GoToFinalState_4();
                return;
            }

            table.SetUsedBy(this);
        }
    }

    private void UpdateWaitToBeAttended_3()
    {
        localState = NpcState.WAITING_TO_BE_ATTENDED;
    }

    private void GoToFinalState_4()
    {

        if (table == null || (localState == NpcState.BEING_ATTENDED && stateTime < MAX_TABLE_WAITING_TIME))
        {
            return;
        }

        if (table != null)
        {
            table.FreeObject();
            table = null;
        }

        localState = NpcState.WALKING_UNRESPAWN;
        ResetMovement();
        unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            //Log("Could not find a path GoToFinalState_4() ");
            return;
        }
    }

    public void GoToFinalState()
    {
        localState = NpcState.WALKING_UNRESPAWN;
        table = null;
        ResetMovement();
        unRespawnTile = BussGrid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            //Log("Could not find a path GoToFinalState ");
            return;
        }
    }

    private void UpdateIsAtRespawn_5()
    {
        if (Util.IsAtDistanceWithObject(transform.position, BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveNpc(this);
            Destroy(gameObject);
        }

        if (!IsMoving())
        {
            GoToFinalState_4();
        }
    }

    //Calculates the path to the current table
    private void GoToWalkingToTable_6()
    {
        try
        {
            targetInWorldPosition = table.GetActionTile();

            target = table.GetActionTileInGridPosition();

            //If we are already at the table
            if (target == Position)
            {
                localState = NpcState.AT_TABLE;
                return;
            }

            if (!GoTo(target))
            {
                //Log("Could not find a path GoToWalkingToTable_6() ");
                return;
            }
        }
        catch (SystemException e)
        {
            GameLog.Log(e.ToString());
        }
    }

    public void RecalculateGoTo()
    {
        if (localState == NpcState.WALKING_TO_TABLE)
        {
            GoToWalkingToTable_6();
        }
        else
        {
            if (!GoTo(target))
            {
                //Log("Could not find a path RecalculateGoTo() ");
                return;
            }
        }
    }

    private void Wander_0()
    {
        timeWandering += Time.fixedDeltaTime;

        if (IsMoving())
        {
            return;
        }

        // we could add more random by deciding to move or not 
        idleTime += Time.fixedDeltaTime;

        if (idleTime < randMax)
        {
            localState = NpcState.IDLE;
            return;
        }

        localState = NpcState.WANDER;
        idleTime = 0;
        randMax = Random.Range(0, IDLE_MAX_TIME);
        target = GetRandomWalkablePosition();

        if (!GoTo(target))
        {
            localState = NpcState.IDLE;
            return;
        }
    }

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
        return localState;
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
        localState = NpcState.ATTENDED;
    }

    public void SetBeingAttended()
    {
        localState = NpcState.BEING_ATTENDED;
    }

    public bool HasTable()
    {
        return table != null;
    }
}
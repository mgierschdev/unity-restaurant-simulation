using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activity properties
    private GameGridObject table;
    private GameController gameController;
    [SerializeField]
    private NpcState localState;
    private GameTile unRespawnTile;
    private PlayerAnimationStateController animationController;
    // Wander properties
    private float idleTime;
    [SerializeField]
    private float timeWandering;
    private const float IDLE_MAX_TIME = 3f; //in seconds
    private const float MAX_TABLE_WAITING_TIME = 20f;
    private float MIN_TIME_TO_FIND_TABLE;// Defined as random 
    private float randMax = 3f;
    private Vector3Int target; // walking to target
    private Vector3 targetInWorldPosition;
    private bool IsNPCVisible;
    //Time in the current state
    private float stateTime;
    private NpcState prevState;
    [SerializeField]
    public Vector2 TmpVelocity;

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        localState = NpcState.WANDER;
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObj.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();
        stateTime = 0;
        timeWandering = 0;
        prevState = localState;
        MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);

        if (gameController == null)
        {
            GameLog.LogWarning("NPCController/GameController null");
        }

        if (animationController == null)
        {
            GameLog.LogWarning("NPCController/animationController null");
        }
    }

    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
        UpdateEnergyBar();

        try
        {
            //Handle NPC States
            if (localState == NpcState.WANDER)
            {
                Wander_0();
            }
            else if (localState == NpcState.IDLE)
            {
                UpdateFindPlace_1();
            }
            else if (localState == NpcState.WALKING_TO_TABLE)
            {
                UpdateIsAtTable_2();
            }
            else if (localState == NpcState.AT_TABLE)
            {
                UpdateWaitToBeAttended_3();
            }
            else if (localState == NpcState.ATTENDED)
            {
                GoToFinalState_4();
            }
            else if (localState == NpcState.WALKING_UNRESPAWN)
            {
                UpdateIsAtRespawn_5();
            }
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController):  " + e);
        }
        // Intended to go at the end
        UpdateTimeInState();
        UpdateAnimation();
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

    private void UpdateAnimation()
    {
        // TODO: for performance reasons only animate inside camera CLAMP --> animationController.SetState(NpcState.IDLE);
        // Animates depending on the current state
        if (IsMoving())
        {
            animationController.SetState(NpcState.WALKING);
        }
        else
        {
            animationController.SetState(localState);
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
            ///sLog("Could not find a path Wander_0 ");
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
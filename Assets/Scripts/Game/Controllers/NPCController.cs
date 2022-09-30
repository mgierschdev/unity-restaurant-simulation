using UnityEngine;

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
    private const float IDLE_MAX_TIME = 6f; //in seconds
    private float randMax = 3f;
    private Vector3Int target; // walking to target
    private Vector3 targetInWorldPosition;
    private bool IsNPCVisible;
    //Time in the current state
    private float stateTime; //TODO: to be used in order for the NPC to leave after certain time
    private NpcState prevState;
    private const float MAX_TABLE_WAITING_TIME = 10f;

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        localState = NpcState.IDLE_0;
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObj.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();
        stateTime = 0;
        prevState = localState;

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
        //Handle NPC States
        UpdateFindPlace();
        UpdateIsAtTable();
        UpdateWaitToBeAttended();
        UpdateTableAttended();
        UpdateIsAtRespawn();

        if (!Grid.IsThereFreeTables() && localState != NpcState.AT_TABLE_2 &&
            localState != NpcState.WALKING_TO_TABLE_1 && localState != NpcState.WAITING_TO_BE_ATTENDED_7 &&
            localState != NpcState.WALKING_UNRESPAWN_8)
        {
            Wander();
        }

        // keeps the time in the current state
        if (prevState == localState)
        {
            //GameLog.Log("Current state time "+stateTime);
            stateTime += Time.deltaTime;
        }
        else
        {
            stateTime = 0;
            prevState = localState;
        }

        // TODO: only change inside camera CLAMP --> animationController.SetState(NpcState.IDLE);
        animationController.SetState(localState);
    }

    private void UpdateIsAtRespawn()
    {
        if (localState == NpcState.WALKING_UNRESPAWN_8)
        {
            if (Util.IsAtDistanceWithObject(transform.position, Grid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
            {
                gameController.RemoveNpc(this);
                Destroy(gameObject);
            }
        }
    }

    // Setted after the client has been attended 
    private void UpdateTableAttended()
    {
        if ((localState == NpcState.WAITING_TO_BE_ATTENDED_7) && !table.GetBusy())
        {
            Debug.Log("Waiting to be attended, go to final state " + transform.name);
            GoToFinalState();
        }
    }

    private void UpdateWaitToBeAttended()
    {
        if (localState == NpcState.AT_TABLE_2)
        {
            Grid.AddClientToTable(table);
            localState = NpcState.WAITING_TO_BE_ATTENDED_7;
        }
    }

    private void UpdateIsAtTable()
    {
        if (localState != NpcState.WALKING_TO_TABLE_1)
        {
            return;
        }

        if (Util.IsAtDistanceWithObjectTraslate(transform.position, targetInWorldPosition, transform))
        {
            localState = NpcState.AT_TABLE_2;
        }
    }

    private void UpdateFindPlace()
    {
        if ((localState != NpcState.WALKING_WANDER_5 && localState != NpcState.IDLE_0) || !Grid.IsThereFreeTables())
        {
            return;
        }

        table = Grid.GetFreeTable();
        table.SetUsed(this);
        table.SetUsedBy(this);
        localState = NpcState.WALKING_TO_TABLE_1;
        GoToWalkingToTable();
    }

    private void GoToWalkingToTable()
    {
        targetInWorldPosition = table.GetActionTile();
        target = Grid.GetPathFindingGridFromWorldPosition(targetInWorldPosition);

        if (!GoTo(target))
        {
            if (table != null)
            {
                Debug.Log("Resetting NPC state (GoToWalkingToTable " + transform.name);
                table.SetUsed(null);
                Grid.AddFreeBusinessSpots(table);
                table = null;
                GoToFinalState();
            }
        }
    }

    public void RecalculateGoTo()
    {
        if (localState == NpcState.WALKING_TO_TABLE_1)
        {
            GoToWalkingToTable();
        }
        else
        {
            GoTo(target);
        }
    }

    public void GoToFinalState()
    {
        Debug.Log("GoToFinalState " + transform.name);
        table = null;
        localState = NpcState.WALKING_UNRESPAWN_8;
        unRespawnTile = Grid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        GoTo(target);
    }

    private void Wander()
    {
        if (IsMoving())
        {
            return;
        }

        // we could add more random by deciding to move or not 
        idleTime += Time.deltaTime;
        localState = NpcState.IDLE_0;

        if (idleTime < randMax)
        {
            return;
        }

        localState = NpcState.WALKING_WANDER_5;
        idleTime = 0;
        randMax = Random.Range(0, IDLE_MAX_TIME);
        target = GetRandomWalkablePosition();
        GoTo(target);
    }

    private Vector3Int GetRandomWalkablePosition()
    {
        Vector3Int wanderPos = Position;
        // There is a small chance that the NPC will go to the same place 
        while (wanderPos == Position)
        {
            wanderPos = Grid.GetRandomWalkableGridPosition();
        }
        return wanderPos;
    }

    public NpcState GetNpcState()
    {
        return localState;
    }

    public GameGridObject GetTable()
    {
        return table;
    }
}
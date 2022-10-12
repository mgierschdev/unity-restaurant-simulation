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
    private const float IDLE_MAX_TIME = 3f; //in seconds
    private const float MAX_TABLE_WAITING_TIME = 10f;
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
        else if (localState == NpcState.WAITING_TO_BE_ATTENDED)
        {
            GoToFinalState_4();
        }
        else if (localState == NpcState.WALKING_UNRESPAWN)
        {
            UpdateIsAtRespawn_5();
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
            //GameLog.Log("Current state time "+stateTime);
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
        if (!Grid.IsThereFreeTables())
        {
            localState = NpcState.WANDER;
            return;
        }

        table = Grid.GetFreeTable();
        table.SetUsed(this);
        table.SetUsedBy(this);
        localState = NpcState.WALKING_TO_TABLE;
        GoToWalkingToTable_6();
    }

    private void UpdateIsAtTable_2()
    {
        if (Util.IsAtDistanceWithObjectTraslate(transform.position, targetInWorldPosition, transform))
        {
            localState = NpcState.AT_TABLE;
        }
    }

    private void UpdateWaitToBeAttended_3()
    {
        Grid.AddClientToTable(table);
        localState = NpcState.WAITING_TO_BE_ATTENDED;
    }

    public void GoToFinalState_4()
    {

        if (table == null || table.GetBusy())
        {
            return;
        }

        if (table != null)
        {
            table.FreeObject();
            table = null;
        }

        ResetMovement();
        localState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = Grid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            GameLog.Log("Could not find a path GoToFinalState_4() ");
            return;
        }
    }

    public void GoToFinalState()
    {
        table = null;
        ResetMovement();
        localState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = Grid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            GameLog.Log("Could not find a path GoToFinalState ");
            return;
        }
    }

    private void UpdateIsAtRespawn_5()
    {
        if (!IsMoving())
        {
            GoToFinalState_4();
        }

        if (Util.IsAtDistanceWithObject(transform.position, Grid.GetWorldFromPathFindingGridPositionWithOffSet(unRespawnTile.GridPosition)))
        {
            gameController.RemoveNpc(this);
            Destroy(gameObject);
        }
    }

    //Calculates the path to the current table
    private void GoToWalkingToTable_6()
    {
        targetInWorldPosition = table.GetActionTile();
        target = Grid.GetPathFindingGridFromWorldPosition(targetInWorldPosition);

        //If we are already at the table
        if (target == Position)
        {
            localState = NpcState.AT_TABLE;
            return;
        }

        if (!GoTo(target))
        {
            GameLog.Log("Could not find a path GoToWalkingToTable_6() ");
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
                GameLog.Log("Could not find a path RecalculateGoTo() ");
                return;
            }
        }
    }

    private void Wander_0()
    {
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
            GameLog.Log("Could not find a path Wander_0 ");
            localState = NpcState.IDLE;
            return;
        }
        else
        {
            //localState = NpcState.WALKING_WANDER;
        }
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
}
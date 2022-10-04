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
        UpdateTimeInState();

        //Handle NPC States
        if ((localState == NpcState.IDLE || localState == NpcState.WANDER) && !Grid.IsThereFreeTables())
        {
            Wander_0();
        }
        else if (localState == NpcState.WANDER && Grid.IsThereFreeTables())
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
        else if (localState == NpcState.WAITING_TO_BE_ATTENDED && table != null && !table.GetBusy())
        {
            GoToFinalState_4();
        }
        else if (localState == NpcState.WALKING_UNRESPAWN)
        {
            UpdateIsAtRespawn_5();
        }

        // Goes at the end
        UpdateAnimation();
    }

    private void UpdateTimeInState()
    {
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

        if(stateTime > MAX_TABLE_WAITING_TIME){
            GoToFinalState_4();
        }
    }

    private void UpdateAnimation()
    {
        // TODO: only change inside camera CLAMP --> animationController.SetState(NpcState.IDLE);
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
        table.FreeObject();
        table = null;
        localState = NpcState.WALKING_UNRESPAWN;
        unRespawnTile = Grid.GetRandomSpamPointWorldPosition();
        target = unRespawnTile.GridPosition;
        if (!GoTo(target))
        {
            return;
        }
    }

    private void UpdateIsAtRespawn_5()
    {
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
        idleTime += Time.deltaTime;
        localState = NpcState.IDLE;

        if (idleTime < randMax)
        {
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
}
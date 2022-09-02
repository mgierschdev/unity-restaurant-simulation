using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activitiy properties
    GameGridObject table;
    GameController gameController;

    [SerializeField]
    private NPCState LocalState;
    private GameTile unRespawnTile;
    private PlayerAnimationStateController animationController;

    // Wander properties
    private float idleTime = 0;
    private float idleMaxTime = 6f; //in seconds
    private float randMax = 3f;
    private Vector3Int target; // walking to target

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        LocalState = NPCState.IDLE;
        GameObject gameObj = GameObject.Find(Settings.CONST_PARENT_GAME_OBJECT);
        gameController = gameObj.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();

        if (gameController == null)
        {
            Debug.LogWarning("NPCController/GameController null");
        }

        if (animationController == null)
        {
            Debug.LogWarning("NPCController/animationController null");
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

        // if the table moved
        if (table != null && !table.IsLastPositionEqual(target))
        {
            GameGrid.AddFreeBusinessSpots(table);
            GoToFinalState();
        }

        if (!GameGrid.IsThereFreeTables() && LocalState != NPCState.AT_TABLE && LocalState != NPCState.WALKING_TO_TABLE && LocalState != NPCState.WAITING_TO_BE_ATTENDED && LocalState != NPCState.WALKING_UNRESPAWN)
        {
            Wander();
        }

        animationController.SetState(LocalState);
    }

    private void UpdateIsAtRespawn()
    {
        if (LocalState == NPCState.WALKING_UNRESPAWN)
        {
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(unRespawnTile.GridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                gameController.RemoveNPC(this);
                Destroy(gameObject);
            }
        }
    }

    private void UpdateTableAttended()
    {
        if (LocalState == NPCState.WAITING_TO_BE_ATTENDED && !table.Busy)
        {
            GoToFinalState();
        }
    }

    private void UpdateWaitToBeAttended()
    {
        if (LocalState == NPCState.AT_TABLE)
        {
            GameGrid.AddClientToTable(table);
            LocalState = NPCState.WAITING_TO_BE_ATTENDED;
        }
    }

    private void UpdateIsAtTable()
    {
        if (LocalState == NPCState.WALKING_TO_TABLE)
        {
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(table.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                LocalState = NPCState.AT_TABLE;
            }
        }
    }

    private void UpdateFindPlace()
    {
        if ((LocalState == NPCState.WALKING_WANDER || LocalState == NPCState.IDLE) && GameGrid.IsThereFreeTables())
        {
            table = GameGrid.GetFreeTable();
            table.Busy = true;
            LocalState = NPCState.WALKING_TO_TABLE;
            target = table.ActionGridPosition;
            GoTo(table.ActionGridPosition);
        }
    }

    private void GoToFinalState()
    {
        table = null;
        LocalState = NPCState.WALKING_UNRESPAWN;
        unRespawnTile = GameGrid.GetRandomSpamPointWorldPosition();
        GoTo(unRespawnTile.GridPosition);
    }

    protected void Wander()
    {
        if (!IsMoving())
        {
            // we could add more random by deciding to move or not 
            idleTime += Time.deltaTime;
            LocalState = NPCState.IDLE;
        }

        if (!IsMoving() && idleTime >= randMax)
        {
            LocalState = NPCState.WALKING_WANDER;
            idleTime = 0;
            randMax = Random.Range(0, idleMaxTime);
            Vector3Int wanderPos = GameGrid.GetRandomWalkableGridPosition();
            GoTo(wanderPos);
        }
    }
}
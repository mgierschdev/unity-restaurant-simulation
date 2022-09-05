using UnityEngine;
using UnityEngine.Serialization;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activity properties
    GameGridObject table;
    GameController gameController;

    [FormerlySerializedAs("LocalState")] [SerializeField]
    private NPCState localState;

    private GameTile unRespawnTile;
    private PlayerAnimationStateController animationController;

    // Wander properties
    private float idleTime = 0;
    private const float IDLE_MAX_TIME = 6f; //in seconds
    private float randMax = 3f;
    private Vector3Int target; // walking to target

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        localState = NPCState.IDLE;
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

        if (!GameGrid.IsThereFreeTables() && localState != NPCState.AT_TABLE &&
            localState != NPCState.WALKING_TO_TABLE && localState != NPCState.WAITING_TO_BE_ATTENDED &&
            localState != NPCState.WALKING_UNRESPAWN)
        {
            Wander();
        }

        animationController.SetState(localState);
    }

    private void UpdateIsAtRespawn()
    {
        if (localState == NPCState.WALKING_UNRESPAWN)
        {
            if (Vector3.Distance(transform.position,
                    GameGrid.GetWorldFromPathFindingGridPosition(unRespawnTile.GridPosition)) <
                Settings.MIN_DISTANCE_TO_TARGET)
            {
                gameController.RemoveNpc(this);
                Destroy(gameObject);
            }
        }
    }

    private void UpdateTableAttended()
    {
        if (localState == NPCState.WAITING_TO_BE_ATTENDED && !table.Busy)
        {
            GoToFinalState();
        }
    }

    private void UpdateWaitToBeAttended()
    {
        if (localState == NPCState.AT_TABLE)
        {
            GameGrid.AddClientToTable(table);
            localState = NPCState.WAITING_TO_BE_ATTENDED;
        }
    }

    private void UpdateIsAtTable()
    {
        if (localState != NPCState.WALKING_TO_TABLE)
        {
            return;
        }

        if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(table.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
        {
            localState = NPCState.AT_TABLE;
        }
    }

    private void UpdateFindPlace()
    {
        if ((localState != NPCState.WALKING_WANDER && localState != NPCState.IDLE) || !GameGrid.IsThereFreeTables())
        {
            return;
        }

        table = GameGrid.GetFreeTable();
        table.Busy = true;
        localState = NPCState.WALKING_TO_TABLE;
        target = table.ActionGridPosition;
        GoTo(table.ActionGridPosition);
    }

    private void GoToFinalState()
    {
        table = null;
        localState = NPCState.WALKING_UNRESPAWN;
        unRespawnTile = GameGrid.GetRandomSpamPointWorldPosition();
        GoTo(unRespawnTile.GridPosition);
    }

    private void Wander()
    {
        if (IsMoving())
        {
            return;
        }

        // we could add more random by deciding to move or not 
        idleTime += Time.deltaTime;
        localState = NPCState.IDLE;

        if (idleTime < randMax)
        {
            return;
        }

        localState = NPCState.WALKING_WANDER;
        idleTime = 0;
        randMax = Random.Range(0, IDLE_MAX_TIME);
        Vector3Int wanderPos = GameGrid.GetRandomWalkableGridPosition();
        GoTo(wanderPos);
    }
}
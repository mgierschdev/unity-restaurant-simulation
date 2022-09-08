using UnityEngine;
using UnityEngine.Serialization;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activity properties
    GameGridObject table;
    GameController gameController;
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

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        localState = NpcState.IDLE;
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        gameController = gameObj.GetComponent<GameController>();
        animationController = GetComponent<PlayerAnimationStateController>();

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

        // if the table moved
        if (table != null && !table.IsLastPositionEqual(targetInWorldPosition))
        {
            // GameGrid.AddFreeBusinessSpots(table);
            GoToFinalState();
        }

        if (!GameGrid.IsThereFreeTables() && localState != NpcState.AT_TABLE &&
            localState != NpcState.WALKING_TO_TABLE && localState != NpcState.WAITING_TO_BE_ATTENDED &&
            localState != NpcState.WALKING_UNRESPAWN)
        {
            Wander();
        }

        animationController.SetState(localState);
    }

    private void UpdateIsAtRespawn()
    {
        if (localState == NpcState.WALKING_UNRESPAWN)
        {
            if (Util.IsAtDistanceWithObject(transform.position,
                    GameGrid.GetWorldFromPathFindingGridPosition(unRespawnTile.GridPosition)))
            {
                gameController.RemoveNpc(this);
                Destroy(gameObject);
            }
        }
    }

    private void UpdateTableAttended()
    {
        if (localState == NpcState.WAITING_TO_BE_ATTENDED && !table.Busy)
        {
            GoToFinalState();
        }
    }

    private void UpdateWaitToBeAttended()
    {
        if (localState == NpcState.AT_TABLE)
        {
            GameGrid.AddClientToTable(table);
            localState = NpcState.WAITING_TO_BE_ATTENDED;
        }
    }

    private void UpdateIsAtTable()
    {
        if (localState != NpcState.WALKING_TO_TABLE)
        {
            return;
        }

        if (Util.IsAtDistanceWithObject(transform.position, targetInWorldPosition))
        {
            localState = NpcState.AT_TABLE;
        }
    }

    private void UpdateFindPlace()
    {
        if ((localState != NpcState.WALKING_WANDER && localState != NpcState.IDLE) || !GameGrid.IsThereFreeTables())
        {
            return;
        }

        table = GameGrid.GetFreeTable();
        table.SetUsed(this);
        table.UsedBy = this;
        localState = NpcState.WALKING_TO_TABLE;
        targetInWorldPosition = table.GetFirstActionTile();
        target = GameGrid.GetPathFindingGridFromWorldPosition(targetInWorldPosition);
        GoTo(target);
    }

    public void GoToFinalState()
    {
        table = null;
        localState = NpcState.WALKING_UNRESPAWN;
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
        localState = NpcState.IDLE;

        if (idleTime < randMax)
        {
            return;
        }

        localState = NpcState.WALKING_WANDER;
        idleTime = 0;
        randMax = Random.Range(0, IDLE_MAX_TIME);
        Vector3Int wanderPos = GameGrid.GetRandomWalkableGridPosition();
        GoTo(wanderPos);
    }
}
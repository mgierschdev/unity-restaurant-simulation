using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    //Doing a different activitiy properties
    GameGridObject table;
    GameController gameController;

    [SerializeField]
    private NPCState state;
    private GameTile unRespawnTile;
    private PlayerAnimationStateController animationController;

    private void Start()
    {
        Type = ObjectType.NPC;
        Name = transform.name;
        state = NPCState.IDLE;
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

        if (!GameGrid.IsThereFreeTables() && state != NPCState.AT_TABLE && state != NPCState.WALKING_TO_TABLE && state != NPCState.WAITING_TO_BE_ATTENDED && state != NPCState.WALKING_UNRESPAWN)
        {
            state = NPCState.WANDER;
            Wander();
        }

        animationController.SetState(state);
    }

    private void UpdateIsAtRespawn()
    {
        if (state == NPCState.WALKING_UNRESPAWN)
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
        if (state == NPCState.WAITING_TO_BE_ATTENDED && !table.Busy)
        {
            table = null;
            state = NPCState.WALKING_UNRESPAWN;
            unRespawnTile = GameGrid.GetRandomSpamPointWorldPosition();
            GoTo(unRespawnTile.GridPosition);
        }
    }

    private void UpdateWaitToBeAttended()
    {
        if (state == NPCState.AT_TABLE)
        {
            GameGrid.AddClientToTable(table);
            state = NPCState.WAITING_TO_BE_ATTENDED;
        }
    }

    private void UpdateIsAtTable()
    {
        if (state == NPCState.WALKING_TO_TABLE)
        {
            if (Vector3.Distance(transform.position, GameGrid.GetWorldFromPathFindingGridPosition(table.ActionGridPosition)) < Settings.MIN_DISTANCE_TO_TARGET)
            {
                state = NPCState.AT_TABLE;
            }
        }
    }

    private void UpdateFindPlace()
    {
        if ((state == NPCState.WANDER || state == NPCState.IDLE) && GameGrid.IsThereFreeTables())
        {
            table = GameGrid.GetFreeTable();
            table.Busy = true;
            state = NPCState.WALKING_TO_TABLE;
            GoTo(table.ActionGridPosition);
        }
    }
}
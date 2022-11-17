using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
    [SerializeField]
    private const float MaxStateTime = 120; // 2min
    [SerializeField]
    private NpcState state;//TODO: for debug

    private void Start()
    {
        type = ObjectType.NPC;
        SetID();
        //TODO: MIN_TIME_TO_FIND_TABLE = Random.Range(0f, 10f);
        stateMachine = NPCStateMachineFactory.GetClientStateMachine(Name);
    }

    private void IsInsideCamera()
    {
        Camera camera = Camera.main;
        Rect rect = camera.rect;
        Vector3Int cameraGridPosition = BussGrid.GetLocalGridFromWorldPosition(camera.transform.position);
        Debug.Log("Main camera rect " + camera.pixelRect);
        Debug.Log("Main aspect " + camera.aspect);
        Debug.Log("Main projectionMatrix " + camera.projectionMatrix);
        Debug.Log("Camera position " + cameraGridPosition);
        //BussGrid.DrawCell(cameraGridPosition);


        // Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit))
        //     print("I'm looking at " + hit.transform.name);
        // else
        //     print("I'm looking at nothing!");
        // choose the margin randomly
        //float margin = Random.Range(0.0f, 0.3f);
        // setup the rectangle
        //camera.rect = new Rect(margin, 0.0f, 1.0f - margin * 2.0f, 1.0f);
    }

    private void FixedUpdate()
    {
        IsInsideCamera();

        try
        {
            UpdatePosition();
            UpdateTimeInState();
            UpdateTargetMovement();

            UpdateTransitionStates();
            UpdateAnimation();
        }
        catch (Exception e)
        {
            GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
            stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
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

        CheckUnrespawn();
        CheckIfTableHasBeenAssigned();
        Wander();
        CheckIfTableMoved();

        state = stateMachine.Current.State;
        stateMachine.CheckTransition();
        MoveNPC();
    }

    private void MoveNPC()
    {
        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN && !stateMachine.GetTransitionState(NpcStateTransitions.MOVING_TO_UNSRESPAWN))
        {
            stateMachine.SetTransition(NpcStateTransitions.MOVING_TO_UNSRESPAWN);
            GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
        }
        else if (stateMachine.Current.State == NpcState.WANDER)
        {
            GoTo(BussGrid.GetRandomWalkablePosition(Position));
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            if (table == null) { return; }
            GoTo(table.GetActionTileInGridPosition());
        }
    }

    private void CheckIfTableMoved()
    {
        if (stateMachine.Current.State == NpcState.WAITING_TO_BE_ATTENDED && table == null)
        {
            stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        }
    }

    private void CheckUnrespawn()
    {
        if (!stateMachine.GetTransitionState(NpcStateTransitions.WALK_TO_UNRESPAWN))
        {
            if (stateTime >= MaxStateTime ||
                stateMachine.GetTransitionState(NpcStateTransitions.TABLE_MOVED) ||
                stateMachine.GetTransitionState(NpcStateTransitions.ATTENDED))
            {
                stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
                stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
            }
        }
    }

    private void Wander()
    {
        float randT = Random.Range(0, 1000);//TODO
        if (stateMachine.Current.State != NpcState.IDLE || randT > 2)
        {
            stateMachine.UnSetTransition(NpcStateTransitions.WANDER);
        }
        else
        {
            stateMachine.SetTransition(NpcStateTransitions.WANDER);
        }
    }

    private void CheckIfTableHasBeenAssigned()
    {
        if (table != null)
        {
            stateMachine.SetTransition(NpcStateTransitions.TABLE_AVAILABLE);
        }
        else
        {
            stateMachine.UnSetTransition(NpcStateTransitions.TABLE_AVAILABLE);
        }
    }

    private void CheckIfAtTarget()
    {
        if (!(currentTargetGridPosition.x == Position.x && currentTargetGridPosition.y == Position.y))
        {
            return;
        }

        if (stateMachine.Current.State == NpcState.WALKING_UNRESPAWN)
        {
            BussGrid.GameController.RemoveNpc(this);

            if (table != null)
            {
                table.FreeObject();
                table = null;
            }
            Destroy(gameObject);
        }
        else if (stateMachine.Current.State == NpcState.WALKING_TO_TABLE)
        {
            stateMachine.SetTransition(NpcStateTransitions.WAITING_AT_TABLE);
        }
    }

    public NpcState GetNpcState()
    {
        return stateMachine.Current.State;
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

    public bool HasTable()
    {
        return table != null;
    }

    public void SetTableMoved()
    {
        table = null;
        stateMachine.SetTransition(NpcStateTransitions.TABLE_MOVED);
        stateMachine.SetTransition(NpcStateTransitions.WALK_TO_UNRESPAWN);
    }

    public void SetAttended()
    {
        stateMachine.SetTransition(NpcStateTransitions.ATTENDED);
        stateMachine.SetTransition(NpcStateTransitions.ORDER_SERVED);
    }

    public void SetBeingAttended()
    {
        stateMachine.SetTransition(NpcStateTransitions.BEING_ATTENDED);
    }
}
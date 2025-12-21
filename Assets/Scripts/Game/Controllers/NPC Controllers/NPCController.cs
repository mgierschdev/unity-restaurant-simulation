using System;
using Game.Grid;
using Game.Players;
using UnityEngine;
using Util;
using IEnumerator = System.Collections.IEnumerator;
using Random = UnityEngine.Random;

// Controls NPCs players
// Attached to: NPC Objects
namespace Game.Controllers.NPC_Controllers
{
    /**
     * Problem: Control client NPC behavior and state transitions.
     * Goal: Move NPCs between wandering, tables, and unspawn states.
     * Approach: Drive a state machine and issue grid movement commands.
     * Time: O(1) per tick plus pathfinding.
     * Space: O(1) per NPC.
     */
    public class NpcController : GameObjectMovementBase
    {
        [SerializeField] private const float MaxStateTime = 120;
        [SerializeField] private NpcState state;

        private void Start()
        {
            Type = ObjectType.Client;
            SetID();
            StateMachine = NpcStateMachineFactory.GetClientStateMachine(Name);
            StartCoroutine(UpdateTransitionStates());
        }

        private void FixedUpdate()
        {
            try
            {
                UpdatePosition();
                UpdateTimeInState();
                UpdateTargetMovement();
                UpdateAnimation();
            }
            catch (Exception e)
            {
                GameLog.LogWarning("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
                StateMachine.SetTransition(NpcStateTransitions.TableMoved);
            }
        }

        public IEnumerator UpdateTransitionStates()
        {
            for (;;)
            {
                if (!IsMoving())
                {
                    CheckIfAtTarget();
                    CheckUnrespawn();
                    CheckIfTableHasBeenAssigned();
                    Wander();
                    CheckIfTableMoved();
                    state = StateMachine.Current.State;
                    StateMachine.CheckTransition();
                    MoveNpc();
                }

                yield return new WaitForSeconds(2f);
            }
        }

        private void MoveNpc()
        {
            if (StateMachine.Current.State == NpcState.WalkingUnRespawn &&
                !StateMachine.GetTransitionState(NpcStateTransitions.MovingToUnsRespawn))
            {
                StateMachine.SetTransition(NpcStateTransitions.MovingToUnsRespawn);
                GoTo(BussGrid.GetRandomSpamPointWorldPosition().GridPosition);
            }
            else if (StateMachine.Current.State == NpcState.Wander)
            {
                GoTo(BussGrid.GetRandomWalkablePosition(Position));
            }
            else if (StateMachine.Current.State == NpcState.WalkingToTable)
            {
                if (Table == null)
                {
                    return;
                }

                GoTo(Table.GetActionTileInGridPosition());
            }
        }

        private void CheckIfTableMoved()
        {
            if (StateMachine.Current.State == NpcState.WaitingToBeAttended && Table == null)
            {
                StateMachine.SetTransition(NpcStateTransitions.TableMoved);
            }
        }

        private void CheckUnrespawn()
        {
            if (!StateMachine.GetTransitionState(NpcStateTransitions.WalkToUnRespawn))
            {
                if (stateTime >= MaxStateTime ||
                    StateMachine.GetTransitionState(NpcStateTransitions.TableMoved) ||
                    StateMachine.GetTransitionState(NpcStateTransitions.Attended))
                {
                    StateMachine.SetTransition(NpcStateTransitions.WalkToUnRespawn);
                    StateMachine.SetTransition(NpcStateTransitions.TableMoved);
                }
            }
        }

        private void Wander()
        {
            // Chance to no wander
            float randT = Random.Range(0, 8);

            if (StateMachine.Current.State != NpcState.Idle || randT > 2)
            {
                StateMachine.UnSetTransition(NpcStateTransitions.Wander);

                if (StateMachine.Current.State == NpcState.Wander)
                {
                    StateMachine.SetTransition(NpcStateTransitions.WanderToIdle);
                }
            }
            else
            {
                StateMachine.UnSetTransition(NpcStateTransitions.WanderToIdle);
                StateMachine.SetTransition(NpcStateTransitions.Wander);
            }
        }

        private void CheckIfTableHasBeenAssigned()
        {
            if (Table != null)
            {
                StateMachine.SetTransition(NpcStateTransitions.TableAvailable);
            }
            else
            {
                StateMachine.UnSetTransition(NpcStateTransitions.TableAvailable);
            }
        }

        private void CheckIfAtTarget()
        {
            if (!(CurrentTargetGridPosition.x == Position.x && CurrentTargetGridPosition.y == Position.y))
            {
                return;
            }

            if (StateMachine.Current.State == NpcState.WalkingUnRespawn)
            {
                //   BussGrid.GameController.RemoveNpc(this);

                if (Table != null)
                {
                    Table.FreeObject();
                    Table = null;
                }

                Destroy(gameObject);
            }
            else if (StateMachine.Current.State == NpcState.WalkingToTable)
            {
                StateMachine.SetTransition(NpcStateTransitions.WaitingAtTable);
            }
        }

        public NpcState GetNpcState()
        {
            return StateMachine.Current.State;
        }

        public float GetNpcStateTime()
        {
            return Mathf.Floor(stateTime);
        }

        public GameGridObject GetTable()
        {
            return Table;
        }

        public void SetTable(GameGridObject obj)
        {
            Table = obj;
        }

        public void FlipTowards(Vector3Int direction)
        {
            StandTowards(direction);
        }

        public bool HasTable()
        {
            return Table != null;
        }

        public void SetTableMoved()
        {
            Table = null;
            StateMachine.SetTransition(NpcStateTransitions.TableMoved);
            StateMachine.SetTransition(NpcStateTransitions.WalkToUnRespawn);
        }

        public void SetAttended()
        {
            StateMachine.SetTransition(NpcStateTransitions.Attended);
            StateMachine.SetTransition(NpcStateTransitions.OrderServed);
        }

        public void SetBeingAttended()
        {
            StateMachine.SetTransition(NpcStateTransitions.BeingAttended);
        }
    }
}

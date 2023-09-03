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
    public class ClientController : GameObjectMovementBase
    {
        private const float MaxStateTime = Settings.MaxStateTime;
        private float _currentEatingTime;
        private SkinSelectorController _skinSelectorController;

        private void Start()
        {
            Type = ObjectType.Client;
            _currentEatingTime = Random.Range(Settings.MinTimeEating, Settings.MaxTimeEating);
            SetID();
            StateMachine = NpcStateMachineFactory.GetClientStateMachine(Name);
            StartCoroutine(UpdateTransitionStates());
            // We select the skin of the client
            _skinSelectorController = transform.GetComponent<SkinSelectorController>();
            int val = Random.Range(2, 5); // We get a random value for the character skin
            _skinSelectorController.SetCharacter((CharacterType)val);
        }

        private void FixedUpdate()
        {
            try
            {
                if (!IsMovementBaseActive())
                {
                    // The Children overrides the FixedUpdate of the parent class expected behaivior
                    // we wait until the parent has all the object references
                    return;
                }

                UpdatePosition();
                UpdateTimeInState();
                UpdateTargetMovement();
                UpdateAnimation();
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, likely missing reference (FixedUpdate NPCController): " + e);
                StateMachine.SetTransition(NpcStateTransitions.TableMoved);
            }
        }

        public IEnumerator UpdateTransitionStates()
        {
            for (;;)
            {
                try
                {
                    if (!IsMoving())
                    {
                        CheckIfAtTarget();
                        CheckUnrespawn();
                        CheckIfTableHasBeenAssigned();
                        Wander();
                        CheckIfTableOrEmployeeMoved();
                        StateMachine.CheckTransition();
                        MoveNpc();
                    }
                }
                catch (Exception e)
                {
                    GameLog.LogError("Exception thrown, NPCController/UpdateTransitionStates(): " + e);
                    StateMachine.SetTransition(NpcStateTransitions.WalkToUnRespawn);
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

        private void CheckIfTableOrEmployeeMoved()
        {
            if ((StateMachine.Current.State == NpcState.WaitingToBeAttended && Table == null) ||
                (StateMachine.Current.State == NpcState.WaitingToBeAttended && Table != null &&
                 !Table.HasAttendedBy()))
            {
                if (stateTime <= MaxStateTime)
                {
                    return;
                }

                StateMachine.SetTransition(NpcStateTransitions.TableMoved);
            }
        }

        private void CheckUnrespawn()
        {
            if (!StateMachine.GetTransitionState(NpcStateTransitions.WalkToUnRespawn))
            {
                if (stateTime >= MaxStateTime ||
                    StateMachine.GetTransitionState(NpcStateTransitions.TableMoved) ||
                    StateMachine.GetTransitionState(NpcStateTransitions.OrderServed))
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
                BussGrid.GameController.RemoveNpc(this);
                Destroy(gameObject);
            }
            else if (StateMachine.Current.State == NpcState.WalkingToTable)
            {
                StateMachine.SetTransition(NpcStateTransitions.WaitingAtTable);
            }
            else if (StateMachine.Current.State == NpcState.Attended)
            {
                if (Table == null)
                {
                    return;
                }

                StandTowards(Table.GridPosition); // stand towards the table
            }
            else if (StateMachine.Current.State == NpcState.EatingFood && stateTime >= _currentEatingTime)
            {
                StateMachine.UnSetAll();
                FreeTable(); // finished eating we free the table 
                StateMachine.SetTransition(NpcStateTransitions.OrderServed);
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
            StateMachine.SetTransition(NpcStateTransitions.EatingFood);
        }

        public void SetBeingAttended()
        {
            StateMachine.SetTransition(NpcStateTransitions.BeingAttended);
        }

        public void FreeTable()
        {
            if (Table != null)
            {
                Table.FreeObject();
                Table = null;
            }
        }
    }
}
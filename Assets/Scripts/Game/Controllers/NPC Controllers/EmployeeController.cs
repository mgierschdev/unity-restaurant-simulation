using System;
using Game.Grid;
using Game.Players;
using UnityEngine;
using Util;
using IEnumerator = System.Collections.IEnumerator;
using Random = UnityEngine.Random;

namespace Game.Controllers.NPC_Controllers
{
    public class EmployeeController : GameObjectMovementBase
    {
        private const float SpeedTimeTakingOrder = 1.5f;
        private GameGridObject _counter;
        private bool _counterAssigned;
        private SkinSelectorController _skinSelectorController;

        private void Start()
        {
            Type = ObjectType.Employee;
            SetID();
            StateMachine = NpcStateMachineFactory.GetEmployeeStateMachine(Name);
            StartCoroutine(UpdateTransitionStates());

            // We select the skin of the employee
            _skinSelectorController = transform.GetComponent<SkinSelectorController>();
            _skinSelectorController.SetCharacter(CharacterType.Employee);
        }

        private void FixedUpdate()
        {
            try
            {
                if (!IsMovementBaseActive())
                {
                    return;
                }

                UpdatePosition();
                UpdateTimeInState();
                UpdateTargetMovement();
                UpdateAnimation();
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, likely missing reference (FixedUpdate EmployeeController): " + e);
            }
        }

        public IEnumerator UpdateTransitionStates()
        {
            for (;;)
            {
                try
                {
                    if (!_counterAssigned || IsMoving() || EnergyBar.IsActive())
                    {
                    }
                    else
                    {
                        CheckIfAtTarget();
                        TableWithCustomer();
                        Unrespawn();
                        CheckCounter();
                        CheckAtCounter();
                        CheckTableMoved();
                        StateMachine.CheckTransition();
                        MoveNpc(); // Move/or not, depending on the state
                    }
                }
                catch (Exception e)
                {
                    GameLog.LogError("Exception thrown, EmployeeController/UpdateTransitionStates(): " + e);
                    StateMachine.SetTransition(NpcStateTransitions.WalkToUnRespawn);
                }

                yield return new WaitForSeconds(1f);
            }
        }

        // if Idle and table moved we unset it since it is not longer attending the table
        private void CheckTableMoved()
        {
            if (StateMachine.Current.State == NpcState.Idle &&
                StateMachine.GetTransitionState(NpcStateTransitions.TableMoved))
            {
                StateMachine.UnSetTransition(NpcStateTransitions.TableMoved);
            }
        }

        private void CheckAtCounter()
        {
            // we check if at counter we set the bit
            if (_counter != null && Position.x == _counter.GetActionTileInGridPosition().x &&
                Position.y == _counter.GetActionTileInGridPosition().y)
            {
                StateMachine.SetTransition(NpcStateTransitions.AtCounter);
            }
        }

        private void CheckCounter()
        {
            if (_counter == null)
            {
                StateMachine.UnSetTransition(NpcStateTransitions.CounterAvailable);
            }
            else
            {
                StateMachine.SetTransition(NpcStateTransitions.CounterAvailable);
            }
        }

        private void TableWithCustomer()
        {
            if (StateMachine.Current.State != NpcState.AtCounter)
            {
                return;
            }

            if (Table != null)
            {
                Table.SetAttendedBy(this);
                StateMachine.SetTransition(NpcStateTransitions.TableAvailable);
                return;
            }

            StateMachine.UnSetTransition(NpcStateTransitions.TableAvailable);
        }

        private void Unrespawn()
        {
            if (StateMachine.Current.State == NpcState.WalkingUnRespawn || _counter != null)
            {
                StateMachine.UnSetTransition(NpcStateTransitions.WalkToUnRespawn);
                return;
            }

            // we clean the table pointer if assigned
            if (Table != null)
            {
                Table.SetAttendedBy(null);
                Table = null;
            }

            StateMachine.SetTransition(NpcStateTransitions.WalkToUnRespawn);
        }

        private void CheckIfAtTarget()
        {
            if (!(CurrentTargetGridPosition.x == Position.x && CurrentTargetGridPosition.y == Position.y))
            {
                return;
            }

            if (StateMachine.Current.State == NpcState.WalkingToCounter &&
                !StateMachine.GetTransitionState(NpcStateTransitions.AtCounter))
            {
                if (_counter == null)
                {
                    return;
                }

                StateMachine.SetTransition(NpcStateTransitions.AtCounter);
                StateMachine.UnSetTransition(NpcStateTransitions.CashRegistered);
                StandTowards(_counter.GridPosition);
            }
            else if (StateMachine.Current.State == NpcState.WalkingToTable)
            {
                StateMachine.SetTransition(NpcStateTransitions.AtTable);
                StateMachine.UnSetTransition(NpcStateTransitions.AtCounter);
                if (Table == null)
                {
                    return;
                }

                var controller = Table.GetUsedBy();

                if (controller == null)
                {
                    return;
                }

                StandTowards(controller.Position); // We flip the Employee -> CLient
                controller.FlipTowards(Position); // We flip client -> employee
                controller.SetBeingAttended();
            }
            else if (StateMachine.Current.State == NpcState.TakingOrder && EnergyBar.IsFinished() &&
                     !StateMachine.GetTransitionState(NpcStateTransitions.OrderServed))
            {
                StateMachine.SetTransition(NpcStateTransitions.OrderServed);

                if (Table == null)
                {
                    return;
                }

                var controller = Table.GetUsedBy();

                if (controller == null)
                {
                    return;
                }

                controller.SetAttended();
            }

            else if (StateMachine.Current.State == NpcState.WalkingToCounterAfterOrder)
            {
                StateMachine.SetTransition(NpcStateTransitions.AtCounter);
                StateMachine.SetTransition(NpcStateTransitions.RegisteringCash);
                StateMachine.UnSetTransition(NpcStateTransitions.AtTable);
            }
            else if (StateMachine.Current.State == NpcState.RegisteringCash && EnergyBar.IsFinished())
            {
                StateMachine.SetTransition(NpcStateTransitions.CashRegistered);
                double orderValue = Random.Range(5, 10);
                double totalOrderCost = orderValue + PlayerData.GetUgrade(UpgradeType.OrderCostPercentage) *
                    (orderValue *
                     Settings.OrderIncreaseCostPercentage
                     / 100);
                PlayerData.AddMoney(totalOrderCost);
                PlayerData.SetCustomerAttended();
            }
            else if (StateMachine.Current.State == NpcState.AtCounterFinal)
            {
                StateMachine.UnSetAll();
                StateMachine.SetTransition(NpcStateTransitions.AtCounterFinal);
            }
            else if (StateMachine.Current.State == NpcState.WalkingUnRespawn)
            {
                BussGrid.GameController.RemoveEmployee(this);
                Destroy(gameObject);
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
            else if (StateMachine.Current.State == NpcState.WalkingToCounter)
            {
                if (_counter == null)
                {
                    return;
                }

                GoTo(_counter.GetActionTileInGridPosition());
            }
            else if (StateMachine.Current.State == NpcState.WalkingToTable)
            {
                if (Table == null)
                {
                    return;
                }

                GoTo(BussGrid.GetClosestPathGridPoint(Position, Table.GetActionTileInGridPosition()));
            }
            else if (StateMachine.Current.State == NpcState.TakingOrder &&
                     !StateMachine.GetTransitionState(NpcStateTransitions.OrderServed))
            {
                EnergyBar.SetActive(SpeedTimeTakingOrder);
            }
            else if (StateMachine.Current.State == NpcState.WalkingToCounterAfterOrder)
            {
                Table = null;
                if (_counter == null)
                {
                    return;
                }

                GoTo(_counter.GetActionTileInGridPosition());
            }
            else if (StateMachine.Current.State == NpcState.RegisteringCash &&
                     !StateMachine.GetTransitionState(NpcStateTransitions.CashRegistered))
            {
                EnergyBar.SetActive(SpeedTimeTakingOrder);
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

        public Vector3Int GetCoordOfTableToBeAttended()
        {
            return Table.GridPosition;
        }

        public void SetUnRespawn()
        {
            StateMachine.SetTransition(NpcStateTransitions.MovingToUnsRespawn);
        }

        public void SetTableMoved()
        {
            StateMachine.UnSetAll();
            StateMachine.SetTransition(NpcStateTransitions.TableMoved);
        }

        public void SetCounter(GameGridObject counter)
        {
            _counterAssigned = true;
            this._counter = counter;
        }

        public void SetTableToAttend(GameGridObject obj)
        {
            Table = obj;
        }

        public bool IsAttendingTable()
        {
            return Table != null;
        }

        private void CreateCoin()
        {
            var initPosition = transform.position;

            var coin =
                Instantiate(Resources.Load("Objects/Coin", typeof(GameObject)), initPosition, Quaternion.identity) as
                    GameObject;
        }
    }
}
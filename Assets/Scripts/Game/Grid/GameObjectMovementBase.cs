using System.Collections;
using System.Collections.Generic;
using Game.Controllers.NPC_Controllers;
using Game.Controllers.Other_Controllers;
using Game.Players;
using UnityEngine;
using UnityEngine.Rendering;
using Util;
using Util.Collections;

// Parent class for all moving npcs on the scene
namespace Game.Grid
{
    /**
     * Problem: Provide shared movement logic for NPCs on the grid.
     * Goal: Handle pathfinding, animation updates, and direction changes.
     * Approach: Manage movement queues and state-driven updates in FixedUpdate.
     * Time: O(n) per path step (n = queued steps).
     * Space: O(n) for movement queue.
     */
    public class GameObjectMovementBase : MonoBehaviour
    {
        public string Name { get; private set; }
        public Vector3Int Position { get; private set; } //PathFindingGrid Position
        public Vector3Int PrevGridPosition { get; private set; }
        private MoveDirection _moveDirection;

        private CharacterSide _side; // false right, true left

        //Energy Bars
        protected LoadSliderController EnergyBar;
        [SerializeField] protected float idleTime, stateTime, speed, timeBeforeRemovingDebugPanel = 0.1f;
        private bool _isMoving;
        private SortingGroup _sortingLayer;
        [SerializeField] private NpcState currentState, prevState;
        protected PlayerAnimationStateController AnimationController;
        protected ObjectType Type;

        private ItemType _itemToAskFor;

        // Attributes for saving the path of the NPC on the grid
        // This will help to void placing objects on top of the NPC
        private Queue _pendingMovementQueue;
        private Vector3 _currentLocalTargetPosition; //step by step target to

        private int _debugColorValue;

        //Final target 
        protected Vector3 CurrentTargetWorldPosition;

        protected Vector3Int CurrentTargetGridPosition;

        //State machine
        protected GameGridObject Table;
        protected StateMachine<NpcState, NpcStateTransitions> StateMachine;

        private void Awake()
        {
            _currentLocalTargetPosition = transform.position;
            speed = Settings.NpcDefaultMovementSpeed;
            _side = CharacterSide.Right;
            _itemToAskFor = ItemType.OrangeJuice;
            _pendingMovementQueue = new Queue();
            AnimationController = GetComponent<PlayerAnimationStateController>();
            _sortingLayer = transform.GetComponent<SortingGroup>();
            EnergyBar = transform.Find(Settings.LoadSlider).GetComponent<LoadSliderController>();

            if (!Util.Util.IsNull(EnergyBar, "GameObjectMovementBase/energyBar null"))
            {
                EnergyBar.SetInactive();
            }

            if (AnimationController == null)
            {
                GameLog.LogWarning("NPCController/animationController-gameObj null");
            }

            if (_sortingLayer == null)
            {
                GameLog.LogWarning("NPCController/sortingLayer null");
            }

            UpdatePosition();
        }

        private void Start()
        {
            idleTime = 0;
            stateTime = 0;
            prevState = currentState;
            _isMoving = false;
            currentState = NpcState.Idle;
            AnimationController.SetInfoPopUItem(_itemToAskFor);
        }

        private void FixedUpdate()
        {
            UpdatePosition();
            UpdateAnimation();
        }

        protected void SetID()
        {
            var objectTransform = transform;
            var id = BussGrid.GameController.GetNpcSet().Count + 1 + "-" + Time.frameCount;
            
            transform.name = Type.ToString() + "." + id;
            Name = objectTransform.name;
        }

        protected void StandTowards(Vector3Int target)
        {
            MoveDirection m = GetDirectionFromPositions(Position, target);

            if (m == MoveDirection.Left || m == MoveDirection.DownLeft || m == MoveDirection.UpLeft ||
                m == MoveDirection.Up)
            {
                FlipToSide(CharacterSide.Left);
            }
            else if (m == MoveDirection.Right || m == MoveDirection.Downright || m == MoveDirection.Upright ||
                     m == MoveDirection.Down)
            {
                FlipToSide(CharacterSide.Right);
            }
        }

        public void UpdatePosition()
        {
            Position = BussGrid.GetPathFindingGridFromWorldPosition(transform.position);
            Position = new Vector3Int(Position.x, Position.y);
            UpdatePrevPosition();
            //Sorting (sprite) the layer depending on the player position
            _sortingLayer.sortingOrder = Util.Util.GetSorting(Position);
        }

        private void UpdateObjectDirection()
        {
            if (_side == CharacterSide.Left &&
                (_moveDirection == MoveDirection.Down ||
                 _moveDirection == MoveDirection.Upright ||
                 _moveDirection == MoveDirection.Downright ||
                 _moveDirection == MoveDirection.Right))
            {
                FlipToSide(CharacterSide.Right);
            }
            else if (_side == CharacterSide.Right &&
                     (_moveDirection == MoveDirection.Up ||
                      _moveDirection == MoveDirection.UpLeft ||
                      _moveDirection == MoveDirection.DownLeft ||
                      _moveDirection == MoveDirection.Left))
            {
                FlipToSide(CharacterSide.Left);
            }
        }

        private void FlipToSide(CharacterSide flipSide)
        {
            Vector3 tmp = gameObject.transform.localScale;

            if (flipSide == CharacterSide.Left)
            {
                tmp.x = tmp.x > 0 ? -tmp.x : tmp.x;
            }
            else
            {
                tmp.x = tmp.x < 0 ? -tmp.x : tmp.x;
            }

            transform.localScale = tmp;
            _side = flipSide;
        }

        protected void UpdateTargetMovement()
        {
            if (!_isMoving)
            {
                return;
            }

            UpdateAStarMovement();
        }

        private void UpdateAStarMovement()
        {
            if (IsInTargetPosition())
            {
                if (_pendingMovementQueue.Count != 0)
                {
                    AddMovement();
                }
                else
                {
                    // Final target reached 
                    _moveDirection = MoveDirection.Idle;
                    BussGrid.GameController.RemoveEmployeePlannedTarget(CurrentTargetGridPosition);
                    _isMoving = false;
                }
            }
            else
            {
                _moveDirection = GetDirectionFromPositions(transform.position, _currentLocalTargetPosition);
                UpdateObjectDirection(); // It flips the side of the object depending on direction

                //Upgrade: NPC/Waiter speed
                speed = Settings.NpcDefaultMovementSpeed + Settings.UpgradePercentageMultiplayer *
                    (ObjectType.Employee == Type
                        ? (PlayerData.GetUgrade(UpgradeType.WaiterSpeed) == 0
                            ? 1
                            : PlayerData.GetUgrade(UpgradeType.WaiterSpeed))
                        : (PlayerData.GetUgrade(UpgradeType.ClientSpeed) == 0
                            ? 1
                            : PlayerData.GetUgrade(UpgradeType.ClientSpeed)));

                transform.position = Vector3.MoveTowards(transform.position, _currentLocalTargetPosition,
                    speed * Time.fixedDeltaTime);
            }
        }

        public void UpdateAnimation()
        {
            // Animates depending on the current state
            if (IsMoving() && prevState == currentState)
            {
                if (Type == ObjectType.Employee && StateMachine.Current.State == NpcState.WalkingToTable)
                {
                    AnimationController.SetState(NpcState.WalkingToTable);
                }
                else
                {
                    AnimationController.SetState(NpcState.Walking);
                }
            }
            else
            {
                // Could be walking to table but the NPC it is not actually moving anymore
                if (StateMachine.Current.State == NpcState.WalkingToTable)
                {
                    if (Type == ObjectType.Client)
                    {
                        AnimationController.SetState(NpcState.Idle);
                    }
                    else
                    {
                        AnimationController.SetState(NpcState.TakingOrder);
                    }
                }
                else
                {
                    AnimationController.SetState(StateMachine.Current.State);
                }
            }
        }

        protected void UpdateTimeInState()
        {
            currentState = StateMachine.Current.State;
            // Keeps the time in the current state
            if (prevState == currentState)
            {
                stateTime += Time.fixedDeltaTime;
            }
            else
            {
                stateTime = 0;
                prevState = currentState;
            }
        }

        private void UpdatePrevPosition()
        {
            //Position changed
            if (PrevGridPosition != Position)
            {
                BussGrid.GameController.RemovePlayerPosition(PrevGridPosition);
                BussGrid.GameController.AddPlayerPositions(Position);
                PrevGridPosition = Position;
            }
        }

        private void AddMovement()
        {
            if (_pendingMovementQueue.Count == 0)
            {
                return;
            }

            Vector3 queuePosition = (Vector3)_pendingMovementQueue.Dequeue();
            Vector3 direction =
                BussGrid.GetWorldFromPathFindingGridPositionWithOffSet(new Vector3Int((int)queuePosition.x,
                    (int)queuePosition.y));
            _currentLocalTargetPosition = new Vector3(direction.x, direction.y);
        }

        protected bool IsMoving()
        {
            return _isMoving;
        }

        private List<Node> MergePath(List<Node> path)
        {
            List<Vector3> queuePath = new List<Vector3>();
            List<Node> merge = new List<Node>();

            while (_pendingMovementQueue.Count > 0)
            {
                queuePath.Add((Vector3)_pendingMovementQueue.Dequeue());
            }

            int index = 0;

            while (index < path.Count && path[index].GetVector3() != queuePath[0])
            {
                index++;
            }

            if (index == path.Count)
            {
                return path;
            }

            for (int i = index; i < path.Count; i++)
            {
                merge.Add(path[i]);
            }

            return merge;
        }

        //A to B direction
        private MoveDirection GetDirectionFromPositions(Vector3 a, Vector3 b)
        {
            Vector3 delta = b - a;
            float radians = Mathf.Atan2(delta.x, delta.y);
            float degrees = radians * Mathf.Rad2Deg;
            // normalizing -180-180, 0-360
            if (degrees < 0)
            {
                degrees += 360;
            }

            return Util.Util.GetDirectionFromAngles(degrees);
        }

        private void AddPath(List<Node> path)
        {
            if (path.Count == 0)
            {
                return;
            }

            if (_pendingMovementQueue.Count != 0)
            {
                //path = MergePath(path); // We merge Paths
                _pendingMovementQueue.Clear();
            }

            _pendingMovementQueue.Enqueue(path[0].GetVector3());

            for (int i = 1; i < path.Count; i++)
            {
                if (Settings.CellDebug)
                {
                    // Draw path
                    Vector3 from = BussGrid.GetWorldFromPathFindingGridPosition(path[i - 1].GetVector3Int());
                    Vector3 to = BussGrid.GetWorldFromPathFindingGridPosition(path[i].GetVector3Int());
                    Debug.DrawLine(from, to, Util.Util.GetRandomColor(_debugColorValue % 10), 15f);
                }

                _pendingMovementQueue.Enqueue(path[i].GetVector3());
            }

            _debugColorValue++;

            if (path.Count == 0)
            {
                GameLog.LogWarning("Path out of reach");
                return;
            }

            AddMovement(); // To set the first target
        }

        public bool GoTo(Vector3Int position)
        {
            if (GoToAStar(position))
            {
                SetGoTo(position);
                return true;
            }

            return false;
        }

        public bool GoToAStar(Vector3Int pos)
        {
            List<Node> path = BussGrid.GetPath(new[] { Position.x, Position.y }, new[] { pos.x, pos.y });

            if (path.Count == 0)
            {
                return false;
            }

            AddPath(path);

            if (_pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }

            return true;
        }

        private void SetGoTo(Vector3Int target)
        {
            CurrentTargetGridPosition = target;
            CurrentTargetWorldPosition = BussGrid.GetWorldFromPathFindingGridPosition(target);
            BussGrid.GameController.AddEmployeePlannedTarget(target);
            _isMoving = true;
        }

        public void RecalculateGoTo()
        {
            if (!GoTo(CurrentTargetGridPosition))
            {
            }
        }

        public float GetSpeed()
        {
            return speed;
        }

        private bool IsInTargetPosition()
        {
            return Util.Util.IsAtDistanceWithObject(_currentLocalTargetPosition, transform.position);
        }

        public StateMachine<NpcState, NpcStateTransitions> GetStateMachine()
        {
            return StateMachine;
        }

        public ItemType GetItemToAskFor()
        {
            return _itemToAskFor;
        }

        public bool IsMovementBaseActive()
        {
            return EnergyBar != null && AnimationController != null && _sortingLayer != null;
        }
    }
}

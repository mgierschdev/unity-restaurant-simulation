using System;
using JetBrains.Annotations;
using Util;
using Util.Collections;

// Returns a state machine for npcs
// Order of the transitions is relevant
// NPCState represents the NPC current state which is used for selecting the correct animation eating/waiting for food 
// NPCStateTransition are the states that are required to move from one state to another, represented by an OR
// Example: TABLE_MOVED OR WALK_TO_UNRESPAWN pass from adjMatrix[WANDER to, IDLE]

namespace Game.Players
{
    /**
     * Problem: Build NPC state machines for clients and employees.
     * Goal: Define allowed transitions between NPC states.
     * Approach: Construct adjacency matrices for StateMachine transitions.
     * Time: O(n^2) for state matrix setup.
     * Space: O(n^2) for transition matrix storage.
     */
    public static class NpcStateMachineFactory
    {
        public static StateMachine<NpcState, NpcStateTransitions> GetClientStateMachine([NotNull] string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            // Keeps the posible transition bewteen the nodes
            StateNodeTransition[,] adjMatrix = new StateNodeTransition[Enum.GetNames(typeof(NpcState)).Length,
                Enum.GetNames(typeof(NpcState)).Length];
            bool[] nodeTransition = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];

            //IDLE -> Other
            nodeTransition[(int)NpcStateTransitions.TableAvailable] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.WalkingToTable] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.Wander] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.Wander] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WANDER -> Other
            nodeTransition[(int)NpcStateTransitions.WanderToIdle] = true;
            adjMatrix[(int)NpcState.Wander, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.Wander, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WALKING_TO_TABLE -> Other
            nodeTransition[(int)NpcStateTransitions.WaitingAtTable] = true;
            adjMatrix[(int)NpcState.WalkingToTable, (int)NpcState.AtTable] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.WalkingToTable, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //AT_TABLE -> Other
            adjMatrix[(int)NpcState.AtTable, (int)NpcState.WaitingToBeAttended] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.AtTable, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WAITING_TO_BE_ATTENDED -> Other
            nodeTransition[(int)NpcStateTransitions.BeingAttended] = true;
            adjMatrix[(int)NpcState.WaitingToBeAttended, (int)NpcState.BeingAttended] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.WaitingToBeAttended, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //BEING_ATTENDED -> Other
            nodeTransition[(int)NpcStateTransitions.Attended] = true;
            adjMatrix[(int)NpcState.BeingAttended, (int)NpcState.Attended] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.BeingAttended, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //ATTENDED -> Other
            nodeTransition[(int)NpcStateTransitions.EatingFood] = true;
            adjMatrix[(int)NpcState.Attended, (int)NpcState.EatingFood] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.Attended, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            // EATING FOOD -> Other
            nodeTransition[(int)NpcStateTransitions.OrderServed] = true;
            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.EatingFood, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            return new StateMachine<NpcState, NpcStateTransitions>(adjMatrix, NpcState.Idle, id);
        }

        public static StateMachine<NpcState, NpcStateTransitions> GetEmployeeStateMachine(string id)
        {
            // Keeps the posible transition bewteen the nodes
            StateNodeTransition[,] adjMatrix = new StateNodeTransition[Enum.GetNames(typeof(NpcState)).Length,
                Enum.GetNames(typeof(NpcState)).Length];
            bool[] nodeTransition = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];

            //IDLE -> Other
            nodeTransition[(int)NpcStateTransitions.CounterAvailable] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.WalkingToCounter] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.AtCounter] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.AtCounter] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.Idle, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WALKING_TO_COUNTER --> Other
            nodeTransition[(int)NpcStateTransitions.AtCounter] = true;
            adjMatrix[(int)NpcState.WalkingToCounter, (int)NpcState.AtCounter] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.WalkingToCounter, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.WalkingToCounter, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //AT_COUNTER --> Other
            nodeTransition[(int)NpcStateTransitions.TableAvailable] = true;
            adjMatrix[(int)NpcState.AtCounter, (int)NpcState.WalkingToTable] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.AtCounter, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.AtCounter, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WALKING_TO_TABLE --> Other
            nodeTransition[(int)NpcStateTransitions.AtTable] = true;
            adjMatrix[(int)NpcState.WalkingToTable, (int)NpcState.TakingOrder] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.WalkingToTable, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.WalkingToTable, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //TAKING_ORDER --> Other
            nodeTransition[(int)NpcStateTransitions.OrderServed] = true;
            adjMatrix[(int)NpcState.TakingOrder, (int)NpcState.WalkingToCounterAfterOrder] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.TakingOrder, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.TakingOrder, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //WALKING_TO_COUNTER_AFTER_ORDER --> Other
            nodeTransition[(int)NpcStateTransitions.RegisteringCash] = true;
            adjMatrix[(int)NpcState.WalkingToCounterAfterOrder, (int)NpcState.RegisteringCash] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.WalkingToCounterAfterOrder, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.WalkingToCounterAfterOrder, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //REGISTERING_CASH --> Other
            nodeTransition[(int)NpcStateTransitions.CashRegistered] = true;
            adjMatrix[(int)NpcState.RegisteringCash, (int)NpcState.AtCounterFinal] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.RegisteringCash, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.RegisteringCash, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            //AT_COUNTER_FINAL --> COUNTER
            nodeTransition[(int)NpcStateTransitions.AtCounterFinal] = true;
            adjMatrix[(int)NpcState.AtCounterFinal, (int)NpcState.AtCounter] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.WalkToUnRespawn] = true;
            adjMatrix[(int)NpcState.AtCounterFinal, (int)NpcState.WalkingUnRespawn] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            nodeTransition[(int)NpcStateTransitions.TableMoved] = true;
            adjMatrix[(int)NpcState.AtCounterFinal, (int)NpcState.Idle] =
                new StateNodeTransition((bool[])nodeTransition.Clone());
            Array.Fill(nodeTransition, false);

            return new StateMachine<NpcState, NpcStateTransitions>(adjMatrix, NpcState.Idle, id);
        }
    }
}

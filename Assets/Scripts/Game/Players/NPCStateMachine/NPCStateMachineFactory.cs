
using System;

// Returns a state machine for npcs
public static class NPCStateMachineFactory
{
    public static StateMachine GetClientStateMachine()
    {
        // Keeps the posible transition bewteen the nodes
        StateNodeTransition[,] adjMatrix = new StateNodeTransition[Enum.GetNames(typeof(NpcState)).Length, Enum.GetNames(typeof(NpcState)).Length];
        bool[] nodeTransition = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];

        //IDLE -> Other
        nodeTransition[(int)NpcStateTransitions.TABLE_AVAILABLE] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_TO_TABLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WANDER] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WANDER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WANDER -> Other
        adjMatrix[(int)NpcState.WANDER, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WANDER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WALKING_TO_TABLE -> Other
        nodeTransition[(int)NpcStateTransitions.TABLE_AVAILABLE] = true;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.AT_TABLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //AT_TABLE -> Other
        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WAITING_TO_BE_ATTENDED] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WAITING_TO_BE_ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.BEING_ATTENDED] = true;
        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.BEING_ATTENDED] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //BEING_ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.ATTENDED] = true;
        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.ATTENDED] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.ORDER_SERVED] = true;
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);
        return new StateMachine(adjMatrix);
    }
}
// NPC states
// IDLE = 0,
// WALKING_TO_TABLE = 1,
// AT_TABLE = 2,
// WALKING_TO_COUNTER = 3,
// AT_COUNTER = 4,
// WALKING_WANDER = 5,
// TAKING_ORDER = 6,
// WAITING_TO_BE_ATTENDED = 7,
// WALKING_UNRESPAWN = 8,
// WALKING_TO_COUNTER_AFTER_ORDER = 9,
// REGISTERING_CASH = 10,
// WAITING_TO_BE_ATTENDED_ANIMATION = 11,
// WANDER = 12,
// WAITING_FOR_ENERGY_BAR_TAKING_ORDER = 13,
// WAITING_FOR_ENERGY_BAR_REGISTERING_CASH = 14,
// WALKING = 15,
// BEING_ATTENDED = 16,
// ATTENDED = 17

//NPC node transitions
// TABLE_AVAILABLE = 1,
// TABLE_MOVED = 2,
// WANDER_TIME = 3,
// WAITING_AT_TABLE_TIME = 4,
// IDLE_TIME = 5,
// ORDER_SERVED = 6,
// ORDER_FINISHED = 7,
// ENERGY_BAR_VALUE = 8,
// COUNTER_MOVED = 9,
// WANDER = 10
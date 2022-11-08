
using System;

// Returns a state machine for npcs
public static class NPCStateMachineFactory
{
    public static StateMachine<NpcState, NpcStateTransitions> GetClientStateMachine(string ID)
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
        nodeTransition[(int)NpcStateTransitions.WAITING_AT_TABLE] = true;
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
        return new StateMachine<NpcState, NpcStateTransitions>(adjMatrix, NpcState.IDLE, ID);
    }

    public static StateMachine<NpcState, NpcStateTransitions> GetEmployeeStateMachine(string ID)
    {
        // Keeps the posible transition bewteen the nodes
        StateNodeTransition[,] adjMatrix = new StateNodeTransition[Enum.GetNames(typeof(NpcState)).Length, Enum.GetNames(typeof(NpcState)).Length];
        bool[] nodeTransition = new bool[Enum.GetNames(typeof(NpcStateTransitions)).Length];

        //IDLE -> Other
        nodeTransition[(int)NpcStateTransitions.COUNTER_AVAILABLE] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_TO_COUNTER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.AT_COUNTER] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.AT_COUNTER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WALKING_TO_COUNTER --> Other
        nodeTransition[(int)NpcStateTransitions.AT_COUNTER] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER, (int)NpcState.AT_COUNTER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //AT_COUNTER --> Other
        nodeTransition[(int)NpcStateTransitions.TABLE_AVAILABLE] = true;
        adjMatrix[(int)NpcState.AT_COUNTER, (int)NpcState.WALKING_TO_TABLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.AT_COUNTER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.AT_COUNTER, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WALKING_TO_TABLE --> Other
        nodeTransition[(int)NpcStateTransitions.AT_TABLE] = true;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.TAKING_ORDER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //TAKING_ORDER --> Other
        nodeTransition[(int)NpcStateTransitions.ORDER_SERVED] = true;
        adjMatrix[(int)NpcState.TAKING_ORDER, (int)NpcState.WALKING_TO_COUNTER_AFTER_ORDER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.TAKING_ORDER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.TAKING_ORDER, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //WALKING_TO_COUNTER_AFTER_ORDER --> Other
        nodeTransition[(int)NpcStateTransitions.REGISTERING_CASH] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER_AFTER_ORDER, (int)NpcState.REGISTERING_CASH] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER_AFTER_ORDER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.WALKING_TO_COUNTER_AFTER_ORDER, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //REGISTERING_CASH --> Other
        nodeTransition[(int)NpcStateTransitions.CASH_REGISTERED] = true;
        adjMatrix[(int)NpcState.REGISTERING_CASH, (int)NpcState.AT_COUNTER_FINAL] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.REGISTERING_CASH, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.REGISTERING_CASH, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        //AT_COUNTER_FINAL --> COUNTER
        nodeTransition[(int)NpcStateTransitions.AT_COUNTER_FINAL] = true;
        adjMatrix[(int)NpcState.AT_COUNTER_FINAL, (int)NpcState.AT_COUNTER] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.WALK_TO_UNRESPAWN] = true;
        adjMatrix[(int)NpcState.AT_COUNTER_FINAL, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = true;
        adjMatrix[(int)NpcState.AT_COUNTER_FINAL, (int)NpcState.IDLE] = new StateNodeTransition((bool[])nodeTransition.Clone());
        Array.Fill(nodeTransition, false);

        return new StateMachine<NpcState, NpcStateTransitions>(adjMatrix, NpcState.IDLE, ID);
    }
}
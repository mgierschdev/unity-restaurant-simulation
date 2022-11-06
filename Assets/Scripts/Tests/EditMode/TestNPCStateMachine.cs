using System;
using NUnit.Framework;

public class TestNPCStateMachine
{

    [Test]
    public void TestNPCStateMachineClient()
    {
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

        // Keeps the posible transition bewteen the nodes
        StateNodeTransition[,] adjMatrix = new StateNodeTransition[Enum.GetNames(typeof(NpcState)).Length, Enum.GetNames(typeof(NpcState)).Length];
        int[] nodeTransition = new int[Enum.GetNames(typeof(NpcStateTransitions)).Length];

        //IDLE -> Other
        nodeTransition[(int)NpcStateTransitions.TABLE_AVAILABLE] = 1;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_TO_TABLE] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.WANDER] = 1;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WANDER] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //WANDER -> Other
        nodeTransition[(int)NpcStateTransitions.NPC_IS_NOT_MOVING] = 1;
        adjMatrix[(int)NpcState.WANDER, (int)NpcState.IDLE] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.WANDER_TIME] = 10;
        adjMatrix[(int)NpcState.WANDER, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //WALKING_TO_TABLE -> Other
        nodeTransition[(int)NpcStateTransitions.TABLE_AVAILABLE] = 1;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.AT_TABLE] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //AT_TABLE -> Other
        nodeTransition[(int)NpcStateTransitions.NPC_IS_NOT_MOVING] = 1;
        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WAITING_TO_BE_ATTENDED] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //WAITING_TO_BE_ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.BEING_ATTENDED] = 1;
        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.BEING_ATTENDED] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //BEING_ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.ATTENDED] = 1;
        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.ATTENDED] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);

        //ATTENDED -> Other
        nodeTransition[(int)NpcStateTransitions.ORDER_SERVED] = 1;
        nodeTransition[(int)NpcStateTransitions.TABLE_MOVED] = 1;
        adjMatrix[(int)NpcState.ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = new StateNodeTransition((int[])nodeTransition.Clone());
        Array.Fill(nodeTransition, 0);
    }

}
using System;
using System.Drawing;
using NUnit.Framework;
using UnityEngine;

public class TestNPCStateMachine
{
    private MinBinaryHeap heap;

    [Test]
    public void TestNPCStateMachineClient()
    {

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

        // Keeps the posible transition bewteen the nodes
        int[,] adjMatrix = new int[Enum.GetNames(typeof(NpcState)).Length, Enum.GetNames(typeof(NpcState)).Length];

        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_TO_TABLE] = 1;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WANDER] = 1;
        adjMatrix[(int)NpcState.IDLE, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.WANDER, (int)NpcState.IDLE] = 1;
        adjMatrix[(int)NpcState.WANDER, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.AT_TABLE] = 1;
        adjMatrix[(int)NpcState.WALKING_TO_TABLE, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WAITING_TO_BE_ATTENDED] = 1;
        adjMatrix[(int)NpcState.AT_TABLE, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.BEING_ATTENDED] = 1;
        adjMatrix[(int)NpcState.WAITING_TO_BE_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.ATTENDED] = 1;
        adjMatrix[(int)NpcState.BEING_ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = 1;

        adjMatrix[(int)NpcState.ATTENDED, (int)NpcState.WALKING_UNRESPAWN] = 1;

        GraphGenerator<NpcState> graphGenerator = new GraphGenerator<NpcState>(adjMatrix);
        graphGenerator.Draw();

        // stateMachine.printStateMachine();
    }

}
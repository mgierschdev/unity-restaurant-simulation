using System.Collections.Generic;

public class StateMachineNode
{
    public List<StateMachineNode> nextStates;

    public StateMachineNode(){
        nextStates = new List<StateMachineNode>();
    }
}
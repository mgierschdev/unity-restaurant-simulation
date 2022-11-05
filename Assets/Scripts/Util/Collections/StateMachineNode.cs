using System.Collections.Generic;

public class StateMachineNode
{
    public List<StateMachineNode> transitionStates;

    public StateMachineNode(){
        transitionStates = new List<StateMachineNode>();
    }
}
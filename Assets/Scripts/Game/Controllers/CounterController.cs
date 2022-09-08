using UnityEngine;
using UnityEngine.Rendering;

// Name sof Objects should be unique
public class CounterController : BaseObjectController
{
    GameGridObject counter;

    private void Start()
    {   
        gameGridObject.Type = ObjectType.NPC_COUNTER;
        Type = ObjectType.NPC_COUNTER;

        if (!Util.IsNull(Grid, "CounterController/IsometricGridController null"))
        {
            Grid.SetGridObject(counter);
        }
    }
}
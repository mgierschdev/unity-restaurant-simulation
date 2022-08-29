using UnityEngine;

public class PlayerController : GameObjectMovementBase
{
    // For Handling non-physics related objects
    private void Update()
    {
        // Player Movement on click
        if (Settings.PLAYER_WALK_ON_CLICK)
        {
            MouseOnClick();
        }
        // Player Moving on long click/touch
        MovingOnLongtouch();
    }

    // Called every physics step, Update called every frame
    private void FixedUpdate()
    {   
        UpdateTargetMovement();
        UpdatePosition();
    }
}
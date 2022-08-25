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
        body.angularVelocity = 0;
        body.rotation = 0;

        // In case of keyboard
        if (Settings.PLAYER_WALK_WITH_KEYBOARD)
        {
            body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;
        }
        
        UpdateTargetMovement();
        UpdatePosition();
    }
}
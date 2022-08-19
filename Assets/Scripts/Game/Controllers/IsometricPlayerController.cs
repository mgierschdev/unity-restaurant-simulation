using UnityEngine;
using System.Collections.Generic;

public class IsometricPlayerController : GameIsometricMovement
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

        // Moves the character depending on the pendingQueue and next target
        UpdateTargetMovement();

        // Updating position in the Grid
        UpdatePosition();
    }
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (Settings.DEBUG_ENABLE)
    //     {
    //         //Debug.Log("Colliding with: " + other.GetType() + " " + other.ToString());
    //     }
    // }
}
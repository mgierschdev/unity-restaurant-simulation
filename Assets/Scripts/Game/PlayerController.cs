using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controls player properties
// Attached to: Player Object
public class PlayerController : GameObjectMovementBase
{
    //MovingOnLongtouch(), Long click or touch vars
    private bool clicking;
    private bool isLongClick;
    private float clickingTime; // To keep the coung if longclick
    private float longClickDuration = 0.5f; // In seconds

    private void Start()
    {
        Type = ObjectType.PLAYER;
        Speed = Settings.PLAYER_MOVEMENT_SPEED;
        // Long Click
        clickingTime = 0;
        clicking = false;
        isLongClick = false;
    }

    private void Update()
    {
        body.angularVelocity = 0;
        body.rotation = 0;

        // In case of keyboard
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;

        // Controls the state of the first and long click
        ClickControl();

        // Moves the character depending on the pendingQueue and next target
        UpdateTargetMovement();

        // Player Movement
        MouseOnClick();

        // Player Moving on long click/touch
        MovingOnLongtouch();

        // Updating position in the Grid
        UpdatePosition();
    }

    private void ClickControl()
    {
        // first click 
        if (Input.GetMouseButtonDown(0))
        {
            clickingTime = 0;
            clicking = true;
        }

        // Resets isLongClick
        if (isLongClick && clickingTime < longClickDuration)
        {
            isLongClick = false;
        }
    }

    private void MovingOnLongtouch()
    {
        if (clicking && Input.GetKey(KeyCode.Mouse0))
        {
            clickingTime += Time.deltaTime;

            if (clickingTime >= longClickDuration)
            {
                ResetMovementIfMoving();

                isLongClick = true;
                Vector3 mousePosition = Util.GetMouseInWorldPosition();
                Vector3 delta = mousePosition - transform.position;

                float radians = Mathf.Atan2(delta.x, delta.y);
                float degrees = radians * Mathf.Rad2Deg;

                // normalizing -180-180, 0-360
                if (degrees < 0)
                {
                    degrees += 360;
                }

                AddMovement(Util.GetVectorFromDirection(Util.GetDirectionFromAngles(degrees)));

                if (Settings.DEBUG_ENABLE)
                {
                    Debug.DrawLine(transform.position, mousePosition, Color.blue);
                }
            }
        }
    }

    private void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0) && !isLongClick)
        {
            ResetMovementIfMoving();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            List<Node> path = GameGrid.GetPath(new int[] { (int)X, (int)Y }, new int[] { mousePositionVector.x, mousePositionVector.y });
            Util.AddPath(path, GameGrid, pendingMovementQueue);

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
        }
    }
}
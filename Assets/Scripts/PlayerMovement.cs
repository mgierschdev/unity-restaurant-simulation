using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    // To get the reference from the object
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // To track the key presses, left, right, the update is checked on every frame
    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal"), body.velocity.y); // To assign how fast or player is moving and which direction (speed 2 directions)

    }
}

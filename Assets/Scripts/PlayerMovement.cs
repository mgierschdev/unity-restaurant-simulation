using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField]private float speed;

    // To get the reference from the object
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // To track the key presses, left, right, the update is checked on every frame
    private void Update()
    {
        // To assign how fast or player is moving and which direction (speed 2 directions)
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y); 

        // To be able to jump
        if (Input.GetKey(KeyCode.Space))
        {
            body.velocity = new Vector2(body.velocity.x, speed);
        }         

        
    }
}

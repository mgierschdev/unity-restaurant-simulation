using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
     //Physics2D.gravity = Vector2.zero; In order to turn off gravity
     //Debug.log(" "); In order to print in the console

    [SerializeField]private float speed; // SerializeField in order to change field var from the UI 

    // To get the reference from the object
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

    }

    // To track the key presses, left, right, the update is checked on every frame
    private void Update()
    {
 
        float horizontalInput = Input.GetAxis("Horizontal");

        // To assign how fast or player is moving and which direction (speed 2 directions)
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y); 


        // Changes the character direction depending on the key press
        if(horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }

        // To be able to jump
        if (Input.GetKey(KeyCode.Space))
        {
            body.velocity = new Vector2(body.velocity.x, speed);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField]
    private float movementSpeed = 20; // SerializeField in order to change field var from the UI

    // To get the reference from the object
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed; ; //normalize to get a consistent value
        // In case of collision




        // check to flip, rotate
        //bool flipped = movement.x < 0;
        //this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f)); 
    }
    
}

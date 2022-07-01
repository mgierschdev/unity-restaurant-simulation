using UnityEngine;

// Controls player properties
// Attached to: Player Object
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField]
    private float movementSpeed = Settings.PLAYER_MOVEMENT_SPEED;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed;

        // To avoid rotation
        body.angularVelocity = 0;
        body.rotation = 0;
    }
    
}

using UnityEngine;

// Controls player properties
// Attached to: Player Object
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = Settings.PLAYER_MOVEMENT_SPEED;
    private Vector2 movement;
    private Vector3 position;
    Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        body.angularVelocity = 0;
        body.rotation = 0;
    }

    public void SetPosition(Vector3 position){
       transform.position = position;
    }
}

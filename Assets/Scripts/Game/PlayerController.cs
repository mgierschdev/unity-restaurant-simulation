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
    private GameGridController gameGrid;
    private int x;
    private int y;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // Getting game grid
        gameGrid = GameObject.Find(Settings.CONST_GAME_GRID).gameObject.GetComponent<GameGridController>();  

    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        body.angularVelocity = 0;
        body.rotation = 0;

        // Updating position in the Grid
        UpdatePositionInGrid();
    }

    public void SetPosition(Vector3 position){
       transform.position = position;
    }

    private void UpdatePositionInGrid(){
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
    }
}

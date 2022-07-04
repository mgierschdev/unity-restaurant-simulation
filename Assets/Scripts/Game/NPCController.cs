using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    //[SerializeField]
   // private int currentDirection;
    //[SerializeField]
    //private float movementSpeed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
    //[SerializeField]
    //private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};

    enum direction
    {
        IDLE = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 3,
        RIGHT = 4,
        UPLEFT = 5,
        UPRIGHT = 6,
        DOWNLEFT = 7,
        DOWNRIGHT = 8
    }

    private EnergyBar energyBar;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
       // currentDirection = 0;

        // Energy bar
        energyBar = gameObject.transform.Find("EnergyBar").gameObject.GetComponent<EnergyBar>();
        energyBar.setInactive();
    }

    private void FixedUpdate()
    {
        // EneryBar controller, only if it is active
        if (energyBar.isActive())
        {
            if (currentEnergy > 0)
            {
                currentEnergy -= Time.deltaTime * 20f;
                energyBar.SetEnergy((int)currentEnergy);
            }
            else
            {
                currentEnergy = 100;
                energyBar.SetEnergy(Settings.NPC_DEFAULT_ENERGY);
            }
        }

        body.angularVelocity = 0;
        body.rotation = 0;
    }

    private void move(direction d)
    {
        Vector3 dir;

        if (d == direction.LEFT)
        {
            dir = Vector3.left;
        }else if(d == direction.RIGHT)
        {
            dir = Vector3.right;
        }else if(d == direction.UP)
        {
            dir = Vector3.up;
        }else if(d == direction.DOWN)
        {
            dir = Vector3.down;
        }else if (d == direction.DOWNLEFT)
        {
            dir = Quaternion.Euler(45, 0, 0) * Vector3.down; // not working
        }else if(d == direction.DOWNRIGHT)
        {
            dir = Quaternion.Euler(-45, 0, 0) * Vector3.down;
        }else if(d == direction.UPLEFT)
        {
            dir = Quaternion.Euler(0, 45, 0) * Vector3.forward;
        }else if(d == direction.UPRIGHT)
        {
            dir = new Vector3(.5f, 0, .5f);
        }
        else
        {
            //in case it is iddle
            dir = new Vector3(0,0,0);
        }

        transform.Translate(dir, Space.World);
    }

    private void OnMouseDown()
    {
        //movementSpeed += Settings.NPC_DEFAULT_MOVEMENT_INCREASE_ON_CLICK;
        move(direction.UPRIGHT);

    }
}
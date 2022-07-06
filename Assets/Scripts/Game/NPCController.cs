using System.Collections;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    private EnergyBar energyBar;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;
    private Queue movementQueue;
    private Vector3 currentTargetPosition;
    private float speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();
        // Movement Queue
        movementQueue = new Queue();

        // Enery Bar
        if (Settings.NPC_ENERGY_ENABLED)
        {
            energyBar.SetActive();

        }
        else
        {
            energyBar.SetInactive();
        }
    }

    private void Update()
    {
        // EneryBar controller, only if it is active
        if (energyBar.IsActive())
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

        // Handling player movement through a queue
        if(currentTargetPosition == transform.position && movementQueue.Count != 0){
            movementQueue.Dequeue();
        }

        if(movementQueue.Count != 0){
            currentTargetPosition = (Vector3) movementQueue.Peek();
            Debug.Log(movementQueue.Peek());
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.deltaTime);
        }
        
    }

    private Vector3 Move(MoveDirection d)
    {
        //in case it is MoveDirection.IDDLE do nothing
        Vector3 dir = transform.position;

        if (d == MoveDirection.LEFT)
        {
            dir = Vector3.left;
        }else if(d == MoveDirection.RIGHT)
        {
            dir = Vector3.right;
        }else if(d == MoveDirection.UP)
        {
            dir = Vector3.up;
        }else if(d == MoveDirection.DOWN)
        {
            dir = Vector3.down;
        }else if (d == MoveDirection.DOWNLEFT)
        {
            dir = new Vector3(-1, -1, 0);
        }else if(d == MoveDirection.DOWNRIGHT)
        {
             dir = new Vector3(1, -1, 0);
        }else if(d == MoveDirection.UPLEFT)
        {
            dir = new Vector3(-1, 1, 0);
        }else if(d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(1, 1, 0);
        }
        return dir;
    }

    private void OnMouseDown()
    {
        AddMovement(MoveDirection.DOWN);
    }

    public void ActivateEnergyBar()
    {
        energyBar.SetActive();
    }

    public void AddEnergyBar()
    {
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();
    }
    
    public void AddMovement(MoveDirection direction)
    {
        Vector3 nextTarget =  Move(direction) + transform.position;
        if(movementQueue.Count == 0){
            currentTargetPosition = nextTarget;
        }
        movementQueue.Enqueue(nextTarget);
    }

    public void SetPosition(Vector3 position)
    {
       transform.position = position;
    }

    public void SetSpeed(float speed){
        this.speed = speed;
    }

    public EnergyBar GetEnergyBar()
    {
        return this.energyBar;
    }

    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

}
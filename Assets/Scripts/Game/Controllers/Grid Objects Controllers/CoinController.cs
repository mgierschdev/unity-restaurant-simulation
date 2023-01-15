using UnityEngine;

public class CoinController : MonoBehaviour
{
    private bool consume;
    private Vector3 targetPosition;
    [SerializeField]
    private float coinSpeed = 4f;

    private void Awake()
    {
        consume = false;
        transform.name = BussGrid.GetObjectID(ObjectType.COIN);
    }

    private void Update()
    {
        if (consume)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, coinSpeed * Time.fixedDeltaTime);
            CheckIfAtTarget();
        }
        else if (Input.GetMouseButtonDown(0) && IsClickingSelf())
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(PlayerData.GetMoneyTextPosition());
            SetTargetPosition(target);
            // TODO: Make it follow the reference of the actual gameObject rather than the coord in that way it will follow the coord even is the player moves with the perspective hand
            // Use some kind of lerp or curve easing 
            consume = true;
        }
    }

    private void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
    }

    private void CheckIfAtTarget()
    {
        if (Util.IsAtDistanceWithObject(transform.position, targetPosition))
        {
            //We call here the increase coin method PlayerData
            Destroy(gameObject);
        }
    }

    private bool IsClickingSelf()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
}
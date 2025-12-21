using Game.Grid;
using Game.Players;
using UnityEngine;
using Util;

// Helper to build the graph: https://cubic-bezier.com/
namespace Game.Controllers.Grid_Objects_Controllers
{
    /**
     * Problem: Animate collectible coins toward the UI.
     * Goal: Move a coin to the money display when clicked.
     * Approach: Use a cubic Bezier curve interpolation.
     * Time: O(1) per frame.
     * Space: O(1).
     */
    public class CoinController : MonoBehaviour
    {
        private bool _consume;
        private Transform _target;
        private readonly float _coinSpeed = 0.5f;
        private Vector3 _startPoint, _targetPosition;
        private Vector3 _tangent1 = new Vector3(0, 1), _tangent2 = new Vector3(1, 0);
        private float _interpolationTime;

        private void Awake()
        {
            var objectTransform = transform;
            _consume = false;
            objectTransform.name = BussGrid.GetObjectID(ObjectType.Coin);
            _startPoint = objectTransform.position;
            _interpolationTime = 0;
        }

        private void Update()
        {
            if (_consume)
            {
                if (Camera.main != null)
                {
                    _targetPosition = Camera.main.ScreenToWorldPoint(_target.transform.position);
                }

                _interpolationTime += Time.deltaTime * _coinSpeed;
                transform.position = CubicBezier(_interpolationTime,
                    _startPoint,
                    _startPoint + _tangent1,
                    _targetPosition + _tangent2,
                    _targetPosition);

                // Vector3 a = new Vector3(transform.position.x, transform.position.y, 0);
                // Vector3 b = new Vector3(targetPosition.x, targetPosition.y, 0);
                // Debug.Log(transform.name + " Target Distance " + Vector3.Distance(a, b));
                // Debug.DrawLine(prevPosition, transform.position, Color.cyan, 15f);
                // prevPosition = transform.position;
                // Vector3.Lerp(transform.position, targetPosition, coinSpeed * Time.fixedDeltaTime);

                CheckIfAtTarget();
            }
            else if (Input.GetMouseButtonDown(0) && IsClickingSelf())
            {
                _target = PlayerData.GetMoneyTextTransform();
                // Use some kind of lerp or curve easing 
                _consume = true;
            }
        }

        private void CheckIfAtTarget()
        {
            var position = transform.position;
            var a = new Vector3(position.x, position.y, 0);
            var b = new Vector3(_targetPosition.x, _targetPosition.y, 0);

            if (Vector3.Distance(a, b) <= 0.1)
            {
                //We call here the increase coin method PlayerData
                Destroy(gameObject);
            }
        }

        private bool IsClickingSelf()
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(Util.Util.GetMouseInWorldPosition());
            foreach (Collider2D r in hits)
            {
                if (r.name.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        // returns the value of a Bazier curve given 3 parameters 
        // Time 1 , and set of point from p0  to p3 (P(n)), where p0 is the start and the last the end of the curve
        // Definition: https://en.wikipedia.org/wiki/Bézier_curve
        // Scale 0 - 1 where (0,0) is the left bot place on the screen and (1,1) the top right corner

        public static Vector3 CubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Cubic Bezier formula
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
    }
}

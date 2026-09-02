using UnityEngine;

public class AIManagement : MonoBehaviour
{
    [SerializeField] private float stopDistance = 3f;
    [SerializeField] private AIPerception perception;
    [SerializeField] private AIMover mover;
    private bool hasTarget;
    private float sqrStopDistance; // 제곱 거리로 캐싱

    private void Awake()
    {
        RecalculateCache();
    }

    private void OnValidate() => RecalculateCache();

    private void RecalculateCache()
    {
        sqrStopDistance = stopDistance * stopDistance;
    }

    private void Update()
    {
        if (!hasTarget)
        {
            if (perception.CanSeePlayer()) hasTarget = true;
            else
            {
                hasTarget = false;
                return;
            }
        }
        Transform player = perception.player;
        if (player == null)
        {
            mover.Stop();
            return;
        }
        if (IsWithinStopDistance(player.position))
        {
            mover.Stop();
            mover.ForwardRotate(player.position);
        }
        else
        {
            mover.EnemyMove(player.position);
        }
    }
    // 거리 제곱 계산을 별도의 메서드로 나누어둠 (나중에 사거리 체크 편함)
    private bool IsWithinStopDistance(Vector3 targetPosition)
    {
        float sqrDist = (targetPosition - transform.position).sqrMagnitude;
        return sqrDist <= sqrStopDistance;
    }
}

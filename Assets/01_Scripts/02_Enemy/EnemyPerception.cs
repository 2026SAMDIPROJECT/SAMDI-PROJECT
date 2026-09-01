using Unity.Netcode;
using UnityEngine;

public class AIPerception : NetworkBehaviour
{
    // 임의로 정해진 수치(나중에 변경)
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform eyePoint;
    [SerializeField] private float perceptionInterval = 0.15f;
    [SerializeField] private float playerRefreshInterval = 0.5f;
    public Transform player {get; private set;}
    private float sqrDetectionRange;
    private float sqrCosHalfViewAngle;
    private float nextCheckTime;
    private float nextPlayerRefreshTime;
    private bool cachedResult;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        if (eyePoint == null) eyePoint = transform;
        RecalculateCache();
    }
    private void OnValidate() => RecalculateCache();
    private void RecalculateCache()
    {
        sqrDetectionRange = detectionRange * detectionRange;
        float clampView = Mathf.Clamp(viewAngle, 0f, 180f);
        float cos = Mathf.Cos(clampView * 0.5f * Mathf.Deg2Rad);
        sqrCosHalfViewAngle = cos * cos; // 제곱근, 정규화 삭제를 위해 사용
    }
    private void Update()
    {
        // player가 없거나 주기적으로 갱신
        if (Time.time >= nextPlayerRefreshTime)
        {
            nextPlayerRefreshTime = Time.time + playerRefreshInterval;
            if (player == null) 
                player = PlayerRegistry.Instance?.GetNearestPlayer(transform.position);
        }
    }
    // 적 시점 안에 들어왔는지 판별
    public bool PlayerInRange()
    {
        if (player == null) return false;
        return (player.position - transform.position).sqrMagnitude <= sqrDetectionRange;
    }
    // 적이 플레이어를 감지했는지 판별
    public bool CanSeePlayer()
    {
        if (player == null) return false;
        if (Time.time < nextCheckTime) return cachedResult; // perception

        nextCheckTime = Time.time + perceptionInterval;
        cachedResult = EvaluateCanSeePlayer();
        return cachedResult;
    }
    private bool EvaluateCanSeePlayer()
    {
        // 1. 플레이어와의 거리 검사
        Vector3 rayOrigin = eyePoint.position;
        Vector3 dirToPlayer = player.position - rayOrigin;
        float sqrDistance = dirToPlayer.sqrMagnitude;

        if (sqrDistance > sqrDetectionRange) return false;
        
        // 2. 시야각 검사 (전방위 레이캐스팅 방지 ,전방위 원하면 이줄 제거)
        Vector3 normalizeDir = dirToPlayer.normalized;
        float dot = Vector3.Dot(eyePoint.forward, normalizeDir);
        if (dot < sqrCosHalfViewAngle) return false;
        
        // 3. 레이캐스트
        float distance = Mathf.Sqrt(sqrDistance);

        // obstacleLayer에 TriggerCollider가 섞여 불필요하게 섞일 가능성 있어 미리 명시적으로 지정
        return !Physics.Raycast(rayOrigin, normalizeDir, distance, obstacleLayer, QueryTriggerInteraction.Ignore);
    }
    // 디버깅용 (지워도 됨)
    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null) eyePoint = transform;

        // 1. 감지 범위 원 그리기
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePoint.position, detectionRange);

        // 2. 시야각(FOV) 부채꼴 방향선 그리기
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * eyePoint.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle * 0.5f, 0) * eyePoint.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(eyePoint.position, leftDir * detectionRange);
        Gizmos.DrawRay(eyePoint.position, rightDir * detectionRange);

        // 3. 플레이어 감지 시야선 그리기 (CanSeePlayer 대신 EvaluateCanSeePlayer 사용!)
        if (player != null)
        {
            // 캐시와 타이머에 영향을 주지 않고 순수 감지 여부만 시각화
            bool canSee = EvaluateCanSeePlayer(); 
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(eyePoint.position, player.position);
        }
    }
}

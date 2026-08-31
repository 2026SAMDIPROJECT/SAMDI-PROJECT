using UnityEngine;

public class AIPerception : MonoBehaviour
{
    // 임의로 정해진 수치(나중에 변경)
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform eyePoint;

    public Transform player {get; private set;}

    private float sqrDetectionRange;
    private float cosHalfViewAngle;
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null) player = playerObj.transform;
        if (eyePoint == null) eyePoint = transform;

        RecalculateCache();
    }
    private void OnValidate()
    {
        RecalculateCache();
    }
    private void RecalculateCache()
    {
        sqrDetectionRange = detectionRange * detectionRange;
        cosHalfViewAngle = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
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
        // 1. 플레이어와의 거리 검사
        Vector3 rayOrigin = eyePoint.position;
        Vector3 dirToPlayer = player.position - rayOrigin;
        float sqrDistance = dirToPlayer.sqrMagnitude;

        if (sqrDistance > sqrDetectionRange) return false;
        
        // 2. 시야각 검사 (전방위 레이캐스팅 방지 ,전방위 원하면 이줄 제거)
        Vector3 normalizeDir = dirToPlayer.normalized;
        float dot = Vector3.Dot(eyePoint.forward, normalizeDir);
        if (dot < cosHalfViewAngle) return false;
        
        // 3. 레이캐스트
        float distance = Mathf.Sqrt(sqrDistance);
        // obstacleLayer에 TriggerCollider가 섞여 불필요하게 섞일 가능성 있어 미리 명시적으로 지정
        return !Physics.Raycast(rayOrigin, normalizeDir, distance, obstacleLayer, QueryTriggerInteraction.Ignore);
    }
}

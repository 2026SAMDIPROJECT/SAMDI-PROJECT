using Unity.Netcode;
using UnityEngine;

public class AIPerception : NetworkBehaviour
{
    [SerializeField] private EnemySpec spec;
    [Header("레이어 / 적 감지 설정")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform eyePoint;
    [SerializeField] private float perceptionInterval = 0.15f;
    [SerializeField] private float playerRefreshInterval = 0.5f;
    public Transform player{get; private set;}
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
        RecalculateCache();
    }
    private void OnValidate() => RecalculateCache();
    private void RecalculateCache() // 어차피 spec은 같음(프리펩으로 적을 더 생성하더라도 그 적의 고유 spec이 있음)
    {
        sqrDetectionRange = spec.detectionRange * spec.detectionRange;
        float clampView = Mathf.Clamp(spec.viewAngle, 0f, 180f); // 적 최대 시야각을 180도로 고정
        float cos = Mathf.Cos(clampView * 0.5f * Mathf.Deg2Rad);
        sqrCosHalfViewAngle = cos * cos; // 제곱근, 정규화 삭제를 위해 사용
    }
    private void Update()
    {
        // player가 없거나 주기적으로 갱신
        if (Time.time >= nextPlayerRefreshTime)
        {
            nextPlayerRefreshTime = Time.time + playerRefreshInterval;
            //이전 :: if(player == null)
            player = PlayerRegistry.Instance?.GetNearestPlayer(transform.position); // 오류 :: 이 함수 실행 안 해서 가장 먼저 나오는 호스트만 쫒아감
        }
    }
    
    // 적이 플레이어를 감지했는지 판별 감지
    public bool CanSeePlayer()
    {
        if (player == null) return false;
        if (Time.time < nextCheckTime) return cachedResult;

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
        
        // 2. 시야각 검사 (전방위 레이캐스팅 방지 ,전방위 원하면 이 단계 제거)
        float dot = Vector3.Dot(eyePoint.forward, dirToPlayer);
        if (dot < 0) return false; // 음수면 아예 뒤쪽 -> 어차피 앞에서 180도 조정을 해 놔서 넘어가면 안 되니 아래 코드 실행 전에 반환함
        
        if((dot * dot) < sqrCosHalfViewAngle * sqrDistance) return false; // sqrMagnitude는 제곱한 값을 주고 sqrCosHalfViewAngle도 제곱한 값임. 따라서 dot도 제곱한 값으로 비교해야 함.
        // 3. 레이캐스트

        // obstacleLayer에 TriggerCollider가 섞여 불필요하게 섞일 가능성 있어 미리 명시적으로 지정
        return !Physics.Raycast(rayOrigin, dirToPlayer, spec.detectionRange, obstacleLayer, QueryTriggerInteraction.Ignore); // 성능 안 좋아지면 spec.detectionRange도 캐싱할것(안 함)
    }
}

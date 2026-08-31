// TODO : 전범위 계산은 연산비용 커서 나중에 줄여볼 예정
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    // 임의로 정해진 수치(나중에 변경)
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform eyePoint;

    public Transform player {get; private set;}

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null) player = playerObj.transform;
        if (eyePoint == null) eyePoint = transform;
    }
    // 적 시점 안에 들어왔는지 판별
    public bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }
    // 적이 플레이어를 감지했는지 판별
    public bool CanSeePlayer()
    {
        if (!PlayerInRange()) return false;

        Vector3 dirToPlayer = (player.position - eyePoint.position).normalized;
        float distance = Vector3.Distance(eyePoint.position, player.position);

        return !Physics.Raycast(eyePoint.position, dirToPlayer, distance, wallLayer);
    }
}

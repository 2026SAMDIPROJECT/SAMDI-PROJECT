using UnityEngine;
using UnityEngine.AI;

public class AIMover : MonoBehaviour
{
    [SerializeField]private NavMeshAgent agent;
    public void EnemyMove(Vector3 targetPosition)
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPosition);
        }
    }
    public void Stop()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
    }
    public void ForwardRotate(Vector3 targetPosition)
    {
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        targetDirection.y = 0f;

        if (targetDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(targetDirection);
    }
}

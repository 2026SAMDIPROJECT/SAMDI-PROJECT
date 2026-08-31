using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private AIPerception perception;
    private AIMover mover;
    private bool hasTarget;

    private void Awake()
    {
        perception = GetComponent<AIPerception>();
        mover = GetComponent<AIMover>();
    }
    private void Update()
    {
        if (!hasTarget)
        {
            if (perception.CanSeePlayer()) hasTarget = true;
            else
            {
                mover.Stop();
                return;
            }
        }
        Transform player = perception.player;
        if (player == null)
        {
            hasTarget = false;
            mover.Stop();
            return;
        }
        else
        {
            mover.EnemyMove(player.position);
        }
    }
}

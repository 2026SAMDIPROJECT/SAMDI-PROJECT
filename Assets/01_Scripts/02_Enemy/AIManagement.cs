using UnityEngine;

public class AIManagement : MonoBehaviour
{
    private AIPerception perception;
    private AIMover mover;

    private void Awake()
    {
        perception = GetComponent<AIPerception>();
        mover = GetComponent<AIMover>();
    }
    private void Update()
    {
        Transform player = perception.player;
        if (player == null)
        {
            mover.Stop();
            return;
        }
        if (perception.CanSeePlayer())
        {
            mover.ForwardRotate(player.position);
            mover.EnemyMove(player.position);
        }
        else
        {
            mover.Stop();
        }
    }
}

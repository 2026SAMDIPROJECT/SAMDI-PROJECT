using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpec", menuName = "Scriptable Objects/EnemySpec")]
public class EnemySpec : ScriptableObject
{
    public float detectionRange = 20f;
    public float viewAngle = 120f;
}

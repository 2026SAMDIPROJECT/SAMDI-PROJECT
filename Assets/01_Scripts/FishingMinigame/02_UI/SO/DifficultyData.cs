using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "DifficultFishing", menuName = "Scriptable Objects/DifficultFishing")]
public class DifficultyData : ScriptableObject
{
    [Header("플레이어 그린 바")]
    [Range(0.1f, 0.5f)] public float barSize = 0.15f;
    public float liftAccel = 2.0f;
    public float gravity = 2.0f;
    public float bottomBounce = 1.0f;

    [Header("진행도")]
    [Range(0f, 1f)] public float startFill = 0.2f;
    public float gainPerSec = 0.15f;
    public float lossPerSec = 0.1f;

    [Header("QTE")]
    public Vector2 qteInterval = new Vector2(6f, 8f);
    public Vector2Int qteCount = new Vector2Int(3,5);
    [Range(0f, 1f)]public float qteSlowScale = 0.1f;
    public float speedEaseTime = 0.3f; // 속도가 바뀌는데 걸리는 시간
    public float qteTimeLimit = 1.2f;
    public float qteFailPenalty = 0.2f;
    public Key[] qteKeys = { Key.W, Key.A, Key.S, Key.D};
}

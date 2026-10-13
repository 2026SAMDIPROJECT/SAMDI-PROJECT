using UnityEngine;

[CreateAssetMenu(fileName = "DifficultFishing", menuName = "Scriptable Objects/DifficultFishing")]
public class DifficultFishingData : ScriptableObject
{
    [Header("플레이어 그린 바")]
    [Range(0.1f, 0.5f)] public float barSize = 0.15f;
    public float liftAccel = 2.0f;
    public float gravity = 2.0f;
    public float bottomBounce = 1.0f;

    [Header("물고기")]
    public float fishSmoothTime = 0.5f;
    public float fishMaxSpeed = 2f;
    public Vector2 retargetInterval = new Vector2(0.4f, 1.2f);

    [Header("진행도")]
    [Range(0f, 1f)] public float startFill = 0.2f;
    public float gainPerSec = 0.15f;
    public float lossPerSec = 0.1f;
}

using UnityEngine;
public enum Quality
{
    Common,
    Rare,
    Epic
}

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ScriptableObject
{
    [Header("물고기")]
    public float fishSmoothTime = 0.5f;
    public float fishMaxSpeed = 2f;
    public Quality quality;
    public int fishScore = 1;
    public Vector2 retargetInterval = new Vector2(0.4f, 1.2f);
}

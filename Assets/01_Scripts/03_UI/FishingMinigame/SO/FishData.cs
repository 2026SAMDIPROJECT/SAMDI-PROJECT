using Unity.Burst.Intrinsics;
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
    public int fishScore
    {
        get
        {
            return quality switch
            {
                Quality.Common => 1,
                Quality.Rare => 6,
                Quality.Epic => 20,
                _ => 0
            };
        }
    }
    public Vector2 retargetInterval = new Vector2(0.4f, 1.2f);
}

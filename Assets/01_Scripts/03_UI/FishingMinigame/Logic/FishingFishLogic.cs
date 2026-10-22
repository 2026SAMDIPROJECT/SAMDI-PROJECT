using UnityEngine;

public class FishingFishLogic
{
    private readonly FishData fishData;
    private float retargetTimer;
    private float fishTarget;
    public float fishPos {get; private set;}
    private float fishVelocity;
    public FishingFishLogic(FishData fishData)
    {
        this.fishData = fishData;

        fishPos = 0.5f;
        fishTarget = 0.4f;
    }
    public void UpdateFish(float dt)
    {
        retargetTimer -= dt;
        if (retargetTimer <= 0f)
        {
            fishTarget = Random.value;
            retargetTimer = Random.Range(fishData.retargetInterval.x, fishData.retargetInterval.y);
        }
        fishPos = Mathf.SmoothDamp(fishPos,fishTarget, ref fishVelocity, fishData.fishSmoothTime, fishData.fishMaxSpeed, dt);
    }
}

using UnityEngine;

public class FishingBarLogic
{
    private readonly DifficultyData difData;
    public float barBottom {get; private set;}
    public float barSize => difData.barSize;
    private float barVelocity;

    public FishingBarLogic(DifficultyData difData) => this.difData = difData;
    public void UpdateBar(float dt, bool holding)
    {
        barVelocity += (holding ? difData.liftAccel : -difData.gravity) * dt;
        barBottom += barVelocity * dt;

        float max = 1f - barSize;
        if (barBottom <= 0f)
        {
            barBottom = 0f;
            barVelocity = -barVelocity * difData.bottomBounce;
        }
        else if (barBottom >= max)
        {
            barBottom = max;
            barVelocity = 0f;
        }
    }
}

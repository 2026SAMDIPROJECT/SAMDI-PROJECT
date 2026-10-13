using System;
using UnityEngine;

public class FishingMinigameLogic
{
    private readonly DifficultFishingData data;

    public float barBottom {get; private set;}
    public float barSize => data.barSize;
    public float fishPos {get; private set;}
    public float progress {get; private set;}

    public event Action<bool> OnFinished;

    private float barVelocity;
    private float fishVelocity;
    private float fishTarget;
    private float retargetTimer;
    private bool finished;

    public FishingMinigameLogic(DifficultFishingData data)
    {
        this.data = data;
        fishPos = 0.5f;
        fishTarget = 0.4f;
        progress = data.startFill;
    }
    private void UpdateBar(float dt, bool holding)
    {
        barVelocity += (holding ? data.liftAccel : -data.gravity) * dt;
        barBottom += barVelocity * dt;

        float max = 1f - data.barSize;
        if (barBottom <= 0f)
        {
            barBottom = 0f;
            barVelocity = -barVelocity * data.bottomBounce;
        }
        else if (barBottom >= max)
        {
            barBottom = max;
            barVelocity = 0f;
        }
    }
    private void UpdateFish(float dt)
    {
        retargetTimer -= dt;
        if (retargetTimer <= 0f)
        {
            fishTarget = UnityEngine.Random.value;
            retargetTimer = UnityEngine.Random.Range(data.retargetInterval.x, data.retargetInterval.y);
        }
        fishPos = Mathf.SmoothDamp(fishPos, fishTarget, ref fishVelocity, data.fishSmoothTime, data.fishMaxSpeed, dt);
    }
    private void UpdateProgress(float dt)
    {
        bool overlap = fishPos >= barBottom && fishPos <= barBottom + data.barSize;
        progress += (overlap? data.gainPerSec : -data.lossPerSec) * dt;
        progress = Mathf.Clamp01(progress);

        if(progress >= 1f) Finish(true);
        else if(progress <= 0f) Finish(false);
    }
    public void Tick(float dt, bool holding)
    {
        if (finished) return;

        UpdateBar(dt, holding);
        UpdateFish(dt);
        UpdateProgress(dt);
    }
    private void Finish(bool finish)
    {
        finished = true;
        OnFinished?.Invoke(finish);
    }
}

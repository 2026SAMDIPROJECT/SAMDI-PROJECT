using System;
using UnityEngine;

public class FishingMinigameLogic
{
    private readonly FishData fishdata;
    private readonly DifficultyData difData;
    // private readonly float barSize;

    public float barBottom {get; private set;}
    public float barSize => difData.barSize;
    public float fishPos {get; private set;}
    public float progress {get; private set;}

    public event Action<bool> OnFinished;

    private float barVelocity;
    private float fishVelocity;
    private float fishTarget;
    private float retargetTimer;
    private bool finished;

    public FishingMinigameLogic(DifficultyData difData, FishData fishdata)
    {
        this.difData = difData;
        this.fishdata = fishdata;
        fishPos = 0.5f;
        fishTarget = 0.4f;
        progress = difData.startFill;
    }
    private void UpdateBar(float dt, bool holding)
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
    private void UpdateFish(float dt)
    {
        retargetTimer -= dt;
        if (retargetTimer <= 0f)
        {
            fishTarget = UnityEngine.Random.value;
            retargetTimer = UnityEngine.Random.Range(fishdata.retargetInterval.x, fishdata.retargetInterval.y);
        }
        fishPos = Mathf.SmoothDamp(fishPos, fishTarget, ref fishVelocity, fishdata.fishSmoothTime, fishdata.fishMaxSpeed, dt);
    }
    private void UpdateProgress(float dt)
    {
        bool overlap = fishPos >= barBottom && fishPos <= barBottom + barSize;
        progress += (overlap? difData.gainPerSec : -difData.lossPerSec) * dt;
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

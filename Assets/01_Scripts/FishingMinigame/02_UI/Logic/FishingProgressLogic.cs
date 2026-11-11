using System;
using UnityEngine;

public class FishingProgressLogic
{
    private readonly DifficultyData difData;
    public float progress {get; private set;}
    public event Action<bool> OnFinished;
    private bool finished;

    public FishingProgressLogic(DifficultyData difData)
    {
        this.difData = difData;
        progress = difData.startFill;
    }
    public void UpdateProgress(float dt, float barBottom, float fishPos, float barSize)
    {
        if (finished) return;
        
        bool overlap = fishPos >= barBottom && fishPos <= barBottom + barSize;
        progress += (overlap? difData.gainPerSec : -difData.lossPerSec) * dt;
        progress = Mathf.Clamp01(progress);

        if(progress >= 1f) Finish(true);
        else if(progress <= 0f) Finish(false);
    }
    public void AddProgress(float dt)
    {
        if (finished) return;
        progress = Mathf.Clamp01(progress + dt); // 0 ~ 1사이 숫자

        if (progress >= 1f) Finish(true);
        else if (progress <= 0f) Finish(false);
    }
    private void Finish(bool success)
    {
        if (finished) return;

        finished = true;
        OnFinished?.Invoke(success);
    }
}

using System;
using UnityEngine;

public class FishingMinigameLogic // TODO: 로직 분리하여 Tick에서 처리
{
    public float barBottom => barLogic.barBottom;
    public float barSize => barLogic.barSize;
    public float fishPos => fishLogic.fishPos;
    public float progress => progressLogic.progress;
    public event Action<bool> OnFinished;
    private bool finished;
    private FishingBarLogic barLogic;
    private FishingFishLogic fishLogic;
    private FishingProgressLogic progressLogic;

    public FishingMinigameLogic(DifficultyData difData, FishData fishdata)
    {
        barLogic = new FishingBarLogic(difData);
        fishLogic = new FishingFishLogic(fishdata);
        progressLogic = new FishingProgressLogic(difData);

        progressLogic.OnFinished += Finish;
    }
    public void AddProgress(float dt)
    {
        if (finished) return;
        progressLogic.AddProgress(dt);
    }
    public void Tick(float dt, bool holding)
    {
        if (finished) return;

        barLogic.UpdateBar(dt, holding);
        fishLogic.UpdateFish(dt);
        progressLogic.UpdateProgress(dt, barBottom, fishPos, barSize);
    }
    private void Finish(bool finish)
    {
        if (finished) return;

        finished = true;
        OnFinished?.Invoke(finish); 
    }
}

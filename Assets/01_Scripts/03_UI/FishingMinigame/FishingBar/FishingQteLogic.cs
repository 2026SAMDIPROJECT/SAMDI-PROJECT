using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingQteLogic
{
    public readonly float timeLimit;
    public float timeLeft {get; private set;}
    public event Action<bool> OnFinished;
    private readonly Key requiredKey;
    private bool finished;

    public FishingQteLogic(Key requiredKey, float timeLimit)
    {
        this.requiredKey = requiredKey;
        this.timeLimit = timeLimit;
        timeLeft = timeLimit;
    }
    public void Tick(float dt, Key? pressed)
    {
        if (finished) return;
        if (pressed.HasValue)
        {
            Finish(pressed.Value == requiredKey);
            return;
        }
        timeLeft -= dt;
        if (timeLeft <= 0f) Finish(false);
    }
    private void Finish(bool success)
    {
        finished = true;
        OnFinished?.Invoke(success);
    }
}

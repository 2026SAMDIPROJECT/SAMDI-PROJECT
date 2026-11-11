using System;
using UnityEngine.InputSystem;

public class FishingQteLogic // TODO: 분리해서 이 로직에서 Tick으로 처리
{
    public readonly float timeLimit;
    public float timeLeft {get; private set;}
    public int clearedCount {get; private set;}
    public event Action<int> OnKeyCleared; 
    public event Action<bool> OnFinished;
    private readonly Key[] requiredKeys;
    private bool finished;

    public FishingQteLogic(Key[] requiredKeys, float timeLimit)
    {
        this.requiredKeys = requiredKeys;
        this.timeLimit = timeLimit;
        timeLeft = timeLimit;
    }
    public void Tick(float dt, Key? pressed) // Controller에 있는 Update에서 계산 관리
    {
        if (finished) return;
        if (pressed.HasValue) {
            if (pressed.Value != requiredKeys[clearedCount])
            {
                Finish(false);
                return;
            }
            clearedCount++;
            OnKeyCleared?.Invoke(clearedCount);
            if (clearedCount >= requiredKeys.Length)
            {
                Finish(true);
                return;
            }
            timeLeft = timeLimit;
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

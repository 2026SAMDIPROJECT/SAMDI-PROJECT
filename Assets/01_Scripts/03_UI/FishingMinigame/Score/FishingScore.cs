using System;
using UnityEngine;

public class FishingScore
{
    public int totalScore {get; private set;}

    public event Action<int> OnChanged;

    public void ScoreIncrease(int amount)
    {
        totalScore += amount;
        OnChanged?.Invoke(totalScore);
    }
}

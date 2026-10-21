using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // 직렬화
    public class FishDropEntry
    {
        public FishData fishData;
        [Min(0f)]
        public float chance;
        public DifficultyData difficulty;
    }

[CreateAssetMenu(fileName = "FishTableData", menuName = "Scriptable Objects/FishTableData")]
public class FishTableData : ScriptableObject
{
    [SerializeField] private List<FishDropEntry> entries = new();
    public FishDropEntry GetRandomEntry()
    {
        float totalChance = 0f;

        foreach (var entry in entries) // 확률 전체 모으기
        {
            if (entry != null && entry.chance > 0f)
                totalChance += entry.chance;
        }
        if (totalChance <= 0f)
                return null;

        float randomValue = Random.Range(0f, totalChance); // 랜덤값 돌려 한개 뽑기

        foreach (var entry in entries) // 그중 한 개 뽑기
        {
            if (entry == null || entry.chance <= 0f) 
                continue;
            
            if (randomValue < entry.chance)
                return entry;
            
            randomValue -= entry.chance;
        }
        return null;
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

        foreach (var entry in entries)
        {
            if (entry != null && entry.chance > 0f)
                totalChance += entry.chance;
        }
        if (totalChance <= 0f)
                return null;
        float randomValue = Random.Range(0f, totalChance);

        foreach (var entry in entries)
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

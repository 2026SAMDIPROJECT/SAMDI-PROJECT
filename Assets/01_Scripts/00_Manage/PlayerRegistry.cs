using System.Collections.Generic;
using UnityEngine;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance {get; private set;}
    private readonly List<Transform> activePlayers = new List<Transform>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null){ Destroy(gameObject); return;}
    }
    // 플레이어 등록
    public void Register(Transform playerTransform)
    {
        if (!activePlayers.Contains(playerTransform))
            activePlayers.Add(playerTransform);
    }
    // 플레이어 삭제
    public void Unregister(Transform playerTransform)
    {
        activePlayers.Remove(playerTransform);
    }
    // 가장 가까운 플레이어 반환(멀티플레이어)
    public Transform GetNearestPlayer(Vector3 fromPosition)
    {
        Transform nearest = null;
        float bestSqrDist = float.MaxValue;

        foreach (var p in activePlayers)
        {
            if (p == null) continue;
            float sqrDist = (p.position - fromPosition).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                nearest = p;
            }
        }
        return nearest;
    }
}

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WheelchairRaceManager : MiniGameManager
{
    [SerializeField] private Transform finishPoint;

    private double raceStartServerTime;
    private readonly Dictionary<ulong, Transform> racers = new Dictionary<ulong, Transform>();
    private readonly Dictionary<ulong, float> startDistances = new Dictionary<ulong, float>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        raceStartServerTime = NetworkManager.ServerTime.Time;
        StartGameRpc();
    }

    public void RegisterRacer(ulong clientId, Transform racer)
    {
        if (!IsServer) return;
        racers[clientId] = racer;
        startDistances[clientId] = Vector3.Distance(racer.position, finishPoint.position);
    }

    public void UnregisterRacer(ulong clientId)
    {
        if (!IsServer) return;
        racers.Remove(clientId);
        startDistances.Remove(clientId);
    }

    // 완주 여부 먼저 비교, 그 다음 점수 높은 순. 완주자가 항상 비완주자보다 위로 감
    protected override Comparison<MiniGameResult> RankComparison =>
        (a, b) =>
        {
            if (a.Finished != b.Finished) return b.Finished.CompareTo(a.Finished);
            return b.Score.CompareTo(a.Score);
        };

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SubmitFinishRpc(ulong clientId, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != clientId) return; // clientId 위조 방지

        double elapsed = NetworkManager.ServerTime.Time - raceStartServerTime;
        float timeRatio = timeLimit > 0f ? (float)(elapsed / timeLimit) : 0f;
        int score = Mathf.RoundToInt(Mathf.Clamp01(1f - timeRatio) * 100f);

        SubmitResult(new MiniGameResult { ClientId = clientId, Score = score, Finished = true });
    }

    // 제한시간 끝났는데 완주 못 한 사람들, 그 순간 위치 기준으로 거리 비율 점수 매김
    protected override void OnTimeExpired(List<ulong> remainingClients)
    {
        foreach (var clientId in remainingClients)
        {
            int score = 0;
            if (racers.TryGetValue(clientId, out var racer)
                && startDistances.TryGetValue(clientId, out var startDistance)
                && startDistance > 0f)
            {
                float remaining = Vector3.Distance(racer.position, finishPoint.position);
                score = Mathf.RoundToInt(Mathf.Clamp01(1f - remaining / startDistance) * 100f);
            }

            SubmitResult(new MiniGameResult { ClientId = clientId, Score = score, Finished = false });
        }
    }

    protected override void OnGameFinished(MiniGameResult[] results)
    {
        foreach (var r in results)
            Debug.Log($"{r.Rank}등 - 클라이언트 {r.ClientId} ({(r.Finished ? "완주" : "미완주")}, {r.Score}점)");
    }
}

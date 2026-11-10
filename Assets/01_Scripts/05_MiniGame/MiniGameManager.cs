using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class MiniGameManager : NetworkBehaviour
{
    [SerializeField] protected float timeLimit;

    private bool isRunning;
    private MiniGameResult[] collectedResults;
    private int collectedIndex;
    private readonly HashSet<ulong> pendingClients = new HashSet<ulong>();

    protected virtual Comparison<MiniGameResult> RankComparison => null;
    
    [Rpc(SendTo.Everyone)]
    public virtual void StartGameRpc()
    {
        if (isRunning) return;
        isRunning = true;

        if (IsServer)
        {
            var connected = NetworkManager.ConnectedClientsIds;
            collectedResults = new MiniGameResult[connected.Count];
            collectedIndex = 0;
            pendingClients.Clear();
            foreach (var id in connected) pendingClients.Add(id);

            NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;
            if (timeLimit > 0f) Invoke(nameof(HandleTimeExpired), timeLimit);
        }
    }

    // 레이싱처럼 각자 따로 끝날 때 씀
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    protected void ReportResultRpc(MiniGameResult myResult, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != myResult.ClientId) return; // 남의 ClientId로 위조한 보고 거부
        SubmitResult(myResult);
    }

    // 서버가 타임아웃 등으로 클라이언트 보고 없이 직접 결과를 확정할 때 씀. 신뢰된 호출이라 RPC로 안 감
    protected void SubmitResult(MiniGameResult result)
    {
        if (!IsServer) return;
        if (!pendingClients.Remove(result.ClientId)) return; // 이미 보고했거나 대상 아님

        collectedResults[collectedIndex++] = result;

        if (pendingClients.Count == 0)
            FinishWithRanking();
    }

    // 끝나기 전에 나간 플레이어는 대기 목록에서 빼고 남은 사람들만으로 종료 가능하게 함
    private void HandleClientDisconnected(ulong clientId)
    {
        if (!isRunning) return;
        if (pendingClients.Remove(clientId) && pendingClients.Count == 0)
            FinishWithRanking();
    }

    // 제한시간 끝났는데 아직 안 끝낸 사람들 처리는 자식 클래스가 구현
    private void HandleTimeExpired()
    {
        if (!isRunning) return;
        OnTimeExpired(new List<ulong>(pendingClients));
    }

    protected virtual void OnTimeExpired(List<ulong> remainingClients) { }

    private void FinishWithRanking()
    {
        var final = collectedResults[..collectedIndex];
        if (RankComparison != null) AssignRanks(final, RankComparison);
        FinishGameRpc(final);
    }

    protected static void AssignRanks(MiniGameResult[] results, Comparison<MiniGameResult> comparison)
    {
        Array.Sort(results, comparison);
        for (int i = 0; i < results.Length; i++)
            results[i].Rank = i + 1;
    }
    
    // 서버가 전체 결과를 이미 알 때 직접 호출하고 즉시 전체한테 뿌려짐
    [Rpc(SendTo.Everyone)]
    protected void FinishGameRpc(MiniGameResult[] results)
    {
        isRunning = false;
        if (IsServer)
        {
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
            CancelInvoke(nameof(HandleTimeExpired));
        }
        OnGameFinished(results);
    }

    // 자식이 결과 받았을 때 할 일을 여기서 오버라이드
    protected virtual void OnGameFinished(MiniGameResult[] results) { }
}

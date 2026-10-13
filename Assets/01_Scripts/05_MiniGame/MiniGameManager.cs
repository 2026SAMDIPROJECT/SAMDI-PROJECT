using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class MiniGameManager : NetworkBehaviour
{
    [SerializeField] protected float timeLimit;

    public event Action<List<MiniGameResult>> OnGameEnded;

    private bool isRunning;
    private readonly List<MiniGameResult> collectedResults = new List<MiniGameResult>();
    private int expectedResultCount;

    // 바깥에서 호출하면 전 클라이언트에서 돌아감
    public void StartGame()
    {
        if (isRunning) return;
        isRunning = true;

        if (IsServer)
        {
            collectedResults.Clear();
            expectedResultCount = NetworkManager.ConnectedClients.Count;
        }

        OnGameStart();
    }

    protected abstract void OnGameStart();

    // 레이싱처럼 각자 따로 끝날 때 씀
    protected void ReportResult(MiniGameResult myResult)
    {
        ReportResultServerRpc(myResult);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReportResultServerRpc(MiniGameResult result)
    {
        collectedResults.Add(result);
        if (collectedResults.Count >= expectedResultCount)
            FinishGame();
    }

    private void FinishGame()
    {
        FinishGame(collectedResults);
    }

    // 체스처럼 서버가 전체 결과를 이미 알 때 씀
    protected void FinishGame(List<MiniGameResult> results)
    {
        isRunning = false;
        BroadcastResultsClientRpc(results.ToArray());
    }

    [ClientRpc]
    private void BroadcastResultsClientRpc(MiniGameResult[] results)
    {
        OnGameEnded?.Invoke(new List<MiniGameResult>(results));
    }
}

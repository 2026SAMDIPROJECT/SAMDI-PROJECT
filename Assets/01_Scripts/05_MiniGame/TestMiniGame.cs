using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// 테스트용 - MiniGameManager 동작 검증 목적, 실제 미니게임 아님
public class TestMiniGame : MiniGameManager
{
    private void Awake()
    {
        OnGameEnded += HandleGameEnded;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        StartGame();
    }

    protected override void OnGameStart()
    {
        Invoke(nameof(ReportMyResult), 2f);
    }

    private void ReportMyResult()
    {
        ReportResult(new MiniGameResult
        {
            ClientId = NetworkManager.LocalClientId,
            Rank = 0,
            Score = 0
        });
    }

    private void HandleGameEnded(List<MiniGameResult> results)
    {
        Debug.Log($"테스트 미니게임 종료, 결과 {results.Count}개 수신");
        foreach (var r in results)
            Debug.Log($"클라이언트 {r.ClientId} 보고됨");
    }
}

using Unity.Netcode;
using UnityEngine;

// 테스트용 - MiniGameManager 동작 검증 목적, 실제 미니게임 아님
public class TestMiniGame : MiniGameManager
{
    private int reported;

    public override void OnStartGame()
    {
        reported = 0;
        gameName = "TestMiniGame";
        Invoke(nameof(ReportMyResult), 2f);
    }

    private void ReportMyResult()
    {
        ReportResultRpc(new MiniGameResult
        {
            ClientId = NetworkManager.LocalClientId,
            Rank = 0,
            Score = 0
        });
    }

    protected override void OnGameFinished(MiniGameResult[] results)
    {
        Debug.Log($"테스트 미니게임 종료, 결과 {results.Length}개 수신");
        foreach (var r in results)
        {
            Debug.Log($"클라이언트 {r.ClientId} 보고됨");
            OnReportedRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void OnReportedRpc()
    {
        reported++;
        if(reported >= NetworkManager.Singleton.ConnectedClients.Count)
        {
            UnloadSceneRpc();
        }
    }
}

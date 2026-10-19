using UnityEngine;

// 테스트용 - MiniGameManager 동작 검증 목적, 실제 미니게임 아님
public class TestMiniGame : MiniGameManager
{
    public override void StartGameRpc()
    {
        base.StartGameRpc();
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
            Debug.Log($"클라이언트 {r.ClientId} 보고됨");
    }
}

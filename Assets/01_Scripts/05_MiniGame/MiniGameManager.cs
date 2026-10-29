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

    public override void OnNetworkSpawn()
    {
        if(IsServer) StartGameRpc();
    }

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
        }
    }

    // 레이싱처럼 각자 따로 끝날 때 씀
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    protected void ReportResultRpc(MiniGameResult myResult, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != myResult.ClientId) return; // 남의 ClientId로 위조한 보고 거부
        if (!pendingClients.Remove(myResult.ClientId)) return; // 이미 보고했거나 대상 아님

        collectedResults[collectedIndex++] = myResult;

        if (pendingClients.Count == 0)
            FinishGameRpc(collectedResults[..collectedIndex]);
    }

    // 끝나기 전에 나간 플레이어는 대기 목록에서 빼고 남은 사람들만으로 종료 가능하게 함
    private void HandleClientDisconnected(ulong clientId)
    {
        if (!isRunning) return;
        if (pendingClients.Remove(clientId) && pendingClients.Count == 0)
            FinishGameRpc(collectedResults[..collectedIndex]);
    }

    // 서버가 전체 결과를 이미 알 때 직접 호출하고 즉시 전체한테 뿌려짐
    [Rpc(SendTo.Everyone)]
    protected void FinishGameRpc(MiniGameResult[] results)
    {
        isRunning = false;
        if (IsServer) NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        OnGameFinished(results);
    }

    // 자식이 결과 받았을 때 할 일을 여기서 오버라이드
    protected virtual void OnGameFinished(MiniGameResult[] results) { }
}

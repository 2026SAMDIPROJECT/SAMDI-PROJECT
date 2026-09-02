using Unity.Netcode;

public class PlayerNetWorkInit : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // AI 감지 => 서버 기준으로만 등록
        PlayerRegistry.Instance?.Register(transform);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        PlayerRegistry.Instance?.Unregister(transform);
    }
}

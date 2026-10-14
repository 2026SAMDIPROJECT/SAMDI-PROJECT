using Unity.Netcode;

public struct MiniGameResult : INetworkSerializeByMemcpy
{
    public ulong ClientId;
    public int Rank;
    public int Score;
}

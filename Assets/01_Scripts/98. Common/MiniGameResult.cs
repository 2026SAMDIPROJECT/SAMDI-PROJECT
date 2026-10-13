using Unity.Netcode;

public struct MiniGameResult : INetworkSerializable
{
    public ulong ClientId;
    public int Rank;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Rank);
        serializer.SerializeValue(ref Score);
    }
}

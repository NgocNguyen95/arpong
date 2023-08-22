using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong relayClientId;
    public int score;

    public bool Equals(PlayerData other)
    {
        return relayClientId == other.relayClientId && score == other.score;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref relayClientId);
        serializer.SerializeValue(ref score);
    }
}

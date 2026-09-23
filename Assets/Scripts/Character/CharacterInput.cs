using Unity.Netcode;
using UnityEngine;

public struct CharacterInput : INetworkSerializable
{
    public Vector2 Direction;
    public bool Jump;
    public bool Sprint;
    public bool Crouch;

    public Vector2 LookDelta;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Direction);
        serializer.SerializeValue(ref Jump);
        serializer.SerializeValue(ref Sprint);
        serializer.SerializeValue(ref Crouch);
        serializer.SerializeValue(ref LookDelta);
    }

    public void ResetInputs() 
    {
        Direction = Vector2.zero;
        Jump = false;
        Sprint = false;
        Crouch = false;
        LookDelta = Vector2.zero;
    }
}

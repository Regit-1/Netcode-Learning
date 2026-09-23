using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


/// <summary>
/// Abstract class for managing inputs
/// </summary>
[RequireComponent(typeof(CharacterMovement))]
public abstract class InputHandler : NetworkBehaviour
{
    // Send continuous input
    public CharacterInput accumulatedInput;
    protected List<IReceiveInput> _subscribers = new();

    /// <summary>
    /// Subscribes an object to receive network input every tick
    /// </summary>
    /// <param name="subscriber">The object that will be subscribed</param>
    public void AddTicks(IReceiveInput subscriber)
    {
        _subscribers.Add(subscriber);
    }

    /// <summary>
    /// Unsubscribes an object to receive network input every tick
    /// </summary>
    /// <param name="subscriber">The object that will be unsubscribed</param>
    public void RemoveTicks(IReceiveInput subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Sends input from the client to the server to calculate server-side movement
    /// </summary>
    /// <param name="characterInput">The total input to be passed to the server</param>
    [ServerRpc]
    public void SendInputServerRpc(CharacterInput characterInput)
    {
        this.accumulatedInput = characterInput;
    }

}

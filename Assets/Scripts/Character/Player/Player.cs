using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private void Awake()
    {
        if (!IsOwner) return;

        GameManager.Singleton.player = this;
    }
}

using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;

    private void Start()
    {

        if (!IsOwner) {
            playerCamera.enabled = false; 
            GetComponent<AudioListener>().enabled = false;
            return; 
        }

        GameManager.Singleton.DisableLobbyCamera();
    }

    private void OnDestroy()
    {
        GameManager.Singleton.EnableLobbyCamera();

    }
}

using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks.Data;
using Steamworks;
using Netcode.Transports.Facepunch;


public class StartLobbyMenu : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private GameObject networkManager;

    private void Start()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }

    private async void HostGame() 
    {
        if (networkManager == null) return;
        Debug.Log("Network Manager: " + networkManager);
        if (await networkManager.GetComponent<NetManager>().CreateLobby(2)) Debug.Log("Started Steam Lobby");
    }

    private async void JoinGame() 
    {
        if (networkManager == null) return;
        // Connects to the first lobby in the list
        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(5).RequestAsync();
        if (lobbies != null && lobbies.Length > 0)
        {
            Debug.Log($"Found {lobbies.Length} lobbies. Attempting to join the first result.");
            await networkManager.GetComponent<NetManager>().JoinLobby(lobbies[0]);
        }
        else 
        {
            Debug.LogWarning("No available lobbies to join");
        }
    }

    
}

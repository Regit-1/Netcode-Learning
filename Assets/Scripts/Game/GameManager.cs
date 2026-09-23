using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages all features relating to the overarching game for the client and the server
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] public Camera lobbyCam;
    public Player player;

    public static GameManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null) 
        {
            Destroy(this.gameObject);
        }

        else 
        {
            Singleton = this;
            DontDestroyOnLoad(Singleton);
        }

        DontDestroyOnLoad(lobbyCam);
    }

    public void DisableLobbyCamera()
    {
        if (lobbyCam != null) lobbyCam.enabled = false;
    }

    public void EnableLobbyCamera()
    {
        if (lobbyCam != null) lobbyCam.enabled = true;
    }
}

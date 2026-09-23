using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    private void Awake()
    {
        try
        {
            if (!SteamClient.IsValid) SteamClient.Init(3730930);
            Debug.Log($"Steam initialized for user: {SteamClient.Name}");
        }
        catch (System.Exception e) 
        {
            Debug.Log("Couldn't initialize Steam Client: " + e.Message);
        } 

        
        DontDestroyOnLoad(this.gameObject); // ensure this object isn't destroyed if the scene is changed
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnDisable()
    {
        SteamClient.Shutdown(); // won't shutdown the game while the editor is still open when exiting playmode
    }


}

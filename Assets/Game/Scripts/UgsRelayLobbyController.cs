using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;

public class UgsRelayLobbyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnityTransport transport;

    [Header("Config")] 
    [SerializeField] private int maxPlayers = 4;

    private Lobby currentLobby;
    private const string RELAY_JOIN_CODE_KEY = "relayJoinCode";
    private float heartbeatTimer;

    private async void Awake()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        
        Debug.Log("CLOUD PROJECT ID: " + Application.cloudProjectId);
        
        await InitUgsAsync();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLobby == null) return;
        if (!AuthenticationService.Instance.IsSignedIn) return;
        if (currentLobby.HostId != AuthenticationService.Instance.PlayerId) return;
        
        heartbeatTimer += Time.deltaTime;
        if (heartbeatTimer >= 15f)
        {
            heartbeatTimer = 0f;
            _ = LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
    }

    public async Task<string> HostAsync(string lobbyName)
    {
        var nm = NetworkManager.Singleton;

        if (nm.IsClient || nm.IsServer)
        {
            Debug.Log("[Netcode] Already running, shutting down before host...");
            nm.Shutdown();
            await Task.Yield();
        }
        
        await InitUgsAsync();
        EnsureNotRunning();

        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        var relayServerData = new RelayServerData(
            alloc.RelayServer.IpV4,
            (ushort)alloc.RelayServer.Port,
            alloc.AllocationIdBytes,
            alloc.ConnectionData,
            alloc.ConnectionData,
            alloc.Key,
            true);
        
        transport.SetRelayServerData(relayServerData);

        var options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            string.IsNullOrWhiteSpace(lobbyName) ? "My Lobby" : lobbyName,
            maxPlayers,
            options);

        NetworkManager.Singleton.StartHost();

        Debug.Log($"LOBBY CODE: {currentLobby.LobbyCode}");
        Debug.Log($"RELAY JOIN CODE (stored in lobby data): {relayJoinCode}");
        
        return currentLobby.LobbyCode;
    }

    public async Task JoinLobbyAsync(string lobbyCode)
    {
        var nm = NetworkManager.Singleton;

        // Hard reset
        if (nm.IsListening || nm.IsClient || nm.IsServer)
        {
            Debug.Log("[Netcode] Hard reset before Join (Shutdown)");
            nm.Shutdown();
        }

        // Wait until it actually stops
        for (int i = 0; i < 120; i++) // up to ~2 seconds
        {
            if (!nm.IsListening && !nm.IsClient && !nm.IsServer) break;
            await Task.Delay(16);
        }

        Debug.Log($"[Netcode] After reset: IsClient={nm.IsClient} IsServer={nm.IsServer} IsListening={nm.IsListening}");

        
        await InitUgsAsync();
        EnsureNotRunning();

        currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode.Trim());
        
        if(currentLobby.Data == null || !currentLobby.Data.ContainsKey(RELAY_JOIN_CODE_KEY))
            throw new Exception("Invalid lobby code");

        string relayJoinCode = currentLobby.Data[RELAY_JOIN_CODE_KEY].Value;
        
        JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
        
        var relayServerData = new RelayServerData(
            joinAlloc.RelayServer.IpV4,
            (ushort)joinAlloc.RelayServer.Port,
            joinAlloc.AllocationIdBytes,
            joinAlloc.ConnectionData,
            joinAlloc.HostConnectionData,
            joinAlloc.Key,
            true);
        transport.SetRelayServerData(relayServerData);
        
        NetworkManager.Singleton.StartClient();
        
        Debug.Log($"Joining lobby code: {lobbyCode}");
        Debug.Log($"Relay join code read from lobby: {relayJoinCode}");
        
        bool started = NetworkManager.Singleton.StartClient();
        Debug.Log("[Netcode] StartClient() returned: " + started);
    }

    private static async Task InitUgsAsync()
    {
        if(UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        /*if (!AuthenticationService.Instance.IsSignedIn)
        {
            const string key = "UGS_CUSTOM_ID";
            if(!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, Guid.NewGuid().ToString("N"));

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }*/
    }
    
    private async void OnDestroy()
    {
        if (currentLobby != null &&
            AuthenticationService.Instance.IsSignedIn &&
            currentLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
        }
    }
    
    private static void EnsureNotRunning()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsClient || nm.IsServer)
        {
            Debug.Log("[Netcode] Shutting down previous session");
            nm.Shutdown();
        }
    }

}

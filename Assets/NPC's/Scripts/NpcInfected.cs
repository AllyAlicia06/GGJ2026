using System;
using Unity.Netcode;
using UnityEngine;

public class NpcInfected : NetworkBehaviour
{
    [Header("State")]
    public NetworkVariable<bool> isInfected = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color infectedColor = Color.red;
    public Color healthyColor = Color.green;
    NetworkObject networkObject;
    private GameState gamestate;

    [ServerRpc(RequireOwnership = false)]
    public void InfectServerRpc() => isInfected.Value = true;
    
    [ServerRpc(RequireOwnership = false)]
    public void CureServerRpc() => isInfected.Value = false;
    
    private void Awake()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    public override void OnNetworkSpawn()
    {
        gamestate = FindFirstObjectByType<GameState>();
        
        ApplyVisual(isInfected.Value);
        isInfected.OnValueChanged += OnInfectedChanged;
        
        if(IsServer && gamestate != null)
            gamestate.OnNpcInfectionChanged(NetworkObjectId, isInfected.Value);
    }

    public override void OnNetworkDespawn()
    {
        isInfected.OnValueChanged -= OnInfectedChanged;
    }

    private void OnInfectedChanged(bool oldValue, bool newValue)
    {
        ApplyVisual(newValue);

        if(IsServer && gamestate != null)
            gamestate.OnNpcInfectionChanged(NetworkObjectId, newValue);
    }
    public void Initialize(bool infected)
    {
        if (!IsServer)
        {
            Debug.LogError($"Initialize called on CLIENT for NPC {gameObject.name}. Call it only on server.");
            return;
        }

        var netObj = GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError($"NPC {gameObject.name} has NO NetworkObject component on the root GameObject.");
            return;
        }
        
        if (!netObj.IsSpawned)
            netObj.Spawn();
        
        isInfected.Value = infected;
        ApplyVisual(infected);
    }

    public void ApplyVisual(bool infected)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = infected ? infectedColor : healthyColor;
    }

    public void InfectServer()
    {
        if (!IsServer) return;
        isInfected.Value = true;
    }

    public void CureServer()
    {
        if (!IsServer) return;
        isInfected.Value = false;
    }
    
    
}

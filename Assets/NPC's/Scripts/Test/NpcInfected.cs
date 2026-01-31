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
    
    void ApplyVisual(bool infected)
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

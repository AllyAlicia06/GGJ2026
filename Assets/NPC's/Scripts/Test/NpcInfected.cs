
using System.Collections;
using System.Collections.Generic;
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

    private bool TemperatureIsToggled = false;
     private float _healthyTemperature = 37.0f;
    public float HealthyTemperature { get { return _healthyTemperature; } set { _healthyTemperature = value; } }
    private float _infectedTemperature = 39.0f;
    public float InfectedTemperature { get { return _infectedTemperature; } set { _infectedTemperature = value; } }
    NetworkObject networkObject;
    private GameState gamestate;

    [ServerRpc(RequireOwnership = false)]
    public void InfectServerRpc() => isInfected.Value = true;
    
    [ServerRpc(RequireOwnership = false)]
    public void CureServerRpc() => isInfected.Value = false;
    
    private void Awake()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _healthyTemperature= Random.Range(36.5f, 37.5f);
        _infectedTemperature= Random.Range(38.5f, 40.0f);
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
        networkObject = GetComponent<NetworkObject>();
        isInfected.Value = infected;
        ApplyVisual(infected);
        if(networkObject == null || !IsServer)
        {
            Debug.LogError("NetworkObject is null for NPC " + gameObject.name);
            return;
        }

        networkObject.Spawn();
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

    public void ToggleTemperature()
    {
        if(TemperatureIsToggled) return; 
        //just for visual feedback, not changing actual infection state
        float currentTemp = isInfected.Value ? InfectedTemperature : HealthyTemperature;
        Debug.Log("Toggling temperature visual for NPC " + currentTemp);
        spriteRenderer.color = Color.blue;
        StartCoroutine(ResetColorAfterDelay(0.5f));
    }
    IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyVisual(isInfected.Value);
        TemperatureIsToggled = false;
    }
    
}

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NpcInfected : NetworkBehaviour
{
    [Header("State")]
    public NetworkVariable<bool> isInfected = new NetworkVariable<bool>(
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    [Header("Local-only infection colors (infected player only)")]
    [SerializeField] private Color infectedColor = Color.red;
    [SerializeField] private Color healthyColor = Color.green;

    [Header("Role IDs")]
    [SerializeField] private int infectedCharacterId = 0;
    [SerializeField] private int guardCharacterId = 1;

    private bool temperatureFlashActive = false;

    private float _healthyTemperature = 37.0f;
    public float HealthyTemperature { get { return _healthyTemperature; } set { _healthyTemperature = value; } }

    private float _infectedTemperature = 39.0f;
    public float InfectedTemperature { get { return _infectedTemperature; } set { _infectedTemperature = value; } }

    private GameState gamestate;

    // Local role cache
    private int localCharacterId = -1;
    private PlayerCharacter localPlayerCharacter;

    [ServerRpc(RequireOwnership = false)]
    public void InfectServerRpc() => isInfected.Value = true;

    [ServerRpc(RequireOwnership = false)]
    public void CureServerRpc() => isInfected.Value = false;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _healthyTemperature = Random.Range(36.5f, 37.5f);
        _infectedTemperature = Random.Range(38.5f, 40.0f);
    }

    public override void OnNetworkSpawn()
    {
        gamestate = FindFirstObjectByType<GameState>();

        CacheLocalRoleAndSubscribe();

        ApplyVisual();
        isInfected.OnValueChanged += OnInfectedChanged;

        if (IsServer && gamestate != null)
            gamestate.OnNpcInfectionChanged(NetworkObjectId, isInfected.Value);
    }

    public override void OnNetworkDespawn()
    {
        isInfected.OnValueChanged -= OnInfectedChanged;

        if (localPlayerCharacter != null)
            localPlayerCharacter.CharacterId.OnValueChanged -= OnLocalCharacterChanged;
    }

    private void OnInfectedChanged(bool oldValue, bool newValue)
    {
        ApplyVisual();

        if (IsServer && gamestate != null)
            gamestate.OnNpcInfectionChanged(NetworkObjectId, newValue);
    }

    // Called by the server spawner after the NPC NetworkObject has been spawned.
    public void Initialize(bool infected)
    {
        if (!IsServer) return;

        isInfected.Value = infected;
        // No need to force visuals here; clients will handle visuals locally via ApplyVisual()
    }

    private void CacheLocalRoleAndSubscribe()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || nm.SpawnManager == null) return;

        var localPlayerObj = nm.SpawnManager.GetLocalPlayerObject();
        if (localPlayerObj == null) return;

        localPlayerCharacter = localPlayerObj.GetComponent<PlayerCharacter>();
        if (localPlayerCharacter == null) return;

        localCharacterId = localPlayerCharacter.CharacterId.Value;
        localPlayerCharacter.CharacterId.OnValueChanged += OnLocalCharacterChanged;
    }

    private void OnLocalCharacterChanged(int oldId, int newId)
    {
        localCharacterId = newId;
        ApplyVisual();
    }

    private bool LocalIsInfectedPlayer()
    {
        return localCharacterId == infectedCharacterId;
    }

    private bool LocalIsGuard()
    {
        return localCharacterId == guardCharacterId;
    }

    private void ApplyVisual()
    {
        if (spriteRenderer == null) return;

        // Guard sees no infection tint (white), except temporary blue flash.
        if (temperatureFlashActive)
            return;

        if (LocalIsInfectedPlayer())
        {
            spriteRenderer.color = isInfected.Value ? infectedColor : healthyColor;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
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

    // Visual-only cue for the guard when checking temperature.
    public void ToggleTemperature()
    {
        // Only the guard should see the blue flash.
        if (!LocalIsGuard()) return;

        if (temperatureFlashActive) return;

        temperatureFlashActive = true;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;

        StartCoroutine(ResetColorAfterDelay(0.5f));
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        temperatureFlashActive = false;
        ApplyVisual(); // re-apply according to local role
    }
}
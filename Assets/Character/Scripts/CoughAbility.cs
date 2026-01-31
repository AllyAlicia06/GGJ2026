using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class CoughAbility : NetworkBehaviour
{
    [Header("Settings")]
    public float minRange = 0.5f;
    public float maxRange = 2f;
    public float timeToMaxCharge = 2.0f; 
    public float cooldownDuration = 5f;
    public LayerMask targetLayer; 

    [Header("Visual Indicator")]
    [Tooltip("Use a circle transform with scale = 2 * minRange to indicate the range of the cough.")]
    public Transform rangeIndicator;

    public NetworkVariable<float> CooldownRemaining = new NetworkVariable<float>(0f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float lastServerTime;
    
    private float currentChargeTime = 0f;
    private bool _isCharging = false;
    
    public bool isCharging
    {
        get => _isCharging;
        private set
        {
            _isCharging = value;
            if (!value)
            {
                currentChargeTime = 0f;
                if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer) lastServerTime = Time.time;

        if (!IsOwner)
        {
            if(rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (IsServer)
        {
            float now = Time.time;
            float dt = now - lastServerTime;
            lastServerTime = now;
            
            if(CooldownRemaining.Value > 0f)
                CooldownRemaining.Value = Mathf.Max(0f, CooldownRemaining.Value - dt);
        }

        if (!IsOwner) return;
        if (CooldownRemaining.Value > 0f) return;
        
        if (_isCharging)
        {
            currentChargeTime += Time.deltaTime;

            float chargePercent = Mathf.Clamp01(currentChargeTime / timeToMaxCharge);
            float currentRadius = Mathf.Lerp(minRange, maxRange, chargePercent);
            if (rangeIndicator != null)
            {
                if(rangeIndicator.gameObject.activeSelf == false) rangeIndicator.gameObject.SetActive(true);
                rangeIndicator.localScale = Vector3.one * (currentRadius * 2f); 
            }
        }
    }

    public void BeginCharge()
    {
        if (!IsOwner) return;
        if(CooldownRemaining.Value > 0f) return;
        if(_isCharging) return;
        _isCharging = true;
        currentChargeTime = 0f;
    }
    
    public void ReleaseCharge()
    {
        if (!IsOwner) return;
        if(!_isCharging) return;
        
        _isCharging = false;
        if(rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);
        
        float chargePercent = Mathf.Clamp01(currentChargeTime / timeToMaxCharge);
        FireCoughServerRpc(chargePercent);
    }

    [ServerRpc]
    void FireCoughServerRpc(float chargePercent, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        if(CooldownRemaining.Value > 0f) return;
        
        chargePercent = Mathf.Clamp01(chargePercent);
        float finalRadius = Mathf.Lerp(minRange, maxRange, chargePercent);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, finalRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {

            if(hit == null) continue;

            var npc = hit.GetComponentInParent<NpcInfected>();
            if(npc == null) continue;
            if (npc.isInfected.Value) continue;

            npc.InfectServer();
            
            Debug.Log($"Coughed on: {hit.name}");
        }
        CooldownRemaining.Value = cooldownDuration;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
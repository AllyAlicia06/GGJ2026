using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System.Runtime.CompilerServices;
public class CoughAbility : NetworkBehaviour
{
    [Header("Settings")]
    public float minRange = 0.5f;
    public float maxRange = 2f;
    public float timeToMaxCharge = 2.0f; 
    public float cooldownDuration = 5f;
    public LayerMask targetLayer; 

    public NetworkVariable<float> currentCooldown = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private float lastServerTime;
    [Header("Visual Indicator")]
    [Tooltip("Use a circle transform with scale = 2 * minRange to indicate the range of the cough.")]
    public Transform rangeIndicator; 

    //private float currentCooldown = 0f;
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
    }
    // void Update()
    // {
    //     //if(currentCooldown > 0f) currentCooldown -= Time.deltaTime;
    //     if (IsServer)
    //     {
    //         float now = Time.time;
    //         float dt =  now - lastServerTime;
    //         lastServerTime = now;
            
    //         if(CooldownRemaining.Value > 0f)
    //             CooldownRemaining.Value = Mathf.Max(0f, CooldownRemaining.Value - dt);
    //     }
    // }


    void Update()
    {
        if(IsServer){
        if (currentCooldown.Value > 0)
        {
            currentCooldown.Value -= Time.deltaTime;
            return;
        }
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
    }

  
  
     public void BeginCharge()
    {
        if (!IsOwner) return;
        //TryInfectServerRpc();
        if(currentCooldown.Value > 0) return;
        if(_isCharging) return;
        _isCharging = true;
        currentChargeTime = 0f;
    }
    public void ReleaseCharge()
    {
        if (!IsOwner) return;
        if(!_isCharging) return;
        FireCoughServerRpc();
    }

    [ServerRpc]
    void FireCoughServerRpc(ServerRpcParams rpcParams = default)
    {
        _isCharging = false;
        if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);

        float chargePercent = Mathf.Clamp01(currentChargeTime / timeToMaxCharge);
        float finalRadius = Mathf.Lerp(minRange, maxRange, chargePercent);

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, finalRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {

            if (hit.gameObject == gameObject) continue;
            NpcInfected npcInfected = hit.GetComponentInParent<NpcInfected>();
            if (npcInfected != null)
            {
                npcInfected.InfectServer();
                Debug.Log($"Coughed on: {hit.name}");
            }
        }
        currentCooldown.Value = cooldownDuration;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
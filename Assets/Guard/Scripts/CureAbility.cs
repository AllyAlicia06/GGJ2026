using System;
using Unity.Netcode;
using UnityEngine;

public class CureAbility : NetworkBehaviour
{
    [Header("Settings")] 
    public float range = 1.5f;
    public float cooldown = 2.5f;
    public LayerMask targetLayer;
    
    public NetworkVariable<float> CooldownRemaining = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float lastServerTime;

    public override void OnNetworkSpawn()
    {
        if(IsServer) lastServerTime = Time.time;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentCooldown > 0f) currentCooldown -= Time.deltaTime;
        if (IsServer)
        {
            float now = Time.time;
            float dt =  now - lastServerTime;
            lastServerTime = now;
            
            if(CooldownRemaining.Value > 0f)
                CooldownRemaining.Value = Mathf.Max(0f, CooldownRemaining.Value - dt);
        }
    }

    public void TryCure()
    {
        
        if (!IsOwner) return;
        TryCureServerRpc();
    }

    [ServerRpc]
    private void TryCureServerRpc(ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        if (CooldownRemaining.Value > 0f) return;

        var target = FindClosestInfectedServer();
        if (target == null) return;

        target.CureServer();
        CooldownRemaining.Value = cooldown;
    }

    private NpcInfected FindClosestInfectedServer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);

        float bestDistSqr = float.PositiveInfinity;
        NpcInfected best = null;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            var infected = hit.GetComponentInParent<NpcInfected>();
            if (infected == null) continue;
            if (!infected.isInfected.Value) continue;
            
            float dSqr = (infected.transform.position - transform.position).sqrMagnitude;
            if (dSqr < bestDistSqr)
            {
                bestDistSqr = dSqr;
                best = infected;
            }
        }

        return best;
    }
}

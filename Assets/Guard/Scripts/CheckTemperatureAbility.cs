using UnityEngine;
using System;
using Unity.Netcode;


public class CheckTemperatureAbility : NetworkBehaviour
{

    public LayerMask targetLayer;
    public float range = 1.5f;
    public float beamThickness = 0.2f;
    private GameObject checkedObject = null;
    private GameObject highlightObject = null;
    

    public float cooldown = 0.2f;

    public CharacterMovement _characterMovement;
    public CharacterMovement characterMovement
    {
       set
        {
          _characterMovement = value;
        }
        get
        {
          return _characterMovement;
     
        }
    }
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

    public void TryCheckTemperature()
    {
        
        if (!IsOwner) return;
       
        TryCheckTemperatureServerRpc();
    }

    [ServerRpc]
    private void TryCheckTemperatureServerRpc(ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        if (CooldownRemaining.Value > 0f) return;

        NpcInfected target = FindNPCInRange();
        if (target == null) return;
        checkedObject = target.gameObject;
        target.ToggleTemperature();
        CooldownRemaining.Value = cooldown;
    }

    private NpcInfected FindNPCInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position,beamThickness,characterMovement.GetLastDirection(),range, targetLayer);

       
        
        if(hits.Length == 0) return null;
       
        return hits[0].collider.GetComponentInParent<NpcInfected>();
    }
   
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
    Vector2 direction = characterMovement.GetLastDirection();
    
    // Draw the start of the beam
    Gizmos.DrawWireSphere(transform.position, beamThickness);
    
    // Draw the end of the beam
    Vector3 endPosition = transform.position + (Vector3)(direction.normalized * range);
    Gizmos.DrawWireSphere(endPosition, beamThickness);
    
    // Draw lines connecting them to show the "tube"
    Gizmos.DrawLine(transform.position, endPosition);
    }

}

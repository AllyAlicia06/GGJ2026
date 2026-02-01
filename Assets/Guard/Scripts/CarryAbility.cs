using UnityEngine;
using System;
using Unity.Netcode;




public class CarryAbility : NetworkBehaviour
{

    public LayerMask targetLayer;
    public float range = 1.5f;
    public float beamThickness = 0.2f;
    private GameObject carriedObject = null;
    private GameObject highlightObject = null;
    //private InfectedController currentCarriedCharacterMovement;
    public bool isCarrying = false;
    private Transform carryPosition;

    public float cooldown = 2.5f;

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
    private NetworkObject carriedNetworkObject;
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
        if(!IsOwner) return;
       
        if(!isCarrying && carriedObject != null)
        {
            carriedObject = null;
        }
        if(!isCarrying && carriedObject == null)
        {
            highlightTarget();
        }
    }

    public void TryCarry()
    {
        
        if (!IsOwner) return;
        if (CooldownRemaining.Value > 0f) return;
        if(isCarrying)
        {
            if(carriedNetworkObject != null)
            {
                carriedNetworkObject.TryRemoveParent();
                InfectedController target = carriedNetworkObject.GetComponent<InfectedController>();
                if (target != null)
                {
                    target.SetCarriedState(false);
                }
                carriedNetworkObject = null;
            }
            isCarrying = false;
            return;
        }
        else{
        InfectedController target = FindPlayerInRange();
        if (target == null) return;
        NetworkObject carriedNetObject = target.GetComponent<NetworkObject>();
        if(carriedNetObject == null) return;
        TryCarryServerRpc(carriedNetObject.NetworkObjectId);
        }
    }

    [ServerRpc]
    private void TryCarryServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        Debug.Log($"[CarryAbility] TryCarryServerRpc called for NetworkObjectId: {networkObjectId}");
       if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
       {

        InfectedController target = netObj.GetComponent<InfectedController>();
        if (target == null) return;
        Debug.Log(netObj.name + " found to carry.");
        netObj.TrySetParent(transform,false);
        netObj.transform.localPosition = carryPosition != null ? carryPosition.localPosition : Vector3.up * 1.0f;
        carriedNetworkObject = netObj;
        target.SetCarriedState(true);
        isCarrying = true;
        CooldownRemaining.Value = cooldown;
       }
    }
    
    private InfectedController FindPlayerInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position,beamThickness,characterMovement.GetLastDirection(),range, targetLayer);

       
        
        if(hits.Length == 0) return null;
       
        return hits[0].collider.GetComponentInParent<InfectedController>();
    }
    private void highlightTarget()
    {
        InfectedController targetInfected = FindPlayerInRange();
        if(targetInfected != null)
        {
           
            GameObject target = targetInfected.gameObject;
            if(target != highlightObject)
            {
                if(highlightObject != null)
                {
                    highlightObject.GetComponentInChildren<HighlightCharacter>().SetHighlight(false);
                }
                highlightObject = target;
                if(highlightObject != null)
                {
                    highlightObject.GetComponentInChildren<HighlightCharacter>().SetHighlight(true);
                }
            }
            return;
        }
        if(highlightObject != null)
        {
            highlightObject.GetComponentInChildren<HighlightCharacter>().SetHighlight(false);
            highlightObject = null;
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
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

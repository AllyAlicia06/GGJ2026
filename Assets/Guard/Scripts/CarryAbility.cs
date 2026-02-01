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
        if(carriedObject != null && isCarrying)
        {
            carriedObject.transform.position = transform.position + Vector3.up * 0.5f;
        }
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
        if(isCarrying)
        {
            carriedObject.GetComponent<InfectedController>().SetCarriedState(false);
            carriedObject = null;
            isCarrying = false;
            return;
        }
        TryCarryServerRpc();
    }

    [ServerRpc]
    private void TryCarryServerRpc(ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        if (CooldownRemaining.Value > 0f) return;

        InfectedController target = FindPlayerInRange();
        if (target == null) return;
        carriedObject = target.gameObject;
        target.SetCarriedState(true);
        isCarrying = true;
        CooldownRemaining.Value = cooldown;
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

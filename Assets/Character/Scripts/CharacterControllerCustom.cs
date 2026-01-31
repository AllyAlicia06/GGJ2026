using UnityEngine;
using Unity.Netcode;

public class CharacterControllerCustom : NetworkBehaviour
{
    protected CharacterMovement characterMovement;
    //private CoughAbility coughAbility;
    
    protected virtual void Awake()
    {
        characterMovement = GetComponentInChildren<CharacterMovement>();
        //coughAbility = GetComponentInChildren<CoughAbility>();
        
        //vezi ca am modificat aici ca sa fie structura ok la guard si las cu override la amandoua si la una pun cure si la alta punem cough
    }
    void Update()
    {
        if (!IsOwner) return;

        HandleMovementInput();
        HandleSprintInput();
        HandleAbilityInput();
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log($"[{name}] IsOwner={IsOwner} OwnerClientId={OwnerClientId} LocalClientId={NetworkManager.Singleton.LocalClientId}");
        enabled = IsOwner;
    }

    void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = new Vector2(x, y).normalized;
        characterMovement.SetMovementInput(inputVector);
    }

    void HandleSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = false;
        }
    }
    
    protected virtual void HandleAbilityInput()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            coughAbility.BeginCharge();
            
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            coughAbility.ReleaseCharge();
        }
        if(coughAbility.isCharging && !characterMovement.IsCoughing)
        {
            characterMovement.IsCoughing = true;
        }
        else if(!coughAbility.isCharging && characterMovement.IsCoughing)
        {
            characterMovement.IsCoughing = false;
        }*/
       
    }
}

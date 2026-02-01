using UnityEngine;
using Unity.Netcode;

public class CharacterControllerCustom : NetworkBehaviour
{
    protected CharacterMovement characterMovement;
    
    
    protected virtual void Awake()
    {
        characterMovement = GetComponentInChildren<CharacterMovement>();
       
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
    public virtual void HandleState()
    {
        //to be overridden in derived classes if needed
    }
    protected virtual void HandleMovementInput()
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
        
    }

    
}

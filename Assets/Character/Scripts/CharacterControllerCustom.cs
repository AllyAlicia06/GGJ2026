using UnityEngine;
using Unity.Netcode;

public class CharacterControllerCustom : NetworkBehaviour
{
    private CharacterMovement characterMovement;
    private CoughAbility coughAbility;
    
    private 
    void Awake()
    {
        characterMovement = GetComponentInChildren<CharacterMovement>();
        coughAbility = GetComponentInChildren<CoughAbility>();
    }
    void Update()
    {
        if (!IsOwner) return;

        HandleMovementInput();
        HandleAbilityInput();
    }

    void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = new Vector2(x, y).normalized;
        characterMovement.SetMovementInput(inputVector);

        
    }

    void HandleAbilityInput()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            coughAbility.BeginCharge();
            
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            coughAbility.ReleaseCharge();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = false;
        }
        if(coughAbility.isCharging && !characterMovement.IsCoughing)
        {
            characterMovement.IsCoughing = true;
        }
        else if(!coughAbility.isCharging && characterMovement.IsCoughing)
        {
            characterMovement.IsCoughing = false;
        }
       
    }
}

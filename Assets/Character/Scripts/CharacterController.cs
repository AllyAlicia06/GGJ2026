using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(CoughAbility))]
[RequireComponent(typeof(CharacterMovement))]
public class CharacterController : NetworkBehaviour
{
    private CharacterMovement characterMovement;
    private CoughAbility coughAbility;
    
    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        coughAbility = GetComponent<CoughAbility>();
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
    }
}

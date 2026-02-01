using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfectedController : CharacterControllerCustom
{
    private CoughAbility coughAbility;

    public bool canMove = false;
    public bool isCarried = false;  
    private void Awake()
    {
        base.Awake();
        coughAbility = GetComponentInChildren<CoughAbility>();
        
        //vezi ca am modificat aici ca sa fie structura ok la guard si las cu override la amandoua si la una pun cure si la alta punem cough
        // Initialize any Infected-specific abilities here
    }
    override protected void HandleAbilityInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
        }
    }
    override protected void HandleMovementInput()
    {
        if (!canMove) return;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = new Vector2(x, y).normalized;
        characterMovement.SetMovementInput(inputVector);
    }
    override public void HandleState()
    {
        if(isCarried)
        {
           return;
        }
    }
    public void SetCarriedState(bool carried)
    {
        Debug.Log($"[InfectedController] SetCarriedState: {carried}", this);
        isCarried = carried;
        canMove = !carried;
        if(carried)
        {
            characterMovement.SetMovementInput(Vector2.zero);
        }
    }
    
}

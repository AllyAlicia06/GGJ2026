using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InfectedController : CharacterControllerCustom
{

    private CoughAbility coughAbility;
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
}

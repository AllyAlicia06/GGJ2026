using UnityEngine;

public class GuardController : CharacterControllerCustom
{
    private CureAbility cureAbility;
    private CarryAbility carryAbility;
    private CheckTemperatureAbility checkTemperatureAbility;
    protected override void Awake()
    {
        base.Awake();
        cureAbility = GetComponentInChildren<CureAbility>();
        carryAbility = GetComponentInChildren<CarryAbility>();
        checkTemperatureAbility = GetComponentInChildren<CheckTemperatureAbility>();
        Debug.Log($"[Guard] cureAbility found? {cureAbility != null}", this);
        Debug.Log($"[Guard] carryAbility found? {carryAbility != null}", this);
        carryAbility.characterMovement = characterMovement;
        checkTemperatureAbility.characterMovement = characterMovement;
    }
    
    //restul gen handlemovementinput ramane ca la charactercontrollercustom, fac eu alta clasa separata pentru infectat ...

    protected override void HandleAbilityInput()
    {
        // aici dai handle la abilitati gen input si pentru fiecare abilitate faci alt script unde faci o metoda pe care o apelezi aici.

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cureAbility != null)
            {
                Debug.Log("[Guard] E pressed", this);
                cureAbility.TryCure();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (carryAbility != null)
            {
                Debug.Log("[Guard] Space pressed", this);
                carryAbility.TryCarry();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (checkTemperatureAbility != null)
            {
                Debug.Log("[Guard] F pressed", this);
                checkTemperatureAbility.TryCheckTemperature();
            }
        }
        
    }
}

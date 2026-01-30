using UnityEngine;

public class GuardController : CharacterControllerCustom
{
    private CureAbility cureAbility;

    protected override void Awake()
    {
        base.Awake();
        cureAbility = GetComponentInChildren<CureAbility>();
        Debug.Log($"[Guard] cureAbility found? {cureAbility != null}", this);
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
    }
}

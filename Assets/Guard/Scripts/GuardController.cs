using UnityEngine;

public class GuardController : CharacterControllerCustom
{
    
    //restul gen handlemovementinput ramane ca la charactercontrollercustom, fac eu alta clasa separata pentru infectat ...
    override void HandleAbilityInput()
    {
        // aici dai handle la abilitati gen input si pentru fiecare abilitate faci alt script unde faci o metoda pe care o apelezi aici.
    }
}

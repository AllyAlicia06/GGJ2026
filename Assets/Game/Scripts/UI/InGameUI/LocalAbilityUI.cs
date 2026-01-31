using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LocalAbilityUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image cooldownOverlay;

    [Header("Icons")]
    [SerializeField] private Sprite guardIcon;
    [SerializeField] private Sprite infectedIcon;

    [Header("Character IDs")] 
    [SerializeField] private int infectedId = 0;
    [SerializeField] private int guardId = 0;
    
    [Range(0f, 1f)]
    [SerializeField] private float overlayAlphaOnCooldown = 0.55f;

    private PlayerCharacter localPlayerChar;
    private CureAbility localCure;
    private CoughAbility localCough;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || nm.SpawnManager == null) return;

        var localPlayer = nm.SpawnManager.GetLocalPlayerObject();
        if (localPlayer == null) return;

        localPlayerChar = localPlayer.GetComponent<PlayerCharacter>();
        localCure = localPlayer.GetComponentInChildren<CureAbility>(true);
        localCough = localPlayer.GetComponentInChildren<CoughAbility>(true);

        if (localPlayerChar == null) return;

        //ApplyCharacter(localPlayerChar.CharacterId.Value);
        localPlayerChar.CharacterId.OnValueChanged += OnCharacterChanged;

        BindForCharacter(localPlayerChar.CharacterId.Value);

        //HookCooldown();
        //ApplyCooldownAlpha(GetCooldownRemaining());
    }

    private void OnDestroy()
    {
        if (localPlayerChar != null)
            localPlayerChar.CharacterId.OnValueChanged -= OnCharacterChanged;

        UnhookCooldown();
    }

    private void OnCharacterChanged(int oldId, int newId)
    {
        BindForCharacter(newId);
    }

    private void BindForCharacter(int characterId)
    {
        ApplyCharacterIcon(characterId);
        
        var nm = NetworkManager.Singleton;
        var localPlayer = nm != null ? nm.SpawnManager.GetLocalPlayerObject() : null;
        if (localPlayer != null)
        {
            localCure = localPlayer.GetComponentInChildren<CureAbility>();
            localCough = localPlayer.GetComponentInChildren<CoughAbility>();
        }

        UnhookCooldown();
        HookCooldown(characterId);
        ApplyCooldownAlpha(GetCooldownRemaining(characterId));
    }

    private void ApplyCharacterIcon(int characterId)
    {
        bool isGuard = (characterId == guardId);

        if (abilityIcon != null)
            abilityIcon.sprite = isGuard ? guardIcon : infectedIcon;
    }

    private void HookCooldown(int characterId)
    {
        if (localCure != null && characterId == guardId)
            localCure.CooldownRemaining.OnValueChanged += OnCooldownChanged;

        //if (localCough != null && characterId == infectedId)
            //localCough.CooldownRemaining.OnValueChanged += OnCooldownChanged;
    }

    private void UnhookCooldown()
    {
        if (localCure != null)
            localCure.CooldownRemaining.OnValueChanged -= OnCooldownChanged;

        //if (localCough != null)
            //localCough.CooldownRemaining.OnValueChanged -= OnCooldownChanged;
    }

    private void OnCooldownChanged(float oldV, float newV) => ApplyCooldownAlpha(newV);

    private float GetCooldownRemaining(int characterId)
    {
        if (localCure != null && characterId == guardId) return localCure.CooldownRemaining.Value;
        //if (localCough != null && characterId == infectedId) return localCough.CooldownRemaining.Value;
        return 0f;
    }

    private void ApplyCooldownAlpha(float remaining)
    {
        if (cooldownOverlay == null) return;

        var c = cooldownOverlay.color;
        c.a = (remaining > 0f) ? overlayAlphaOnCooldown : 0f;
        cooldownOverlay.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject visuals;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text characterNameText;
    
    [Header("Lock in Visuals")]
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockSprites = new Sprite[2];

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterId != -1)
        {
            var character = characterDatabase.GetCharacterById(state.CharacterId);
            characterIconImage.sprite = character.Icon;
            characterIconImage.enabled = true;
            characterNameText.text = character.DisplayName;
        }
        else
        {
            characterIconImage.enabled = false;
        }

        playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";

        SetLockState(state.IsLockedIn);
        
        visuals.SetActive(true);
    }

    private void SetLockState(bool isLockedIn)
    {
        if (lockImage == null) return;
        if (lockSprites == null || lockSprites.Length < 2) return;
        
        lockImage.sprite = isLockedIn ? lockSprites[1] : lockSprites[0];
        lockImage.enabled = true;
    }

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }
}

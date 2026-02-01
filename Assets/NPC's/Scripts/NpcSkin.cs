using Unity.Netcode;
using UnityEngine;

public class NpcSkin : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Index-matched")]
    [SerializeField] private RuntimeAnimatorController[] skins;
    [SerializeField] private Sprite[] idleSprites;

    public int SkinCount => skins != null ? skins.Length : 0;

    public NetworkVariable<int> SkinId = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>(true);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    public override void OnNetworkSpawn()
    {
        ApplySkin(SkinId.Value);
        SkinId.OnValueChanged += OnSkinChanged;
    }

    public override void OnNetworkDespawn()
    {
        SkinId.OnValueChanged -= OnSkinChanged;
    }

    private void OnSkinChanged(int oldV, int newV) => ApplySkin(newV);

    public void SetSkinServer(int id)
    {
        if (!IsServer) return;
        if (skins == null || skins.Length == 0) return;
        SkinId.Value = Mathf.Clamp(id, 0, skins.Length - 1);
    }

    public Sprite GetIdleSprite()
    {
        if (idleSprites == null || idleSprites.Length == 0) return null;
        int id = Mathf.Clamp(SkinId.Value, 0, idleSprites.Length - 1);
        return idleSprites[id];
    }

    private void ApplySkin(int id)
    {
        if (skins == null || skins.Length == 0) return;

        id = Mathf.Clamp(id, 0, skins.Length - 1);

        if (anim != null)
        {
            anim.runtimeAnimatorController = skins[id];
            anim.Rebind();
            anim.Update(0f);
        }
        
        if (spriteRenderer != null && idleSprites != null && id < idleSprites.Length && idleSprites[id] != null)
            spriteRenderer.sprite = idleSprites[id];
    }
}
using UnityEngine;

public class NpcAnimatorFromMovement : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private NpcSkin skin;

    [SerializeField] private float movingThreshold = 0.01f;

    private Vector3 lastPos;
    private Vector2 lastDir = Vector2.down;
    private bool wasMoving;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>(true);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        if (skin == null) skin = GetComponent<NpcSkin>();
    }

    private void OnEnable()
    {
        lastPos = transform.position;
        wasMoving = false;
        ApplyIdleSprite();
    }

    private void Update()
    {
        if (anim == null) return;

        Vector3 pos = transform.position;
        Vector3 delta = pos - lastPos;
        lastPos = pos;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector2 vel = new Vector2(delta.x, delta.y) / dt;
        bool moving = vel.sqrMagnitude > (movingThreshold * movingThreshold);

        if (moving)
            lastDir = Quantize4Dir(vel);

        anim.SetBool(IsMovingHash, moving);
        anim.SetFloat(MoveXHash, lastDir.x);
        anim.SetFloat(MoveYHash, lastDir.y);

        if (wasMoving && !moving)
            ApplyIdleSprite();

        wasMoving = moving;
    }

    private void ApplyIdleSprite()
    {
        if (spriteRenderer == null || skin == null) return;
        Sprite idle = skin.GetIdleSprite();
        if (idle != null) spriteRenderer.sprite = idle;
    }

    private static Vector2 Quantize4Dir(Vector2 v)
    {
        if (v.sqrMagnitude < 0.000001f) return Vector2.zero;
        return Mathf.Abs(v.x) > Mathf.Abs(v.y)
            ? new Vector2(Mathf.Sign(v.x), 0f)
            : new Vector2(0f, Mathf.Sign(v.y));
    }
}
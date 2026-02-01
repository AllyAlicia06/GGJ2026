using UnityEngine;
using Unity.Netcode;

public class CharacterControllerCustom : NetworkBehaviour
{
    protected CharacterMovement characterMovement;
    
    [Header("Animations")]
    [SerializeField] private Animator animator;
    
    private Vector2 lastDir = Vector2.down;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    
    protected virtual void Awake()
    {
        characterMovement = GetComponentInChildren<CharacterMovement>();
       
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (!IsOwner) return;

        HandleMovementInput();
        HandleSprintInput();
        HandleAbilityInput();
        
        Debug.Log("Controller Update " + name);
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log($"[{name}] IsOwner={IsOwner} OwnerClientId={OwnerClientId} LocalClientId={NetworkManager.Singleton.LocalClientId}");
        enabled = IsOwner;
        Debug.Log($"Controller spawned on {name} IsOwner={IsOwner} IsServer={IsServer} IsClient={IsClient}");
    }
    public virtual void HandleState()
    {
        //to be overridden in derived classes if needed
    }
    protected virtual void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = new Vector2(x, y).normalized;
        characterMovement.SetMovementInput(inputVector);
        UpdateAnimator(inputVector);
    }

    private void UpdateAnimator(Vector2 rawInput)
    {
        if (animator == null) return;

        Vector2 dir4 = Quantize4Dir(rawInput);
        bool moving = dir4 != Vector2.zero;

        if (moving)
            lastDir = dir4;

        animator.SetBool(IsMovingHash, moving);
        animator.SetFloat(MoveXHash, lastDir.x);
        animator.SetFloat(MoveYHash, lastDir.y);
        
        Debug.Log($"moving={moving} lastDir={lastDir} raw={rawInput}");
    }
    
    private static Vector2 Quantize4Dir(Vector2 v)
    {
        if (v.sqrMagnitude < 0.001f) return Vector2.zero;

        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return new Vector2(Mathf.Sign(v.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(v.y));
    }
    
    void HandleSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterMovement.IsSprinting = false;
        }
    }
    
    protected virtual void HandleAbilityInput()
    {
        
    }

    
}

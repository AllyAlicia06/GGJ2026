using UnityEngine;
using Unity.Netcode;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 1.5f;
    public float sprintMultiplier = 2.5f;
    public float coughSpeedPenalty = 0.5f;
    public float sprintAccelerationSmoothTime = 0.2f; 
    private float _currentSpeedMultiplier = 1f;
    private float _targetSpeedMultiplier = 1f;
    private Vector2 _currentVelocity = Vector2.zero; 
    private bool _isSprinting = false;
    public bool IsSprinting
    {
        get => _isSprinting;
        set 
        { 
            _isSprinting = value;
            _targetSpeedMultiplier = value ? sprintMultiplier : 1f;
        }
    }
    private bool _isCoughing = false;
    public bool IsCoughing
    {
        get => _isCoughing;
        set 
        { 
            _isCoughing = value; 
            _currentSpeedMultiplier = value ? _currentSpeedMultiplier * coughSpeedPenalty : _currentSpeedMultiplier / coughSpeedPenalty;
        }
    }
    private Rigidbody2D rb;
    private Vector2 currentInput;
    private NetworkObject rootNetObj;
    
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        rb.gravityScale = 0;

        //netObj = GetComponentInParent<NetworkObject>();
        var all = GetComponentsInParent<NetworkObject>(true);
        if (all != null && all.Length > 0)
            rootNetObj = all[all.Length - 1];
    }

    private void FixedUpdate()
    {
        if (rootNetObj != null && !rootNetObj.IsOwner) return;
        if (rootNetObj != null)
            Debug.Log($"{name} rootNetObj={rootNetObj.name} IsOwner={rootNetObj.IsOwner} OwnerClientId={rootNetObj.OwnerClientId} LocalClientId={NetworkManager.Singleton.LocalClientId}");
        
        _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, _targetSpeedMultiplier, Time.fixedDeltaTime / sprintAccelerationSmoothTime);

        if(currentInput == Vector2.zero)
        {
            _currentVelocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector2 targetVelocity = currentInput * movementSpeed * _currentSpeedMultiplier;
            _currentVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime / sprintAccelerationSmoothTime);
            rb.linearVelocity = _currentVelocity;
        }
    }

    public void SetMovementInput(Vector2 input)
    {
        currentInput = input;
    }

    
    
}

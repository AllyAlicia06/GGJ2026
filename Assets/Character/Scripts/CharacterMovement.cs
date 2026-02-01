

using UnityEngine;



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
    private Vector2 _lastDirection = Vector2.right;
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
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
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
            _lastDirection = _currentVelocity.normalized;
            rb.linearVelocity = _currentVelocity;
        }
    }

    public void SetMovementInput(Vector2 input)
    {
        currentInput = input;
    }
    public Vector2 GetCurrentVelocity()
    {
        return _currentVelocity;
    }
    public Vector2 GetLastDirection()
    {
        
        return _lastDirection;
    }

    
    
}

using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 0.5f;

    private Rigidbody2D rb;
    private Vector2 currentInput;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = currentInput * movementSpeed;
    }


    public void SetMovementInput(Vector2 input)
    {
        currentInput = input;
    }

    
}

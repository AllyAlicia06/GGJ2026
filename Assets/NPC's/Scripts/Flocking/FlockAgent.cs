
using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class FlockAgent : MonoBehaviour
{

    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    private Collider2D _agentCollider;
    public Collider2D AgentCollider { get { return _agentCollider; } }
    [HideInInspector]public float agentSpeed;
    [HideInInspector]public float squareMaxSpeed;
    private Transform _spriteTransform;

    [HideInInspector]public Vector2 cohesionVelocity;
    [HideInInspector]public Vector2 avoidanceVelocity;
    [HideInInspector]public Vector2 alignmentVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agentCollider = GetComponent<Collider2D>();
        //_agentCollider = GetComponentsInChildren<Collider2D>()[1];
        Debug.Log("Collider for " + gameObject.name + " is " + _agentCollider.name);
        _spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
    }
    public void Initialize(Flock flock)
    {
        agentFlock = flock;
        agentSpeed = Random.Range(flock.minSpeed, flock.maxSpeed);
        squareMaxSpeed = agentSpeed * agentSpeed;
    }
    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }

    void LateUpdate()
    {
        if (_spriteTransform != null)
        {
            // Quaternion.identity means "No Rotation" (0, 0, 0)
            // This forces the sprite to stay upright regardless of the parent
            _spriteTransform.rotation = Quaternion.identity;
        }
    }
}

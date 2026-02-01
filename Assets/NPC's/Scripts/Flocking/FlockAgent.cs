
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;





public class FlockAgent : MonoBehaviour
{

    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }
    private Collider2D _agentCollider;
    public Collider2D AgentCollider { get { return _agentCollider; } }
    //[HideInInspector]
    public float agentSpeed;
    //[HideInInspector]
    public float squareMaxSpeed;
    private Transform _spriteTransform;
    NpcInfected npcInfectedScript;
    public NpcInfected NpcInfectedScript { get { return npcInfectedScript; } }

    [HideInInspector]public Vector2 cohesionVelocity;
    [HideInInspector]public Vector2 avoidanceVelocity;
    [HideInInspector]public Vector2 alignmentVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_agentCollider = GetComponent<Collider2D>();
        _agentCollider = GetComponentsInChildren<Collider2D>()[0];
        Debug.Log("Collider for " + gameObject.name + " is " + _agentCollider.name);
        _spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
    
    }
    public void Initialize(Flock flock)
    {   
        agentFlock = flock;
        agentSpeed = Random.Range(flock.minSpeed, flock.maxSpeed);
        squareMaxSpeed = 4f;//agentSpeed * agentSpeed; NU STIU CE E AICI DAR MERGE :))
        npcInfectedScript = GetComponent<NpcInfected>();
        if (npcInfectedScript == null)
            Debug.LogError("FlockAgent " + gameObject.name + " has no NpcInfected script attached!");
        else{ Debug.Log(npcInfectedScript.name + " found on " + gameObject.name); 
        npcInfectedScript.Initialize(Random.value > 0.5f);
        }
    }
    public void Move(Vector2 velocity)
    {
        transform.up = velocity*agentSpeed;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AgentFlock.neighborRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agentFlock.avoidanceRadiusMultiplier * AgentFlock.neighborRadius);
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

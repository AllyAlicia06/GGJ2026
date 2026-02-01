using UnityEngine;
using Unity.Netcode;

public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }

    private Collider2D _agentCollider;
    public Collider2D AgentCollider { get { return _agentCollider; } }

    public float agentSpeed;
    public float squareMaxSpeed;

    private Transform _spriteTransform;

    NpcInfected npcInfectedScript;
    public NpcInfected NpcInfectedScript { get { return npcInfectedScript; } }

    [HideInInspector] public Vector2 cohesionVelocity;
    [HideInInspector] public Vector2 avoidanceVelocity;
    [HideInInspector] public Vector2 alignmentVelocity;

    void Start()
    {
        _agentCollider = GetComponentsInChildren<Collider2D>()[0];
        _spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
        agentSpeed = Random.Range(flock.minSpeed, flock.maxSpeed);
        squareMaxSpeed = 4f;

        npcInfectedScript = GetComponent<NpcInfected>();
        if (npcInfectedScript == null)
        {
            Debug.LogError("FlockAgent " + gameObject.name + " has no NpcInfected script attached!");
            return;
        }
        
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            npcInfectedScript.Initialize(Random.value > 0.5f);
        }
    }

    public void Move(Vector2 velocity)
    {
        if (velocity.sqrMagnitude > 0.0001f)
            transform.up = velocity.normalized;

        transform.position += (Vector3)(velocity * agentSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (_spriteTransform != null)
            _spriteTransform.rotation = Quaternion.identity;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;




public class Flock : NetworkBehaviour
{

    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;


    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 100f)]
    public float minSpeed = 2f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.2f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.3f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
            newAgent.Initialize(this);
            //Debug.Log("Spawned agent " + i + " with speed " + newAgent.agentSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) return;
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            //agent.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.Lerp(Color.white, Color.red, context.Count / 6f);
            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > agent.squareMaxSpeed)
            {
                move = move.normalized * agent.agentSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}

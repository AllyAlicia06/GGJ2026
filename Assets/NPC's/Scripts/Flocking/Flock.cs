using UnityEngine;
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

    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(1f, 100f)] public float maxSpeed = 5f;
    [Range(1f, 100f)] public float minSpeed = 2f;
    [Range(1f, 10f)]  public float neighborRadius = 1.2f;
    [Range(0f, 1f)]   public float avoidanceRadiusMultiplier = 0.3f;

    private bool spawnedOnce;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        if (spawnedOnce) return;
        spawnedOnce = true;

        CleanupExistingAgentsServer();
        SpawnAgents();
    }

    private void CleanupExistingAgentsServer()
    {
        var existingAgents = GetComponentsInChildren<FlockAgent>(true);
        foreach (var a in existingAgents)
        {
            if (a == null) continue;

            var no = a.GetComponent<NetworkObject>();
            if (no != null && no.IsSpawned)
                no.Despawn(true);
            else
                Destroy(a.gameObject);
        }

        agents.Clear();
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < startingCount; i++)
        {
            Vector3 pos = (Vector3)(Random.insideUnitCircle * startingCount * AgentDensity);
            Quaternion rot = Quaternion.identity;

            FlockAgent newAgent = Instantiate(agentPrefab, pos, rot);
            newAgent.name = "Agent " + i;

            var netObj = newAgent.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                Debug.LogError("Agent prefab is missing NetworkObject!");
                Destroy(newAgent.gameObject);
                continue;
            }

            netObj.Spawn();
            
            var npcSkin = newAgent.GetComponent<NpcSkin>();
            if (npcSkin != null && npcSkin.SkinCount > 0)
                npcSkin.SetSkinServer(Random.Range(0, npcSkin.SkinCount));
            
            agents.Add(newAgent);
            newAgent.Initialize(this);
        }
    }

    void Update()
    {
        if (!IsServer) return;

        for (int i = agents.Count - 1; i >= 0; i--)
        {
            var agent = agents[i];
            if (agent == null)
            {
                agents.RemoveAt(i);
                continue;
            }

            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;

            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        var context = new List<Transform>();
        if (agent == null) return context;

        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
                context.Add(c.transform);
        }
        return context;
    }
}
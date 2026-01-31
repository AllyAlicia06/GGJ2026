using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    public NetworkVariable<int> infectedCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> thermosPhase = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [Header("Thermos mapping")] 
    [SerializeField] private int phaseCount = 10;
    [SerializeField] private int maxForFull = 5;
    
    private readonly HashSet<ulong> infectedNpcIds = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        var npcs = FindObjectsByType<NpcInfected>(FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            if (npc.isInfected.Value) infectedNpcIds.Add(npc.NetworkObjectId);
        }

        ApplyCounts();
    }

    public void OnNpcInfectionChanged(ulong npcId, bool isInfected)
    {
        if (!IsServer) return;
        
        if(isInfected) infectedNpcIds.Add(npcId);
        else infectedNpcIds.Remove(npcId);

        ApplyCounts();
    }

    private void ApplyCounts()
    {
        infectedCount.Value = infectedNpcIds.Count;
        thermosPhase.Value = MapToPhase(infectedCount.Value);
    }

    private int MapToPhase(int infected)
    {
        if(phaseCount <= 1) return 0;
        float t = Mathf.Clamp01(infected / (float)maxForFull);
        int phase = Mathf.RoundToInt(t * (phaseCount - 1));
        return Mathf.Clamp(phase, 0, phaseCount - 1);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

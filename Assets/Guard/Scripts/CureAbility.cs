using System;
using UnityEngine;

public class CureAbility : MonoBehaviour
{
    [Header("Settings")] 
    public float range = 1.5f;
    public float cooldown = 2.5f;
    public LayerMask targetLayer;
    
    private float currentCooldown = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCooldown > 0f) currentCooldown -= Time.deltaTime;
    }

    public void TryCure()
    {
        if (currentCooldown > 0f)
        {
            Debug.Log($"Cooldown {currentCooldown:0.00}s");
            return;
        }
        
        NpcInfected target = FindClosestInfected();
        if (target == null) return;
        
        target.Cure();
        Debug.Log($"Cured {target.name}");
        
        currentCooldown = cooldown;
    }

    private NpcInfected FindClosestInfected()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);

        float bestDistSqr = float.PositiveInfinity;
        NpcInfected best = null;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            var infected = hit.GetComponentInParent<NpcInfected>();
            if (infected == null) continue;
            if (!infected.isInfected) continue;
            
            float dSqr = (infected.transform.position - transform.position).sqrMagnitude;
            if (dSqr < bestDistSqr)
            {
                bestDistSqr = dSqr;
                best = infected;
            }
        }

        return best;
    }
}

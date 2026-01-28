using UnityEngine;
using System.Collections;

public class CoughAbility : MonoBehaviour
{
    [Header("Settings")]
    public float minRange = 0.5f;
    public float maxRange = 2f;
    public float timeToMaxCharge = 2.0f; 
    public float cooldownDuration = 5f;
    public LayerMask targetLayer; 

    [Header("Visual Indicator")]
    [Tooltip("Use a circle transform with scale = 2 * minRange to indicate the range of the cough.")]
    public Transform rangeIndicator; 

    private float currentCooldown = 0f;
    private float currentChargeTime = 0f;
    private bool isCharging = false;

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            return;
        }
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;

            float chargePercent = Mathf.Clamp01(currentChargeTime / timeToMaxCharge);
            float currentRadius = Mathf.Lerp(minRange, maxRange, chargePercent);
            if (rangeIndicator != null)
            {
                if(rangeIndicator.gameObject.activeSelf == false) rangeIndicator.gameObject.SetActive(true);
                rangeIndicator.localScale = Vector3.one * (currentRadius * 2f); 
            }
        }
    }

    public void BeginCharge()
    {
        if(currentCooldown > 0) return;
        if(isCharging) return;
        isCharging = true;
        currentChargeTime = 0f;
    }
    public void ReleaseCharge()
    {
        if(!isCharging) return;
        FireCough();
    }

    void FireCough()
    {
        isCharging = false;
        if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);

        float chargePercent = Mathf.Clamp01(currentChargeTime / timeToMaxCharge);
        float finalRadius = Mathf.Lerp(minRange, maxRange, chargePercent);

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, finalRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {

            if (hit.gameObject == gameObject) continue;

            Debug.Log($"Coughed on: {hit.name}");
            
        }
        currentCooldown = cooldownDuration;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
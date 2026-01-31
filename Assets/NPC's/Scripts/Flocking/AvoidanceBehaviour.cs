using UnityEngine;
using System.Collections;
using System.Collections.Generic;




[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehaviour : FilteredFlockBehavior
{   
    public float smoothTime = 0.1f;
    public float avoidanceRadius = 1.0f;

    float squareAvoidanceRadius;


    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbors, return no adjustment
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        if (filteredContext.Count == 0)
            return Vector2.zero;

        // Find the average position of the neighbors
        Vector2 avoidanceMove = Vector2.zero;
        int nAvoid = 0;
        
        foreach (Transform item in filteredContext)
        {
           //Debug.Log("Checking avoidance for " + item.name);
           Vector3 closestPoint = item.gameObject.GetComponent<Collider2D>().ClosestPoint(agent.transform.position);
           if (Vector2.SqrMagnitude(closestPoint-agent.transform.position) < squareAvoidanceRadius)
           {
             nAvoid++;
             avoidanceMove += (Vector2)(agent.transform.position - closestPoint);
           }
        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        //avoidanceMove = Vector2.SmoothDamp(agent.transform.up, avoidanceMove, ref agent.avoidanceVelocity, smoothTime);
        return avoidanceMove;
    }

   public void OnDrawGizmos()
   {
       Gizmos.color = Color.yellow;
       Gizmos.DrawWireSphere(Vector3.zero, avoidanceRadius);
    }

    private void OnValidate()
    {
        squareAvoidanceRadius = avoidanceRadius * avoidanceRadius;
    }
    private void OnEnable()
    {
        squareAvoidanceRadius = avoidanceRadius * avoidanceRadius;
    }
}
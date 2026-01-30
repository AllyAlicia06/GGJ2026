using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Flock/Behavior/Cohesion")]
public class CohesionBehavior : FilteredFlockBehavior
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbors, return no adjustment
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        if (filteredContext.Count == 0)
            return Vector2.zero;

        // Find the center of mass of the neighbors
        Vector2 cohesionMove = Vector2.zero;
        foreach (Transform item in filteredContext)
        {
            cohesionMove += (Vector2)item.position;
        }
        cohesionMove /= filteredContext.Count;

        // Create a vector pointing from the agent to the center of mass
        cohesionMove -= (Vector2)agent.transform.position;
        return cohesionMove;
    }
}

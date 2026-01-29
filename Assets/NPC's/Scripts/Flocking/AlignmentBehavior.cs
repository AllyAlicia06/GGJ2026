using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FilteredFlockBehavior
{
    public float smoothTime = 1f;
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbors, return no adjustment
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        if (filteredContext.Count == 0)
            return agent.transform.up;

        // Find the average velocity of the neighbors
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform item in filteredContext)
        {
            alignmentMove += (Vector2)item.transform.up;
        }
        alignmentMove /= filteredContext.Count;

        alignmentMove = Vector2.SmoothDamp(agent.transform.up, alignmentMove, ref agent.alignmentVelocity, smoothTime);
        return alignmentMove;
    }
    
}

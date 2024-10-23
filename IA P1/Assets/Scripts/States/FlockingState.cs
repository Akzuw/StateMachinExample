using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingState : BoidState
{
    public FlockingState(Boid boid) : base(boid) { }

    public override void Execute()
    {
        Vector3 separation = boid.Separate();
        Vector3 alignment = boid.Align();
        Vector3 cohesion = boid.Cohere();
        Vector3 avoidance = boid.AvoidObstacles();

        boid.acceleration += separation + alignment + cohesion + avoidance;

        if (boid.hunter != null && Vector3.Distance(boid.transform.position, boid.hunter.position) <= boid.visionRadius)
        {
            boid.ChangeState(new EvadingState(boid));
        }
        else if (boid.targetFood != null && Vector3.Distance(boid.transform.position, boid.targetFood.position) <= boid.visionRadius)
        {
            boid.ChangeState(new ArrivingState(boid));
        }
        else if (boid.centralWaypoint != null && boid.targetFood == null && boid.hunter == null)
        {
            boid.ChangeState(new WaypointState(boid));
        }
    }
}
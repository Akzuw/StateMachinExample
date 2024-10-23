using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingState : BoidState
{
    public EvadingState(Boid boid) : base(boid) { }

    public override void Execute()
    {
        if (boid.hunter == null)
        {
            boid.ChangeState(new FlockingState(boid));
            return;
        }

        Vector3 desired = boid.transform.position - boid.hunter.position;
        desired.Normalize();
        desired *= boid.maxSpeed;
        Vector3 steer = desired - boid.velocity;
        steer = Vector3.ClampMagnitude(steer, boid.maxForce);
        steer.y = 0; 
        boid.acceleration += steer;

        if (Vector3.Distance(boid.transform.position, boid.hunter.position) > boid.visionRadius)
        {
            boid.ChangeState(new FlockingState(boid));
        }
    }
}
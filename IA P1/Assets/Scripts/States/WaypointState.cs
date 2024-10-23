using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointState : BoidState
{
    public WaypointState(Boid boid) : base(boid) { }

    public override void Enter() { }

    public override void Execute()
    {
        if (boid.centralWaypoint != null)
        {
            Vector3 seekForce = boid.Seek(boid.centralWaypoint.position);
            boid.acceleration += seekForce;
        }
    }

    public override void Exit() { }
}
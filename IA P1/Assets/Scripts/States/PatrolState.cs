using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public PatrolState(Hunter hunter) : base(hunter) { }

    public override void Enter() { }

    public override void Execute()
    {
        if (hunter.waypoints.Length == 0) return;

        Transform targetWaypoint = hunter.waypoints[hunter.currentWaypointIndex];
        Vector3 desired = (targetWaypoint.position - hunter.transform.position).normalized * hunter.maxSpeed;
        desired = hunter.AvoidObstacles(desired);
        Vector3 steer = desired - hunter.velocity;
        steer = Vector3.ClampMagnitude(steer, hunter.maxForce);
        hunter.acceleration += steer;

        if (Vector3.Distance(hunter.transform.position, targetWaypoint.position) < hunter.waypointThreshold)
        {
            if (!hunter.isReversing)
            {
                hunter.currentWaypointIndex++;
                if (hunter.currentWaypointIndex >= hunter.waypoints.Length)
                {
                    hunter.currentWaypointIndex = hunter.waypoints.Length - 1;
                    hunter.isReversing = true;
                }
            }
            else
            {
                hunter.currentWaypointIndex--;
                if (hunter.currentWaypointIndex < 0)
                {
                    hunter.currentWaypointIndex = 0;
                    hunter.isReversing = false;
                }
            }
        }

        hunter.energy -= hunter.energyDrainRate * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new RestState(hunter));
            return; 
        }

        GameObject targetBoid = hunter.FindClosestBoid();
        if (targetBoid != null && Vector3.Distance(hunter.transform.position, targetBoid.transform.position) <= hunter.visionRadius)
        {
            hunter.ChangeState(new ChaseState(hunter));
        }
    }

    public override void Exit() { }
}

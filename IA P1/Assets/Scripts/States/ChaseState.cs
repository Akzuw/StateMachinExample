using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Hunter hunter) : base(hunter) { }

    public override void Enter() { }

    public override void Execute()
    {
        GameObject targetBoid = hunter.FindClosestBoid();
        if (targetBoid == null)
        {
            hunter.ChangeState(new PatrolState(hunter));
            return;
        }

        Vector3 desired = hunter.Pursuit(targetBoid);
        desired = hunter.AvoidObstacles(desired);
        Vector3 steer = desired - hunter.velocity;
        steer = Vector3.ClampMagnitude(steer, hunter.maxForce);
        hunter.acceleration += steer;

        hunter.energy -= hunter.energyDrainRate * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new RestState(hunter));
        }
    }

    public override void Exit() { }
}

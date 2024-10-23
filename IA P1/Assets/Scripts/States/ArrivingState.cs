using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivingState : BoidState
{
    public ArrivingState(Boid boid) : base(boid) { }

    public override void Execute()
    {
        if (boid.targetFood == null)
        {
            boid.ChangeState(new FlockingState(boid));
            return;
        }

        Vector3 desired = boid.targetFood.position - boid.transform.position;
        float distance = desired.magnitude;
        desired.Normalize();

        if (distance < boid.arriveRadius)
        {
            desired *= boid.maxSpeed * (distance / boid.arriveRadius);
        }
        else
        {
            desired *= boid.maxSpeed;
        }

        Vector3 steer = desired - boid.velocity;
        steer = Vector3.ClampMagnitude(steer, boid.maxForce);
        boid.acceleration += steer;

        if (distance < boid.arriveRadius)
        {
            boid.targetFood.gameObject.SetActive(false);
            boid.targetFood = null;
            boid.ChangeState(new FlockingState(boid));
        }
    }
}
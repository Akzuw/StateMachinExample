using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestState : State
{
    private float restTimer;

    public RestState(Hunter hunter) : base(hunter) { }

    public override void Enter()
    {
        restTimer = 0f;
        hunter.velocity = Vector3.zero; 
        hunter.acceleration = Vector3.zero; 
    }

    public override void Execute()
    {
        restTimer += Time.deltaTime;
        if (restTimer >= hunter.restDuration)
        {
            hunter.energy = hunter.maxEnergy;
            hunter.ChangeState(new PatrolState(hunter));
        }
    }

    public override void Exit() { }
}
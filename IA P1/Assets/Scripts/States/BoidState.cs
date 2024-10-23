using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoidState
{
    protected Boid boid;

    public BoidState(Boid boid)
    {
        this.boid = boid;
    }

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}

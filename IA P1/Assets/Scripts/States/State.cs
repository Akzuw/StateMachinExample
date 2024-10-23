using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Hunter hunter;

    public State(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}
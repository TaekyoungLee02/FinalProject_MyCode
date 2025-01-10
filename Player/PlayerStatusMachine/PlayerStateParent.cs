using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateParent : BaseState
{
    protected PlayerStateMachine parentMachine;
    protected StateMachine currentMachine;
    protected Dictionary<string, PlayerStateBase> states;

    public PlayerStateParent(PlayerStateMachine parentMachine)
    {
        this.parentMachine = parentMachine;
        currentMachine = new();
    }

    public abstract bool CanMoveToOtherStateParent(PlayerStateParent otherStateParent);

    public override void FixedUpdate(float time)
    {
        currentMachine.FixedUpdate(time);
    }

    public override void Update()
    {
        currentMachine.Update();
    }

    public T GetState<T>() where T : PlayerStateBase
    {
        return states[typeof(T).Name] as T;
    }
}

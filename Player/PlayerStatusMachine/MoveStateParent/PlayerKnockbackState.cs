using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerKnockbackState : PlayerStateBase
{
    public event Action OnKnockbackStart;
    public event Action OnKnockbacking;
    public event Action OnKnockbackEnd;

    public override void Enter()
    {
        OnKnockbackStart?.Invoke();
    }

    public override void Exit()
    {
        OnKnockbackEnd?.Invoke();
    }

    public override void FixedUpdate(float time)
    {
        OnKnockbacking?.Invoke();
    }

    public override void Update()
    {
        
    }
}

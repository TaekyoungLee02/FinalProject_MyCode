using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public event Action<Vector2> OnMoveStart;
    public event Action<Vector2> OnMoving;
    public event Action<Vector2> OnMoveEnd;

    private PlayerInputManager inputManager;

    public PlayerMoveState(PlayerInputManager playerInputManager)
    {
        inputManager = playerInputManager;
    }

    public override void Enter()
    {
        OnMoveStart?.Invoke(inputManager.PlayerMove);
    }

    public override void Exit()
    {
        OnMoveEnd?.Invoke(inputManager.PlayerMove);
    }

    public override void FixedUpdate(float time)
    {
        OnMoving?.Invoke(inputManager.PlayerMove);
    }

    public override void Update()
    {
        
    }
}

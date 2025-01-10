using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    public PlayerStateMachine(Player player)
    {
        //player.InputManager.

        states = new()
        {
            { nameof(PlayerMoveStateParent), new PlayerMoveStateParent(this) },
            { nameof(PlayerAttackStateParent), new PlayerAttackStateParent(this) },
            { nameof(PlayerInteractionStateParent), new PlayerInteractionStateParent(this) },
        };

        ChangeState(states[nameof(PlayerMoveStateParent)]);
    }

    protected Dictionary<string, PlayerStateParent> states;

    public bool OnStateMove<T>() where T : PlayerStateParent
    {
        bool canMove = (currentState as PlayerStateParent).CanMoveToOtherStateParent(states[typeof(T).Name]);

        if (canMove)
            ChangeState(states[typeof(T).Name]);

        return canMove;
    }

    public T GetState<T>() where T : PlayerStateParent
    {
        return states[typeof(T).Name] as T;
    }
}

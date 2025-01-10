using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackStateParent : PlayerStateParent
{
    public PlayerAttackStateParent(PlayerStateMachine parentState) : base(parentState)
    {
        var player = GameManager.Instance.Player;


        player.InputManager.OnPlayerNormalAttack += () => Attack(false);
        player.InputManager.OnPlayerChargeAttack += () => Attack(true);

        states = new()
        {
            { nameof(PlayerAttackState), new PlayerAttackState(this) },
        };
    }

    private void Attack(bool isChargeAttack)
    {
        if (parentMachine.OnStateMove<PlayerAttackStateParent>())
        {
            GetState<PlayerAttackState>().AttackInit(isChargeAttack);

            currentMachine.ChangeState(states[nameof(PlayerAttackState)]);
        }
    }

    public void AttackEnd()
    {
        parentMachine.OnStateMove<PlayerMoveStateParent>();
    }

    public override bool CanMoveToOtherStateParent(PlayerStateParent otherStateParent)
    {
        if (otherStateParent is PlayerAttackStateParent)
        {
            return false;
        }

        return true;
    }

    public override void Enter() { }
    public override void Exit() { }
}

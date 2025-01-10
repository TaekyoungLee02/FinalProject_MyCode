using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionStateParent : PlayerStateParent
{
    private DialogManager dialogManager;

    public PlayerInteractionStateParent(PlayerStateMachine parentState) : base(parentState)
    {
        dialogManager = GameManager.Instance.GetManager<DialogManager>();
        dialogManager.OnDialogStart += () => parentMachine.OnStateMove<PlayerInteractionStateParent>();
        dialogManager.OnDialogEnd += () => parentMachine.OnStateMove<PlayerMoveStateParent>();

        states = new()
        {
            { nameof(PlayerDialogState), new PlayerDialogState() },
            { nameof(PlayerInteractionState), new PlayerInteractionState() },

        };
    }


    public override bool CanMoveToOtherStateParent(PlayerStateParent otherStateParent)
    {
        return true;
    }

    public override void Enter()
    {
        // PlayerInteractionState ·Î ¹Ù²Þ
        currentMachine.ChangeState(states[nameof(PlayerInteractionState)]);
    }

    public override void Exit()
    {

    }
}

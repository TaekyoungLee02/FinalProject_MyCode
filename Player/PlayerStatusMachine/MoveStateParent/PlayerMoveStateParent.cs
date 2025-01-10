using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveStateParent : PlayerStateParent
{
    private UserInterface mobileControllerUI;

    public PlayerMoveStateParent(PlayerStateMachine parentState) : base(parentState)
    {
        Player player = GameManager.Instance.Player;

        player.PlayerStatus.OnKnockbackEvent += OnKnockBackStart;
        player.PlayerMovement.PowerApplyingEnd += OnKnockBackEnd;

        player.InputManager.OnPlayerMoveStart += OnMoveStart;
        player.InputManager.OnPlayerMovePerformed += OnMoveStart;
        player.InputManager.OnPlayerMoveCanceled += OnMoveEnd;

        player.InputManager.Joystick.Started += OnMoveStart;
        player.InputManager.Joystick.Performed += OnMoveStart;
        player.InputManager.Joystick.Canceled += OnMoveEnd;

        GameManager.Instance.GetManager<DialogManager>().OnDialogStart += () => parentMachine.OnStateMove<PlayerInteractionStateParent>();

        states = new()
        {
            { nameof(PlayerIdleState), new PlayerIdleState() },
            { nameof(PlayerMoveState), new PlayerMoveState(player.InputManager) },
            { nameof(PlayerKnockbackState), new PlayerKnockbackState() }
        };
    }

    public void OnMoveStart(Vector2 vector2)
    {
        // ���� State �� ��쿡�� ����
        if (parentMachine.currentState != this) return;

        // Idle �� ��쿡
        if (currentMachine.currentState is PlayerIdleState)
        {
            // MoveState �� �ٲ�
            currentMachine.ChangeState(states[nameof(PlayerMoveState)]);
        }

    }
    public void OnMoveEnd(Vector2 vector2)
    {
        // ���� State �� ��쿡�� ����
        if (parentMachine.currentState != this) return;

        // Nove �� ��쿡
        if (currentMachine.currentState is PlayerMoveState)
        {
            // IdleState �� �ٲ�
            currentMachine.ChangeState(states[nameof(PlayerIdleState)]);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="knockbackDirection">Knockback Direction</param>
    /// <param name="value">NOT USED</param>
    public void OnKnockBackStart(Vector2 knockbackDirection, float value)
    {
        // ���� State �� ��쿡�� ����
        if (parentMachine.currentState != this) return;

        // Idle Ȥ�� Move �� ��쿡
        if (currentMachine.currentState is PlayerIdleState || currentMachine.currentState is PlayerMoveState)
        {
            // KnockbackState �� �ٲ�
            currentMachine.ChangeState(states[nameof(PlayerKnockbackState)]);
        }
    }
    public void OnKnockBackEnd()
    {
        // ���� State �� ��쿡�� ����
        if (parentMachine.currentState != this) return;

        // Knockback �� ��쿡
        if (currentMachine.currentState is PlayerKnockbackState)
        {
            // IdleState �� �ٲ�
            currentMachine.ChangeState(states[nameof(PlayerIdleState)]);
        }
    }


    public override void Enter()
    {
        // ����� ȯ���� �� ����� ��Ʈ�ѷ� Ű��
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            GameManager.Instance.GetManager<UIManager>().UIDictionary[UITYPE.MOBILECONTROLLERUI].EnableUI();

        // IdleState �� �ٲ�
        currentMachine.ChangeState(states[nameof(PlayerIdleState)]);
    }

    public override void Exit()
    {
        
    }

    public override bool CanMoveToOtherStateParent(PlayerStateParent otherStateParent)
    {
        if (otherStateParent is PlayerAttackStateParent)
        {
            return currentMachine.currentState is PlayerIdleState || currentMachine.currentState is PlayerMoveState;
        }

        if (otherStateParent is PlayerInteractionStateParent)
        {
            return currentMachine.currentState is PlayerIdleState || currentMachine.currentState is PlayerMoveState;
        }

        return false;
    }
}

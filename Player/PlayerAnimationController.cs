using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // �÷��̾� �ִϸ��̼� ��Ʈ�ѷ��� �ּ�
    [SerializeField] private RuntimeAnimatorController animatorControllerAsset;

    // �ִϸ��̼� ��Ʈ�ѷ��� Parameters �� name, hash �� ������ �ִ� Dictionary
    private Dictionary<string, int> animatorParameters;

    private Animator playerAnimator;
    private PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAnimator.runtimeAnimatorController = animatorControllerAsset;

        playerStateMachine = GetComponent<PlayerController>().StateMachine;

        animatorParameters ??= new();

        InitAnimatorCondition();
        playerAnimator.enabled = true;
    }

    private void InitAnimatorCondition()
    {
        InitMoveAnimatorCondition();
        InitAttackAnimatorCondition();
    }

    private void InitMoveAnimatorCondition()
    {
        // StateMachine ���� State�� ������ ��, ���� �� �ִϸ��̼� Bool �� �ٲ��.
        StringBuilder sb;
        string move = AnimatorParameterConstants.isMove;
        var moveStateParent = playerStateMachine.GetState<PlayerMoveStateParent>();


        // �̵� State�� ������ ���� ���� ���� ���� �ִϸ��̼� Move State �� �����ϴ� ���ǰ� ������ ������ �޾� ��.
        var moveState = moveStateParent.GetState<PlayerMoveState>();
        sb = new StringBuilder(move).Append("_").Append(AnimatorParameterConstants.Move);

        int moveHash = GetAnimatorParameter(sb.ToString());

        int playerDirectionXHash = GetAnimatorParameter(AnimatorParameterConstants.PlayerDirectionX);
        int playerDirectionYHash = GetAnimatorParameter(AnimatorParameterConstants.PlayerDirectionY);

        moveState.OnMoveStart += (Vector2 direction) => 
        { 
            playerAnimator.SetBool(moveHash, true);
            playerAnimator.SetFloat(playerDirectionXHash, direction.x);
            playerAnimator.SetFloat(playerDirectionYHash, direction.y);
        };

        moveState.OnMoving += (Vector2 direction) =>
        {
            playerAnimator.SetFloat(playerDirectionXHash, direction.x);
            playerAnimator.SetFloat(playerDirectionYHash, direction.y);
        };

        moveState.OnMoveEnd += (Vector2 notUsed) => playerAnimator.SetBool(moveHash, false);


        // �˹� State�� ������ ���� ���� ���� ���� �ִϸ��̼� Knockback State �� �����ϴ� ���ǰ� ������ ������ �޾� ��.
        var knockbackState = moveStateParent.GetState<PlayerKnockbackState>();
        sb = new StringBuilder(move).Append("_").Append(AnimatorParameterConstants.Knockback);
        int knockbackHash = GetAnimatorParameter(sb.ToString());
        knockbackState.OnKnockbackStart += () => playerAnimator.SetBool(knockbackHash, true);
        knockbackState.OnKnockbackEnd += () => playerAnimator.SetBool(knockbackHash, false);
    }

    private void InitAttackAnimatorCondition()
    {
        var attackState = playerStateMachine.GetState<PlayerAttackStateParent>().GetState<PlayerAttackState>();

        int isAttackHash = GetAnimatorParameter(AnimatorParameterConstants.isAttack);
        int dashSpeedHash = GetAnimatorParameter(AnimatorParameterConstants.DashSpeed);
        int playerDirectionXHash = GetAnimatorParameter(AnimatorParameterConstants.PlayerDirectionX);
        int playerDirectionYHash = GetAnimatorParameter(AnimatorParameterConstants.PlayerDirectionY);

        attackState.OnAttackStart += (float dashSpeed) =>
        {
            playerAnimator.SetFloat(dashSpeedHash, dashSpeed);
            playerAnimator.SetBool(isAttackHash, true);
        };

        attackState.OnAttacking += (Vector2 direction) =>
        {
            if (direction.magnitude == 0) return;

            playerAnimator.SetFloat(playerDirectionXHash, direction.x);
            playerAnimator.SetFloat(playerDirectionYHash, direction.y);
        };

        attackState.OnAttackEnd += () => playerAnimator.SetBool(isAttackHash, false);
    }

    private int GetAnimatorParameter(string parameter)
    {
        if (animatorParameters.ContainsKey(parameter)) return animatorParameters[parameter];

        animatorParameters.Add(parameter, Animator.StringToHash(parameter));

        return animatorParameters[parameter];
    }

    // Dictionary ���� ���� Key(Name) ���� ���� �����
    private class AnimatorParameterConstants
    {
        public const string isAttack = "isAttack";

        public const string isMove = "isMove";
        public const string Move = "Move";
        public const string Knockback = "Knockback";

        public const string PlayerDirectionX = "PlayerDirectionX";
        public const string PlayerDirectionY = "PlayerDirectionY";

        public const string DashSpeed = "DashSpeed";
        public const string AttackEnd = "AttackEnd";
    }
}

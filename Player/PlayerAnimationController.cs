using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // 플레이어 애니메이션 컨트롤러의 애셋
    [SerializeField] private AnimatorController animatorControllerAsset;

    // 애니메이션 컨트롤러의 Parameters 를 name, hash 로 가지고 있는 Dictionary
    private Dictionary<string, int> animatorParameters;

    private Animator playerAnimator;
    private PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAnimator.runtimeAnimatorController = animatorControllerAsset;

        playerStateMachine = GetComponent<PlayerController>().StateMachine;

        InitAnimatorCondition();
        playerAnimator.enabled = true;
    }

    private void InitAnimatorCondition()
    {
        InitAnimatorParameters();
        InitMoveAnimatorCondition();
        InitAttackAnimatorCondition();
    }

    private void InitMoveAnimatorCondition()
    {
        // StateMachine 에서 State가 시작할 때, 끝날 때 애니메이션 Bool 이 바뀐다.
        StringBuilder sb;
        string move = AnimatorParameterConstants.isMove;
        var moveStateParent = playerStateMachine.GetState<PlayerMoveStateParent>();


        // 이동 State가 시작할 때와 끝날 때에 각각 애니메이션 Move State 가 시작하는 조건과 끝나는 조건을 달아 줌.
        var moveState = moveStateParent.GetState<PlayerMoveState>();
        sb = new StringBuilder(move).Append("_").Append(AnimatorParameterConstants.Move);

        int moveHash = animatorParameters[sb.ToString()];

        int playerDirectionXHash = animatorParameters[AnimatorParameterConstants.PlayerDirectionX];
        int playerDirectionYHash = animatorParameters[AnimatorParameterConstants.PlayerDirectionY];

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


        // 넉백 State가 시작할 때와 끝날 때에 각각 애니메이션 Knockback State 가 시작하는 조건과 끝나는 조건을 달아 줌.
        var knockbackState = moveStateParent.GetState<PlayerKnockbackState>();
        sb = new StringBuilder(move).Append("_").Append(AnimatorParameterConstants.Knockback);
        int knockbackHash = animatorParameters[sb.ToString()];
        knockbackState.OnKnockbackStart += () => playerAnimator.SetBool(knockbackHash, true);
        knockbackState.OnKnockbackEnd += () => playerAnimator.SetBool(knockbackHash, false);
    }

    private void InitAttackAnimatorCondition()
    {
        var attackState = playerStateMachine.GetState<PlayerAttackStateParent>().GetState<PlayerAttackState>();

        int isAttackHash = animatorParameters[AnimatorParameterConstants.isAttack];
        int dashSpeedHash = animatorParameters[AnimatorParameterConstants.DashSpeed];
        int playerDirectionXHash = animatorParameters[AnimatorParameterConstants.PlayerDirectionX];
        int playerDirectionYHash = animatorParameters[AnimatorParameterConstants.PlayerDirectionY];

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

    private void InitAnimatorParameters()
    {
        animatorParameters = new();
        var param = animatorControllerAsset.parameters;

        for (int i = 0; i < param.Length; i++)
        {
            animatorParameters.Add(param[i].name, param[i].nameHash);
        }
    }

    // Dictionary 에서 사용될 Key(Name) 값을 가진 상수들
    private class AnimatorParameterConstants
    {
        public const string isMove = "isMove";
        public const string Move = "Move";
        public const string Knockback = "Knockback";

        public const string isAttack = "isAttack";

        public const string PlayerDirectionX = "PlayerDirectionX";
        public const string PlayerDirectionY = "PlayerDirectionY";

        public const string DashSpeed = "DashSpeed";
        public const string AttackEnd = "AttackEnd";
    }
}

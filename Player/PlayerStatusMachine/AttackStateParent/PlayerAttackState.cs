using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerAttackState : PlayerStateBase
{
    public event Action<float> OnAttackStart;
    public event Action<Vector2> OnAttacking;
    public event Action OnAttackEnd;

    private PlayerEquipment playerEquip;
    private PlayerAttackStateParent parent;
    private PlayerStatus playerStatus;
    private PlayerMovement playerMovement;
    private PlayerInputManager playerInput;
    private Weapon playerWeapon;

    private float attackTimer;
    private float dashTimer;

    private float dashSpeed;
    private float dashTime;

    private Vector2 dashDirection;

    private Image attackCooltimeUI;
    private float cooltimeUIFilled;

    private ActionData attackData;

    public PlayerAttackState(PlayerAttackStateParent parent)
    {
        this.parent = parent;

        var player = GameManager.Instance.Player;
        playerEquip = player.Equipment;
        playerStatus = player.PlayerStatus;
        playerMovement = player.PlayerMovement;
        playerInput = player.InputManager;

        attackTimer = 0;
        dashTimer = 0;

        attackCooltimeUI = GameManager.Instance.GetManager<UIManager>().UIDictionary[UITYPE.ACTIONUI].GetUI<AttackButtonUI>().AttackCooltimeUI;
    }

    public void AttackInit(bool isChargeAttack)
    {
        // Attack 종류 결정
        if (isChargeAttack) attackData = playerEquip.chargeAttackData;
        else attackData = playerEquip.normalAttackData;

        // 현재 Weapon 받아오기
        playerWeapon = playerEquip.Weapon;

        // Attack 종류에 따라 대시 설정
        SetDash();

        // 공격 방향 설정, 공격
        playerEquip.actor.SetDirection(dashDirection);
    }

    // 실제 공격 실행
    public override void Enter()
    {
        playerEquip.actor.UseAction(attackData);

        attackCooltimeUI.fillAmount = 1;

        // 대시 속도 설정
        float dashSpeed = dashTime;
        if (dashSpeed <= 0) dashSpeed = 1;

        OnAttackStart?.Invoke(1 / dashSpeed);
    }

    public override void Exit()
    {
        // 이전 상태로 돌아감
        dashTimer = 0;
        attackTimer = 0;
        attackCooltimeUI.fillAmount = 0;
        OnAttackEnd?.Invoke();
    }

    public override void FixedUpdate(float time)
    {
        dashTimer += Time.fixedDeltaTime;

        if (dashTimer < dashTime)
        {
            Dash();
        }
        else
        {
            OnAttacking?.Invoke(playerInput.PlayerMove);
        }
    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;

        attackCooltimeUI.fillAmount = 1 - (attackTimer / playerWeapon.WeaponAttackCooltime);

        if (attackTimer > playerWeapon.WeaponAttackCooltime)
        {
            Exit();
            parent.AttackEnd();
        }
    }

    private void SetDash()
    {
        dashDirection = (playerMovement.CurrentPosition - playerMovement.LastPosition).normalized;
        dashTime = attackData.dashTime;
        dashSpeed = attackData.dashSpeed;
    }

    private void Dash()
    {
        playerMovement.Dash(dashDirection, dashSpeed);
    }
}
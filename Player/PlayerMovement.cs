/*
    플레이어의 이동에 관련된 코드입니다.

    원래는 rigidbody 를 사용하지 않는 방향으로 가려고 했는데(오버헤드로 인한 문제 때문)

    사용하는 것이 더 쉽게 문제를 해결할 수 있을 것 같아서 그렇게 하기로 결정했습니다.

    전부 제가 작성했습니다.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _maxKnockbackTime = 0.5f;
    private float _knockbackSlopeHanler = 2f;
    private float _knockbackPower;
    private float _knockbackTimer;
    private Vector2 _knockbackDirection;

    private Rigidbody2D _playerRB;
    private PlayerMoveStateParent _playerMoveStateParent;
    private PlayerAttackStateParent _playerAttackStateParent;
    private PlayerStatus _playerStatus;

    public event Action PowerApplyingEnd;

    private const float _speedMultiplyer = 6.5f;

    private Vector2 lastPosition;
    public Vector2 LastPosition { get { return lastPosition; } }
    public Vector2 CurrentPosition { get { return _playerRB.position; } }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerMoveStateParent = GetComponent<PlayerController>().StateMachine.GetState<PlayerMoveStateParent>();
        _playerAttackStateParent = GetComponent<PlayerController>().StateMachine.GetState<PlayerAttackStateParent>();
        _playerStatus = GetComponent<PlayerStatus>();
    }


    private void OnEnable()
    {
        InitMovementEvent();
    }

    private void OnDisable()
    {
        DeleteMovementEvent();
    }

    private void InitMovementEvent()
    {
        _playerAttackStateParent.GetState<PlayerAttackState>().OnAttacking += Move;

        _playerMoveStateParent.GetState<PlayerMoveState>().OnMoving += Move;
        _playerMoveStateParent.GetState<PlayerKnockbackState>().OnKnockbacking += Knockback;
        _playerStatus.OnKnockbackEvent += StartKnockback;
    }

    private void DeleteMovementEvent()
    {
        _playerAttackStateParent.GetState<PlayerAttackState>().OnAttacking -= Move;

        _playerMoveStateParent.GetState<PlayerMoveState>().OnMoving -= Move;
        _playerMoveStateParent.GetState<PlayerKnockbackState>().OnKnockbacking -= Knockback;
        _playerStatus.OnKnockbackEvent -= StartKnockback;
    }

    public void StartKnockback(Vector2 knockbackDirection, float knockbackPower)
    {
        _knockbackDirection = knockbackDirection;
        _knockbackPower = knockbackPower;
        _knockbackTimer = 0;
    }

    private void Knockback()
    {
        _knockbackTimer += Time.fixedDeltaTime;
        if (_knockbackTimer > _maxKnockbackTime)
        {
            _knockbackPower = 0;
            PowerApplyingEnd?.Invoke();
            return;
        }

        float currentPower = KnockbackCalculator(_knockbackPower, _knockbackTimer, _knockbackSlopeHanler);

        Vector2 endPoint = _playerRB.position + _knockbackDirection.normalized;
        Vector2 newPosition = Vector2.MoveTowards(_playerRB.position, endPoint, currentPower * Time.fixedDeltaTime);

        _playerRB.MovePosition(newPosition);
    }

    private float KnockbackCalculator(float knockbackPower, float time, float knockbackSlopeHandler)
    {
        float parameter = -(time - 1);
        float power = Mathf.Pow(knockbackPower, parameter * _knockbackSlopeHanler);
        return power;
    }

    private void Move(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
            lastPosition = _playerRB.position;

        Vector2 endPoint = _playerRB.position + moveDirection.normalized;

        float totalSpeedStatus = _playerStatus.extraStatus[(int)EXTRA_STATUS.SPEED] + _playerStatus.buffedExtraStatus[(int)EXTRA_STATUS.SPEED];

        Vector2 newPosition = Vector2.MoveTowards(_playerRB.position, endPoint, (totalSpeedStatus * _speedMultiplyer / 100.0f) * Time.fixedDeltaTime);

        _playerRB.MovePosition(newPosition);
    }

    public void Dash(Vector2 dashDirection, float dashSpeed)
    {
        Vector2 endPoint = _playerRB.position + dashDirection;

        Vector2 newPosition = Vector2.MoveTowards(_playerRB.position, endPoint, dashSpeed * 0.01f * Time.fixedDeltaTime);

        _playerRB.MovePosition(newPosition);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _playerRB.velocity = Vector2.zero;
    }
}

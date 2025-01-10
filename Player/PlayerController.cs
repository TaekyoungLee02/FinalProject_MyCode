using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

    /*
     * �÷��̾� ��Ʈ�ѷ�. �÷��̾��� �ൿ�� StateMachine���� ������
     */

{
    private PlayerStateMachine stateMachine;

    public PlayerStateMachine StateMachine
    {
        get
        {
            return stateMachine ??= new(GetComponent<Player>());
        }
    }

    private void Awake()
    {
        stateMachine ??= new(GetComponent<Player>());
    }

    /// <summary>
    /// StateMachine �� Update() �� ȣ��.
    /// </summary>
    private void Update()
    {
        stateMachine?.Update();
    }

    /// <summary>
    /// StateMachine �� FixedUpdate() �� ȣ��.
    /// </summary>
    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate(Time.fixedDeltaTime);
    }
}

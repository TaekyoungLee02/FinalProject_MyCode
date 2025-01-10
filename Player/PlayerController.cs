/*
    플레이어의 행동을 StateMachine 으로 정의하는 클래스입니다.

    플레이어의 State 에 따라 Update() 를 호출합니다.

    전부 제가 작성했습니다.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

    /*
     * 플레이어 컨트롤러. 플레이어의 행동을 StateMachine으로 통제함
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
    /// StateMachine 의 Update() 를 호출.
    /// </summary>
    private void Update()
    {
        stateMachine?.Update();
    }

    /// <summary>
    /// StateMachine 의 FixedUpdate() 를 호출.
    /// </summary>
    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate(Time.fixedDeltaTime);
    }
}

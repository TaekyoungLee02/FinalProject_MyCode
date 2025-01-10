/*
    StateMachine에 대해 고민해 본 결과, 두 가지 구조에 대해 생각해 보았습니다.
    
    첫 번째 구조는 Enum 등을 사용해 현재 어떤 State인지만 알려주고, 실제 작업은 PlayerController가 담당하는 방식입니다. 두 번째 구조는 StateMachine 안의 각 State가 직접 작업을 수행하는 방식입니다.

    처음에는 작업의 주체가 하나인 것이 더 좋다고 판단하여 첫 번째 구조를 구상했으나, Enum을 사용할 경우 if문이나 switch문이 끝없이 늘어날 가능성이 있고,
    
    State가 많아질수록 그 동작 원리를 하나의 코드 안에 기술하는 것이 어렵다는 점 때문에 두 번째 구조로 방향을 정했습니다.



    그렇다면 이제 State에 대해 고민해 보았습니다. 간단히 생각해 보아도 플레이어의 "상태"는 셀 수 없이 많습니다.
    
    예를 들어, 이동만 하더라도 실제 이동, 넉백될 때, 점프할 때 등 모든 경우의 동작 원리가 달라 별도로 기술해야 합니다. 공격의 경우에도 평타, 스킬, 군중 제어 등 각각의 동작 원리가 다르기 때문에 따로 작성해야 합니다.

    플레이어의 이러한 수많은 State를 어떻게 효율적이고 명확하게 구조화할 수 있을지 고민한 결과, 이중 State를 도입하기로 결정했습니다.
    
    간단히 설명드리자면, StateMachine은 플레이어의 "공격", "이동", "상호작용" 등과 같은 큰 범주의 State 흐름만 관리하고, 이러한 큰 State는 다시 하나의 작은 StateMachine이 되는 구조입니다.

    이러한 방식으로 큰 범주의 State들은 외부에서 관리할 때 전체적인 State의 흐름을 한눈에 파악하기 쉬워지며,
    
    각 큰 State 안에서는 세분화된 작은 State들로 나뉘어 있어 보다 상세한 행동을 기술할 수 있습니다. 이렇게 하면 전체적으로 간단하고 명확한 구조를 유지할 수 있을 것이라 판단했습니다.

    이렇게 하여 플레이어의 StateMachine 이 완성되었습니다.

    전부 제가 작성한 부분입니다.
*/

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

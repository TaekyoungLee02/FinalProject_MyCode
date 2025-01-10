/*
    각 버튼마다 이벤트를 네 개씩 넣어 두어서 각 이벤트가 발생했을 때 실행할 행위들을 정의하게 했습니다.

    이 부분이 IUserInterfaceEventHandler 인터페이스입니다.

    전부 제가 작성했습니다.
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUserInterfaceEventHandler<InputAction.CallbackContext>
{
    private event Action<InputAction.CallbackContext> OnTouchscreenAttackStarted;
    private event Action<InputAction.CallbackContext> OnTouchscreenAttackCanceled;

    private Button attackButton;

    private const int ATTACK_COOLTIME_UI_CHILDNUM = 2;
    private Image attackCooltimeUI;

    public Image AttackCooltimeUI { get { return attackCooltimeUI; } }

    Action<InputAction.CallbackContext> IUserInterfaceEventHandler<InputAction.CallbackContext>.Started { get => OnTouchscreenAttackStarted; set => OnTouchscreenAttackStarted = value; }
    Action<InputAction.CallbackContext> IUserInterfaceEventHandler<InputAction.CallbackContext>.Performed { get => null; set => throw new NotImplementedException(); }
    Action<InputAction.CallbackContext> IUserInterfaceEventHandler<InputAction.CallbackContext>.Hold { get => null; set => throw new NotImplementedException(); }
    Action<InputAction.CallbackContext> IUserInterfaceEventHandler<InputAction.CallbackContext>.Canceled { get => OnTouchscreenAttackCanceled; set => OnTouchscreenAttackCanceled = value; }

    private void Awake()
    {
        attackCooltimeUI = transform.GetChild(ATTACK_COOLTIME_UI_CHILDNUM).GetComponent<Image>();

        attackButton = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouchscreenAttackStarted?.Invoke(default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnTouchscreenAttackCanceled?.Invoke(default);
    }
}

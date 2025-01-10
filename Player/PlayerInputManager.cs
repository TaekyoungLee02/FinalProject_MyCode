using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInteract interact;

    private IUserInterfaceEventHandler<Vector2> joystick;
    private IUserInterfaceEventHandler<InputAction.CallbackContext> attackButton;
    private IUserInterfaceEventHandler interactionButton;

    private Vector2 playerMove;

    private UIManager uiManager;
    private UserInterface mobileController;
    private UserInterface actionUI;

    public event Action<Vector2> OnPlayerMoveStart;
    public event Action<Vector2> OnPlayerMovePerformed;
    public event Action<Vector2> OnPlayerMoveCanceled;

    public event Action OnPlayerNormalAttack;
    public event Action OnPlayerChargeAttack;

    private AttackChargeBar attackChargeBar;
    private bool isPointerOverUI;

    private PlayerStateMachine playerStateMachine;
    private PlayerEquipment playerEquipment;

    private EstusSystem estusSystem;
    private PauseMenu pauseMenu;

    private DialogManager dialogManager;

    public PlayerInput PlayerInput { get { return playerInput; } }
    private UIManager UIManager { get { return uiManager = uiManager != null ? uiManager : GameManager.Instance.GetManager<UIManager>(); } }
    private UserInterface MobileController { get { return mobileController ??= UIManager.UIDictionary[UITYPE.MOBILECONTROLLERUI]; } }
    private UserInterface ActionUI { get { return actionUI ??= UIManager.UIDictionary[UITYPE.ACTIONUI]; } }

    public IUserInterfaceEventHandler<Vector2> Joystick {  get { return joystick ??= MobileController.GetUI<VirtualJoystickUI>(); } set { if (joystick != null) joystick = value; else return; } }
    public IUserInterfaceEventHandler<InputAction.CallbackContext> AttackButton {  get { return attackButton ??= ActionUI.GetUI<AttackButtonUI>(); ; } set { if (attackButton != null) attackButton = value; else return; } }
    public IUserInterfaceEventHandler InteractionButton {  get { return interactionButton ??= ActionUI.GetUI<PlayerInteractionUI>(); ; } set { if (interactionButton != null) interactionButton = value; else return; } }

    public Vector2 PlayerMove { get { return playerMove; } }

    private void Awake()
    {
        playerInput = new();

        playerStateMachine = GetComponent<PlayerController>().StateMachine;
        playerEquipment = GetComponent<PlayerEquipment>();
        dialogManager = GameManager.Instance.GetManager<DialogManager>();

        estusSystem = GameManager.Instance.EstusSystem;

        interact = GetComponentInChildren<PlayerInteract>();
        attackChargeBar = UIManager.UIDictionary[UITYPE.ACTIONUI].GetUI<AttackChargeBar>();
        pauseMenu = UIManager.UIDictionary[UITYPE.PAUSEUI].GetUI<PauseMenu>();

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            UIManager.UIDictionary[UITYPE.MOBILECONTROLLERUI].EnableUI();
    }

    private void Update()
    {
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    private void OnEnable()
    {
        InitPlayerInputEvent();
    }

    private void OnDisable()
    {
        DeletePlayerInputEvent();
    }

    private void InitPlayerInputEvent()
    {
        Joystick.Performed += TouchScreenMoveStarted;
        Joystick.Canceled += TouchScreenMoveCanceled;
        AttackButton.Started += AttackStarted;
        AttackButton.Canceled += AttackCanceled;
        InteractionButton.Started += TouchScreenInterAction;

        playerInput.Enable();
        playerInput.Player.Move.performed += KeyboardMovePerformed;
        playerInput.Player.Move.canceled += KeyboardMoveCanceled;
        playerInput.Player.Attack.started += AttackStarted;
        playerInput.Player.Attack.canceled += AttackCanceled;
        playerInput.Player.Interaction.started += InterAction;
        playerInput.Player.ToNextDialog.started += ToNextDialog;

        playerInput.Player.Heal.started += (InputAction.CallbackContext context) => estusSystem.HealCharacter();
        playerInput.Player.DeckHeal.started += (InputAction.CallbackContext context) => estusSystem.RebuildDeck();
        playerInput.Player.Menu.started += (InputAction.CallbackContext context) =>
        {
            if (pauseMenu.gameObject.activeSelf) pauseMenu.DisableUI();
            else pauseMenu.EnableUI();
        };
    }

    private void DeletePlayerInputEvent()
    {
        Joystick.Performed -= TouchScreenMoveStarted;
        Joystick.Canceled -= TouchScreenMoveCanceled;
        AttackButton.Started -= AttackStarted;
        AttackButton.Canceled -= AttackCanceled;
        InteractionButton.Started -= TouchScreenInterAction;

        playerInput.Disable();
        playerInput.Player.Move.performed -= KeyboardMovePerformed;
        playerInput.Player.Move.canceled -= KeyboardMoveCanceled;
        playerInput.Player.Attack.started -= AttackStarted;
        playerInput.Player.Attack.canceled -= AttackCanceled;
        playerInput.Player.Interaction.started -= InterAction;
        playerInput.Player.ToNextDialog.started -= ToNextDialog;
    }

    private void KeyboardMovePerformed(InputAction.CallbackContext context)
    {
        playerMove = context.ReadValue<Vector2>();
        OnPlayerMovePerformed?.Invoke(playerMove);
    }
    private void KeyboardMoveCanceled(InputAction.CallbackContext context)
    {
        playerMove = context.ReadValue<Vector2>();
        OnPlayerMoveCanceled?.Invoke(playerMove);
    }

    private void AttackStarted(InputAction.CallbackContext context)
    {
        bool isInputSystem = context.control != null;
        bool isAttacking = playerStateMachine.currentState.GetType() == typeof(PlayerAttackStateParent);

        if (isPointerOverUI && isInputSystem) return;
        if (isAttacking) return;


        float chargeTime = playerEquipment.Weapon.WeaponChargeTime;
        attackChargeBar.ChargeStart(chargeTime);
    }

    private void AttackCanceled(InputAction.CallbackContext context)
    {
        int result = attackChargeBar.ChargeEnd();

        if (result == 1) OnPlayerChargeAttack?.Invoke();
        else if (result == 0) OnPlayerNormalAttack?.Invoke();
        else return;
    }

    private void TouchScreenMoveStarted(Vector2 movePosition)
    {
        playerMove = movePosition;
    }

    private void TouchScreenMoveCanceled(Vector2 movePosition)
    {
        playerMove = Vector2.zero;
    }

    private void TouchScreenInterAction()
    {
        if (interact != null)
            interact.OnInteractBtnClick();
    }

    private void InterAction(InputAction.CallbackContext context)
    {
        if (interact != null)
            interact.OnInteractBtnClick();
    }

    private void ToNextDialog(InputAction.CallbackContext context)
    {
        if (!dialogManager.IsActive) return;

        dialogManager.SetNextDialog();
    }
}

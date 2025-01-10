using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Player player;
    public PlayerStatus status;
    public ActionActor actor;
    private PlayerInputManager inputManager;
    private EquipSystem equipSystem;

    // 공격 상태 관련 변수
    private bool _isAttacking;
    private PlayerAttackStateParent playerAttackState;

    // 현재 장비 중인 무기
    private Weapon weapon;
    //public ActionData normalAttackData;
    //public ActionData chargeAttackData;
    public ActionData normalAttackData;
    public ActionData chargeAttackData;

    //현재 장비하고 있는 보패 리스트
    private List<Amulet> amuletSlots = new List<Amulet>(3);
    private int maxSlot;

    // 장비 UI 갱신용 이벤트
    public event Action OnEquipmentUpdateEvent;

    public Weapon Weapon { get { return weapon; } }

    public List<Amulet> AmuletSlots { get { return amuletSlots; } }
    public int MaxSlot { get { return maxSlot; } }

    private void Awake()
    {
        player = GameManager.Instance.Player;
        status = player.PlayerStatus;
        actor = player.ActionActor;

        equipSystem = GameManager.Instance.EquipSystem;

        inputManager = player.InputManager;

        playerAttackState = player.PlayerController.StateMachine.GetState<PlayerAttackStateParent>();
        playerAttackState.GetState<PlayerAttackState>().OnAttackStart += OnAttackStart;
        playerAttackState.GetState<PlayerAttackState>().OnAttackEnd += OnAttackEnd;
        _isAttacking = false;
        maxSlot = 3;
    }

    public void Initialize()
    {
        weapon = equipSystem.weapon;
        weapon.SetEquipSystem(this);
        normalAttackData = weapon.normalActionData;
        chargeAttackData = weapon.chargeActionData;

        for (int i = 0; i < 3; i++)
        {
            if (i < equipSystem.amuletList.Count)
            {
                amuletSlots.Add(equipSystem.amuletList[i]);
                amuletSlots[i].SetEquipSystem(this);
            }
        }

        status.UpdateStatus();
    }

    public void EquipWeapon(Weapon weapon)
    {
        if(this.weapon == weapon) return;

        if (this.weapon != null)
            this.weapon.Unequip();
        
        this.weapon = weapon;
        normalAttackData = weapon.normalActionData;
        chargeAttackData = weapon.chargeActionData;

        weapon.SetEquipSystem(this);

        status.UpdateStatus();
        OnEquipmentUpdateEvent?.Invoke();
        equipSystem.UpdateEquipment(this);
    }

    public void UnequipWeapon(Weapon weapon)
    {
        if (this.weapon != weapon) return;

        weapon = null;

        normalAttackData = null;
        chargeAttackData = null;

        status.UpdateStatus();
        OnEquipmentUpdateEvent?.Invoke();
        equipSystem.UpdateEquipment(this);
    }

    public bool EquipAmulet(Amulet amulet)
    {
        if (amuletSlots.Contains(amulet)) return false;

        if (amuletSlots.Count >= maxSlot)
            return false;

        amuletSlots.Add(amulet);

        OnEquipmentUpdateEvent?.Invoke();
        equipSystem.UpdateEquipment(this);
        status.UpdateStatus();
        return true;
    }

    public bool UnequipAmulet(Amulet amulet)
    {
        if (!amuletSlots.Contains(amulet)) return false;

        amuletSlots.Remove(amulet);

        OnEquipmentUpdateEvent?.Invoke();
        equipSystem.UpdateEquipment(this);
        status.UpdateStatus();
        return true;
    }

    public void UpdateEquipmentStatus()
    {
        ApplyEquipmentStatus();
        ApplyEquipmentTrait();
    }

    public void ApplyEquipmentStatus()
    {
        if (weapon != null)
            weapon.ApplyEquipment();

        for (int i = 0; i < amuletSlots.Count; i++)
        {
            if (amuletSlots[i] != null)
                amuletSlots[i].ApplyEquipment();
        }
    }

    public void ApplyEquipmentTrait()
    {
        if (weapon != null)
            weapon.ApplyTrait(status);

        for (int i = 0; i < amuletSlots.Count; i++)
        {
            if (amuletSlots[i] != null)
                amuletSlots[i].ApplyTrait(status);
        }
    }

    private void TryNormalAttack()
    {
        if (_isAttacking) return;

        actor.UseAction(normalAttackData);
    }

    private void TryChargeAttack()
    {
        if (_isAttacking) return;

        actor.UseAction(chargeAttackData);
    }

    private void OnAttackStart(float notUsed)
    {
        _isAttacking = true;
    }

    private void OnAttackEnd()
    {
        _isAttacking = false;
    }
}
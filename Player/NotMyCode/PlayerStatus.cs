using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerStatus : CharacterStatus//, IDataInitializer<DataTypeBase> // 나중에 Player 데이터 타입 생기면 변경 예정
{
    private UserInterface playerUI;
    private GaugeBar gaugeBar;
    private StateUI playerStateUI;

    public int attackWeaponMin;
    public int attackWeaponMax;
    public int attackMagicMin;
    public int attackMagicMax;

    private Player player;
    private PlayerEquipment equipment;

    public event Action OnStatusChangeEvent;

    public float playerAttackCooltime;

    public StateUI PlayerStateUI { get { return playerStateUI; } }

    private void Awake()
    {
        InitializeCharacter();

        playerUI = GameManager.Instance.GetManager<UIManager>().UIDictionary[UITYPE.PLAYERUI];

        gaugeBar = playerUI.GetUI<GaugeBar>();
        playerStateUI = playerUI.GetUI<StateUI>();

        gaugeBar.SetGauge(Health);

        player = GameManager.Instance.Player;
        equipment = player.Equipment;

        UpdateStatus();


        playerAttackCooltime = 0.3f;
    }

    private void Start()
    {
        playerUI.EnableUI();
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        for (int i = 0; i < stats.Length; i++)
            stats[i] = 100;

        for (int i = 0; i < elementAtk.Length; i++)
            elementAtk[i] = 0;

        for (int i = 0; i < elementDef.Length; i++)
            elementDef[i] = 0;

        for(int i = 0; i < extraStatus.Length; i++)
            extraStatus[i] = 100;

        equipment.UpdateEquipmentStatus();
        UpdateGaugeParameter();
        OnStatusChangeEvent?.Invoke();
    }

    public override float CalculateAttackDamage(AttackInfo info)
    {
        float damage = base.CalculateAttackDamage(info);

        switch (info.attackType)
        {
            case ATTACKTYPE.Weapon:
                damage *= UnityEngine.Random.Range(attackWeaponMin, attackWeaponMax) / 100.0f;
                break;

            case ATTACKTYPE.Spell:
                damage *= UnityEngine.Random.Range(attackMagicMin, attackMagicMax) / 100.0f;
                break;
        }

        return damage;
    }

    public override float CalculateKnockbackPower(AttackInfo info)
    {
        float power = base.CalculateKnockbackPower(info);

        switch (info.attackType)
        {
            case ATTACKTYPE.Weapon:
                power *= UnityEngine.Random.Range(attackWeaponMin, attackWeaponMax) / 100.0f;
                break;

            case ATTACKTYPE.Spell:
                power *= UnityEngine.Random.Range(attackMagicMin, attackMagicMax) / 100.0f;
                break;
        }

        return power;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        GameManager.Instance.GameOver(false);
    }

    public override void GetBuff(IBuffApplier buff)
    {
        base.GetBuff(buff);
        playerStateUI.GenerateStateIcon((BuffBase)buff);
    }
}

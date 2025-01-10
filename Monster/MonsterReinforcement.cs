using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReinforcement
{
    public enum ReinforceableStats
    {
        MaxHealth,
        Speed,
        WeaponAttack,
        MagicAttack,
        AttackFrequency,
        AllElementAttack,
        WeaponDefense,
        MagicDefense,
        AllElementDefense
    }

    public enum ReinforcementMethod
    {
        Sum,
        Product
    }

    private const int GOLD_DROP_REINFORCE_MULTIPIER = 10;

    private DataContainer<MonsterReinforcementElementData> _dataContainer;
    private int _reinforcementLevelSum;
    private List<MonsterReinforcementElementData> _reinforcementElements;

    public Dictionary<int, MonsterReinforcementElementData> MonsterReinforcementData { get { return _dataContainer.Data; } }
    public int ReinforcementLevelSum { get { return _reinforcementLevelSum; } }

    public MonsterReinforcement()
    {
        _reinforcementElements = new();
        _dataContainer = new();
    }

    private void UpdateReinforcement(MonsterStatus monsterStatus, ReinforcementMethod method)
    {
        for (int i = 0; i < _reinforcementElements.Count; i ++)
        {
            if (_reinforcementElements[i].method != method) continue;

            CalculateStatus(monsterStatus, i);
        }
    }

    private void CalculateStatus(MonsterStatus monsterStatus, int elementIndex)
    {
        var reinforcementElement = _reinforcementElements[elementIndex];
        var method = reinforcementElement.method;
        var number = reinforcementElement.number;

        switch (reinforcementElement.stat)
        {
            case ReinforceableStats.MaxHealth:
                if (method == ReinforcementMethod.Sum) monsterStatus.stats[(int)STATUSTYPE.HEALTH_MAX] += number;
                else monsterStatus.stats[(int)STATUSTYPE.HEALTH_MAX] += (int)(monsterStatus.stats[(int)STATUSTYPE.HEALTH_MAX] * number * 0.01f);
                break;

            case ReinforceableStats.Speed:
                if (method == ReinforcementMethod.Sum) monsterStatus.extraStatus[(int)EXTRA_STATUS.SPEED] += number;
                else monsterStatus.extraStatus[(int)EXTRA_STATUS.SPEED] += (int)(monsterStatus.extraStatus[(int)EXTRA_STATUS.SPEED] * number * 0.01f);
                break;

            case ReinforceableStats.WeaponAttack:
                if (method == ReinforcementMethod.Sum) monsterStatus.stats[(int)STATUSTYPE.POWER_WEAPON] += number;
                else monsterStatus.stats[(int)STATUSTYPE.POWER_WEAPON] += (int)(monsterStatus.stats[(int)STATUSTYPE.POWER_WEAPON] * number * 0.01f);
                break;

            case ReinforceableStats.MagicAttack:
                if (method == ReinforcementMethod.Sum) monsterStatus.stats[(int)STATUSTYPE.POWER_MAGIC] += number;
                else monsterStatus.stats[(int)STATUSTYPE.POWER_MAGIC] += (int)(monsterStatus.stats[(int)STATUSTYPE.POWER_MAGIC] * number * 0.01f);
                break;

            case ReinforceableStats.AttackFrequency:
                if (method == ReinforcementMethod.Sum) monsterStatus.extraStatus[(int)EXTRA_STATUS.ATTACK_FREQUENCY] += number;
                else monsterStatus.extraStatus[(int)EXTRA_STATUS.ATTACK_FREQUENCY] += (int)(monsterStatus.extraStatus[(int)EXTRA_STATUS.ATTACK_FREQUENCY] * number * 0.01f);
                break;

            case ReinforceableStats.AllElementAttack:
                if (method == ReinforcementMethod.Sum)
                {
                    for(int i = 0; i < monsterStatus.elementAtk.Length; i ++)
                    {
                        monsterStatus.elementAtk[i] += number;
                    }
                }
                else
                {
                    for (int i = 0; i < monsterStatus.elementAtk.Length; i++)
                    {
                        monsterStatus.elementAtk[i] += (int)(monsterStatus.elementAtk[i] * number * 0.01f);
                    }
                }
                break;

            case ReinforceableStats.WeaponDefense:
                if (method == ReinforcementMethod.Sum) monsterStatus.stats[(int)STATUSTYPE.DEFENCE_WEAPON] += number;
                else monsterStatus.stats[(int)STATUSTYPE.DEFENCE_WEAPON] += (int)(monsterStatus.stats[(int)STATUSTYPE.DEFENCE_WEAPON] * number * 0.01f);
                break;

            case ReinforceableStats.MagicDefense:
                if (method == ReinforcementMethod.Sum) monsterStatus.stats[(int)STATUSTYPE.DEFENCE_MAGIC] += number;
                else monsterStatus.stats[(int)STATUSTYPE.DEFENCE_MAGIC] += (int)(monsterStatus.stats[(int)STATUSTYPE.DEFENCE_MAGIC] * number * 0.01f);
                break;

            case ReinforceableStats.AllElementDefense:
                if (method == ReinforcementMethod.Sum)
                {
                    for (int i = 0; i < monsterStatus.elementAtk.Length; i++)
                    {
                        monsterStatus.elementDef[i] += number;
                    }
                }
                else
                {
                    for (int i = 0; i < monsterStatus.elementAtk.Length; i++)
                    {
                        monsterStatus.elementDef[i] += (int)(monsterStatus.elementDef[i] * number * 0.01f);
                    }
                }
                break;
        }
    }


    public bool HasReinforcement(MonsterReinforcementElementData reinforcementElement)
    {
        return _reinforcementElements.Contains(reinforcementElement);
    }

    /// <summary>
    /// Adds reinforcementElement to list
    /// </summary>
    /// <param name="reinforcementElement"></param>
    public void AddReinforcement(MonsterReinforcementElementData reinforcementElement)
    {
        if (_reinforcementElements.Contains(reinforcementElement)) return;

        _reinforcementElements.Add(reinforcementElement);
        _reinforcementLevelSum += reinforcementElement.reinforcementLevel;
    }

    /// <summary>
    /// Removes reinforcementElement to list
    /// </summary>
    /// <param name="reinforcementElement"></param>
    public void RemoveReinforcement(MonsterReinforcementElementData reinforcementElement)
    {
        if (!_reinforcementElements.Contains(reinforcementElement)) return;

        _reinforcementElements.Remove(reinforcementElement);
        _reinforcementLevelSum -= reinforcementElement.reinforcementLevel;
    }

    /// <summary>
    /// Apply Monster Reinforcement
    /// </summary>
    /// <param name="monsterStatus"></param>
    public void Reinforce(Monster monster)
    {
        UpdateReinforcement(monster.status, ReinforcementMethod.Sum);
        UpdateReinforcement(monster.status, ReinforcementMethod.Product);
        monster.itemGenerator.ReinforceDropGold(_reinforcementLevelSum * GOLD_DROP_REINFORCE_MULTIPIER);
    }
}
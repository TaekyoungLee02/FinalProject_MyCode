/*
        몬스터를 강화하는 시스템입니다.

        예시 플레이 동영상에서 이 강화를 위한 UI가 나옵니다.

        이 스탯들을 한번에 어떻게 관리할 방법이 없을까 하다가 결국 그냥 노가다를 하게 되었습니다.

        코딩에서는 이렇게 노가다가 필요한 부분이 가끔 가다 꼭 생기는 것 같네요..
        아니면 제가 아직 생각해내지 못한 좋은 구조가 있을 수도 있습니다.

        다음엔 좀 더 여유롭게 생각해보고 작성해야겠습니다.

        전부 제가 작성했습니다.
*/

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

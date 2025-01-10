using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EstusSystem
{
    public int maxCount = 5;
    public int curCount;
    public float healAmount = 0.4f;
    public CharacterStatus character;
    public DeckSystem gameDeck;

    public event Action<int> OnEstusAmountChangedEvent;

    public EstusSystem()
    {
        ResetEstus();
    }

    public void ResetEstus()//���� ����� �ִ� ������ �ʱ�ȭ
    {
        curCount = maxCount;
        OnEstusAmountChangedEvent?.Invoke(curCount);
    }

    public void HealCharacter()//ȸ�� ����
    {
        if(!ConsumeEstus(1))
        {
            Debug.Log("�̹� ����Ʈ�� �� ����߽��ϴ�.");
            return;
        }

        if (character == null)
            character = GameManager.Instance.Player.PlayerStatus;

        character.GetHeal(character.Health.max * healAmount);
    }

    public void RebuildDeck()//�� �籸�� ����
    {
        if (!ConsumeEstus(1))
        {
            Debug.Log("�̹� ����Ʈ�� �� ����߽��ϴ�.");
            return;
        }

        if (gameDeck == null) gameDeck = GameManager.Instance.GameDeck;
        gameDeck.RefillRemainingCardFromUsedCard();
    }

    private bool ConsumeEstus(int amount)
    {
        if(curCount < amount) return false;

        curCount -= amount;
        OnEstusAmountChangedEvent?.Invoke(curCount);
        return true;
    }

    public void AddEstus(int amount)
    {
        curCount += amount;

        if (curCount > maxCount)
            curCount = maxCount;

        OnEstusAmountChangedEvent?.Invoke(curCount);
    }
}

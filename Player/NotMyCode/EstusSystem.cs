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

    public void ResetEstus()//던전 입장시 최대 갯수로 초기화
    {
        curCount = maxCount;
        OnEstusAmountChangedEvent?.Invoke(curCount);
    }

    public void HealCharacter()//회복 사용시
    {
        if(!ConsumeEstus(1))
        {
            Debug.Log("이미 에스트를 다 사용했습니다.");
            return;
        }

        if (character == null)
            character = GameManager.Instance.Player.PlayerStatus;

        character.GetHeal(character.Health.max * healAmount);
    }

    public void RebuildDeck()//덱 재구성 사용시
    {
        if (!ConsumeEstus(1))
        {
            Debug.Log("이미 에스트를 다 사용했습니다.");
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

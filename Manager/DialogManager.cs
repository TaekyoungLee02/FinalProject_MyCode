/*
        플레이어 대화를 불러오는 시스템입니다.

        여기서부터는 시간에 다소 쫓겨서 만들어서 그런지 스스로도 완성도가 떨어진다고 느낍니다..(변명)

        조금 더 구조를 단단하게 해서 객체 지향적으로 작성할 수 있었을 텐데 아쉽게 생각합니다.

        이 부분은 전부 제가 작성했습니다.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogManager : ManagerBase
{
    private DataContainer<DialogData> dataContainer;
    private Dictionary<int, DialogData> dialogData;

    private EventManager eventManager;

    private DialogUI dialogUI;
    private int curDialogKey;

    private List<int> currentEventKeys;

    private int currentDialogIndex;
    private string currentNPCName;
    private Sprite currentNPCSprite;

    public event Action OnDialogStart;
    public event Action OnDialogEnd;

    private bool isActive;

    private DialogUI DialogUI { get { if (dialogUI == null) { dialogUI = GameManager.Instance.GetManager<UIManager>().UIDictionary[UITYPE.DIALOGUI] as DialogUI; } return dialogUI; } }

    public bool IsActive { get { return isActive; } }

    public override void Initialize()
    {
        dataContainer = new();
        dialogData = dataContainer.Data;

        eventManager = GameManager.Instance.GetManager<EventManager>();
    }

    public void StartDialog(Npc npc, int eventKey)
    {
        InitDialog(eventManager.GetEvent(eventKey), npc.NpcName, npc.NpcSprite);

        curDialogKey = currentEventKeys[currentDialogIndex++];
        DialogUI.SetDialog(dialogData[curDialogKey], currentNPCName, currentNPCSprite);
        DialogUI.EnableUI();

        isActive = true;
        OnDialogStart?.Invoke();
    }

    public bool SetNextDialog()
    {
        if (GetNextDialogKey(out curDialogKey))
        {
            DialogUI.SetDialog(dialogData[curDialogKey], currentNPCName, currentNPCSprite);
            return true;
        }
        else
        {
            EndDialog();
            return false;
        }
    }

    private void InitDialog(EventData eventData, string npcName, Sprite npcSprite)
    {
        currentEventKeys = eventData.dialogKeys;
        currentNPCName = npcName;
        currentNPCSprite = npcSprite;
        currentDialogIndex = 0;
    }

    private bool GetNextDialogKey(out int dialogKey)
    {
        if (currentEventKeys.Count > currentDialogIndex)
        {
            dialogKey = currentEventKeys[currentDialogIndex++];
            return true;
        }
        else
        {
            dialogKey = -1;
            return false;
        }
    }

    private void EndDialog()
    {
        curDialogKey = -1;
        DialogUI.DisableUI();

        OnDialogEnd?.Invoke();
        isActive = false;
    }

    public DialogData GetDialogData(int key)
    {
        return dialogData.TryGetValue(key, out var dialog) ? dialog : null;
    }
}

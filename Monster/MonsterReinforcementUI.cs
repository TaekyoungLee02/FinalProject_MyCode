/*
        몬스터 강화 UI 입니다.

        이쯤 되니 너무 급하게 작성해서 주석조차 없는 코드가 많네요..

        주석 작성을 생활화하자고 마음먹었는데도 쉽지 않은 것 같습니다.

        전부 제가 작성한 부분입니다.
*/


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonsterReinforcementUI : UserInterface
{
    private class ReinforcementElementUI
    {
        public static string LEVEL_STRING = "Level : ";
        public Button Button {  get; private set; }
        public GameObject Check { get; private set; }
        public TextMeshProUGUI Name { get; private set; }
        public TextMeshProUGUI Level { get; private set; }

        public ReinforcementElementUI(Transform elementUI)
        {
            Button = elementUI.GetComponent<Button>();
            Check = elementUI.GetChild(0).GetComponent<Image>().gameObject;
            Name = elementUI.GetChild(1).GetComponent<TextMeshProUGUI>();
            Level = elementUI.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
    }

    private class DragEventSender : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ScrollRect scrollRect;
        public void Init(ScrollRect scrollRect) { this.scrollRect = scrollRect; }

        public void OnBeginDrag(PointerEventData e)
        {
            scrollRect.OnBeginDrag(e);
        }
        public void OnDrag(PointerEventData e)
        {
            scrollRect.OnDrag(e);
        }
        public void OnEndDrag(PointerEventData e)
        {
            scrollRect.OnEndDrag(e);
        }
    }

    [SerializeField] private GameObject reinforcementElementUIPrefab;
    [SerializeField] private TextMeshProUGUI reinforcementLevel;
    [SerializeField] private Button closeUI;

    private ScrollRect scrollRect;
    private Transform scrollViewContent;
    private Dictionary<int, MonsterReinforcementElementData> reinforcementData;

    private MonsterReinforcement monsterReinforcement;

    public override void Initialize()
    {
        uiType = UITYPE.MONSTERREINFORCEMENTUI;
        canvasType = CANVASTYPE.UICanvas;

        monsterReinforcement = GameManager.Instance.MonsterReinforcement;
        reinforcementData = monsterReinforcement.MonsterReinforcementData;
        scrollViewContent = GetComponentInChildren<VerticalLayoutGroup>().transform;
        scrollRect = GetComponentInChildren<ScrollRect>();

        closeUI.onClick.AddListener(() => DisableUI());

        InitScrollView();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Initiates Reinforcement Scroll View
    /// </summary>
    private void InitScrollView()
    {
        foreach (var element in reinforcementData.Values)
        {
            var uiGameobject = Instantiate(reinforcementElementUIPrefab, scrollViewContent);

            var sender = uiGameobject.AddComponent<DragEventSender>();
            sender.Init(scrollRect);

            ReinforcementElementUI ui = new(uiGameobject.transform);

            ui.Name.text = element.name;
            ui.Level.text = ReinforcementElementUI.LEVEL_STRING + element.reinforcementLevel.ToString();
            ui.Check.SetActive(monsterReinforcement.HasReinforcement(element));
            ui.Button.onClick.AddListener(() => OnElementButtonClicked(ui, element));
        }

        reinforcementLevel.text = monsterReinforcement.ReinforcementLevelSum.ToString();
    }

    private void OnElementButtonClicked(ReinforcementElementUI reinforcementElementUI, MonsterReinforcementElementData monsterReinforcementElementData)
    {
        reinforcementElementUI.Check.SetActive(!reinforcementElementUI.Check.activeSelf);

        // 이미 적용된 강화효과라면 삭제, 아니면 추가
        if (monsterReinforcement.HasReinforcement(monsterReinforcementElementData)) monsterReinforcement.RemoveReinforcement(monsterReinforcementElementData);
        else monsterReinforcement.AddReinforcement(monsterReinforcementElementData);

        reinforcementLevel.text = monsterReinforcement.ReinforcementLevelSum.ToString();
    }
}

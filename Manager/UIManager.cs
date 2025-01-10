/*
        게임 안의 UI를 관리하는 UI Manager 입니다.

        수많은 UI들을 한번에 관리하기 위해 Dictionary 와 Enum 으로 작성했습니다.

        그런데 주석 부분이 전부 깨져 있네요..

        전부 제가 작성한 부분입니다.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CANVASTYPE
{
    WorldSpaceUICanvas,
    UICanvas,
    PlayerInfoCanvas,
    MonsterInfoCanvas,
    NPCInfoCanvas,
    DialogCanvas,
    InventoryCanvas,
    ResultCanvas,
    MenuCanvas
}
public enum UITYPE
{
    MOBILECONTROLLERUI,
    SHOPUI,
    PLAYERUI,
    DIALOGUI,
    ACTIONUI,
    INVENTORYUI_CONTAINER,
    RESULTUI,
    ITEMLOGCONTAINER,
    TITLESCENEUI,
    DECKBUILDUI,
    GOLD_UI,
    DECKLISTUI,
    CARDUPGRADEUI,
    ESTUSUI,
    MONSTERREINFORCEMENTUI,
    PAUSEUI
}

public class UIManager : ManagerBase
{
    private readonly int SCREEN_RESOLUTION_X = 1920;
    private readonly int SCREEN_RESOLUTION_Y = 1080;

    private DataContainer<UIData> dataContainer;

    private Transform canvasParent;
    private EventSystem eventSystem;
    private Dictionary<int, GameObject> uiPrefabs;
    private Dictionary<UITYPE, UserInterface> uiDictionary;
    private Dictionary<CANVASTYPE, Canvas> canvasDictionary;

    public Dictionary<UITYPE, UserInterface> UIDictionary
    {
        get
        {
            if (uiDictionary == null) LoadUI();
            return uiDictionary;
        }
    }
    public EventSystem EventSystem
    {
        get
        {
            if (eventSystem == null) return CreateEventSystem();
            return eventSystem;
        }
    }
    private Transform CanvasParent
    {
        get
        {
            if (canvasParent == null) canvasParent = new GameObject("CanvasParent").transform;
            return canvasParent;
        }
    }

    public Canvas GetCanvas(CANVASTYPE canvasName = CANVASTYPE.UICanvas)
    {
        if (canvasDictionary == null) CreateCanvas();

        if (canvasDictionary.TryGetValue(canvasName, out var canvas)) return canvas;
        else return CreateCanvas(canvasName);
    }

    public override void Initialize()
    {
        dataContainer = new();

        CreateCanvas();

        SceneManager.sceneUnloaded += DeleteUIOnSceneUnloaded;
    }

    /// <summary>
    /// Initializes Dictionaries and Fields When Scene Unload
    /// </summary>
    /// <param name="scene"></param>
    private void DeleteUIOnSceneUnloaded(Scene scene)
    {
        uiDictionary = null;
        canvasDictionary = null;
        canvasParent = null;
        eventSystem = null;
    }

    /// <summary>
    /// Dynamically Create Canvas
    /// </summary>
    /// <param name="canvasName"></param>
    /// <returns></returns>
    private Canvas CreateCanvas(CANVASTYPE canvasName = CANVASTYPE.UICanvas)
    {
        if (eventSystem == null) eventSystem = CreateEventSystem();
        canvasDictionary ??= new();

        var hasKey = canvasDictionary.ContainsKey(canvasName);

        if (hasKey)
        {
            if (canvasDictionary[canvasName] != null) return canvasDictionary[canvasName];
        }

        var gameObject = new GameObject(canvasName.ToString());
        gameObject.transform.SetParent(CanvasParent);

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(SCREEN_RESOLUTION_X, SCREEN_RESOLUTION_Y);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        gameObject.AddComponent<GraphicRaycaster>();

        if (hasKey) canvasDictionary[canvasName] = canvas;
        else canvasDictionary.Add(canvasName, canvas);

        canvas.sortingOrder = (int)canvasName;
        return canvas;
    }

    /// <summary>
    /// Dynamically Create Event System
    /// </summary>
    /// <returns></returns>
    private EventSystem CreateEventSystem()
    {
        var gameObject = new GameObject("EventSystem");

        var eventSystem = gameObject.AddComponent<EventSystem>();
        gameObject.AddComponent<InputSystemUIInputModule>();

        return eventSystem;
    }

    /// <summary>
    /// Dynamically Create UI
    /// </summary>
    /// <exception cref="MissingComponentException"></exception>
    private void LoadUI()
    {
        uiDictionary ??= new();

        if (uiPrefabs == null)
        {
            // ���ҽ� �ε�. �ε� �� ������ �߻��Ѵٸ� �Լ��� ����
            if (!dataContainer.TryLoadResource(out uiPrefabs)) return;
        }

        foreach (var ui in uiPrefabs)
        {
            // UI�� �̹� �ε�Ǿ����� üũ
            if (!ui.Value.TryGetComponent<UserInterface>(out var uiComponent)) throw new MissingComponentException($"{ui.Key} : Not Contains IUserInterface. Please Check Prefab or Script.");
            var key = uiComponent.GetUIType();
            var hasKey = uiDictionary.ContainsKey(key);

            if (hasKey)
            {
                if ((uiDictionary[key] as MonoBehaviour) != null) continue;
            }

            // UI ��Ҹ� ���� ����
            var uiRuntimeObject = Object.Instantiate(ui.Value, GetCanvas(uiComponent.GetCanvasType()).transform);
            var uiRuntimeComponent = uiRuntimeObject.GetComponent<UserInterface>();
            uiRuntimeComponent.Initialize();

            // �ҷ��� UI ��Ҹ� ��ųʸ��� �־� ��.
            if (hasKey) uiDictionary[key] = uiRuntimeComponent;
            else uiDictionary.Add(key, uiRuntimeComponent);
        }
    }
}

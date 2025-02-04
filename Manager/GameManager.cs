/*
        보통 제일 중요한 게임매니저 부분입니다.
        이 부분은 물론 저 혼자 전부 작성한 건 아니지만
        핵심 부분을 제가 작성해서 여기에 올립니다.

        이전에 프로젝트들을 하면서 다른 사람들이 하는 걸 봤을 때
        모든 매니저를 싱글톤으로 만들고 관리하는 모습을 보곤 했습니다.

        그런데 싱글톤이라는 이 패턴이, 남용 금물! 인 패턴이라서
        막 쓰게 되면 프로젝트가 난잡 난잡 난잡하게 되어버린다고 생각합니다.

        그래서 다른 매니저들은 MonoBehaviour 조차 빼버리고 그냥 이 GameManager 안의 변수로서만 존재하게 했습니다.
        이런 매니저들을 한번에 관리하기 위해 Dictionary 를 만들고 제너릭 함수로 매니저들을 가져오게 했습니다.

        이 부분을 작성했습니다.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public enum Manager
    {
        DialogManager,
        NPCManager,
        EventManager,
        SoundManager,
        UIManager,
        ActionDataManager,
        ItemDataManager,
        CardDataManager,
        CostEffectDataManager
    }

    private ObjectPoolList poolList;

    private GameObject playerPrefab;
    private Player player;
    private PlayerRuntimeData runtimePlayerData;
    private Camera mainCamera;
    private Dictionary<string, ManagerBase> managers;

    private DungeonInventory dungeonInventory;
    private VillageInventory villageInventory;
    private ResultInventory resultInventory;
    private DeckSystem gameDeck;
    private EquipSystem equipSystem;
    private Deck deck;
    private DeckInventory deckInventory;
    private EstusSystem estusSystem;
    private CardUpgradeSystem cardUpgradeSystem;

    private ActionEnumToTypeConverter actionEnumToTypeConverter;

    private int lastPlayedDungeonLevel;
    private int saveOrder;
    private float playTime;

    public event Action OnGameOverEvent;
    public event Action<SCENE> OnSceneChangedEvent;

    private MonsterReinforcement monsterReinforcement;

    private new void Awake()
    {
        base.Awake();

        if (mainCamera == null) CreateCamera();
        SceneManager.sceneLoaded += OnSceneLoaded;

        playerPrefab = Resources.Load<GameObject>(DataPath.PlayerPath);
    }

    private void FixedUpdate()
    {
        playTime += Time.deltaTime;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        CreateCamera();
    }

    public ObjectPoolList PoolList
    {
        get
        {
            if (poolList == null)
                poolList = new ObjectPoolList();

            return poolList;
        }
    }

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();

                if (runtimePlayerData != null)
                    player.PlayerRuntimeData = runtimePlayerData;
            }
            return player;
        }
    }

    public PlayerRuntimeData RuntimePlayerData
    {
        get { return runtimePlayerData; }
        set { runtimePlayerData = value; }
    }

    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                CreateCamera();
            }
            return mainCamera;
        }
    }

    public EstusSystem EstusSystem
    {
        get
        {
            if (estusSystem == null)
                estusSystem = new();

            return estusSystem;
        }
    }

    public CardUpgradeSystem CardUpgradeSystem
    {
        get
        {
            if (cardUpgradeSystem == null)
                cardUpgradeSystem=new();
            return cardUpgradeSystem;
        }
    }

    public int LastPlayedDungeonLevel { get { return lastPlayedDungeonLevel; } set {lastPlayedDungeonLevel = value; } }
    public int SaveOrder { get { return saveOrder; } set {saveOrder = value;} }
    public float PlayTime { get { return playTime; } set { playTime = value; } }

    public T GetManager<T>() where T : ManagerBase, new()
    {
        managers ??= new();

        var managerName = typeof(T).Name;

        if (!managers.ContainsKey(managerName))
        {
            managers.Add(managerName, CreateManager<T>());
        }

        return (T)managers[managerName];
    }
    private ManagerBase CreateManager<T>() where T : ManagerBase, new()
    {
        T manager = new();
        manager.Initialize();
        return manager;
    }

    private void CreateCamera()
    {
        if (mainCamera != null) return;

        var cameraObject = new GameObject("Main Camera");

        mainCamera = cameraObject.AddComponent<Camera>();

        cameraObject.AddComponent<AudioListener>();

        cameraObject.AddComponent<UniversalAdditionalCameraData>();

        cameraObject.AddComponent<CameraScaler>();

        Color tempColor = new Color();
        ColorUtility.TryParseHtmlString("#262626", out tempColor);
        mainCamera.backgroundColor = tempColor;

        Player player = Player;
        if (player != null)
        {
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0, 0, -10);
        }
    }

    public DungeonInventory DungeonInventory
    {
        get
        {
            if (dungeonInventory == null)
                dungeonInventory = new DungeonInventory();

            return dungeonInventory;
        }
    }

    public VillageInventory VillageInventory
    {
        get
        {
            if (villageInventory == null)
                villageInventory = new VillageInventory();

            return villageInventory;
        }
    }

    public ResultInventory ResultInventory
    {
        get
        {
            if (resultInventory == null)
                resultInventory = new ResultInventory();

            return resultInventory;
        }
    }

    public Deck Deck
    {
        get
        {
            if (deck == null)
            {
                deck = DeckInventory.GetDeck(0);
            }

            return deck;
        }
        set
        {
            deck = value;
        }
    }
    public DeckSystem GameDeck
    {
        get
        {
            if (gameDeck == null)
            {
                gameDeck = new DeckSystem();
                gameDeck.SetStartDeck(Deck);
            }
            return gameDeck;
        }
        set
        {
            gameDeck = value;
        }
    }

    public EquipSystem EquipSystem
    {
        get
        {
            if (equipSystem == null)
                equipSystem = new EquipSystem();

            return equipSystem;

        }
    }
    public DeckInventory DeckInventory
    {
        get
        {
            if (deckInventory == null)
            {
                deckInventory = new DeckInventory();
            }
            return deckInventory;
        }
        set
        {
            deckInventory = value;
        }
    }

    public ActionEnumToTypeConverter ActionEnumToTypeConverter
    {
        get
        {
            if (actionEnumToTypeConverter == null)
                actionEnumToTypeConverter = new ActionEnumToTypeConverter();

            return actionEnumToTypeConverter;
        }
    }

    public MonsterReinforcement MonsterReinforcement { get { monsterReinforcement ??= new(); return monsterReinforcement; } }

    public void GameOver(bool isEscaped)
    {
        ItemDeliverOnGameOver(isEscaped);
        Time.timeScale = 0.0f;
        GetManager<UIManager>().UIDictionary[UITYPE.RESULTUI].EnableUI();
        OnGameOverEvent?.Invoke();
    }

    private void ItemDeliverOnGameOver(bool isEscaped)
    {
        ResultInventory.CheckItemRemain(isEscaped);
        ResultInventory.DeliverToVillageInventory();
        DungeonInventory.ClearInventory();
    }

    public void LoadScene(SCENE scene)
    {
        switch (scene)
        {
            default:
                break;

            case SCENE.DungeonScene:
                GetManager<SoundManager>().PlayBgm(90000200);
                break;

            case SCENE.VillageScene:
                LastPlayedDungeonLevel = MonsterReinforcement.ReinforcementLevelSum;
                GetManager<SaveLoadManager>().SaveSaveData(saveOrder);
                GetManager<SoundManager>().PlayBgm(90000100);
                break;

            case SCENE.TitleScene:
                DestroyThis();
                break;
        }

        SceneManager.LoadScene((int)scene);

        Time.timeScale = 1.0f;
        poolList = null;
        EstusSystem.ResetEstus();
        OnSceneChangedEvent?.Invoke(scene);
    }

    public void LoadPlayer()
    {

    }
}

public enum SCENE
{
    TitleScene,
    VillageScene,
    DungeonScene
}

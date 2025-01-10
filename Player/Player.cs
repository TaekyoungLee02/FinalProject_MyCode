using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDataInitializer<PlayerRuntimeData>
{
    private PlayerInputManager inputManager;
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    private PlayerStatus playerStatus;
    private PlayerEquipment equipment;

    private ActionActor actionActor;

    private PlayerRuntimeData runtimeData;

    private void Start()
    {
        PlayerStatus.UpdateBuffedStats();
        Equipment.Initialize();

        if (runtimeData != null)
            Initialize(runtimeData);

        SceneManager.sceneUnloaded += OnSceneDestroyed;
    }

    public void Initialize(PlayerRuntimeData data)
    {
        playerStatus.stats = data.stats;
        playerStatus.elementAtk = data.elementAtk;
        playerStatus.elementDef = data.elementDef;
        playerStatus.extraStatus = data.extraStatus;

        if (data.weapon != null)
        {
            data.weapon.Equip(equipment);
            equipment.EquipWeapon(data.weapon);
        }

        if (data.amulets != null)
        {
            for (int i = 0; i < data.amulets.Count; i++)
            {
                equipment.EquipAmulet(data.amulets[i]);
            }
        }
    }

    public void OnSceneDestroyed(Scene scene)
    {
        PlayerRuntimeData playerData = new(playerStatus.stats, playerStatus.elementAtk, playerStatus.elementDef, playerStatus.extraStatus, equipment.Weapon, equipment.AmuletSlots);

        GameManager.Instance.RuntimePlayerData = playerData;
    }

    public PlayerInputManager InputManager { get { return inputManager = inputManager != null ? inputManager : GetComponent<PlayerInputManager>(); } }
    public PlayerMovement PlayerMovement { get { return playerMovement = playerMovement != null ? playerMovement : GetComponent<PlayerMovement>(); } }
    public PlayerController PlayerController { get { return playerController = playerController != null ? playerController : GetComponent<PlayerController>(); } }
    public PlayerStatus PlayerStatus { get { return playerStatus = playerStatus != null ? playerStatus : GetComponent<PlayerStatus>(); } }
    public PlayerEquipment Equipment { get { return equipment = equipment != null ? equipment : GetComponent<PlayerEquipment>(); } }
    public ActionActor ActionActor
    {
        get
        {
            if (actionActor == null)
            {
                actionActor = GetComponentInChildren<ActionActor>();
                actionActor.Initialize(true, PlayerStatus);

                InputManager.OnPlayerMoveStart += actionActor.SetDirection;
                InputManager.OnPlayerMovePerformed += actionActor.SetDirection;
                InputManager.OnPlayerMoveCanceled += actionActor.SetDirection;
            }

            return actionActor;
        }
        set { actionActor = value; }
    }

    public PlayerRuntimeData PlayerRuntimeData { get { return runtimeData; } set { runtimeData = value; } }
}

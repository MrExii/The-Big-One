using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchReferences : MonoBehaviour
{
    [SerializeField] DisplayDialogue playerDisplayDialogue;
    [SerializeField] GameManager gameManager;

    PlayerController playerController;
    PlayerHealth playerHealth;
    RoomPool roomPool;
    RoomPoolTest roomPoolTest;
    CameraZoom cameraZoom;
    PauseMenu pauseMenu;
    SimulationsPlaceHolder simulationsPlaceHolder;
    RoomCount roomCount;
    EnemyPool enemyPool;
    PlayerStatistics playerStatistics;
    PassiveObject passiveObject;
    Sector sector;
    Curse curse;
    Fighter fighter;
    Achievements achievements;
    Trinket trinket;
    MainMenu mainMenu;
    SectorTimer sectorTimer;
    Experience experience;
    GlitchDisplay glitchDisplay;
    PassiveDisplay passiveDisplay;
    TrinketDisplay trinketDisplay;
    Keyboard playerKeyboard;
    PotionsDisplay potionsDisplay;
    WeaponDisplay weaponDisplay;
    HealthDisplay healthDisplay;
    CurseDisplay curseDisplay;
    EndGame endGame;
    StatsUpBoard statsUpBoard;
    CameraShake cameraShake;
    SavingWrapper savingWrapper;
    ControllsMenu controllsMenu;
    Potion potion;
    HeartStalker heartStalker;
    Translations translations;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController)
        {
            playerHealth = playerController.GetComponent<PlayerHealth>();
            playerStatistics = playerController.GetComponent<PlayerStatistics>();
            passiveObject = playerController.GetComponent<PassiveObject>();
            passiveDisplay = playerController.GetComponentInChildren<PassiveDisplay>();
            trinketDisplay = playerController.GetComponentInChildren<TrinketDisplay>();
            playerKeyboard = playerController.GetComponentInChildren<Keyboard>();
            potionsDisplay = playerController.GetComponentInChildren<PotionsDisplay>();
            weaponDisplay = playerController.GetComponentInChildren<WeaponDisplay>();
            experience = playerController.GetComponent<Experience>();
            trinket = playerController.GetComponent<Trinket>();
            curse = playerController.GetComponent<Curse>();
            fighter = playerController.GetComponent<Fighter>();
            simulationsPlaceHolder = playerController.GetComponent<SimulationsPlaceHolder>();
            potion = playerController.GetComponent<Potion>();
            heartStalker = playerController.GetComponent<HeartStalker>();
        }
        
        roomPool = FindObjectOfType<RoomPool>();
        roomPoolTest = FindObjectOfType<RoomPoolTest>();
        cameraZoom = FindObjectOfType<CameraZoom>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        roomCount = FindObjectOfType<RoomCount>();
        enemyPool = FindObjectOfType<EnemyPool>();
        sector = FindObjectOfType<Sector>();
        achievements = GetComponent<Achievements>();
        mainMenu = FindObjectOfType<MainMenu>();
        sectorTimer = FindObjectOfType<SectorTimer>();
        glitchDisplay = FindObjectOfType<GlitchDisplay>();
        healthDisplay = FindObjectOfType<HealthDisplay>();
        curseDisplay = FindObjectOfType<CurseDisplay>();
        endGame = FindObjectOfType<EndGame>();
        statsUpBoard = FindObjectOfType<StatsUpBoard>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        savingWrapper = FindObjectOfType<SavingWrapper>();
        controllsMenu = FindObjectOfType<ControllsMenu>();
        translations = FindObjectOfType<Translations>();
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public PlayerHealth GetPlayerHealth()
    {
        return playerHealth;
    }

    public RoomPool GetRoomPool()
    {
        return roomPool;
    }

    public RoomPoolTest GetRoomPoolTest()
    {
        return roomPoolTest;
    }

    public CameraZoom GetCameraZoom()
    {
        return cameraZoom;
    }

    public PauseMenu GetPauseMenu()
    {
        return pauseMenu;
    }

    public SimulationsPlaceHolder GetSimulationsPlaceHolder()
    {
        return simulationsPlaceHolder;
    }

    public RoomCount GetRoomCount()
    {
        return roomCount;
    }

    public EnemyPool GetEnemyPool()
    {
        return enemyPool;
    }

    public PlayerStatistics GetPlayerStatistics()
    {
        return playerStatistics;
    }

    public PassiveObject GetPassiveObject()
    {
        return passiveObject;
    }

    public Sector GetSector()
    {
        return sector;
    }

    public Curse GetCurse()
    {
        return curse;
    }

    public Fighter GetFighter()
    {
        return fighter;
    }

    public Achievements GetAchievements()
    {
        return achievements;
    }

    public DisplayDialogue GetPlayerDisplayDialogue()
    {
        return playerDisplayDialogue;
    }

    public Trinket GetTrinket()
    {
        return trinket;
    }

    public MainMenu GetMainMenu()
    {
        return mainMenu;
    }

    public SectorTimer GetSectorTimer()
    {
        return sectorTimer;
    }

    public Experience GetExperience()
    {
        return experience;
    }

    public GlitchDisplay GetGlitchDisplay()
    {
        return glitchDisplay;
    }

    public PassiveDisplay GetPassiveDisplay()
    {
        return passiveDisplay;
    }

    public TrinketDisplay GetTrinketDisplay()
    {
        return trinketDisplay;
    }

    public Keyboard GetPlayerKeyboard()
    {
        return playerKeyboard;
    }

    public PotionsDisplay GetPotionsDisplay()
    {
        return potionsDisplay;
    }

    public WeaponDisplay GetWeaponDisplay()
    {
        return weaponDisplay;
    }

    public HealthDisplay GetHealthDisplay()
    {
        return healthDisplay;
    }

    public CurseDisplay GetCurseDisplay()
    {
        return curseDisplay;
    }

    public EndGame GetEndGame()
    {
        return endGame;
    }

    public StatsUpBoard GetStatsUpBoard()
    {
        return statsUpBoard;
    }

    public CameraShake GetCameraShake()
    {
        return cameraShake;
    }

    public SavingWrapper GetSavingWrapper()
    {
        return savingWrapper;
    }

    public ControllsMenu GetControllsMenu()
    {
        return controllsMenu;
    }

    public GameManager GetGameManager()
    {
        return gameManager;
    }

    public Potion GetPotion()
    {
        return potion;
    }

    public HeartStalker GetHeartStalker()
    {
        return heartStalker;
    }

    public Translations GetTranslations()
    {
        return translations;
    }
}

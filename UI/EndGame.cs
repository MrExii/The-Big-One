using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Steamworks;
using System;

public class EndGame : MonoBehaviour
{
    [SerializeField] GameObject endGamePanel;
    [SerializeField] TextMeshProUGUI gameTimerTxt;
    [SerializeField] TextMeshProUGUI numberOfRoomsClearedTxt;
    [SerializeField] TextMeshProUGUI numberOfEnemiesKilledTxt;
    [SerializeField] TextMeshProUGUI numberOfGlitchGainedTxt;
    [SerializeField] TextMeshProUGUI numberOfExperienceGainedTxt;
    [SerializeField] TextMeshProUGUI simulationLevelTxt;
    [SerializeField] Image[] keyToPressImg;
    [SerializeField] Sprite[] keyToPressSprites;
    [SerializeField] GameObject keyboardEscapeImg;
    [SerializeField] GameObject controllerEscapeImg;

    CatchReferences references;
    PlayerInputSystem inputActions;

    bool isKeySwap;

    const float timeBetweenKeySwap = 0.07f;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIEndGame.Confirm.performed += numberOfRoomsClearedTxt => OnConfirm();

        inputActions.UIEndGame.Enable();
    }

    private void OnDisable()
    {
        inputActions.UIEndGame.Disable();
    }

    private void Start()
    {
        endGamePanel.SetActive(false);

        keyboardEscapeImg.SetActive(false);
        controllerEscapeImg.SetActive(false);
    }

    private void Update()
    {
        RefreshUI();
    }

    private void OnConfirm()
    {
        if (!endGamePanel.activeInHierarchy) return;

        Time.timeScale = 1;

        references.GetGameManager().SetSimulationReccord();

        StartCoroutine(SwitchKeySprite());

        StartCoroutine(references.GetGameManager().LoadEndScene());
    }

    private void RefreshUI()
    {
        if (!keyboardEscapeImg.activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(true);
            controllerEscapeImg.SetActive(false);
        }
        else if (!controllerEscapeImg.activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(false);
            controllerEscapeImg.SetActive(true);
        }

        if (references.GetGameManager().GetChangeUI() && !isKeySwap)
        {
            keyToPressImg[1].sprite = keyToPressSprites[4];
        }
        else if (!isKeySwap)
        {
            keyToPressImg[1].sprite = keyToPressSprites[2];
        }
    }

    public void DisplayPanel()
    {
        Time.timeScale = 0;

        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().StopPlayer(true);

        endGamePanel.SetActive(true);

        UpdateGameTimer();

        numberOfRoomsClearedTxt.text = "Rooms cleared : <color=#414456>" + (references.GetSector().GetRoomsCleared() + 1).ToString() + "</color>";
        numberOfEnemiesKilledTxt.text = "Enemies killed : <color=#BC132C>" + references.GetSector().GetNumberOfKills().ToString() + "</color>";
        numberOfGlitchGainedTxt.text = "Glitch gained : " + references.GetGlitchDisplay().GetGlitchGained().ToString();
        numberOfExperienceGainedTxt.text = "Experience gained : <color=#00cdb9>" + references.GetExperience().GetExperienceGained().ToString() + "</color>";
        simulationLevelTxt.text = "Simulation level : <color=#F79824>" + references.GetGameManager().GetCurrentSimulationDifficulty().ToString() + "</color>";

        references.GetGameManager().IncreaseNumberOfWins();

        CheckAchievements();
    }

    private void CheckAchievements()
    {
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            if (references.GetGameManager().GetNoobMode())
            {
                SteamUserStats.GetAchievement("ACH_NOOB_MODE", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    SteamUserStats.SetAchievement("ACH_NOOB_MODE");

                    references.GetAchievements().SetAchievement("ACH_NOOB_MODE");
                }

                return;
            }

            NumberOfWinsAchievements();
            SimulationsAchievements();
            ObjectsHoldAchievements();
            TrinketHoldAchievements();
            WeaponHoldAchievements();

            if (!references.GetPlayerHealth().GetDamageTaken())
            {
                SteamUserStats.GetAchievement("ACH_NO_HIT", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    SteamUserStats.SetAchievement("ACH_NO_HIT");

                    references.GetAchievements().SetAchievement("ACH_NO_HIT");
                }
            }

            if (references.GetRoomPool().GetCurrentRoom().Boss1HP())
            {
                SteamUserStats.GetAchievement("ACH_BOSS_1HP", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_BOSS_1HP");

                    SteamUserStats.SetAchievement("ACH_BOSS_1HP");
                }
                
            }

            SteamUserStats.StoreStats();
        }
    }

    private void NumberOfWinsAchievements()
    {
        int numberOfWins = references.GetGameManager().GetNumberOfWins();

        SteamUserStats.GetAchievement("ACH_WIN_1_GAME", out bool achievementUnlock);
        SteamUserStats.GetAchievement("ACH_WIN_25_GAMES", out bool achievement2Unlock);
        SteamUserStats.GetAchievement("ACH_WIN_50_GAMES", out bool achievement3Unlock);
        SteamUserStats.GetAchievement("ACH_WIN_75_GAMES", out bool achievement4Unlock);
        SteamUserStats.GetAchievement("ACH_WIN_100_GAMES", out bool achievement5Unlock);
        
        if (!achievementUnlock && numberOfWins == 1)
        {
            references.GetAchievements().SetAchievement("ACH_WIN_1_GAME");
            SteamUserStats.SetAchievement("ACH_WIN_1_GAME");
        }

        if (!achievement2Unlock && numberOfWins == 25)
        {
            references.GetAchievements().SetAchievement("ACH_WIN_25_GAMES");
            SteamUserStats.SetAchievement("ACH_WIN_25_GAMES");
        }

        if (!achievement3Unlock && numberOfWins == 50)
        {
            references.GetAchievements().SetAchievement("ACH_WIN_50_GAMES");
            SteamUserStats.SetAchievement("ACH_WIN_50_GAMES");
        }

        if (!achievement4Unlock && numberOfWins == 75)
        {
            references.GetAchievements().SetAchievement("ACH_WIN_75_GAMES");
            SteamUserStats.SetAchievement("ACH_WIN_75_GAMES");
        }

        if (!achievement5Unlock && numberOfWins == 100)
        {
            references.GetAchievements().SetAchievement("ACH_WIN_100_GAMES");
            SteamUserStats.SetAchievement("ACH_WIN_100_GAMES");
        }
    }

    private void SimulationsAchievements()
    {
        if (references.GetGameManager().GetCurrentSimulationDifficulty() == 1 && !references.GetGameManager().GetNoobMode())
        {
            SteamUserStats.GetAchievement("ACH_WIN_GAME_1_DIFFICULTY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_WIN_GAME_1_DIFFICULTY");

                references.GetAchievements().SetAchievement("ACH_WIN_GAME_1_DIFFICULTY");
            }
        }

        if (references.GetGameManager().GetCurrentSimulationDifficulty() == 10 && !references.GetGameManager().GetNoobMode())
        {
            SteamUserStats.GetAchievement("ACH_WIN_GAME_10_DIFFICULTY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_WIN_GAME_10_DIFFICULTY");

                references.GetAchievements().SetAchievement("ACH_WIN_GAME_10_DIFFICULTY");
            }
        }

        if (references.GetGameManager().GetCurrentSimulationDifficulty() == 20 && !references.GetGameManager().GetNoobMode())
        {
            SteamUserStats.GetAchievement("ACH_WIN_GAME_20_DIFFICULTY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_WIN_GAME_20_DIFFICULTY");

                references.GetAchievements().SetAchievement("ACH_WIN_GAME_20_DIFFICULTY");
            }
        }

        if (references.GetGameManager().GetCurrentSimulationDifficulty() == 31 && !references.GetGameManager().GetNoobMode())
        {
            SteamUserStats.GetAchievement("ACH_WIN_GAME_31_DIFFICULTY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_WIN_GAME_31_DIFFICULTY");

                references.GetAchievements().SetAchievement("ACH_WIN_GAME_31_DIFFICULTY");
            }
        }
    }

    private void ObjectsHoldAchievements()
    {
        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.Chemotherapy))
        {
            SteamUserStats.GetAchievement("ACH_CHEMOTHERAPY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_CHEMOTHERAPY");

                references.GetAchievements().SetAchievement("ACH_CHEMOTHERAPY");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.BedrockArmor))
        {
            SteamUserStats.GetAchievement("ACH_BEDROCK_ARMOR", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_BEDROCK_ARMOR");

                references.GetAchievements().SetAchievement("ACH_BEDROCK_ARMOR");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.BloodForge))
        {
            SteamUserStats.GetAchievement("ACH_BLOOD_FORGE", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_BLOOD_FORGE");

                references.GetAchievements().SetAchievement("ACH_BLOOD_FORGE");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.BloodTransfusion))
        {
            SteamUserStats.GetAchievement("ACH_BLOOD_TRANSFUSION", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_BLOOD_TRANSFUSION");

                references.GetAchievements().SetAchievement("ACH_BLOOD_TRANSFUSION");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.CactusSeed))
        {
            SteamUserStats.GetAchievement("ACH_CACTUS_SEED", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_CACTUS_SEED");

                references.GetAchievements().SetAchievement("ACH_CACTUS_SEED");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.GlitchedKey))
        {
            SteamUserStats.GetAchievement("ACH_GLITCHED_KEY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_GLITCHED_KEY");

                references.GetAchievements().SetAchievement("ACH_GLITCHED_KEY");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.HeartSeed))
        {
            SteamUserStats.GetAchievement("ACH_HEART_SEED", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_HEART_SEED");

                references.GetAchievements().SetAchievement("ACH_HEART_SEED");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.HotFatty))
        {
            SteamUserStats.GetAchievement("ACH_HOT_FATTY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_HOT_FATTY");

                references.GetAchievements().SetAchievement("ACH_HOT_FATTY");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.HotSpicy))
        {
            SteamUserStats.GetAchievement("ACH_HOT_SPICY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_HOT_SPICY");

                references.GetAchievements().SetAchievement("ACH_HOT_SPICY");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.HylianShield))
        {
            SteamUserStats.GetAchievement("ACH_HYLIAN_SHIELD", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_HYLIAN_SHIELD");

                references.GetAchievements().SetAchievement("ACH_HYLIAN_SHIELD");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.LetTheMusicPlay))
        {
            SteamUserStats.GetAchievement("ACH_LET_THE_MUSIC_PLAY", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_LET_THE_MUSIC_PLAY");

                references.GetAchievements().SetAchievement("ACH_LET_THE_MUSIC_PLAY");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.PentatonicMinor))
        {
            SteamUserStats.GetAchievement("ACH_PENTATONIC_MINOR", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_PENTATONIC_MINOR");

                references.GetAchievements().SetAchievement("ACH_PENTATONIC_MINOR");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.Purification))
        {
            SteamUserStats.GetAchievement("ACH_PURIFICATION", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_PURIFICATION");

                references.GetAchievements().SetAchievement("ACH_PURIFICATION");
            }
        }

        if (references.GetPassiveObject().GetPassiveObject(PassiveIndex.SteelBandage))
        {
            SteamUserStats.GetAchievement("ACH_STEEL_BANDAGE", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_STEEL_BANDAGE");

                references.GetAchievements().SetAchievement("ACH_STEEL_BANDAGE");
            }
        }
    }

    private void TrinketHoldAchievements()
    {
        if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.Leukemia)
        {
            SteamUserStats.GetAchievement("ACH_LEUKEMIA", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_LEUKEMIA");

                references.GetAchievements().SetAchievement("ACH_LEUKEMIA");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.BoringSong)
        {
            SteamUserStats.GetAchievement("ACH_BORING_SONG", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_BORING_SONG");

                references.GetAchievements().SetAchievement("ACH_BORING_SONG");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.FavorOfGod)
        {
            SteamUserStats.GetAchievement("ACH_FAVOR_OF_GOD", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_FAVOR_OF_GOD");

                references.GetAchievements().SetAchievement("ACH_FAVOR_OF_GOD");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.MidnightSnack)
        {
            SteamUserStats.GetAchievement("ACH_MIDNIGHT_SNACK", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_MIDNIGHT_SNACK");

                references.GetAchievements().SetAchievement("ACH_MIDNIGHT_SNACK");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.NotchedPickaxe)
        {
            SteamUserStats.GetAchievement("ACH_NOTCHED_PICKAXE", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_NOTCHED_PICKAXE");

                references.GetAchievements().SetAchievement("ACH_NOTCHED_PICKAXE");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.ScrapsOfMetal)
        {
            SteamUserStats.GetAchievement("ACH_SCRAPS_OF_METAL", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_SCRAPS_OF_METAL");

                references.GetAchievements().SetAchievement("ACH_SCRAPS_OF_METAL");
            }
        }
        else if (references.GetTrinket().GetCurrentTrinket() == TrinketIndex.StrongRoots)
        {
            SteamUserStats.GetAchievement("ACH_STRONG_ROOTS", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                SteamUserStats.SetAchievement("ACH_STRONG_ROOTS");

                references.GetAchievements().SetAchievement("ACH_STRONG_ROOTS");
            }
        }
    }

    private void WeaponHoldAchievements()
    {
        if (references.GetFighter().GetWeaponType() == WeaponType.BrokenSword)
        {
            SteamUserStats.GetAchievement("ACH_BROKEN_SWORD", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_BROKEN_SWORD");

                SteamUserStats.SetAchievement("ACH_BROKEN_SWORD");
            }
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.AssassinDagger)
        {
            SteamUserStats.GetAchievement("ACH_ASSASSIN_DAGGER", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_ASSASSIN_DAGGER");

                SteamUserStats.SetAchievement("ACH_ASSASSIN_DAGGER");
            }
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.ChainWhip)
        {
            SteamUserStats.GetAchievement("ACH_CHAIN_WHIP", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_CHAIN_WHIP");

                SteamUserStats.SetAchievement("ACH_CHAIN_WHIP");
            }
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.DoubleAxe)
        {
            SteamUserStats.GetAchievement("ACH_DOUBLE_AXE", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_DOUBLE_AXE");

                SteamUserStats.SetAchievement("ACH_DOUBLE_AXE");
            }
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.GlitchedWeapon)
        {
            SteamUserStats.GetAchievement("ACH_GLITCHED_WEAPON", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_GLITCHED_WEAPON");

                SteamUserStats.SetAchievement("ACH_GLITCHED_WEAPON");
            }
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.StickOfTruth)
        {
            SteamUserStats.GetAchievement("ACH_STICK_OF_TRUTH", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_STICK_OF_TRUTH");

                SteamUserStats.SetAchievement("ACH_STICK_OF_TRUTH");
            }
        }
    }

    private void UpdateGameTimer()
    {
        float[] gameTimer = references.GetSectorTimer().GetGameTimer();

        float seconds = gameTimer[0];
        float minutes = gameTimer[1];
        float hours = gameTimer[2];

        string secondsTxt;
        string minutesTxt;
        string hoursTxt;

        if (seconds < 10)
        {
            secondsTxt = "0" + seconds;
        }
        else
        {
            secondsTxt = seconds.ToString();
        }

        if (minutes < 10)
        {
            minutesTxt = "0" + minutes;
        }
        else
        {
            minutesTxt = minutes.ToString();
        }

        if (hours < 10)
        {
            hoursTxt = "0" + hours;
        }
        else
        {
            hoursTxt = hours.ToString();
        }

        gameTimerTxt.text = "Game Timer : " + hoursTxt + ":" + minutesTxt + ":" + secondsTxt;
    }

    private IEnumerator SwitchKeySprite()
    {
        isKeySwap = true;

        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].sprite = keyToPressSprites[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            keyToPressImg[0].sprite = keyToPressSprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                keyToPressImg[1].sprite = keyToPressSprites[5];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToPressImg[1].sprite = keyToPressSprites[4];
            }
            else
            {
                keyToPressImg[1].sprite = keyToPressSprites[3];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToPressImg[1].sprite = keyToPressSprites[2];
            }
        }

        isKeySwap = false;
    }
}

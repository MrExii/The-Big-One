using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    [SerializeField] Intro intro;
    [SerializeField] Fader fader;
    [SerializeField] PlayerInput playerInput;

    CatchReferences references;

    int prisonnersNumber = -1;
    const string prisonnersNumberKey = "prisonnersNumber";

    int currentSimulationDifficulty;
    const string currentSimulationDifficultyKey = "currentSimulationDifficulty";

    int simulationRecord;
    const string simulationRecordKey = "simulationRecord";

    bool hasCurrentSave;
    const string hasCurrentSaveKey = "hasCurrentSave";

    float currentAudioVolume = 1f;
    const string currentAudioVolumeKey = "currentAudioVolume";

    bool keyboardControl = true;
    const string keyboardControlKey = "keyboardControl";

    bool noobMode;
    const string noobModeKey = "noobMode";

    bool changeUI;
    const string changeUIKey = "changeUI";

    bool azertyControl;
    const string azertyControlKey = "azertyControl";

    int winStreak;
    const string winStreakKey = "winStreak";

    string defaultControlScheme = "Keyboard";
    const string defaultControlSchemeKey = "defaultControlScheme";

    bool skipDialogue;
    const string skipDialogueKey = "skipDialogue";

    bool tutoDone;
    const string tutoDoneKey = "tutoDone";

    int numberOfKill;
    const string numberOfKillKey = "numberOfKill";

    int numberOfDeath;
    const string numberOfDeathKey = "numberOfDeath";

    int numberOfRoomClear;
    const string numberOfRoomClearKey = "numberOfRoomClear";

    int numberOfGlitch;
    const string numberOfGlitchKey = "numberOfGlitch";

    int numberOfWeaponsUpgraded;
    const string numberOfWeaponsUpgradedKey = "numberOfWeaponsUpgraded";

    int numberOfNPCSpeak;
    const string numberOfNPCSpeakKey = "numberOfNPCSpeak";

    int numberOfOfferAccepted;
    const string numberOfOfferAcceptedKey = "numberOfOfferAccepted";

    int numberOfGambling;
    const string numberOfGamblingKey = "numberOfGambling";

    int numberOfWins;
    const string numberOfWinsKey = "numberOfWins";

    int numberOfScrollRead;
    const string numberOfScrollReadKey = "numberOfScrollRead";

    readonly bool disableSteamworks;
    bool cantLoseNextKeySeries;
    bool cantLoseAllKeySeries;

    protected Callback m_GameOverlayActivated;

    private void Awake()
    {
        references = GetComponent<CatchReferences>();
    }

    private void Start()
    {
        Application.targetFrameRate = 240;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        references.GetSavingWrapper().Load();

        ChangeVolume(-1);

        if (SceneManager.GetActiveScene().name == "Tuto")
        {
            if (prisonnersNumber == -1)
            {
                prisonnersNumber = 1;

                SetHasCurrentSave(true);

                references.GetSavingWrapper().Save();
            }
        }
        
        if (SceneManager.GetActiveScene().name == "Home")
        {
            StartCoroutine(intro.IntroDialogue(prisonnersNumber));
        }

        playerInput.SwitchCurrentControlScheme(defaultControlScheme);

        if (SteamManager.Initialized && disableSteamworks)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            references.GetPauseMenu().EnablePauseMenu();
        }
    }

    public void IncreasePrisonnersNumber()
    {
        prisonnersNumber++;

        if (prisonnersNumber == 258 || prisonnersNumber == 37 || prisonnersNumber == 104 || prisonnersNumber == 81 || prisonnersNumber == 179
            || prisonnersNumber == 392 || prisonnersNumber == 125 || prisonnersNumber == 186)
        {
            prisonnersNumber++;
        }
    }

    public IEnumerator ReturnHome(bool isDead)
    {
        if (references.GetPlayerController())
        {
            references.GetPlayerController().SetIsInActivity(true);
            references.GetPlayerController().disableControl = true;
        }

        yield return fader.FadeOut(1.5f);

        if (isDead)
        {
            IncreasePrisonnersNumber();

            winStreak = 0;
        }

        references.GetSavingWrapper().Save();
        
        yield return SceneManager.LoadSceneAsync("Home");
    }

    public IEnumerator Quit()
    {
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);
        references.GetPlayerController().StopPlayer(true);

        yield return fader.FadeOut(1.5f);

        if (SceneManager.GetActiveScene().name != "Home")
        {
            IncreasePrisonnersNumber();
        }

        references.GetSavingWrapper().Save();

        yield return SceneManager.LoadSceneAsync("Main Menu");
    }

    public IEnumerator StartGame()
    {
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);
        references.GetPlayerController().StopPlayer(true);

        yield return fader.FadeOut(1f);

        references.GetSavingWrapper().Save();

        yield return SceneManager.LoadSceneAsync("Sector");
    }

    public IEnumerator LoadCredits()
    {
        references.GetSavingWrapper().Save();

        yield return fader.FadeOut(1.5f);

        yield return SceneManager.LoadSceneAsync("Credits");
    }

    public IEnumerator LoadEndScene()
    {
        references.GetSavingWrapper().Save();

        yield return fader.FadeOut(0.5f);

        yield return SceneManager.LoadSceneAsync("End Scene");
    }

    public void ChangeVolume(float volume)
    {
        if (volume == -1)
        {
            AudioListener.volume = currentAudioVolume;
        }
        else
        {
            AudioListener.volume = volume;
            currentAudioVolume = volume;
        }
    }

    public void ResetSaveData()
    {
        prisonnersNumber = -1;
        simulationRecord = 0;
        currentSimulationDifficulty = 0;
        hasCurrentSave = false;
    }

    public float GetCurrentAudioVolume()
    {
        return currentAudioVolume;
    }

    public bool GetHasCurrentSave()
    {
        return hasCurrentSave;
    }

    public int GetSimulationReccord()
    {
        return simulationRecord;
    }

    public int GetNumberOfWeaponsUpgraded()
    {
        return numberOfWeaponsUpgraded;
    }

    public int GetNumberOfOfferAccepted()
    {
        return numberOfOfferAccepted;
    }

    public float GetRoomClearMultiplier(bool health)
    {
        if (health)
        {
            return references.GetSector().GetRoomsCleared() / 300;
        }
        else
        {
            return references.GetSector().GetRoomsCleared() / 100;
        }
    }

    public int GetNumberOfKill()
    {
        return numberOfKill;
    }

    public int GetNumberOfDeath()
    {
        return numberOfDeath;
    }

    public int GetNumberOfRoomClear()
    {
        return numberOfRoomClear;
    }

    public int GetNumberOfGlitch()
    {
        return numberOfGlitch;
    }

    public int GetNumberOfNPCSpeak()
    {
        return numberOfNPCSpeak;
    }

    public int GetNumberOfGambling()
    {
        return numberOfGambling;
    }

    public int GetNumberOfWins()
    {
        return numberOfWins;
    }

    public int GetNumberOfScrollRead()
    {
        return numberOfScrollRead;
    }

    public bool GetKeyboardControl()
    {
        if (playerInput.currentControlScheme == "Keyboard")
        {
            if (!keyboardControl)
            {
                EndCurrentActivity();
            }

            defaultControlScheme = "Keyboard";

            keyboardControl = true;
        }
        else
        {
            if (keyboardControl)
            {
                EndCurrentActivity();
            }

            defaultControlScheme = "Gamepad";

            keyboardControl = false;
        }

        return keyboardControl;
    }

    public bool GetAzertyControl()
    {
        return azertyControl;
    }

    private void EndCurrentActivity()
    {
        if (references.GetPlayerController())
        {
            if (references.GetPlayerController().GetIsInActivity())
            {
                foreach (var keyboard in FindObjectsOfType<Keyboard>())
                {
                    keyboard.DisableKeyboard();
                }
            }
        }
    }

    public bool GetNoobMode()
    {
        return noobMode;
    }

    public int GetCurrentSimulationDifficulty()
    {
        return currentSimulationDifficulty;
    }

    public void ChangeKeyboardType()
    {
        azertyControl = !azertyControl;
    }

    public void SkipDialogue()
    {
        skipDialogue = !skipDialogue;
    }

    public void IncreaseNumberOfKill()
    {
        numberOfKill++;
    }

    public void IncreaseNumberOfDeath()
    {
        numberOfDeath++;
    }

    public void IncreaseNumberOfRoomClear()
    {
        numberOfRoomClear++;
    }

    public void IncreaseNumberOfWeaponsUpgraded()
    {
        numberOfWeaponsUpgraded++;
    }

    public void IncreaseNumberOfGlitch(int numberToAdd)
    {
        numberOfGlitch += numberToAdd;
    }

    public void IncreaseNumberOfNPCSpeak()
    {
        numberOfNPCSpeak++;
    }

    public void IncreaseNumberOfOfferAccepted()
    {
        numberOfOfferAccepted++;
    }

    public void IncreaseNumberOfGambling()
    {
        numberOfGambling++;
    }

    public void IncreaseNumberOfWins()
    {
        numberOfWins++;
    }

    public void IncreaseNumberOfScrollRead()
    {
        numberOfScrollRead++;
    }

    //Use in player input component
    public void CheckIfControllerDisconnected()
    {
        references.GetPauseMenu().EnablePauseMenu();
    }

    //Use in player input component
    public void ChangeControllerUI()
    {
        if (Gamepad.all.Count == 0) return;
        
        string gamepad = Gamepad.current.displayName;
        string value = "Xbox";

        if (!gamepad.Contains(value)) //If playstation controller
        {
            changeUI = true;
        }
        else //If Xbox controller
        {
            changeUI = false;
        }
    }

    public bool GetChangeUI()
    {
        return changeUI;
    }

    public int GetWinStreak()
    {
        return winStreak;
    }

    public bool GetSkipDialogue()
    {
        return skipDialogue;
    }

    public bool GetDisableSteamworks()
    {
        return disableSteamworks;
    }

    public bool GetTutoDone()
    {
        return tutoDone;
    }

    public bool GetCantLoseNextKeySeries()
    {
        return cantLoseNextKeySeries;
    }

    public bool GetCantLoseAllKeySeries()
    {
        return cantLoseAllKeySeries;
    }

    public void AddWinStreak()
    {
        if (noobMode) return;
        if (disableSteamworks) return;

        winStreak++;

        if (SteamManager.Initialized)
        {
            if (winStreak >= 10)
            {
                SteamUserStats.GetAchievement("ACH_WIN_STREAK", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    GetComponent<Achievements>().SetAchievement("ACH_WIN_STREAK");

                    SteamUserStats.SetAchievement("ACH_WIN_STREAK");
                    SteamUserStats.StoreStats();
                }
            }
        }
    }

    public void SetSimulationReccord()
    {
        if (currentSimulationDifficulty > simulationRecord)
        {
            simulationRecord = currentSimulationDifficulty;
        }
    }

    public void SetCantLoseNextKeySeries(bool state)
    {
        cantLoseNextKeySeries = state;
    }

    public void SetCantLoseAllKeySeries()
    {
        cantLoseAllKeySeries = true;
    }

    public void SetTutoDone(bool state)
    {
        tutoDone = state;
    }

    public void ChangeNoobMode()
    {
        noobMode = !noobMode;
    }

    public void SetHasCurrentSave(bool state)
    {
        hasCurrentSave = state;
    }
    
    public void SetCurrentSimulationDifficulty(int gameDifficulty)
    {
        currentSimulationDifficulty = gameDifficulty;
    }

    private void OnApplicationQuit()
    {
        references.GetSavingWrapper().Save();
    }

    public object CaptureState()
    {
        Dictionary<string, object> data = new()
        {
            { prisonnersNumberKey, prisonnersNumber },
            { simulationRecordKey, simulationRecord },
            { currentSimulationDifficultyKey, currentSimulationDifficulty },
            { hasCurrentSaveKey, hasCurrentSave },
            { currentAudioVolumeKey, currentAudioVolume },
            { keyboardControlKey, keyboardControl },
            { noobModeKey, noobMode },
            { changeUIKey, changeUI },
            { azertyControlKey, azertyControl },
            { winStreakKey, winStreak },
            { defaultControlSchemeKey, defaultControlScheme },
            { skipDialogueKey, skipDialogue },
            { tutoDoneKey, tutoDone },
            { numberOfKillKey, numberOfKill },
            { numberOfDeathKey, numberOfDeath },
            { numberOfRoomClearKey, numberOfRoomClear },
            { numberOfGlitchKey, numberOfGlitch },
            { numberOfWeaponsUpgradedKey, numberOfWeaponsUpgraded },
            { numberOfNPCSpeakKey, numberOfNPCSpeak },
            { numberOfOfferAcceptedKey, numberOfOfferAccepted },
            { numberOfGamblingKey, numberOfGambling },
            { numberOfWinsKey, numberOfWins },
            { numberOfScrollReadKey, numberOfScrollRead }
        };
        
        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> data = (Dictionary<string, object>)state;

        foreach (KeyValuePair<string, object> item in data)
        {
            if (item.Key == prisonnersNumberKey)
            {
                prisonnersNumber = (int)item.Value;
            }
            else if (item.Key == simulationRecordKey)
            {
                simulationRecord = (int)item.Value;
            }
            else if (item.Key == currentSimulationDifficultyKey)
            {
                currentSimulationDifficulty = (int)item.Value;
            }
            else if (item.Key == hasCurrentSaveKey)
            {
                hasCurrentSave = (bool)item.Value;
            }
            else if (item.Key == currentAudioVolumeKey)
            {
                currentAudioVolume = (float)item.Value;
            }
            else if (item.Key == keyboardControlKey)
            {
                keyboardControl = (bool)item.Value;
            }
            else if (item.Key == noobModeKey)
            {
                noobMode = (bool)item.Value;
            }
            else if (item.Key == changeUIKey)
            {
                changeUI = (bool)item.Value;
            }
            else if (item.Key == azertyControlKey)
            {
                azertyControl = (bool)item.Value;
            }
            else if (item.Key == winStreakKey)
            {
                winStreak = (int)item.Value;
            }
            else if (item.Key == defaultControlSchemeKey)
            {
                defaultControlScheme = (string)item.Value;
            }
            else if (item.Key == skipDialogueKey)
            {
                skipDialogue = (bool)item.Value;
            }
            else if (item.Key == tutoDoneKey)
            {
                tutoDone = (bool)item.Value;
            }
            else if (item.Key == numberOfKillKey)
            {
                numberOfKill = (int)item.Value;
            }
            else if (item.Key == numberOfDeathKey)
            {
                numberOfDeath = (int)item.Value;
            }
            else if (item.Key == numberOfRoomClearKey)
            {
                numberOfRoomClear = (int)item.Value;
            }
            else if (item.Key == numberOfGlitchKey)
            {
                numberOfGlitch = (int)item.Value;
            }
            else if (item.Key == numberOfWeaponsUpgradedKey)
            {
                numberOfWeaponsUpgraded = (int)item.Value;
            }
            else if (item.Key == numberOfNPCSpeakKey)
            {
                numberOfNPCSpeak = (int)item.Value;
            }
            else if (item.Key == numberOfOfferAcceptedKey)
            {
                numberOfOfferAccepted = (int)item.Value;
            }
            else if (item.Key == numberOfGamblingKey)
            {
                numberOfGambling = (int)item.Value;
            }
            else if (item.Key == numberOfWinsKey)
            {
                numberOfWins = (int)item.Value;
            }
            else if (item.Key == numberOfScrollReadKey)
            {
                numberOfScrollRead = (int)item.Value;
            }
        }
    }
}

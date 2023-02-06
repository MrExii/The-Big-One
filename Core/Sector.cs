using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sector : MonoBehaviour
{
    [SerializeField] AudioSource inGameAS;
    [SerializeField] Fader fader;

    CatchReferences references;
    PlayerInputSystem inputActions;

    int numberOfKills;
    int roomsRemaining;
    int roomsCleared = -1;

    float timeSinceRestartKeyPress;
    float inGameASVolume;

    bool reloadSectorkeyPressed;
    bool nextRoomIsClear;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inGameASVolume = inGameAS.volume;

        inputActions = new();
        inputActions.Player.ReloadSector.performed += ctx => OnReloadSector();
        inputActions.Player.ReloadSector.canceled += ctx => OnCancelReloadSector();

        inputActions.Player.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Tuto") return;
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (references.GetPlayerHealth().isDead) return;

        if (reloadSectorkeyPressed)
        {
            timeSinceRestartKeyPress += Time.deltaTime;

            inGameAS.volume -= Time.deltaTime;

            if (timeSinceRestartKeyPress > 0.5f)
            {
                timeSinceRestartKeyPress = 0.5f;
            }

            fader.SectorReload(timeSinceRestartKeyPress * 2);
        }
        else if (timeSinceRestartKeyPress > 0f)
        {
            timeSinceRestartKeyPress -= Time.deltaTime;

            inGameAS.volume += Time.deltaTime;

            if (inGameAS.volume > inGameASVolume)
            {
                inGameAS.volume = inGameASVolume;
            }

            if (timeSinceRestartKeyPress < 0)
            {
                timeSinceRestartKeyPress = 0;
            }

            fader.SectorReload(timeSinceRestartKeyPress * 2);
        }

        if (timeSinceRestartKeyPress > 0.49f)
        {
            if (references.GetRoomPool().GetCurrentRoom().GetRoomName() != "Miscs Room - Start")
            {
                references.GetGameManager().IncreasePrisonnersNumber();
            }

            references.GetSavingWrapper().Save();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnReloadSector()
    {
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (references.GetPlayerHealth().isDead) return;

        reloadSectorkeyPressed = true;
    }

    private void OnCancelReloadSector()
    {
        reloadSectorkeyPressed = false;
    }

    public void IncreaseNumberOfKills()
    {
        numberOfKills++;
    }

    public void DecreaseRoomsRemaining()
    {
        roomsRemaining--;

        IncreaseRoomsCleared();
    }

    public void IncreaseRoomsCleared()
    {
        roomsCleared++;
    }

    public int GetNumberOfKills()
    {
        return numberOfKills;
    }

    public int GetRoomsRemaining()
    {
        return roomsRemaining;
    }

    public int GetRoomsCleared()
    {
        return roomsCleared;
    }

    public bool GetNextRoomIsClear()
    {
        return nextRoomIsClear;
    }

    public int GetRoomsInTotal()
    {
        return roomsCleared + roomsRemaining;
    }

    public void SetRoomsRemaining(int numberOfRooms)
    {
        roomsRemaining = numberOfRooms;
    }

    public void SetNextRoomIsClear(bool state)
    {
        nextRoomIsClear = state;
    }
}

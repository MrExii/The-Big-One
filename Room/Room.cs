using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;

public class Room : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameTxt;
    [SerializeField] Transform spawnPosition;
    [SerializeField] Transform exitPosition;
    [SerializeField] GameObject smokeVFX;
    [SerializeField] Transform PNJSpawnPosition;
    [SerializeField] GameObject pressKey;
    [SerializeField] GameObject pickups;
    [SerializeField] KeyCode[] keyToExitRoom;
    [SerializeField] bool hostileRoom;
    [SerializeField] GameObject merchant;
    [SerializeField] Animator woodenDoorAnimator;
    [SerializeField] SpriteRenderer woodenDoorHighlight;
    [SerializeField] RoomZoom roomZoom;
    [SerializeField] float timeToCompleteRoom = -1;
    [SerializeField] int roomPnjIndex = -1;
    [SerializeField] GameObject villagerGhost;
    [SerializeField] Collider2D borderCollider;
    [SerializeField] Fader fader;
    [SerializeField] RoomTimer roomTimer;
    [SerializeField] bool bossRoom;
    [SerializeField] bool displayRoomName;
    [SerializeField] TextMeshProUGUI nextRoomTxt;
    [SerializeField] bool displayNextRoomName;
    [SerializeField] bool closedRoom;
    [SerializeField] GameObject scroll;

    Activity[] activitiesInCurrentRoom;
    PlayerInputSystem inputActions;
    CatchReferences references;

    bool canExitRoom;
    bool roomCleared;
    bool borderHit;
    bool asCopyName;
    bool haveOneHP;

    private void Awake()
    {
        activitiesInCurrentRoom = GetComponentsInChildren<Activity>(true);
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.Player.ExitRoom.performed += ctx => OnRoomExit();

        inputActions.Player.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        if (bossRoom || !references.GetPlayerController().GetCursed())
        {
            villagerGhost.SetActive(false);
        }

        ReverseRoom();
        DisplayRoomName();
        DisplayNextRoomName();

        woodenDoorHighlight.enabled = false;

        if (!hostileRoom && !closedRoom)
        {
            StartCoroutine(SpawnExitRoom());
        }

        Instantiate(smokeVFX, new Vector2(spawnPosition.position.x, spawnPosition.position.y + 1), Quaternion.identity);
        references.GetPlayerController().transform.position = spawnPosition.position;

        pressKey.SetActive(false);

        references.GetCameraZoom().Zoom(roomZoom);

        roomTimer.SetTimeToCompleteRoom(timeToCompleteRoom);

        StartCoroutine(fader.FadeIn(0.2f));

        if (references.GetPlayerHealth().GetCurrentHealth() <= 1)
        {
            haveOneHP = true;
        }

        if (references.GetSector() && references.GetSector().GetNextRoomIsClear())
        {
            StartCoroutine(ClearRoom());
        }
    }

    private void Update()
    {
        if (!asCopyName)
        {
            asCopyName = true;

            GUIUtility.systemCopyBuffer = roomNameTxt.text;
        }

        CheckBorderCollider();

        if (!roomCleared && !closedRoom)
        {
            ActivityCheck();
            return;
        }
        
        if (canExitRoom)
        {
            RoomExit();
        }
    }

    private IEnumerator ClearRoom()
    {
        references.GetSector().SetNextRoomIsClear(false);

        yield return new WaitForSeconds(0.6f);

        foreach (var enemy in FindObjectsOfType<EnemyHealth>(true))
        {
            enemy.TakeDamage(Mathf.Infinity, true, true);

            yield return new WaitForSeconds(0.2f);
        }

        foreach (var activity in activitiesInCurrentRoom)
        {
            if (!activity.GetIsActivityCompleted())
            {
                activity.FinishActivity();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DisplayRoomName()
    {
        if (!displayRoomName)
        {
            roomNameTxt.enabled = false;
            return;
        }

        char[] roomName = gameObject.name.ToCharArray();
        char[] newRoomName = new char[roomName.Length - 7];

        for (int i = 0; i < newRoomName.Length; i++)
        {
            newRoomName[i] = roomName[i];
        }

        roomNameTxt.text = newRoomName.ArrayToString();
    }

    private void DisplayNextRoomName()
    {
        if (!displayNextRoomName || !references.GetPlayerController().GetCursed())
        {
            nextRoomTxt.enabled = false;
            return;
        }

        nextRoomTxt.text = references.GetRoomPool().GetNextRoomName();
    }

    private void CheckBorderCollider()
    {
        if (borderCollider.IsTouching(references.GetPlayerController().GetBodyCollider()) && !borderHit)
        {
            borderHit = true;

            references.GetPlayerController().transform.position = spawnPosition.position;
            references.GetPlayerHealth().TakeDamage(5, false, true, false, true, false);
        }
        else if (references.GetPlayerController().GetBodyCollider().IsTouchingLayers(LayerMask.GetMask("Ground")) && borderHit)
        {
            borderHit = false;
        }
    }

    private void OnRoomExit()
    {
        if (!canExitRoom) return;
        if (!exitPosition.GetComponent<BoxCollider2D>().IsTouching(references.GetPlayerController().GetBodyCollider())) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        StartCoroutine(pressKey.GetComponent<PressKey>().SwitchKeySprites());
        StartCoroutine(FadeIn());
    }

    private void RoomExit()
    {
        if (exitPosition.GetComponent<BoxCollider2D>().IsTouching(references.GetPlayerController().GetBodyCollider()))
        {
            if (references.GetPlayerController().GetIsInActivity())
            {
                if (!pressKey) return;

                pressKey.SetActive(false);
            }
            else
            {
                if (!pressKey) return;

                pressKey.SetActive(true);
            }
        }
        else
        {
            if (!pressKey) return;

            pressKey.SetActive(false);
        }
    }

    private void ReverseRoom()
    {
        int index = UnityEngine.Random.Range(1, 101);
        
        if (references.GetSimulationsPlaceHolder().GetReverseRooms() && index <= 20)
        {
            Camera.main.transform.rotation = new(0, 0, 180, 0);

            references.GetPlayerController().ReverseRoom(true);
        }
        else
        {
            Camera.main.transform.rotation = new();

            references.GetPlayerController().ReverseRoom(false);
        }
    }

    private IEnumerator FadeIn()
    {
        yield return fader.FadeOut(0.2f);

        if (references.GetRoomPoolTest()) //use for debuging
        {
            references.GetRoomPoolTest().NextRoom();
        }
        else
        {
            references.GetRoomPool().NextRoom();
        }
    }

    private void ActivityCheck()
    {
        int i = 0;

        foreach (Activity activity in activitiesInCurrentRoom)
        {
            if (activity.GetIsActivityCompleted())
            {
                i++;
            }
        }

        if (references.GetPlayerController().GetCursed() && i == activitiesInCurrentRoom.Length)
        {
            StartCoroutine(SpawnExitRoom());
        }
        else if (!references.GetPlayerController().GetCursed() && i == activitiesInCurrentRoom.Length - 1)
        {
            StartCoroutine(SpawnExitRoom());
        }
    }

    private IEnumerator SpawnExitRoom()
    {
        if (bossRoom) yield break;

        roomCleared = true;

        yield return new WaitForSeconds(0.5f);

        woodenDoorAnimator.SetTrigger("open");

        if (hostileRoom)
        {
            SpawnMerchant();
            SpawnScroll();
        }

        yield return new WaitForSeconds(0.5f);

        woodenDoorHighlight.enabled = true;

        canExitRoom = true;

        if (roomPnjIndex != -1) yield break;

        float chanceForStatsUp = UnityEngine.Random.Range(0, 100);

        if (chanceForStatsUp <= references.GetStatsUpBoard().GetCurrentChanceForStatsUp())
        {
            references.GetStatsUpBoard().EnableStatsUpIndication();
            references.GetStatsUpBoard().ResetCurrentChanceForStatsUp();
        }
        else
        {
            references.GetStatsUpBoard().IncreaseChanceToStatsUp();
        }
    }

    private void SpawnMerchant()
    {
        if (!merchant) return;
        if (FindObjectOfType<NPC>())
        {
            if (FindObjectOfType<NPC>().GetComponent<SpriteRenderer>().enabled == true) return; //Check if there already a NPC spawned
        }
        else if (FindObjectOfType<Scroll>())
        {
            if (FindObjectOfType<Scroll>().GetComponentInChildren<SpriteRenderer>().enabled == true) return; //Check if there a Scroll
        }

        int spawnChance = UnityEngine.Random.Range(0, 100);

        if (spawnChance == 0)
        {
            Instantiate(smokeVFX, new Vector2(PNJSpawnPosition.position.x, PNJSpawnPosition.position.y + 1), Quaternion.identity);
            Instantiate(merchant, PNJSpawnPosition.position, Quaternion.identity, transform);
        }
    }

    public void ReloadRoom()
    {
        references.GetPlayerController().transform.position = spawnPosition.position;

        int damage = UnityEngine.Random.Range(2, 6);
        references.GetPlayerHealth().TakeDamage(damage, false, true, false, true, false, true);

        foreach (Activity activity in activitiesInCurrentRoom)
        {
            activity.ResetActivity();
        }

        foreach (Transform pickup in pickups.transform)
        {
            Destroy(pickup.gameObject);
        }

        foreach (WeakPlatform platform in FindObjectsOfType<WeakPlatform>())
        {
            platform.ReloadWeakPlatform();
        }

        foreach (TextInfo item in FindObjectsOfType<TextInfo>())
        {
            Destroy(item.gameObject);
        }

        foreach (Projectile item in FindObjectsOfType<Projectile>())
        {
            Destroy(item.gameObject);
        }

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("VFX"))
        {
            Destroy(item);
        }
    }

    public int GetRoomIndex()
    {
        return roomPnjIndex;
    }

    public string GetRoomName()
    {
        return roomNameTxt.text;
    }

    public bool GetCanExitRoom()
    {
        return canExitRoom;
    }

    public GameObject GetPickupsPool()
    {
        return pickups;
    }

    public Vector3 GetPNJSpawnPosition()
    {
        return PNJSpawnPosition.position;
    }

    public bool GetHostileRoom()
    {
        return hostileRoom;
    }

    public bool Boss1HP()
    {
        if (haveOneHP && GetRoomName() == "Miscs Room - Skeleton King")
        {
            return true;
        }

        return false;
    }

    public void OpenRoom()
    {
        closedRoom = false;
    }

    public void SetCanExitRoom(bool state)
    {
        canExitRoom = state;
    }

    public void SpawnScroll()
    {
        if (!scroll) return;
        if (FindObjectOfType<NPC>())
        {
            if (FindObjectOfType<NPC>().GetComponent<SpriteRenderer>().enabled == true) return;
        }

        if (FindObjectOfType<Scroll>())
        {
            if (FindObjectOfType<Scroll>().GetComponentInChildren<SpriteRenderer>().enabled == true) return;
        }

        if (Mathf.RoundToInt(references.GetSector().GetRoomsInTotal() * 0.3f) == references.GetSector().GetRoomsCleared() ||
            Mathf.RoundToInt(references.GetSector().GetRoomsInTotal() * 0.8f) == references.GetSector().GetRoomsCleared())
        {
            Instantiate(scroll, PNJSpawnPosition.position, Quaternion.identity, transform);
        }
    }
}

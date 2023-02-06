using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomPoolTest : MonoBehaviour
{
    [SerializeField] GameObject[] roomList;

    readonly Queue<GameObject> roomsInFloor = new();

    CatchReferences references;
    GameObject currentRoom;

    int roomsRemaining;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        BuildFloor();
    }

    private void BuildFloor()
    {
        foreach (GameObject room in roomList)
        {
            GameObject roomPrefab = Instantiate(room, transform);

            roomsInFloor.Enqueue(roomPrefab);

            roomPrefab.SetActive(false);

            roomsRemaining++;
        }

        references.GetSector().SetRoomsRemaining(roomsRemaining);

        NextRoom();
    }

    public void NextRoom()
    {
        if (currentRoom && roomsInFloor.Count != 0)
        {
            DestroyLastRoom();
        }

        if (roomsInFloor.Count == 0)
        {
            if (SceneManager.GetActiveScene().name == "Tuto")
            {
                references.GetGameManager().SetTutoDone(true);

                SceneManager.LoadScene("Sector");
            }
            else
            {
                FindObjectOfType<EndGame>().DisplayPanel();
                FindObjectOfType<SectorTimer>().SetGameFinished();
                references.GetGameManager().AddWinStreak();
            }
        }
        else
        {
            references.GetSector().DecreaseRoomsRemaining();

            references.GetPlayerHealth().HealBetweenRoom();

            references.GetPassiveObject().LetTheMusicPlay();
            references.GetPassiveObject().CactusSeed();
            references.GetPassiveObject().HeartSeed();
            references.GetPassiveObject().ResetPurification();

            currentRoom = roomsInFloor.Dequeue();
            currentRoom.SetActive(true);

            if (references.GetRoomCount())
            {
                references.GetRoomCount().AddRoom();
            }

            references.GetEnemyPool().ClearList();
        }
    }

    private void DestroyLastRoom()
    {
        foreach (TextInfo item in FindObjectsOfType<TextInfo>())
        {
            Destroy(item.gameObject);
        }

        foreach (Projectile item in FindObjectsOfType<Projectile>())
        {
            Destroy(item.gameObject);
        }

        references.GetPlayerController().LavaVFX(false);

        Destroy(currentRoom);
    }

    public Queue<GameObject> GetRoomsInFloor()
    {
        return roomsInFloor;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom.GetComponent<Room>();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRoomName : MonoBehaviour
{
    [SerializeField] GameObject[] roomsPrefab;
    [SerializeField] List<string> roomsName;

    private void Start()
    {
        foreach (var room in roomsPrefab)
        {
            roomsName.Add(room.name);
        }
    }
}

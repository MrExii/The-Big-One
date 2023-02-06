using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RoomCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomCountTxt;

    int roomCount;

    public void AddRoom()
    {
        roomCount++;

        if (roomCount < 10)
        {
            roomCountTxt.text = "Room : 0" + roomCount;
        }
        else
        {
            roomCountTxt.text = "Room : " + roomCount;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public void Zoom(RoomZoom roomZoom)
    {
        if (roomZoom == RoomZoom.None) return;

        if (roomZoom == RoomZoom.Normal)
        {
            Camera.main.orthographicSize = 7.2f;
        }
        else if (roomZoom == RoomZoom.Medium)
        {
            Camera.main.orthographicSize = 5.8f;
        }
        else if (roomZoom == RoomZoom.Small)
        {
            Camera.main.orthographicSize = 4.6f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomTimerTxt;
    [SerializeField] ColorGradient colorGradient;

    Room room;
    PlayerHealth playerHealth;

    float timerValue;
    float timeToCompleteRoom;

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
        if (timeToCompleteRoom == -1) return;
        if (room.GetCanExitRoom()) return;
        if (!room.GetHostileRoom()) return;
        if (playerHealth.isDead) return;
        
        UpdateTimerValue();
    }

    private void UpdateTimerValue()
    {
        timerValue -= Time.deltaTime;

        if (timerValue > 0)
        {
            float fillfraction = timerValue / timeToCompleteRoom;
            float timerTxt = Mathf.Round(timerValue * 10) / 10;

            roomTimerTxt.text = timerTxt.ToString();

            roomTimerTxt.color = colorGradient.UpdateColorGradient(fillfraction);
        }
        else
        {
            timerValue = timeToCompleteRoom;

            room.ReloadRoom();
        }
    }

    public void SetTimeToCompleteRoom(float timer)
    {
        if (timer == -1)
        {
            roomTimerTxt.enabled = false;
        }
        else
        {
            roomTimerTxt.enabled = true;
        }

        timeToCompleteRoom = timer;
        timerValue = timer;
    }
}

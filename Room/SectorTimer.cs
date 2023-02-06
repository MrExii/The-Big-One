using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SectorTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sectorTimer;

    CatchReferences references;

    float milliseconds;

    int seconds;
    int minutes;
    int hours;

    string secondsTxt;
    string minutesTxt;
    string hoursTxt;

    bool gameFinished;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Update()
    {
        if (references.GetPlayerHealth().isDead) return;
        if (gameFinished) return;

        milliseconds += Time.deltaTime;

        UpdateGameTimer();

        UpdateTxt();
    }

    private void UpdateSectorTimer()
    {
        if (milliseconds >= 1f)
        {
            seconds--;
            milliseconds = 0f;
        }

        if (seconds < 0)
        {
            minutes--;
            seconds = 59;
        }

        if (seconds == 0 && minutes == 0)
        {
            references.GetPlayerHealth().TakeDamage(Mathf.Infinity, true, true, false, true, true, true);
        }
    }

    private void UpdateGameTimer()
    {
        if (milliseconds > 1f)
        {
            milliseconds = 0f;
            seconds++;
        }

        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }

        if (minutes == 60)
        {
            minutes = 0;
            hours++;
        }
    }

    public float[] GetGameTimer()
    {
        float[] gameTimer = new float[3];

        gameTimer[0] = seconds;
        gameTimer[1] = minutes;
        gameTimer[2] = hours;

        return gameTimer;
    }

    private void UpdateTxt()
    {
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

        sectorTimer.text = hoursTxt + ":" + minutesTxt + ":" + secondsTxt;
    }

    public void SetGameFinished()
    {
        gameFinished = true;
    }
}

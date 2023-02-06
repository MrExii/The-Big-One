using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProgressionBar : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] Image fillImg;
    [SerializeField] TextMeshProUGUI valueTxt;
    [SerializeField] bool progressionTimer;

    public void SetProgressionValue(float valueTotal, float currentValue)
    {
        valueTxt.text = Mathf.RoundToInt(currentValue).ToString();

        SetImageFillAmount(valueTotal, currentValue);
    }

    public void UpdateTimerValue(int minutes, int seconds, float totalTimer, float currentTimer)
    {
        if (seconds < 10)
        {
            valueTxt.text = "0" + minutes + ":0" + seconds;
        }
        else
        {
            valueTxt.text = "0" + minutes + ":" + seconds;
        }

        SetImageFillAmount(totalTimer, currentTimer);
    }

    private void SetImageFillAmount(float valueTotal, float currentValue)
    {
        float fillAmount = 0;

        if (currentValue != 0)
        {
            fillAmount = currentValue / valueTotal;
        }

        fillImg.fillAmount = fillAmount;
    }
}

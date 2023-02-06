using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Drawing;

public class ActivityTimer : MonoBehaviour
{
    [SerializeField] ColorGradient colorGradient;
    [SerializeField] Image activityTimerImg;
    [SerializeField] Keyboard keyboard;

    CatchReferences references;

    float timerValue;
    float timeToCompleteActivity;

    bool enableActivityTimer;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        enableActivityTimer = references.GetSimulationsPlaceHolder().GetEnableActivityTimer();

        if (!enableActivityTimer || timerValue == -1)
        {
            activityTimerImg.enabled = false;
        }
    }

    private void Update()
    {
        if (!enableActivityTimer) return;
        if (timerValue == -1) return;

        UpdateTimerValue();
    }

    private void UpdateTimerValue()
    {
        timerValue -= Time.deltaTime;
        
        if (timerValue > 0)
        {
            float fillfraction = timerValue / timeToCompleteActivity;

            activityTimerImg.fillAmount = fillfraction;
            activityTimerImg.color = colorGradient.UpdateColorGradient(fillfraction);
        }
        else
        {
            int damage = Random.Range(2, 5);

            references.GetPlayerHealth().TakeDamage(damage, false, true, false, true, true, true);
            
            keyboard.DisableKeyboard();
        }
    }

    public void SetTimeToCompleteActivity(float timer, float minTimer)
    {
        timer += references.GetSimulationsPlaceHolder().GetActivityTimerMalus();

        if (timer < minTimer)
        {
            timer = minTimer;
        }

        timerValue = timeToCompleteActivity = timer;
    }
}

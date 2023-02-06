using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartStalker : MonoBehaviour
{
    [SerializeField] KeyCode keyToLaunchActivity;
    [SerializeField] KeySeriesSO[] keySeries;
    [SerializeField] Image[] keyToPressImg;
    [SerializeField] Sprite[] keyToPressSprites;
    [SerializeField] ProgressionBar progressionBar;
    [SerializeField] AudioClip[] heartSFX;

    CatchReferences references;
    AudioSource audioSource;
    PlayerInputSystem inputActions;

    float timerValue;
    float heartStalkerTimer;
    float milliseconds;

    int seconds;
    int minutes;

    bool isKeySwap;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.PlayerHeartStalker.LaunchActivity.performed += ctx => OnLaunchActivity();

        if (progressionBar)
        {
            audioSource = progressionBar.GetComponent<AudioSource>();
        }
    }

    private void OnDisable()
    {
        inputActions.PlayerHeartStalker.Disable();
    }

    private void Start()
    {
        if (keyToPressImg[0] == null) return;

        keyToPressImg[0].enabled = false;
        keyToPressImg[1].enabled = false;
        
        if (!references.GetSimulationsPlaceHolder().GetHeartStalker())
        {
            progressionBar.gameObject.SetActive(false);

            return;
        }

        inputActions.PlayerHeartStalker.Enable();

        SetupTimerValue();
    }

    private void SetupTimerValue()
    {
        heartStalkerTimer = references.GetSimulationsPlaceHolder().GetHeartStalkerTimer();

        timerValue = heartStalkerTimer / 2f;

        RefreshTimerValue();
    }

    private void Update()
    {
        if (keyToPressImg[0] == null) return;
        if (!references.GetSimulationsPlaceHolder().GetHeartStalker()) return;
        if (references.GetPlayerHealth().isDead) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        RefreshUI();

        timerValue -= Time.deltaTime;

        if (timerValue > 0)
        {
            UpdateTimerValue();
        }
        else
        {
            progressionBar.UpdateTimerValue(0, 0, heartStalkerTimer, timerValue);

            references.GetPlayerHealth().TakeDamage(Mathf.Infinity, true, true, false, true, true, true);
        }

        if (!audioSource.isPlaying && timerValue < 20 && timerValue > 10)
        {
            if (audioSource.clip == heartSFX[0]) return;

            audioSource.clip = heartSFX[0];
            audioSource.Play();
        }
        else if (timerValue < 10 && timerValue > 0)
        {
            if (audioSource.clip == heartSFX[1]) return;

            audioSource.clip = heartSFX[1];
            audioSource.Play();
        }
        else if (timerValue <= 0)
        {
            if (audioSource.clip == heartSFX[2]) return;

            audioSource.clip = heartSFX[2];
            audioSource.Play();
        }
    }

    private void OnLaunchActivity()
    {
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (!references.GetSimulationsPlaceHolder().GetHeartStalker()) return;
        if (references.GetPlayerHealth().isDead) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        StartCoroutine(SwitchKeySprite());
    }

    private void RefreshUI()
    {
        if (!keyToPressImg[0].enabled && references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].enabled = true;
            keyToPressImg[1].enabled = false;
        }
        else if (!keyToPressImg[1].enabled && !references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].enabled = false;
            keyToPressImg[1].enabled = true;
        }

        if (references.GetGameManager().GetChangeUI() && !isKeySwap)
        {
            keyToPressImg[1].sprite = keyToPressSprites[4];
        }
        else if (!isKeySwap)
        {
            keyToPressImg[1].sprite = keyToPressSprites[2];
        }
    }

    public void EndActivity()
    {
        timerValue += references.GetSimulationsPlaceHolder().GetHeartStalkerActivityBonus();

        if (timerValue > heartStalkerTimer)
        {
            timerValue = heartStalkerTimer;
        }

        RefreshTimerValue();
    }

    private void RefreshTimerValue()
    {
        minutes = (int)timerValue / 60;
        seconds = (int)timerValue - (minutes * 60);
    }

    private void UpdateTimerValue()
    {
        milliseconds += Time.deltaTime;

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

        progressionBar.UpdateTimerValue(minutes, seconds, heartStalkerTimer, timerValue);
    }

    public void DisableHeartStalker()
    {
        progressionBar.gameObject.SetActive(false);

        keyToPressImg[0].enabled = false;
        keyToPressImg[1].enabled = false;

        inputActions.PlayerHeartStalker.Disable();
    }

    public void RefreshHeartStalkerTimer()
    {
        SetupTimerValue();
    }

    private IEnumerator SwitchKeySprite()
    {
        isKeySwap = true;

        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].sprite = keyToPressSprites[1];

            yield return new WaitForSeconds(0.07f);

            keyToPressImg[0].sprite = keyToPressSprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                keyToPressImg[1].sprite = keyToPressSprites[5];

                yield return new WaitForSeconds(0.07f);

                keyToPressImg[1].sprite = keyToPressSprites[4];
            }
            else
            {
                keyToPressImg[1].sprite = keyToPressSprites[3];

                yield return new WaitForSeconds(0.07f);

                keyToPressImg[1].sprite = keyToPressSprites[2];
            }
        }

        if (heartStalkerTimer == 150 || heartStalkerTimer == 120)
        {
            references.GetPlayerKeyboard().EnableKeyboard(keySeries[0].GetTimeToCompleteActivity(), keySeries[0].GetMinTimeToCompleteActivity());
            references.GetPlayerKeyboard().StartActivity(this, keySeries[0]);
        }
        else if (heartStalkerTimer == 60)
        {
            references.GetPlayerKeyboard().EnableKeyboard(keySeries[1].GetTimeToCompleteActivity(), keySeries[1].GetMinTimeToCompleteActivity());
            references.GetPlayerKeyboard().StartActivity(this, keySeries[1]);
        }
        else
        {
            references.GetPlayerKeyboard().EnableKeyboard(keySeries[2].GetTimeToCompleteActivity(), keySeries[2].GetMinTimeToCompleteActivity());
            references.GetPlayerKeyboard().StartActivity(this, keySeries[2]);
        }

        isKeySwap = false;
    }
}

using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlitchDisplay : MonoBehaviour, ISaveable
{
    [SerializeField] TextMeshProUGUI glitchTotalTxt;
    [SerializeField] TextMeshProUGUI glitchAddRemoveTxt;
    [SerializeField] int glitchAmount;
    [SerializeField] GameObject statisticsBoard;
    [SerializeField] Fader fader;
    [SerializeField] CanvasGroup canvasGroup;

    GameManager gameManager;

    float timeSinceLastGlitchAdd;
    int startGameGlitchAmount;
    int glitchAddRemove;
    bool startCoroutine;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        glitchAddRemoveTxt.enabled = false;

        canvasGroup.alpha = 0f;

        glitchTotalTxt.text = glitchAmount.ToString();

        startGameGlitchAmount = glitchAmount;
    }

    private void Update()
    {
        if (statisticsBoard && statisticsBoard.activeInHierarchy && canvasGroup.alpha != 1f && !startCoroutine)
        {
            startCoroutine = true;

            StartCoroutine(fader.FadeOut(0.2f));
        }
        else if (statisticsBoard && !statisticsBoard.activeInHierarchy && canvasGroup.alpha != 0f && startCoroutine)
        {
            startCoroutine = false;

            Invoke(nameof(FadeIn), 2f);
        }

        if (timeSinceLastGlitchAdd > 3f && glitchAddRemoveTxt.isActiveAndEnabled)
        {
            glitchAddRemoveTxt.GetComponent<Animator>().SetTrigger("add");

            Invoke(nameof(FinishGlitchAnimation), 0.65f);
        }

        timeSinceLastGlitchAdd += Time.deltaTime;
    }

    public int GetGlitchAmount()
    {
        return glitchAmount;
    }

    public void AddGlitch(int glitchAmount)
    {
        StopAllCoroutines();
        CancelInvoke();

        if (!statisticsBoard || !statisticsBoard.activeInHierarchy)
        {
            StartCoroutine(fader.FadeOut(0.2f));
        }

        timeSinceLastGlitchAdd = 0f;

        glitchAddRemove += glitchAmount;
        this.glitchAmount += glitchAmount;

        glitchAddRemoveTxt.enabled = true;
        glitchAddRemoveTxt.text = "+" + glitchAddRemove;

        gameManager.IncreaseNumberOfGlitch(glitchAmount);
    }

    public void RemoveGlitch(int glitchAmount)
    {
        StopAllCoroutines();
        CancelInvoke();

        if (!statisticsBoard || !statisticsBoard.activeInHierarchy)
        {
            StartCoroutine(fader.FadeOut(0.2f));
        }

        timeSinceLastGlitchAdd = 0f;

        glitchAddRemove -= glitchAmount;
        this.glitchAmount -= glitchAmount;

        glitchAddRemoveTxt.enabled = true;
        glitchAddRemoveTxt.text = glitchAddRemove.ToString();
    }

    private void FinishGlitchAnimation()
    {
        glitchTotalTxt.text = glitchAmount.ToString();

        glitchAddRemoveTxt.enabled = false;

        glitchAddRemove = 0;
        timeSinceLastGlitchAdd = 0f;

        if (!statisticsBoard || !statisticsBoard.activeInHierarchy)
        {
            Invoke(nameof(FadeIn), 2f);
        }

        CheckAchievement();
    }

    private void CheckAchievement()
    {
        if (gameManager.GetNoobMode()) return;
        if (gameManager.GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfGlitch = gameManager.GetNumberOfGlitch();

            if (numberOfGlitch >= 10000)
            {
                SteamUserStats.GetAchievement("ACH_GLITCH", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    FindObjectOfType<Achievements>().SetAchievement("ACH_GLITCH");
                    SteamUserStats.SetAchievement("ACH_GLITCH");
                }
            }

            SteamUserStats.StoreStats();
        }
    }

    private void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(fader.FadeIn(0.8f));
    }

    public int GetGlitchGained()
    {
        return glitchAmount - startGameGlitchAmount;
    }

    public object CaptureState()
    {
        return glitchAmount;
    }

    public void RestoreState(object state)
    {
        glitchAmount = (int)state;

        glitchTotalTxt.text = glitchAmount.ToString();
    }
}

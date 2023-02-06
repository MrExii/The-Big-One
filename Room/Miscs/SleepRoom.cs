using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SleepRoom : MonoBehaviour
{
    [SerializeField] GameObject pressKey;
    [SerializeField] KeyCode[] keyToSleep;
    [SerializeField] TextMeshProUGUI dialogueTxt;
    [SerializeField] float timeBetweenLetter;
    [SerializeField] SpriteRenderer highlight;
    [SerializeField] Fader fader;

    CatchReferences references;
    PlayerInputSystem inputActions;

    bool sleepAnimationStarted;
    char[] charArray;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.PressKey.SleepRoom.performed += ctx => OnSleep();
    }

    private void OnEnable()
    {
        inputActions.PressKey.Enable();
    }

    private void Start()
    {
        pressKey.SetActive(false);
        dialogueTxt.enabled = false;

        fader.SetCanvasGroupAlpha(0f);
    }

    private void OnSleep()
    {
        if (sleepAnimationStarted || !pressKey.activeInHierarchy) return;

        sleepAnimationStarted = true;

        StartCoroutine(SleepAnimation());
    }

    private IEnumerator SleepAnimation()
    {
        CheckAchievement();

        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);

        yield return StartCoroutine(fader.FadeOut(1f));

        references.GetPlayerHealth().Heal(999);
        references.GetTrinket().RechargeTrinket();
        pressKey.SetActive(false);

        yield return StartCoroutine(DialogueAnimation("Z...z...Z..."));

        references.GetPlayerController().disableControl = false;
        references.GetPlayerController().SetIsInActivity(false);

        highlight.enabled = false;

        yield return StartCoroutine(fader.FadeIn(1f));

        inputActions.PressKey.Disable();
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_SLEEP_ROOM", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_SLEEP_ROOM");

                SteamUserStats.SetAchievement("ACH_SLEEP_ROOM");
                SteamUserStats.StoreStats();
            }
        }
    }

    private IEnumerator DialogueAnimation(string textToAnimate)
    {
        dialogueTxt.enabled = true;
        dialogueTxt.text = "";

        charArray = textToAnimate.ToCharArray();

        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueTxt.text += charArray[i].ToString();

            yield return new WaitForSeconds(timeBetweenLetter);
        }

        dialogueTxt.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sleepAnimationStarted)
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sleepAnimationStarted)
        {
            pressKey.SetActive(false);
        }
    }
}

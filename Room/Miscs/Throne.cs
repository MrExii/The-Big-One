using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Throne : MonoBehaviour
{
    [SerializeField] GameObject pressKey;
    [SerializeField] GameObject king;
    [SerializeField] DisplayDialogue kingDisplayDialogue;
    [SerializeField] DialogueSO dialogue;
    [SerializeField] GameObject smearVFX;
    [SerializeField] SpriteRenderer highlight;
    [SerializeField] Villager villager;

    CatchReferences references;
    PlayerInputSystem inputActions;

    bool kingDead;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.PressKey.Throne.performed += ctx => OnSeat();

        inputActions.Enable();
    }

    private void Start()
    {
        pressKey.SetActive(false);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    private void OnSeat()
    {
        if (!pressKey.activeInHierarchy) return;

        kingDead = true;

        pressKey.SetActive(false);

        villager.SpeakWithPlayer(true);

        highlight.enabled = false;

        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        references.GetPlayerController().SetIsInActivity(true);
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().StopPlayer(true);

        foreach (var dialogue in dialogue.GetAllDialogue())
        {
            if (dialogue.isPlayerSpeaking)
            {
                yield return StartCoroutine(references.GetPlayerDisplayDialogue().LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
            }
            else
            {
                yield return StartCoroutine(kingDisplayDialogue.LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
            }
        }

        SpawnSmear();

        yield return new WaitForSeconds(0.2f);

        villager.GetComponentInChildren<Animator>().SetTrigger("death");

        yield return new WaitForSeconds(1f);

        references.GetPlayerController().PlayerCrown();

        villager.GetComponentInChildren<Animator>().SetTrigger("noCrown");

        references.GetPlayerController().SetIsInActivity(false);
        references.GetPlayerController().disableControl = false;

        CheckAchievement();
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_KILL_KING", out bool achivementUnlock);

            if (!achivementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_KILL_KING");

                SteamUserStats.SetAchievement("ACH_KILL_KING");
                SteamUserStats.StoreStats();
            }
        }
    }

    private void SpawnSmear()
    {
        GameObject smear = Instantiate(smearVFX, king.GetComponentInChildren<SpriteRenderer>().transform);
        smear.transform.localPosition = new(1.8f, 0, 0);
        smear.GetComponentInChildren<SpriteRenderer>().flipX = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pressKey.activeInHierarchy && !kingDead)
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pressKey.activeInHierarchy && !kingDead)
        {
            pressKey.SetActive(false);
        }
    }
}

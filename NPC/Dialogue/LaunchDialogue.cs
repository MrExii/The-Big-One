using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchDialogue : MonoBehaviour, ISaveable
{
    [SerializeField] GameObject pressKey;
    [SerializeField] GameObject downArrow;
    [SerializeField] DialogueSO[] startDialogue;
    [SerializeField] DialogueSO nothingToTakeDialogue;
    [SerializeField] KeyCode keyToLaunchDialogue;
    [SerializeField] GameObject disapperVFX;
    [SerializeField] bool isPNJDialogue;
    [SerializeField] bool isBossDialogue;
    [SerializeField] bool facePlayer;
    [SerializeField] bool storyDialogue;
    [SerializeField] DialogueSO returnHomeSO;

    CatchReferences references;
    NPC npc;
    Animator animator;
    DisplayDialogue PNJDisplayDialogue;
    SpriteRenderer spriteRenderer;
    PlayerInputSystem inputActions;
    Scroll scroll;

    bool canLaunchDialogue;
    bool firstEncounter;
    bool revealDialogue;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        npc = GetComponent<NPC>();

        if (isPNJDialogue)
        {
            animator = GetComponent<Animator>();
        }
        else
        {
            animator = GetComponentInChildren<Animator>();
        }

        PNJDisplayDialogue = GetComponentInChildren<DisplayDialogue>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        scroll = GetComponent<Scroll>();

        inputActions = new();
        inputActions.PressKey.LaunchDialogue.performed += ctx => OnLaunchDialogue();

        if (GetComponent<SaveableEntity>())
        {
            references.GetSavingWrapper().LoadOneState(GetComponent<SaveableEntity>().GetUniqueIdentifier());
        }
    }

    private void OnEnable()
    {
        inputActions.PressKey.Enable();
    }

    private void OnDestroy()
    {
        inputActions.PressKey.Disable();
    }

    private void Start()
    {
        if (!pressKey) return;

        pressKey.SetActive(false);
    }

    private void Update()
    {
        if (facePlayer)
        {
            if (references.GetPlayerController().transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        if (references.GetPlayerController().GetIsInActivity() && pressKey)
        {
            pressKey.SetActive(false);

            if (downArrow)
            {
                downArrow.SetActive(false);
            }
            
            return;
        }

        if (!canLaunchDialogue) return;

        if (Input.GetKeyDown(keyToLaunchDialogue) && references.GetGameManager().GetKeyboardControl() && !references.GetPauseMenu().GetInPauseMenu())
        {
            StartCoroutine(StartDialogue());
        }
    }

    private void OnLaunchDialogue()
    {
        if (!pressKey) return;
        if (references.GetPlayerController().GetIsInActivity() || !pressKey.activeInHierarchy || references.GetPauseMenu().GetInPauseMenu()) return;

        pressKey.SetActive(false);

        StartCoroutine(StartDialogue());
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_SPEAK_NPC", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                int numberOfNPCSpeak = references.GetGameManager().GetNumberOfNPCSpeak();

                if (numberOfNPCSpeak == 50)
                {
                    references.GetAchievements().SetAchievement("ACH_SPEAK_NPC");

                    SteamUserStats.SetAchievement("ACH_SPEAK_NPC");
                }
            }

            SteamUserStats.GetAchievement("ACH_MERCHANT", out bool achievementUnlock2);

            if (!achievementUnlock2 && npc && npc.GetNPCIndex() == NPCIndex.Merchant)
            {
                references.GetAchievements().SetAchievement("ACH_MERCHANT");

                SteamUserStats.SetAchievement("ACH_MERCHANT");
            }

            SteamUserStats.StoreStats();
        }
    }

    public IEnumerator StartDialogue()
    {
        CheckAchievement();
        
        if (scroll)
        {
            scroll.SetAlreadyRead();
        }

        if (pressKey)
        {
            yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();
        }

        references.GetPlayerController().SetIsInActivity(true);
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().StopPlayer(true);

        if (isPNJDialogue)
        {
            if (references.GetPlayerController().transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true;

                if (!references.GetPlayerController().GetComponentInChildren<SpriteRenderer>().flipX)
                {
                    references.GetPlayerController().GetComponentInChildren<SpriteRenderer>().flipX = true;
                }
            }
            else
            {
                spriteRenderer.flipX = false;

                if (references.GetPlayerController().GetComponentInChildren<SpriteRenderer>().flipX)
                {
                    references.GetPlayerController().GetComponentInChildren<SpriteRenderer>().flipX = false;
                }
            }

            if (!storyDialogue)
            {
                animator.SetTrigger("activity");
            }
        }

        if (!isPNJDialogue && !isBossDialogue)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

        int dialogueIndex = 0;

        if (isPNJDialogue)
        {
            dialogueIndex = PickDialogue();
        }

        if (npc && npc.GetIsNothingToTake())
        {
            StartCoroutine(EndDialogue(nothingToTakeDialogue, false));
        }
        else
        {
            if (storyDialogue || !references.GetGameManager().GetSkipDialogue())
            {
                foreach (var dialogue in startDialogue[dialogueIndex].GetAllDialogue())
                {
                    if (dialogue.isPlayerSpeaking)
                    {
                        yield return StartCoroutine(references.GetPlayerDisplayDialogue().LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
                    }
                    else
                    {
                        yield return StartCoroutine(PNJDisplayDialogue.LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
                    }
                }
            }

            if (isPNJDialogue && !storyDialogue)
            {
                PNJDisplayDialogue.gameObject.SetActive(true);
                npc.EnablePropositions(true);
                PNJDisplayDialogue.EnableEscapeKey();

                references.GetGameManager().IncreaseNumberOfNPCSpeak();
            }
            else
            {
                references.GetPlayerController().SetIsInActivity(false);
                references.GetPlayerController().disableControl = false;

                if (storyDialogue)
                {
                    StartCoroutine(EndDialogue(returnHomeSO, false));
                }
            }
        }
    }

    private int PickDialogue()
    {
        if (references.GetGameManager().GetSkipDialogue()) return 1;

        SteamUserStats.GetAchievement("ACH_ALCHEMIST", out bool achievementUnlock);
        SteamUserStats.GetAchievement("ACH_BARD", out bool achievementUnlock2);
        SteamUserStats.GetAchievement("ACH_BLACKSMITH", out bool achievementUnlock3);
        SteamUserStats.GetAchievement("ACH_CHIEF", out bool achievementUnlock4);
        SteamUserStats.GetAchievement("ACH_FARMER", out bool achievementUnlock5);
        SteamUserStats.GetAchievement("ACH_MINER", out bool achievementUnlock6);
        SteamUserStats.GetAchievement("ACH_PRIEST", out bool achievementUnlock7);
        
        if (!firstEncounter)
        {
            firstEncounter = true;

            return 0;
        }
        else if (!revealDialogue)
        {
            if (npc.GetNPCIndex() == NPCIndex.Merchant) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Alchemist && !achievementUnlock) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Bard && !achievementUnlock2) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Blacksmith && !achievementUnlock3) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Chief && !achievementUnlock4) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Farmer && !achievementUnlock5) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Miner && !achievementUnlock6) return 1;
            if (npc.GetNPCIndex() == NPCIndex.Priest && !achievementUnlock7) return 1;

            revealDialogue = true;

            return 2;
        }

        return 1;
    }

    public IEnumerator EndDialogue(DialogueSO endDialogue, bool skeletonDeath)
    {
        if (storyDialogue || !references.GetGameManager().GetSkipDialogue() || skeletonDeath)
        {
            foreach (var dialogue in endDialogue.GetAllDialogue())
            {
                if (dialogue.isPlayerSpeaking)
                {
                    yield return StartCoroutine(references.GetPlayerDisplayDialogue().LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
                }
                else
                {
                    yield return StartCoroutine(PNJDisplayDialogue.LaunchDialogue(dialogue.textDialogue, dialogue.vocalSFX, dialogue.fontStlye));
                }
            }
        }
        else
        {
            PNJDisplayDialogue.DisableDisplayDialogue();
        }

        if (skeletonDeath) yield break;

        if (disapperVFX)
        {
            Instantiate(disapperVFX, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
        }

        references.GetPlayerController().disableControl = false;
        references.GetPlayerController().SetIsInActivity(false);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        Destroy(downArrow);

        if (storyDialogue)
        {
            StartCoroutine(GetComponentInParent<StoryDialogues>().IncreaseStoryDialoguesIndex());
        }
    }

    public void SetupDialogueSO(DialogueSO dialogueSO)
    {
        startDialogue[0] = dialogueSO;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(true);

            if (downArrow)
            {
                downArrow.SetActive(false);
            }

            canLaunchDialogue = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(false);

            if (downArrow)
            {
                downArrow.SetActive(true);
            }

            canLaunchDialogue = false;
        }
    }

    public object CaptureState()
    {
        bool[] data = new bool[2];

        data[0] = firstEncounter;
        data[1] = revealDialogue;

        return data;
    }

    public void RestoreState(object state)
    {
        bool[] data = (bool[])state;

        firstEncounter = data[0];
        revealDialogue = data[1];
    }
}

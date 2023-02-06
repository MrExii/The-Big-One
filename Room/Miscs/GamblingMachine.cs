using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblingMachine : MonoBehaviour
{
    [System.Serializable]
    private class GamblingStatsUp
    {
        public GamblingStatsIndex statIndex;
        public int statChance;
        public float[] value;
        public int epicValuePercentage;
    }

    [SerializeField] GameObject pressKey;
    [SerializeField] GamblingStatsUp[] statsUp;
    [SerializeField] AnimationClip gamblingAnimation;
    [SerializeField] SpriteRenderer highlightSpriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] TextInfoSpawner textInfoSpawner;
    [SerializeField] AudioClip[] winAndLost;
    [SerializeField] AudioSource audioSource;

    CatchReferences references;
    PlayerInputSystem inputActions;

    int numberOfGambling = 3;
    bool isAlreadyGambling;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.PressKey.GamblingMachine.performed += ctx => OnGambling();

        inputActions.PressKey.Enable();

        pressKey.SetActive(false);
    }

    private void OnDestroy()
    {
        inputActions.PressKey.Disable();
    }

    private void Update()
    {
        if (!isAlreadyGambling && !pressKey.activeInHierarchy)
        {
            highlightSpriteRenderer.enabled = true;
        }
    }

    private void OnGambling()
    {
        if (!pressKey.activeInHierarchy) return;
        if (numberOfGambling == 0) return;

        audioSource.volume = 0.3f;
        audioSource.Play();

        numberOfGambling--;
        isAlreadyGambling = true;

        pressKey.SetActive(false);
        highlightSpriteRenderer.enabled = false;

        GamblingStatsIndex statIndex = GamblingStatsIndex.None;
        int chanceToWin = Random.Range(0, 101);

        foreach (var stat in statsUp)
        {
            if (stat.statChance >= chanceToWin)
            {
                statIndex = stat.statIndex;
            }
        }

        if (statIndex == GamblingStatsIndex.None)
        {
            StartCoroutine(Lose());
        }
        else
        {
            foreach (var stat in statsUp)
            {
                if (statIndex == stat.statIndex)
                {
                    int chanceForEpic = Random.Range(0, 101);

                    if (chanceForEpic <= stat.epicValuePercentage)
                    {
                        StartCoroutine(GainBonus(statIndex, stat.value[1], true));
                    }
                    else
                    {
                        StartCoroutine(GainBonus(statIndex, stat.value[0], false));
                    }
                }
            }
        }

        CheckAchievement();
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfGambling = references.GetGameManager().GetNumberOfGambling();

            if (numberOfGambling == 100)
            {
                SteamUserStats.GetAchievement("ACH_GAMBLING_100", out bool achievementUnlock);
                
                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_GAMBLING_100");
                    SteamUserStats.SetAchievement("ACH_GAMBLING_100");
                }
            }
            
            SteamUserStats.StoreStats();
        }
    }

    private IEnumerator GainBonus(GamblingStatsIndex statIndex, float value, bool isEpic)
    {
        references.GetGameManager().IncreaseNumberOfGambling();

        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        animator.SetTrigger("win");

        yield return new WaitForSeconds(2.6f);

        audioSource.Stop();
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(winAndLost[0]);

        ColorUtility.TryParseHtmlString("#D65108", out Color epicColor);
        ColorUtility.TryParseHtmlString("#3152A6", out Color normalColor);

        if (statIndex == GamblingStatsIndex.Heal)
        {
            references.GetPlayerHealth().Heal(value);

            if (isEpic)
            {
                textInfoSpawner.SpawnNPCBonus("Healing", epicColor);
            }
            else
            {
                textInfoSpawner.SpawnNPCBonus("Healing", normalColor);
            }
        }
        else if (statIndex == GamblingStatsIndex.DamageUp)
        {
            references.GetPlayerStatistics().AddBonusDamage(value);

            if (isEpic)
            {
                textInfoSpawner.SpawnNPCBonus("Damage +" + value, epicColor);
            }
            else
            {
                textInfoSpawner.SpawnNPCBonus("Damage +" + value, normalColor);
            }
        }
        else if (statIndex == GamblingStatsIndex.HealthUp)
        {
            references.GetPlayerStatistics().AddBonusHealth(value);

            if (isEpic)
            {
                textInfoSpawner.SpawnNPCBonus("Health +" + value, epicColor);
            }
            else
            {
                textInfoSpawner.SpawnNPCBonus("Health +" + value, normalColor);
            }
        }
        else if (statIndex == GamblingStatsIndex.ArmorUp)
        {
            references.GetPlayerStatistics().AddArmorPNJBonus(value);
            references.GetPlayerHealth().DisplayCurrentArmor();

            if (isEpic)
            {
                textInfoSpawner.SpawnNPCBonus("Armor +" + value, epicColor);
            }
            else
            {
                textInfoSpawner.SpawnNPCBonus("Armor +" + value, normalColor);
            }
        }
        else if (statIndex == GamblingStatsIndex.UpgradeWeapon)
        {
            references.GetFighter().UpgradeWeapon(true);

            textInfoSpawner.SpawnNPCBonus(references.GetFighter().GetWeaponType().ToString() + "+", epicColor);
        }

        yield return new WaitForSeconds(gamblingAnimation.length - 2.06f);

        if (numberOfGambling != 0)
        {
            isAlreadyGambling = false;
        }
    }

    private IEnumerator Lose()
    {
        yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();

        animator.SetTrigger("lose");

        yield return new WaitForSeconds(gamblingAnimation.length - 1f);

        audioSource.Stop();
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(winAndLost[1]);

        if (numberOfGambling != 0)
        {
            isAlreadyGambling = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pressKey.activeInHierarchy && !isAlreadyGambling)
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pressKey.activeInHierarchy && !isAlreadyGambling)
        {
            pressKey.SetActive(false);
        }
    }
}

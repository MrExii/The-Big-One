using Steamworks;
using System;
using UnityEngine;

public class Curse : MonoBehaviour
{
    CatchReferences references;
    TextInfoSpawner textInfoSpawner;

    float currentCurse;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        textInfoSpawner = GetComponentInChildren<TextInfoSpawner>();
    }

    public void AddCurse(float amount, bool spawnTextInfo)
    {
        if (!references.GetPlayerController().GetCursed()) return;

        currentCurse += amount;

        if (spawnTextInfo && amount >= 1)
        {
            textInfoSpawner.SpawnDamage(amount, false, true);
        }

        if (currentCurse >= 100)
        {
            currentCurse = 100;

            references.GetPlayerHealth().TakeDamage(Mathf.Infinity, false, true, false, false, false, true);

            CheckAchievement();
        }

        references.GetCurseDisplay().RefreshCurse(currentCurse);
    }

    public void RemoveCurse(float amount)
    {
        if (!references.GetPlayerController().GetCursed()) return;

        currentCurse -= amount;

        if (currentCurse < 0)
        {
            currentCurse = 0;
        }

        references.GetCurseDisplay().RefreshCurse(currentCurse);
    }

    public float GetCurrentCurse()
    {
        return currentCurse;
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_CURSE", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_CURSE");

                SteamUserStats.SetAchievement("ACH_CURSE");
                SteamUserStats.StoreStats();
            }
        }
    }
}

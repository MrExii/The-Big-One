using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static DialogueSO;

public class Scroll : MonoBehaviour, ISaveable
{
    [Serializable]
    private class ScrollSettings
    {
        public DialogueSO scrollDialogue;
        public bool alreadyRead;
    }

    [Serializable]
    private class NPCScrolls
    {
        public NPCIndex npcIndex;
        public ScrollSettings[] scrollSettings;
    }

    [SerializeField] NPCScrolls[] npcScrolls;

    CatchReferences references;

    int npcIndex;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        FindObjectOfType<SavingWrapper>().LoadOneState(GetComponent<SaveableEntity>().GetUniqueIdentifier());
    }

    private void Start()
    {
        npcIndex = UnityEngine.Random.Range(0, npcScrolls.Length);

        for (int i = 0; i < npcScrolls.Length; i++)
        {
            if (i == npcIndex)
            {
                foreach (var scroll in npcScrolls[i].scrollSettings)
                {
                    if (!scroll.alreadyRead)
                    {
                        GetComponent<LaunchDialogue>().SetupDialogueSO(scroll.scrollDialogue);

                        return;
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    public void SetAlreadyRead()
    {
        for (int i = 0; i < npcScrolls.Length; i++)
        {
            if (i == npcIndex)
            {
                foreach (var scroll in npcScrolls[npcIndex].scrollSettings)
                {
                    if (!scroll.alreadyRead)
                    {
                        scroll.alreadyRead = true;

                        references.GetGameManager().IncreaseNumberOfScrollRead();

                        CheckAchievements();
                        
                        break;
                    }
                }
            }
        }
    }

    private void CheckAchievements()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_OLD_SCROLLS", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                int numberOfScrollRead = references.GetGameManager().GetNumberOfScrollRead();

                if (numberOfScrollRead == 35)
                {
                    references.GetAchievements().SetAchievement("ACH_OLD_SCROLLS");
                    SteamUserStats.SetAchievement("ACH_OLD_SCROLLS");
                }
            }

            SteamUserStats.GetAchievement("ACH_ALCHEMIST", out bool achievementUnlock2);
            SteamUserStats.GetAchievement("ACH_BARD", out bool achievementUnlock3);
            SteamUserStats.GetAchievement("ACH_BLACKSMITH", out bool achievementUnlock4);
            SteamUserStats.GetAchievement("ACH_CHIEF", out bool achievementUnlock5);
            SteamUserStats.GetAchievement("ACH_FARMER", out bool achievementUnlock6);
            SteamUserStats.GetAchievement("ACH_MINER", out bool achievementUnlock7);
            SteamUserStats.GetAchievement("ACH_PRIEST", out bool achievementUnlock8);

            if (!achievementUnlock2 && npcIndex == 0 && CheckScrollAlreadyRead(0))
            {
                references.GetAchievements().SetAchievement("ACH_ALCHEMIST");
                SteamUserStats.SetAchievement("ACH_ALCHEMIST");
            }
            else if (!achievementUnlock3 && npcIndex == 1 && CheckScrollAlreadyRead(1))
            {
                references.GetAchievements().SetAchievement("ACH_BARD");
                SteamUserStats.SetAchievement("ACH_BARD");
            }
            else if (!achievementUnlock4 && npcIndex == 2 && CheckScrollAlreadyRead(2))
            {
                references.GetAchievements().SetAchievement("ACH_BLACKSMITH");
                SteamUserStats.SetAchievement("ACH_BLACKSMITH");
            }
            else if (!achievementUnlock5 && npcIndex == 3 && CheckScrollAlreadyRead(3))
            {
                references.GetAchievements().SetAchievement("ACH_CHIEF");
                SteamUserStats.SetAchievement("ACH_CHIEF");
            }
            else if (!achievementUnlock6 && npcIndex == 4 && CheckScrollAlreadyRead(4))
            {
                references.GetAchievements().SetAchievement("ACH_FARMER");
                SteamUserStats.SetAchievement("ACH_FARMER");
            }
            else if (!achievementUnlock7 && npcIndex == 5 && CheckScrollAlreadyRead(5))
            {
                references.GetAchievements().SetAchievement("ACH_MINER");
                SteamUserStats.SetAchievement("ACH_MINER");
            }
            else if (!achievementUnlock8 && npcIndex == 6 && CheckScrollAlreadyRead(6))
            {
                references.GetAchievements().SetAchievement("ACH_PRIEST");
                SteamUserStats.SetAchievement("ACH_PRIEST");
            }

            SteamUserStats.StoreStats();
        }
    }

    private bool CheckScrollAlreadyRead(int index)
    {
        int scrollRemaining = npcScrolls[index].scrollSettings.Length;

        foreach (var scroll in npcScrolls[index].scrollSettings)
        {
            if (scroll.alreadyRead)
            {
                scrollRemaining--;
            }
        }
        
        if (scrollRemaining == 0)
        {
            return true;
        }

        return false;
    }

    public object CaptureState()
    {
        Dictionary<int, bool> data = new();
        int index = 0;

        foreach (NPCScrolls npc in npcScrolls)
        {
            for (int i = 0; i < npc.scrollSettings.Length; i++)
            {
                data.Add(index, npc.scrollSettings[i].alreadyRead);

                index++;
            }
        }

        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<int, bool> data = (Dictionary<int, bool>)state;
        int index = 0;

        foreach (NPCScrolls npc in npcScrolls)
        {
            for (int i = 0; i < npc.scrollSettings.Length; i++)
            {
                foreach (KeyValuePair<int, bool> scroll in data)
                {
                    if (scroll.Key == index)
                    {
                        npc.scrollSettings[i].alreadyRead = scroll.Value;
                    }
                }

                index++;
            }
        }
    }
}

using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Achievements : MonoBehaviour, ISaveable
{
    [System.Serializable]
    private class Achievement
    {
        public string achievementID;
        public bool isUnlock;
    }

    [SerializeField] Achievement[] achievements;
    [SerializeField] bool syncAchievements;

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        //SetAllAchievements(false);

        if (!syncAchievements) return;

        SyncAchievements();
    }

    private void SetAllAchievements(bool state)
    {
        foreach (var ach in achievements)
        {
            if (ach.isUnlock != state)
            {
                ach.isUnlock = state;
            }
        }
    }

    private void SyncAchievements()
    {
        foreach (var ach in achievements)
        {
            if (ach.isUnlock)
            {
                if (SteamManager.Initialized)
                {
                    SteamUserStats.SetAchievement(ach.achievementID);

                    SteamUserStats.StoreStats();
                }

                CheckIfAllUnlocked();
            }
        }
    }

    public void SetAchievement(string achievementID)
    {
        if (gameManager.GetDisableSteamworks()) return;

        foreach (var ach in achievements)
        {
            if (ach.achievementID == achievementID && !ach.isUnlock)
            {
                ach.isUnlock = true;

                CheckIfAllUnlocked();

                if (SteamManager.Initialized)
                {
                    SteamUserStats.StoreStats();
                }

                print(achievementID + " as been unlock !");
            }
            else if (ach.achievementID == achievementID)
            {
                print(achievementID + " already unlocked !");
            }
        }
    }

    public bool GetAchievementUnlock(string achievementID)
    {
        foreach (var ach in achievements)
        {
            if (ach.achievementID == achievementID && ach.isUnlock)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckIfAllUnlocked()
    {
        int index = 0;

        foreach (var item in achievements)
        {
            if (item.isUnlock)
            {
                index++;
            }
        }
        
        if (index == achievements.Length - 1 && gameManager.GetNoobMode()) //Minus the last achievement
        {
            if (SteamManager.Initialized)
            {
                SteamUserStats.GetAchievement("ACH_ALL_UNLOCK", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    SteamUserStats.SetAchievement("ACH_ALL_UNLOCK");
                    SteamUserStats.StoreStats();
                }
            }

            foreach (var item in achievements)
            {
                if (item.achievementID == "ACH_ALL_UNLOCK" && !item.isUnlock)
                {
                    item.isUnlock = true;

                    print(item.achievementID + " as been unlock !");
                }
                else if (item.achievementID == "ACH_ALL_UNLOCK")
                {
                    print(item.achievementID + " already unlocked !");
                }
            }
        }
    }

    public object CaptureState()
    {
        Dictionary<string, bool> data = new();

        foreach (var item in achievements)
        {
            if (string.IsNullOrEmpty(item.achievementID)) continue;

            data.Add(item.achievementID, item.isUnlock);
        }

        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, bool> data = (Dictionary<string, bool>)state;

        foreach (var achievement in achievements)
        {
            foreach (KeyValuePair<string, bool> item in data)
            {
                if (item.Key == achievement.achievementID)
                {
                    achievement.isUnlock = item.Value;
                }
            }
        }
    }
}

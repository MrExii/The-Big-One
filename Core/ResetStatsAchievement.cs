using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatsAchievement : MonoBehaviour
{
    [SerializeField] bool resetStats;
    [SerializeField] bool resetAchievements;

    private void Start()
    {
        if (FindObjectOfType<GameManager>().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            if (resetStats)
            {
                SteamUserStats.ResetAllStats(resetAchievements);
            }
        }
    }
}

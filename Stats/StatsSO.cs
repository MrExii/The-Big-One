using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats Levels", menuName = "New Stats Levels", order = 0)]
public class StatsSO : ScriptableObject
{
    [System.Serializable]
    private class Stat
    {
        public StatsIndex statsIndex;
        public int[] levelCost;
        public float[] bonusStat;
    }

    [SerializeField] List<Stat> stats;

    public int GetLevelCost(StatsIndex statIndex, int statLevel)
    {
        foreach (Stat item in stats)
        {
            if (statIndex == item.statsIndex)
            {
                return item.levelCost[statLevel];
            }
        }

        return 0;
    }

    public float GetStatBonus(StatsIndex statIndex, int statLevel)
    {
        foreach (Stat item in stats)
        {
            if (statIndex == item.statsIndex)
            {
                return item.bonusStat[statLevel];
            }
        }

        return 0;
    }
}

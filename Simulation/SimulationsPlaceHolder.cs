using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationsPlaceHolder : MonoBehaviour, ISaveable
{
    [SerializeField] SimulationSO simulationSO;

    CatchReferences references;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    //Simulation 1
    bool enableTimerActivity;
    const string enableTimerActivityKey = "enableTimerActivity";

    //Simulation 2
    bool damageKeySeries;
    const string damageKeySeriesKey = "damageKeySeries";

    //Simulation 3
    float enemyDamageMultiplier = 1f;
    const string enemyDamageMultiplierKey = "enemyDamageMultiplier";

    //Simulation 4
    float enemyHealthMultiplier = 1f;
    const string enemyHealthMultiplierKey = "enemyHealthMultiplier";

    //Simulation 5
    bool reverseKeySeries;
    const string reverseKeySeriesKey = "reverseKeySeries";

    //Simulation 6
    float increaseSectorWidth = 1;
    const string increaseSectorWidthKey = "increaseSectorWidth";

    //Simulation 7
    float activityTimerMalus;
    const string activityTimerMalusKey = "activityTimerMalus";

    //Simulation 8
    bool heartStalker;
    const string heartStalkerKey = "heartStalker";

    //Simulation 9
    float heartStalkerTimer = 120;
    const string heartStalkerTimerKey = "heartStalkerTimer";

    //Simulation 10
    float heartStalkerActivityBonus = 30;
    const string heartStalkerActivityBonusKey = "heartStalkerActivityBonus";

    //Simulation 11
    float enemyArmor;
    const string enemyArmorKey = "enemyArmor";

    //Simulation 12
    bool disableUI;
    const string disableUIKey = "disableUI";

    //Simulation 13
    bool randomKeySeries;
    const string randomKeySeriesKey = "randomKeySeries";

    //Simulation 14
    bool reverseRooms;
    const string reverseRoomsKey = "reverseRooms";

    public void SaveAllSimulationsSettings()
    {
        SimulationSettings[] simulationsSettings = FindObjectsOfType<SimulationSettings>();

        foreach (SimulationSettings item in simulationsSettings)
        {
            SimulationIndex index = item.GetSimulationIndex();

            //Simulation 1
            if (index == SimulationIndex.EnableTimerActivity)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    enableTimerActivity = false;
                }
                else
                {
                    enableTimerActivity = true;
                }
            }

            //Simulation 2
            if (index == SimulationIndex.DamageKeySeries)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    damageKeySeries = false;
                }
                else
                {
                    damageKeySeries = true;
                }
            }

            //Simulation 3
            else if (index == SimulationIndex.EnemyMoreDamage)
            {
                enemyDamageMultiplier = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 4
            else if (index == SimulationIndex.EnemyMoreHealth)
            {
                enemyHealthMultiplier = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 5
            else if (index == SimulationIndex.ReverseKeySeries)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    reverseKeySeries = false;
                }
                else
                {
                    reverseKeySeries = true;
                }
            }

            //Simulation 6
            else if (index == SimulationIndex.IncreaseSectorWidth)
            {
                increaseSectorWidth = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 7
            else if (index == SimulationIndex.ActivityTimerMalus)
            {
                activityTimerMalus = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 8
            else if (index == SimulationIndex.EnableHeartStalker)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    heartStalker = false;
                }
                else
                {
                    heartStalker = true;
                }
            }

            //Simulation 9
            else if (index == SimulationIndex.HeartStalkerTimer)
            {
                heartStalkerTimer = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 10
            else if (index == SimulationIndex.HeartStalkerActivityBonus)
            {
                heartStalkerActivityBonus = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 11
            else if (index == SimulationIndex.EnemyArmor)
            {
                enemyArmor = simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel());
            }

            //Simulation 12
            else if (index == SimulationIndex.DisableUI)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    disableUI = false;
                }
                else
                {
                    disableUI = true;
                }
            }

            //Simulation 13
            else if (index == SimulationIndex.RandomKeySeries)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    randomKeySeries = false;
                }
                else
                {
                    randomKeySeries = true;
                }
            }

            //Simulation 14
            else if (index == SimulationIndex.ReverseRooms)
            {
                if (simulationSO.GetSimulationLvl(index, item.GetCurrentSimulationLevel()) == 0)
                {
                    reverseRooms = false;
                }
                else
                {
                    reverseRooms = true;
                }
            }
        }
    }

    //Simulation 1
    public bool GetEnableActivityTimer()
    {
        return enableTimerActivity;
    }

    //Simulation 2
    public bool GetDamageKeySeries()
    {
        return damageKeySeries;
    }

    //Simulation 3
    public float GetEnemyDamageMultiplier()
    {
        return enemyDamageMultiplier;
    }

    //Simulation 4
    public float GetEnemyHealthMultiplier()
    {
        return enemyHealthMultiplier;
    }

    //Simulation 5
    public bool GetReverseKeySeries()
    {
        return reverseKeySeries;
    }

    //Simulation 6
    public float GetIncreaseSectorWidth()
    {
        return increaseSectorWidth;
    }

    //Simulation 7
    public float GetActivityTimerMalus()
    {
        return activityTimerMalus;
    }

    //Simulation 8
    public bool GetHeartStalker()
    {
        return heartStalker;
    }

    //Simulation 9
    public float GetHeartStalkerTimer()
    {
        return heartStalkerTimer;
    }

    //Simulation 10
    public float GetHeartStalkerActivityBonus()
    {
        return heartStalkerActivityBonus;
    }

    //Simulation 11
    public float GetEnemyArmor()
    {
        return enemyArmor;
    }

    //Simulation 12
    public bool GetDisableUI()
    {
        return disableUI;
    }

    //Simulation 13
    public bool GetRandomKeySeries()
    {
        return randomKeySeries;
    }

    //Simulation 14
    public bool GetReverseRooms()
    {
        return reverseRooms;
    }

    public void RemoveSimulationLevel()
    {
        int index;

        while (true)
        {
            index = UnityEngine.Random.Range(0, 13);

            if (index == 0 && enableTimerActivity)
            {
                enableTimerActivity = false;

                break;
            }
            else if (index == 1 && damageKeySeries)
            {
                damageKeySeries = false;

                break;
            }
            else if (index == 2 && enemyDamageMultiplier > 1)
            {
                if (enemyDamageMultiplier == 1.2f)
                {
                    enemyDamageMultiplier = 1f;
                }
                else if (enemyDamageMultiplier == 1.5f)
                {
                    enemyDamageMultiplier = 1.2f;
                }
                else if (enemyDamageMultiplier == 2f)
                {
                    enemyDamageMultiplier = 1.5f;
                }

                break;
            }
            else if (index == 3 && enemyHealthMultiplier > 1)
            {
                if (enemyHealthMultiplier == 1.2f)
                {
                    enemyHealthMultiplier = 1f;
                }
                else if (enemyHealthMultiplier == 1.5f)
                {
                    enemyHealthMultiplier = 1.2f;
                }
                else if (enemyHealthMultiplier == 2f)
                {
                    enemyHealthMultiplier = 1.5f;
                }

                break;
            }
            else if (index == 4 && reverseKeySeries)
            {
                reverseKeySeries = false;

                break;
            }
            else if (index == 5 && activityTimerMalus < 0)
            {
                if (activityTimerMalus == -1)
                {
                    activityTimerMalus = 0;
                }
                else if (activityTimerMalus == -2)
                {
                    activityTimerMalus = -1;
                }
                else if (activityTimerMalus == -3)
                {
                    activityTimerMalus = -2;
                }
                else if (activityTimerMalus == -99)
                {
                    activityTimerMalus = -3;
                }

                break;
            }
            else if (index == 6 && heartStalker)
            {
                if (heartStalkerTimer == 120 && heartStalkerActivityBonus == 30)
                {
                    heartStalker = false;

                    references.GetHeartStalker().DisableHeartStalker();

                    break;
                }
            }
            else if (index == 7 && heartStalkerTimer < 120)
            {
                if (heartStalkerTimer == 90)
                {
                    heartStalkerTimer = 120;
                }
                else if (heartStalkerTimer == 60)
                {
                    heartStalkerTimer = 90;
                }
                else if (heartStalkerTimer == 30)
                {
                    heartStalkerTimer = 60;
                }

                references.GetHeartStalker().RefreshHeartStalkerTimer();

                break;
            }
            else if (index == 8 && heartStalkerActivityBonus < 30)
            {
                if (heartStalkerActivityBonus == 20)
                {
                    heartStalkerActivityBonus = 30;
                }
                else if (heartStalkerActivityBonus == 15)
                {
                    heartStalkerActivityBonus = 20;
                }
                else if (heartStalkerActivityBonus == 10)
                {
                    heartStalkerActivityBonus = 15;
                }

                break;
            }
            else if (index == 9 && enemyArmor > 0)
            {
                if (enemyArmor == 0.1f)
                {
                    enemyArmor = 0;
                }
                else if (enemyArmor == 0.2f)
                {
                    enemyArmor = 0.1f;
                }
                else if (enemyArmor == 0.3f)
                {
                    enemyArmor = 0.2f;
                }

                break;
            }
            else if (index == 10 && disableUI)
            {
                disableUI = false;

                references.GetCurseDisplay().RefreshCurse(references.GetCurse().GetCurrentCurse());
                references.GetHealthDisplay().RefreshHealth(references.GetPlayerHealth().GetCurrentHealth(), references.GetPlayerStatistics().GetBaseHealth());
                references.GetHealthDisplay().RefreshArmor(references.GetPlayerHealth().GetCurrentArmor(), references.GetPlayerStatistics().GetArmor());

                break;
            }
            else if (index == 11 && randomKeySeries)
            {
                randomKeySeries = false;

                break;
            }
            else if (index == 12 && reverseRooms)
            {
                reverseRooms = false;

                break;
            }
        }
    }

    public object CaptureState()
    {
        Dictionary<string, object> data = new()
        {
            { enableTimerActivityKey, enableTimerActivity }, //Simulation 1
            { damageKeySeriesKey, damageKeySeries }, //Simulation 2
            { enemyDamageMultiplierKey, enemyDamageMultiplier }, //Simulation 3
            { enemyHealthMultiplierKey, enemyHealthMultiplier }, //Simulation 4
            { reverseKeySeriesKey, reverseKeySeries }, //Simulation 5
            { increaseSectorWidthKey, increaseSectorWidth }, //Simulation 6
            { activityTimerMalusKey, activityTimerMalus }, //Simulation 7
            { heartStalkerKey, heartStalker }, //Simulation 8
            { heartStalkerTimerKey, heartStalkerTimer }, //Simulation 9
            { heartStalkerActivityBonusKey, heartStalkerActivityBonus }, //Simulation 10
            { enemyArmorKey, enemyArmor }, //Simulation 11
            { disableUIKey, disableUI }, //Simulation 12
            { randomKeySeriesKey, randomKeySeries }, //Simulation 13
            { reverseRoomsKey, reverseRooms }, //Simulation 14
        };

        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> data = (Dictionary<string, object>)state;

        foreach (KeyValuePair<string, object> item in data)
        {
            //Simulation 1
            if (item.Key == enableTimerActivityKey)
            {
                enableTimerActivity = (bool)item.Value;
            }

            //Simulation 2
            else if (item.Key == damageKeySeriesKey)
            {
                damageKeySeries = (bool)item.Value;
            }

            //Simulation 3
            else if (item.Key == enemyDamageMultiplierKey)
            {
                enemyDamageMultiplier = (float)item.Value;
            }

            //Simulation 4
            else if (item.Key == enemyHealthMultiplierKey)
            {
                enemyHealthMultiplier = (float)item.Value;
            }

            //Simulation 5
            else if (item.Key == reverseKeySeriesKey)
            {
                reverseKeySeries = (bool)item.Value;
            }

            //Simulation 6
            else if (item.Key == increaseSectorWidthKey)
            {
                increaseSectorWidth = (float)item.Value;
            }

            //Simulation 7
            else if (item.Key == activityTimerMalusKey)
            {
                activityTimerMalus = (float)item.Value;
            }

            //Simulation 8
            else if (item.Key == heartStalkerKey)
            {
                heartStalker = (bool)item.Value;
            }

            //Simulation 9
            else if (item.Key == heartStalkerTimerKey)
            {
                heartStalkerTimer = (float)item.Value;
            }

            //Simulation 10
            else if (item.Key == heartStalkerActivityBonusKey)
            {
                heartStalkerActivityBonus = (float)item.Value;
            }

            //Simulation 11
            else if (item.Key == enemyArmorKey)
            {
                enemyArmor = (float)item.Value;
            }

            //Simulation 12
            else if (item.Key == disableUIKey)
            {
                disableUI = (bool)item.Value;
            }

            //Simulation 13
            else if (item.Key == randomKeySeriesKey)
            {
                randomKeySeries = (bool)item.Value;
            }

            //Simulation 14
            else if (item.Key == reverseRoomsKey)
            {
                reverseRooms = (bool)item.Value;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour, ISaveable
{
    [SerializeField] StatsSO statsSO;

    [Header("Base Stats")]
    [SerializeField] float baseHealth;
    [SerializeField] float baseStm;
    [SerializeField] float baseDamage;
    [SerializeField] float baseMoveSpeed;
#if UNITY_EDITOR
    [SerializeField] bool haveArmor;
#endif

    [Header("Stamina")]
    [SerializeField] float stmRegenPerSecond;
    [SerializeField] float stmUseWhenJump;
    [SerializeField] float stmUseWhenRoll;

    CatchReferences references;

    /* -- Stats -- */
    //Stat 1
    float healthBonus;
    const string healthBonusKey = "healthBonus";

    //Stat 2
    float regenPvBetweenRoom;
    const string regenPvBetweenRoomKey = "regenPvBetweenRoom";

    //Stat 3
    bool unlockSleepRoom;
    const string unlockSleepRoomKey = "unlockSleepRoom";

    //Stat 4
    float statBoardDamageBonus;
    const string damageBonusKey = "damageBonus";

    //Stat 5
    float baseArmor;
    const string baseArmorKey = "baseArmor";

    //Stat 6
    float potionNumber = 1;
    const string potionNumberKey = "potionNumber";
    
    //Stat 7
    float statsArmor;
    const string armorBonusKey = "armorBonus";

    //Stat 8
    float criticalDamageBonus = 1f;
    const string criticalDamageBonusKey = "criticalDamageBonus";

    //Stat 9
    float statBoardSecondChance;
    const string gainSecondChanceKey = "gainSecondChance";

    //Stat 10
    float moreCriticalChance;
    const string moreCriticalChanceKey = "moreCriticalChance";

    int divideDamage;

    int divideHP;

    float timeSinceLastPentatonicMinor;
    int numberOfTurn;

    float bonusArmor;

    float damagePNJBonus;

    float healthBonusForLimitedTime;
    float healthBonusDivider;

    float damageBonusForLimitedTime;
    float damageBonusDivider;

    float hotSpicy;
    const float hotSpicyDamagePerSeconds = 2f;

    float hotFatty;
    const float hotFattyHealthPerSeconds = 10f;

    float healthPNJBonus;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        UpdateStatistics();
    }

    private void Update()
    {
        if (references.GetPassiveObject().HotSpicy())
        {
            HotSpicy();
        }

        if (references.GetPassiveObject().HotFatty() && !references.GetPassiveObject().Chemotherapy())
        {
            HotFatty();
        }

        if (references.GetPassiveObject().PentatonicMinor())
        {
            PentatonicMinor();
        }

        DamageBonusForLimitedTime();

        timeSinceLastPentatonicMinor += Time.deltaTime;
    }

    public void SaveAllStatsChanges()
    {
        foreach (StatsSettings stat in FindObjectsOfType<StatsSettings>())
        {
            //Stat 1
            if (stat.GetStatIndex() == StatsIndex.IncreaseHP)
            {
                healthBonus = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 2
            else if (stat.GetStatIndex() == StatsIndex.RegenPvBetweenRoom)
            {
                regenPvBetweenRoom = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 3
            else if (stat.GetStatIndex() == StatsIndex.UnlockSleepRoom)
            {
                if (statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel()) == 0)
                {
                    unlockSleepRoom = false;
                }
                else
                {
                    unlockSleepRoom = true;
                }
            }

            //Stat 4
            else if (stat.GetStatIndex() == StatsIndex.IncreaseDamage)
            {
                statBoardDamageBonus = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 5
            else if (stat.GetStatIndex() == StatsIndex.UnlockArmor)
            {
                baseArmor = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 6
            else if (stat.GetStatIndex() == StatsIndex.AddPotions)
            {
                potionNumber = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 7
            else if (stat.GetStatIndex() == StatsIndex.IncreaseArmor)
            {
                statsArmor = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 8
            else if (stat.GetStatIndex() == StatsIndex.IncreaseCriticalDamage)
            {
                criticalDamageBonus = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 9
            else if (stat.GetStatIndex() == StatsIndex.GainSecondChance)
            {
                statBoardSecondChance = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }

            //Stat 10
            else if (stat.GetStatIndex() == StatsIndex.IncreaseCriticalChance)
            {
                moreCriticalChance = statsSO.GetStatBonus(stat.GetStatIndex(), stat.GetStatLevel());
            }
        }

        UpdateStatistics();

        references.GetSavingWrapper().Save();
    } 

    public void UpdateStatistics()
    {
        foreach (IUpdateStats stat in GetComponents<IUpdateStats>())
        {
            stat.UpdateStats();
        }
    }

    public float GetMoveSpeed()
    {
        return baseMoveSpeed;
    }

    /* -- Stats -- */
    //Stat 1
    public float GetBaseHealth()
    {
        if (references.GetPassiveObject().Chemotherapy())
        {
            return 1;
        }

        float health;

        health = baseHealth + healthBonus + healthPNJBonus + healthBonusForLimitedTime + hotFatty;

        for (int i = 0; i < divideHP; i++)
        {
            health /= 2;
        }

        return health;
    }

    //Stat 2
    public float GetRegenPvBetweenRoom()
    {
        return regenPvBetweenRoom;
    }

    //Stat 3
    public bool GetUnlockSleepRoom()
    {
        return unlockSleepRoom;
    }

    //Stat 4
    public float GetBaseDamage()
    {
        if (references.GetPassiveObject().Chemotherapy())
        {
            return 999f;
        }

        float damage = baseDamage + damagePNJBonus + statBoardDamageBonus + damageBonusForLimitedTime + hotSpicy;

        for (int i = 0; i < divideDamage; i++)
        {
            damage /= 2;
        }

        return Mathf.Round(damage * 10f) / 10f;
    }

    //Stat 5 & 7
    public float GetArmor()
    {
#if UNITY_EDITOR
        if (haveArmor)
        {
            return 50f;
        }
#endif

        if (GetComponent<PassiveObject>().Chemotherapy())
        {
            return 0f;
        }

        if (baseArmor > 0)
        {
            return baseArmor + statsArmor + bonusArmor;
        }
        else
        {
            return bonusArmor;
        }
    }

    //Stat 6
    public float GetPotionNumber()
    {
        return potionNumber;
    }

    //Stat 8
    public float GetCriticalDamageBonus()
    {
        return criticalDamageBonus;
    }

    //Stat 9
    public float GetSecondChance()
    {
        return statBoardSecondChance;
    }

    //Stat 10
    public float GetMoreCriticalChance()
    {
        return moreCriticalChance;
    }

    public void DivideHP()
    {
        divideHP++;
    }

    public void DivideDamage()
    {
        divideDamage++;
    }

    private void PentatonicMinor()
    {
        if (references.GetRoomPool().GetCurrentRoom().GetCanExitRoom()) return;

        if (timeSinceLastPentatonicMinor > 90f && numberOfTurn < 10)
        {
            numberOfTurn++;

            timeSinceLastPentatonicMinor = 0f;

            healthPNJBonus += 10;
            damagePNJBonus += 0.5f;

            references.GetPlayerHealth().Heal(5);
            references.GetFighter().UpdateStats();
        }
    }

    public void AddArmorPNJBonus(float amount)
    {
        bonusArmor += amount;
    }

    public void AddBonusDamage(float amount)
    {
        damagePNJBonus += amount;
    }

    public void AddHealthForLimitedTime(float amount)
    {
        healthBonusForLimitedTime += amount;

        healthBonusDivider = healthBonusForLimitedTime / 180f;

        if (healthBonusForLimitedTime < 0f)
        {
            healthBonusForLimitedTime = 0f;
            healthBonusDivider = 0f;
        }
    }

    public float GetHealthBonusDivider()
    {
        return healthBonusDivider;
    }

    public void AddDamageForLimitedTime(float amount)
    {
        damageBonusForLimitedTime += amount;

        damageBonusDivider = damageBonusForLimitedTime / 180f;

        if (damageBonusForLimitedTime < 0f)
        {
            damageBonusForLimitedTime = 0f;
            damageBonusDivider = 0f;
        }
    }

    private void DamageBonusForLimitedTime()
    {
        if (damageBonusDivider == 0) return;
        if (references.GetRoomPool().GetCurrentRoom().GetCanExitRoom()) return;

        float damage = damageBonusDivider * Time.deltaTime * -1;

        AddDamageForLimitedTime(damage);

        references.GetFighter().UpdateStats();
    }

    private void HotSpicy()
    {
        if (hotSpicy < 0)
        {
            hotSpicy = 0f;
            return;
        }
        else if (hotSpicy > 10f)
        {
            hotSpicy = 10f;
            return;
        }

        if (references.GetPlayerController().GetRigidbody2D().velocity.x == 0f)
        {
            hotSpicy += hotSpicyDamagePerSeconds * Time.deltaTime;
        }
        else
        {
            hotSpicy -= hotSpicyDamagePerSeconds * 2 * Time.deltaTime;
        }

        if (hotSpicy > 0)
        {
            references.GetFighter().UpdateStats();
        }
    }

    private void HotFatty()
    {
        if (hotFatty < 0)
        {
            hotFatty = 0f;
            return;
        }
        else if (hotFatty > 150f)
        {
            hotFatty = 150f;
            return;
        }
        
        if (references.GetPlayerController().GetRigidbody2D().velocity.x == 0f && !references.GetPlayerController().isRolling && hotFatty < 150)
        {
            hotFatty += hotFattyHealthPerSeconds * Time.deltaTime;

            references.GetPlayerHealth().RefreshCurrentHealth(hotFattyHealthPerSeconds * Time.deltaTime);
        }
        else if (references.GetPlayerController().GetRigidbody2D().velocity.x != 0f && hotFatty > 0)
        {
            hotFatty -= hotFattyHealthPerSeconds * 2 * Time.deltaTime;

            references.GetPlayerHealth().RefreshCurrentHealth(hotFattyHealthPerSeconds * -2 * Time.deltaTime);
        }
    }

    public void AddBonusHealth(float amount)
    {
        healthPNJBonus += amount;
    }

    public object CaptureState()
    {
        Dictionary<string, object> data = new()
        {
            /* -- Stats -- */
            { healthBonusKey, healthBonus }, //Stat 1
            { regenPvBetweenRoomKey, regenPvBetweenRoom }, //Stat 2
            { unlockSleepRoomKey, unlockSleepRoom }, //Stat 3
            { damageBonusKey, statBoardDamageBonus }, //Stat 4
            { baseArmorKey, baseArmor }, //Stat 5
            { potionNumberKey, potionNumber }, //Stat 6
            { armorBonusKey, statsArmor }, //Stat 7
            { criticalDamageBonusKey, criticalDamageBonus }, //Stat 8
            { gainSecondChanceKey, statBoardSecondChance }, //Stat 9
            { moreCriticalChanceKey, moreCriticalChance } //Stat 10
        };

        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> data = (Dictionary<string, object>)state;

        foreach (KeyValuePair<string, object> item in data)
        {
            /* -- Stats -- */
            //Stat 1
            if (item.Key == healthBonusKey)
            {
                healthBonus = (float)item.Value;
            }

            //Stat 2
            else if (item.Key == regenPvBetweenRoomKey)
            {
                regenPvBetweenRoom = (float)item.Value;
            }

            //Stat 3
            else if (item.Key == unlockSleepRoomKey)
            {
                unlockSleepRoom = (bool)item.Value;
            }

            //Stat 4
            else if (item.Key == damageBonusKey)
            {
                statBoardDamageBonus = (float)item.Value;
            }

            //Stat 5
            else if (item.Key == baseArmorKey)
            {
                baseArmor = (float)item.Value;
            }

            //Stat 6
            else if (item.Key == potionNumberKey)
            {
                potionNumber = (float)item.Value;
            }

            //Stat 7
            else if (item.Key == armorBonusKey)
            {
                statsArmor = (float)item.Value;
            }

            //Stat 8
            else if (item.Key == criticalDamageBonusKey)
            {
                criticalDamageBonus = (float)item.Value;
            }

            //Stat 9
            else if (item.Key == gainSecondChanceKey)
            {
                statBoardSecondChance = (float)item.Value;
            }

            //Stat 10
            else if (item.Key == moreCriticalChanceKey)
            {
                moreCriticalChance = (float)item.Value;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDamage : MonoBehaviour, IUpdateStats
{
    CatchReferences references;

    const float minDamageMultiplier = 0.4f;
    const float baseCriticalChance = 5;

    float moreCriticalDamage;
    float criticalDamageBoost;

    bool isCriticalDamage;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    public float GetRandomDamage(float baseDamage, bool isPlayerInstigator, bool forceCritical)
    {
        float minDamage; 
        float maxDamage;

        int criticalDamageChance = Random.Range(0, 101);

        if (isPlayerInstigator)
        {
            if ((criticalDamageChance <= baseCriticalChance + moreCriticalDamage) || forceCritical)
            {
                minDamage = (baseDamage + 2) * criticalDamageBoost;
                maxDamage = (baseDamage + 4) * criticalDamageBoost;

                isCriticalDamage = true;
            }
            else
            {
                minDamage = baseDamage - (baseDamage * minDamageMultiplier);
                maxDamage = baseDamage;

                isCriticalDamage = false;
            }
        }
        else
        {
            baseDamage *= 1 + references.GetGameManager().GetRoomClearMultiplier(false);

            if ((criticalDamageChance <= baseCriticalChance) && !references.GetPassiveObject().HylianShield())
            {
                float damageToAdd = Random.Range(5, 9);

                minDamage = (baseDamage - (baseDamage * minDamageMultiplier)) + damageToAdd;
                maxDamage = baseDamage + damageToAdd;

                isCriticalDamage = true;
            }
            else
            {
                minDamage = baseDamage - (baseDamage * minDamageMultiplier);
                maxDamage = baseDamage;

                isCriticalDamage = false;
            }
        }

        return Random.Range(minDamage, maxDamage);
    }

    public bool GetIsCriticalDamage()
    {
        return isCriticalDamage;
    }

    public void UpdateStats()
    {
        moreCriticalDamage = references.GetPlayerStatistics().GetMoreCriticalChance();
        criticalDamageBoost = references.GetPlayerStatistics().GetCriticalDamageBonus();
    }
}

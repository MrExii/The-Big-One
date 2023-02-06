using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageTxt;
    [SerializeField] TextMeshProUGUI canReviveTxt;
    [SerializeField] PlayerStatistics playerStatistics;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] RectTransform damageRectTransform;
    [SerializeField] RectTransform livesRectTransform;

    private void Update()
    {
        UpdateDamage();
        UpdateCanRevive();

        LayoutRebuilder.ForceRebuildLayoutImmediate(damageRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(livesRectTransform);
    }

    private void UpdateDamage()
    {
        damageTxt.text = playerStatistics.GetBaseDamage().ToString();
    }

    private void UpdateCanRevive()
    {
        if (playerHealth.GetSecondChance() > 0)
        {
            canReviveTxt.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            canReviveTxt.transform.parent.gameObject.SetActive(false);
        }

        if (playerHealth.GetSecondChance() > 0)
        {
            canReviveTxt.text = playerHealth.GetSecondChance().ToString();
        }
        else
        {
            canReviveTxt.text = "0";
        }
    }
}

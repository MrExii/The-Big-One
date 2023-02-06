using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Image healthFillImg;
    [SerializeField] Image healthBackgroundImg;
    [SerializeField] TextMeshProUGUI healthTxt;
    [SerializeField] Image bloodImg;

    [Header("Armor")]
    [SerializeField] GameObject armor;
    [SerializeField] Image armorFillImg;
    [SerializeField] Image armorBackgroundImg;

    [Header("Miscs")]
    [SerializeField] float speed;
    [SerializeField] float timeBeforeUpdateBackgroundImg;

    CatchReferences references;

    float timeSinceLastHealthBarUpdate;
    float currentHealthFillAmount = 1f;
    float currentArmorFillAmount = 1f;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        armor.SetActive(false);

        DisableUI();
    }

    private void Update()
    {
        if (references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        if (references.GetPlayerStatistics().GetArmor() > 0 && !armor.activeInHierarchy)
        {
            armor.SetActive(true);
        }
        else if (references.GetPlayerStatistics().GetArmor() <= 0 && armor.activeInHierarchy)
        {
            armor.SetActive(false);
        }

        if (timeSinceLastHealthBarUpdate > timeBeforeUpdateBackgroundImg)
        {
            RefreshBackgroundHealth();
            RefreshBackgroundArmor();
        }

        timeSinceLastHealthBarUpdate += Time.deltaTime;
    }

    private void DisableUI()
    {
        if (!references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        healthTxt.text = "?";
    }

    private void RefreshBackgroundHealth()
    {
        if (healthFillImg.fillAmount < healthBackgroundImg.fillAmount)
        {
            float value = healthBackgroundImg.fillAmount - healthFillImg.fillAmount;

            currentHealthFillAmount -= (value * speed) * Time.deltaTime;

            if (healthBackgroundImg.fillAmount > healthFillImg.fillAmount && 
                healthBackgroundImg.fillAmount < healthFillImg.fillAmount + 0.004f)
            {
                healthBackgroundImg.fillAmount = healthFillImg.fillAmount;
            }
            else
            {
                healthBackgroundImg.fillAmount = currentHealthFillAmount;
            }
        }
        else
        {
            healthBackgroundImg.fillAmount = healthFillImg.fillAmount;

            currentHealthFillAmount = healthBackgroundImg.fillAmount;
        }

        if (healthBackgroundImg.fillAmount < healthFillImg.fillAmount)
        {
            healthBackgroundImg.fillAmount = healthFillImg.fillAmount;
        }
    }

    private void RefreshBackgroundArmor()
    {
        if (armorFillImg.fillAmount < armorBackgroundImg.fillAmount)
        {
            float value = armorBackgroundImg.fillAmount - armorFillImg.fillAmount;

            currentArmorFillAmount -= (value * speed) * Time.deltaTime;

            if (armorBackgroundImg.fillAmount > armorFillImg.fillAmount && 
                armorBackgroundImg.fillAmount < armorFillImg.fillAmount + 0.004f)
            {
                armorBackgroundImg.fillAmount = armorFillImg.fillAmount;
            }
            else
            {
                armorBackgroundImg.fillAmount = currentArmorFillAmount;
            }
        }
        else
        {
            armorBackgroundImg.fillAmount = armorFillImg.fillAmount;

            currentArmorFillAmount = armorBackgroundImg.fillAmount;
        }

        if (armorBackgroundImg.fillAmount < armorFillImg.fillAmount)
        {
            armorBackgroundImg.fillAmount = armorFillImg.fillAmount;
        }
    }

    public void RefreshHealth(float currentHealth, float baseHealth)
    {
        if (currentHealth < 0)
        {
            currentHealth = 0f;
        }

        if (currentHealth < 1)
        {
            healthTxt.text = Mathf.CeilToInt(currentHealth) + "/" + Mathf.RoundToInt(baseHealth);
        }
        else
        {
            healthTxt.text = Mathf.RoundToInt(currentHealth) + "/" + Mathf.RoundToInt(baseHealth);
        }

        healthFillImg.fillAmount = currentHealth / baseHealth;

        RefreshBackgroundHealth();

        timeSinceLastHealthBarUpdate = 0f;
    }

    public void RefreshArmor(float currentArmor, float baseArmor)
    {
        if (currentArmor < 0)
        {
            currentArmor = 0f;
        }

        armorFillImg.fillAmount = currentArmor / baseArmor;

        RefreshBackgroundArmor();

        timeSinceLastHealthBarUpdate = 0f;
    }

    public void Bleeding(bool state)
    {
        bloodImg.enabled = state;
    }
}

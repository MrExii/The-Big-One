using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsSettings : MonoBehaviour, ISaveable
{
    [SerializeField] StatsIndex statIndex;
    [SerializeField] string statName;
    [SerializeField] TextMeshProUGUI statNameTxt;
    [SerializeField] string[] statBonus;
    [SerializeField][TextArea] string statDefinition;
    [SerializeField] bool oneLevelStat;
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundSprites;
    [SerializeField] Image keyToAdd;
    [SerializeField] ProgressionBar progressionBar;
    [SerializeField] Sprite[] rightArrow;
    [SerializeField] int experienceToUnlocked;
    [SerializeField] int simulationReccordToUnlock;
    [SerializeField] Image notSelectedImg;

    GameManager gameManager;

    int statLevel;
    int currentGlitchSpent;

    const string statLevelKey = "statLevel";
    const string currentGlitchSpentKey = "currentGlitchSpent";
    const float timeBetweenKeySwap = 0.07f;

    bool isSwapKeySprite;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        IsLevelMax();

        statNameTxt.text = "<b><color=#00cdb9>" + statName + "</color></b>";
    }

    private void Start()
    {
        keyToAdd.enabled = false;
        notSelectedImg.enabled = false;
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (isSwapKeySprite) return;

        if (gameManager.GetKeyboardControl())
        {
            keyToAdd.sprite = rightArrow[0];
        }
        else if (!gameManager.GetKeyboardControl())
        {
            if (gameManager.GetChangeUI())
            {
                keyToAdd.sprite = rightArrow[4];
            }
            else
            {
                keyToAdd.sprite = rightArrow[2];
            }
        }
    }

    public void AddGlitch(int upgradePrice)
    {
        currentGlitchSpent++;

        StartCoroutine(SwitchKeySprite());

        if (currentGlitchSpent >= upgradePrice)
        {
            currentGlitchSpent = 0;

            statLevel++;

            IsLevelMax();
        }
    }

    public void DisableArrow()
    {
        IsLevelMax();

        keyToAdd.enabled = false;

        notSelectedImg.enabled = true;

        background.sprite = backgroundSprites[0];
    }

    public void EnableArrow()
    {
        IsLevelMax();

        keyToAdd.enabled = true;

        notSelectedImg.enabled = false;

        background.sprite = backgroundSprites[1];
    }

    public void DisableKeyToAdd()
    {
        keyToAdd.gameObject.SetActive(false);
    }

    public bool IsLevelMax()
    {
        if (oneLevelStat)
        {
            if (statLevel == 1)
            {
                progressionBar.gameObject.SetActive(false);

                DisableKeyToAdd();

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (statLevel == statBonus.Length - 1)
            {
                progressionBar.gameObject.SetActive(false);

                DisableKeyToAdd();

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private IEnumerator SwitchKeySprite()
    {
        isSwapKeySprite = true;

        if (gameManager.GetKeyboardControl())
        {
            keyToAdd.sprite = rightArrow[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            keyToAdd.sprite = rightArrow[0];

        }
        else
        {
            if (gameManager.GetChangeUI())
            {
                keyToAdd.sprite = rightArrow[5];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToAdd.sprite = rightArrow[4];
            }
            else
            {
                keyToAdd.sprite = rightArrow[3];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToAdd.sprite = rightArrow[2];
            }
        }

        isSwapKeySprite = false;
    }

    public string GetStatDefinition()
    {
        return statDefinition;
    }

    public StatsIndex GetStatIndex()
    {
        return statIndex;
    }

    public int GetExperienceToUnlocked()
    {
        return experienceToUnlocked;
    }

    public int GetSimulationReccordToUnlock()
    {
        return simulationReccordToUnlock;
    }

    public int GetStatLevel()
    {
        return statLevel;
    }

    public int GetCurrentGlitchAmount()
    {
        return currentGlitchSpent;
    }

    public ProgressionBar GetProgressionBar()
    {
        return progressionBar;
    }

    public string GetCurrentStatBonus()
    {
        return statBonus[statLevel];
    }

    public string GetNextStatBonus()
    {
        return statBonus[statLevel + 1];
    }

    public bool IsOneLevelStat()
    {
        return oneLevelStat;
    }

    public object CaptureState()
    {
        Dictionary<string, object> data = new()
        {
            { statLevelKey, statLevel },
            { currentGlitchSpentKey, currentGlitchSpent }
        };

        return data;
    }

    public void RestoreState(object state)
    {
        Dictionary<string, object> data = (Dictionary<string, object>)state;

        foreach (KeyValuePair<string, object> item in data)
        {
            if (item.Key == statLevelKey)
            {
                statLevel = (int)item.Value;
            }
            else if (item.Key == currentGlitchSpentKey)
            {
                currentGlitchSpent = (int)item.Value;
            }
        }
    }
}

using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatisticsBoard : MonoBehaviour
{
    [Header("Statistics Board")]
    [SerializeField] GameObject pressKey;
    [SerializeField] GameObject statisticsBoard;
    [SerializeField] StatsSO statsSO;
    [SerializeField] GameObject keyboardEscapeImg;
    [SerializeField] GameObject controllerEscapeImg;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] RectTransform contentRectTransform;

    [Header("Stats")]
    [SerializeField] GameObject[] stats;
    [SerializeField] GameObject[] locked;

    [Header("Information Panel")]
    [SerializeField] GameObject informationPanel;
    [SerializeField] RectTransform informationPanelRectTransform;
    [SerializeField] TextMeshProUGUI[] informationPanelTxt;

    [Header("Experience Panel")]
    [SerializeField] TextMeshProUGUI experienceTxt;

    [Header("Key Images Settings")]
    [SerializeField] Image[] escapeKeyImg;
    [SerializeField] Sprite[] escapeKeySprites;

    CatchReferences references;
    Slider slider;
    PlayerInputSystem inputActions;

    int index;

    bool isSwitchingSprite;

    const float statisticsBoardOffset = 165f;
    const float timeBetweenKeySwap = 0.07f;
    float timeSinceAddGlitch;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        slider = GetComponentInChildren<Slider>();

        inputActions = new();
        inputActions.UIStatsBoard.OpenBoard.performed += ctx => OnEnableBoard();
        inputActions.UIStatsBoard.CloseBoard.performed += ctx => OnDisableBoard();
        inputActions.UIStatsBoard.UpArrow.performed += ctx => OnGoUp();
        inputActions.UIStatsBoard.DownArrow.performed += ctx => OnGoDown();
        inputActions.UIStatsBoard.RightArrow.performed += ctx => OnAddGlitch();
        inputActions.UIStatsBoard.RightArrow.canceled += ctx => OnReleaseAddGlitch();

        inputActions.UIStatsBoard.Enable();
    }

    private void OnDestroy()
    {
        inputActions.UIStatsBoard.Disable();
    }

    private void Start()
    {
        RefreshStatsInfo(index);
        EnableStats();
        DisplayCurrentExperience();

        statisticsBoard.SetActive(false);

        keyboardEscapeImg.SetActive(false);
        controllerEscapeImg.SetActive(false);

        pressKey.SetActive(false);
    }

    private void Update()
    {
        RefreshUI();

        timeSinceAddGlitch += Time.deltaTime;
    }

    public void OnEnableBoard()
    {
        if (!statisticsBoard.activeInHierarchy && pressKey.activeInHierarchy && !references.GetPauseMenu().GetInPauseMenu())
        {
            selectionAS.Play();

            StartCoroutine(EnableStatisticsBoard(true));
        }
    }

    public void OnDisableBoard()
    {
        if (!statisticsBoard.activeInHierarchy) return;

        StopAllCoroutines();

        selectionAS.Play();

        StartCoroutine(SwitchKeySprite());
    }

    public void OnGoDown()
    {
        if (!statisticsBoard.activeInHierarchy) return;

        StopAllCoroutines();

        index++;

        if (index == stats.Length)
        {
            index = 0;
        }

        selectionAS.Play();

        RefreshContentRectTransform(false);
        RefreshSelectedStat();

        slider.value = index;
    }

    public void OnGoUp()
    {
        if (!statisticsBoard.activeInHierarchy) return;

        StopAllCoroutines();

        index--;

        if (index < 0)
        {
            index = stats.Length - 1;
        }

        selectionAS.Play();

        RefreshContentRectTransform(true);
        RefreshSelectedStat();

        slider.value = index;
    }

    public void OnAddGlitch()
    {
        if (!statisticsBoard.activeInHierarchy) return;
        if (locked[index].activeInHierarchy) return;
        if (references.GetGlitchDisplay().GetGlitchAmount() == 0) return;

        StatsSettings statsSettings = stats[index].GetComponent<StatsSettings>();

        if (statsSettings.IsLevelMax()) return;

        statsSettings.AddGlitch(statsSO.GetLevelCost(statsSettings.GetStatIndex(), statsSettings.GetStatLevel()));

        references.GetGlitchDisplay().RemoveGlitch(1);

        RefreshStatsInfo(index);

        selectionAS.Play();

        StartCoroutine(FastAddGlitch(statsSettings));

        CheckIfAllStatsAtLevelMax();
    }

    private IEnumerator FastAddGlitch(StatsSettings statsSettings)
    {
        float timeBetweenAdd = 0.1f;

        timeSinceAddGlitch = 0f;

        yield return new WaitUntil(() => timeSinceAddGlitch > 0.8f);

        while (true)
        {
            if (statsSettings.IsLevelMax() || references.GetGlitchDisplay().GetGlitchAmount() == 0)
            {
                break;
            }

            statsSettings.AddGlitch(statsSO.GetLevelCost(statsSettings.GetStatIndex(), statsSettings.GetStatLevel()));

            references.GetGlitchDisplay().RemoveGlitch(1);

            RefreshStatsInfo(index);

            yield return new WaitForSeconds(timeBetweenAdd);

            if (timeBetweenAdd > 0.02f)
            {
                timeBetweenAdd -= Time.deltaTime;
            }
        }
    }

    private void OnReleaseAddGlitch()
    {
        if (!statisticsBoard.activeInHierarchy) return;

        StopAllCoroutines();
    }

    private void CheckIfAllStatsAtLevelMax()
    {
        int index = 0;

        foreach (var stat in stats)
        {
            if (stat.GetComponent<StatsSettings>().IsLevelMax())
            {
                index++;
            }
        }
        
        if (index == stats.Length && references.GetGameManager().GetNoobMode())
        {
            //Don't use CheckAchievement() because the condition above have to be true
            SteamUserStats.GetAchievement("ACH_ALL_STATS_MAX", out bool achievement2Unlock);

            if (!achievement2Unlock)
            {
                FindObjectOfType<Achievements>().SetAchievement("ACH_ALL_STATS_MAX");

                SteamUserStats.SetAchievement("ACH_ALL_STATS_MAX");
                SteamUserStats.StoreStats();
            }
        }
    }

    private void RefreshContentRectTransform(bool up)
    {
        if (!up)
        {
            if (index >= 3)
            {
                if (index > stats.Length - 3) return;

                contentRectTransform.offsetMax -= new Vector2(0, -statisticsBoardOffset);
            }
            else
            {
                contentRectTransform.offsetMax = Vector2.zero;
            }
        }
        else
        {
            if (index < stats.Length - 3)
            {
                if (index <= 1) return;

                contentRectTransform.offsetMax -= new Vector2(0, statisticsBoardOffset);
            }
            else
            {
                contentRectTransform.offsetMax = new Vector2(0, (stats.Length - 5) * statisticsBoardOffset);
            }
        }
    }

    private void RefreshUI()
    {
        if (!keyboardEscapeImg.activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(true);
            controllerEscapeImg.SetActive(false);
        }
        else if (!controllerEscapeImg.activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            keyboardEscapeImg.SetActive(false);
            controllerEscapeImg.SetActive(true);
        }

        if (isSwitchingSprite) return;

        if (references.GetGameManager().GetChangeUI())
        {
            escapeKeyImg[1].sprite = escapeKeySprites[4];
        }
        else
        {
            escapeKeyImg[1].sprite = escapeKeySprites[2];
        }
    }

    private void EnableStats()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            StatsSettings statsSettings = stats[i].GetComponent<StatsSettings>();

            if (statsSettings.GetExperienceToUnlocked() > references.GetExperience().GetCurrentExperience() 
                || references.GetGameManager().GetSimulationReccord() < statsSettings.GetSimulationReccordToUnlock())
            {
                stats[i].transform.GetChild(1).gameObject.SetActive(false);

                locked[i].SetActive(true);

                if (statsSettings.GetSimulationReccordToUnlock() <= 1)
                {
                    if (statsSettings.GetSimulationReccordToUnlock() == 0)
                    {
                        locked[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked (Need : <b><color=#00cdb9>" 
                            + statsSettings.GetExperienceToUnlocked() + "</b></color> exp)";
                    }
                    else
                    {
                        locked[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked (Need : <b><color=#00cdb9>" + statsSettings.GetExperienceToUnlocked()
                        + "</b></color> exp & simulation <b><color=#F79824>" + statsSettings.GetSimulationReccordToUnlock() + "</color></b>)";
                    }
                }
                else
                {
                    locked[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked (Need : <b><color=#00cdb9>" + statsSettings.GetExperienceToUnlocked() 
                        + "</b></color> exp & simulations <b><color=#F79824>" + statsSettings.GetSimulationReccordToUnlock() + "</color></b>)";
                }
            }
            else
            {
                locked[i].SetActive(false);

                if (i == stats.Length - 1)
                {
                    CheckAchievement();
                }
            }
        }
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement("ACH_UNLOCK_ALL_STATS", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                references.GetAchievements().SetAchievement("ACH_UNLOCK_ALL_STATS");

                SteamUserStats.SetAchievement("ACH_UNLOCK_ALL_STATS");
            }

            SteamUserStats.StoreStats();
        }
    }

    public IEnumerator EnableStatisticsBoard(bool state)
    {
        if (state)
        {
            yield return pressKey.GetComponent<PressKey>().SwitchKeySprites();
        }

        if (state)
        {
            pressKey.SetActive(false);
        }
        else
        {
            pressKey.SetActive(true);
        }

        references.GetPlayerController().disableControl = state;
        references.GetPlayerController().SetIsInActivity(state);

        references.GetPlayerController().StopPlayer(true);

        statisticsBoard.SetActive(state);

        RefreshStatsInfo(index);
        RefreshSelectedStat();
    }

    private void RefreshSelectedStat()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (index == i)
            {
                stats[i].GetComponent<StatsSettings>().EnableArrow();

                RefreshInformationPanel(i);
            }
            else
            {
                stats[i].GetComponent<StatsSettings>().DisableArrow();
            }
        }
    }

    private void RefreshInformationPanel(int index)
    {
        StatsSettings statsSettings = stats[index].GetComponent<StatsSettings>();

        informationPanelTxt[0].text = statsSettings.GetStatDefinition();

        if (statsSettings.GetExperienceToUnlocked() > references.GetExperience().GetCurrentExperience())
        {
            informationPanel.SetActive(false);

            return;
        }
        else
        {
            informationPanel.SetActive(true);
        }

        if (statsSettings.IsOneLevelStat())
        {
            informationPanelTxt[1].gameObject.SetActive(false);

            if (statsSettings.IsLevelMax())
            {
                informationPanelTxt[2].gameObject.SetActive(false);
            }
            else
            {
                int upgradePrice = statsSO.GetLevelCost(statsSettings.GetStatIndex(), statsSettings.GetStatLevel());
                informationPanelTxt[2].text = "Price : <b><color=#00cdb9>" + upgradePrice.ToString() + "</color></b> Glitchs";
            }
        }
        else
        {
            informationPanelTxt[1].gameObject.SetActive(true);
            

            if (statsSettings.IsLevelMax())
            {
                informationPanelTxt[1].text = "Bonus : <b><color=#00cdb9>" + statsSettings.GetCurrentStatBonus() + "</color></b>";

                informationPanelTxt[2].gameObject.SetActive(false);
            }
            else
            {
                informationPanelTxt[1].text = "Bonus : <b><color=#00cdb9>" + statsSettings.GetCurrentStatBonus() 
                    + "</color> (<color=#00cdb9>" + statsSettings.GetNextStatBonus() + "</color>)</b>";

                int upgradePrice = statsSO.GetLevelCost(statsSettings.GetStatIndex(), statsSettings.GetStatLevel());
                informationPanelTxt[2].text = "Price : <b><color=#00cdb9>" + upgradePrice.ToString() + "</color></b> Glitchs";
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(informationPanelRectTransform);
    }

    private void RefreshStatsInfo(int index)
    {
        foreach (GameObject stat in stats)
        {
            if (!stat.activeInHierarchy) continue;

            StatsSettings statsSettings = stat.GetComponent<StatsSettings>();
            ProgressionBar progressionBar = statsSettings.GetProgressionBar();

            if (statsSettings.IsLevelMax()) continue;

            int upgradePrice = statsSO.GetLevelCost(statsSettings.GetStatIndex(), statsSettings.GetStatLevel());

            progressionBar.SetProgressionValue(upgradePrice, statsSettings.GetCurrentGlitchAmount());
        }

        RefreshInformationPanel(index);
    }

    private void DisplayCurrentExperience()
    {
        experienceTxt.text = "Experience : " + "<color=#00cdb9>" + FindObjectOfType<Experience>().GetCurrentExperience() + "</color>";
    }

    private IEnumerator SwitchKeySprite()
    {
        isSwitchingSprite = true;

        if (keyboardEscapeImg.activeInHierarchy)
        {
            escapeKeyImg[0].sprite = escapeKeySprites[1];

            yield return new WaitForSeconds(timeBetweenKeySwap);

            escapeKeyImg[0].sprite = escapeKeySprites[0];
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                escapeKeyImg[1].sprite = escapeKeySprites[5];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                escapeKeyImg[1].sprite = escapeKeySprites[4];
            }
            else
            {
                escapeKeyImg[1].sprite = escapeKeySprites[3];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                escapeKeyImg[1].sprite = escapeKeySprites[2];
            }
        }

        foreach (GameObject stat in stats)
        {
            stat.SetActive(true);
        }

        references.GetPlayerStatistics().SaveAllStatsChanges();

        EnableStats();

        isSwitchingSprite = false;

        StartCoroutine(EnableStatisticsBoard(false));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pressKey.SetActive(false);
        }
    }
}

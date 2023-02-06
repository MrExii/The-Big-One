using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;
using Steamworks;

public class SimulationBoard : MonoBehaviour
{
    [SerializeField] GameObject pressKey;
    [SerializeField] GameObject simulationBoard;
    [SerializeField] GameObject[] simulations;
    [SerializeField] GameObject[] locked;
    [SerializeField] TextMeshProUGUI gameDifficultyTxt;
    [SerializeField] RectTransform informationPanelRectTransform;
    [SerializeField] TextMeshProUGUI informationPanelTxt;
    [SerializeField] GameObject[] launchSimulationPanel;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] RectTransform contentRectTransform;
    [SerializeField] GameObject keyboardEscapeImg;
    [SerializeField] GameObject controllerEscapeImg;
    [SerializeField] TextMeshProUGUI winStreakTxt;

    [Header("Noob Mode")]
    [SerializeField] GameObject[] noobModePanel;
    [SerializeField] Image[] noobPanelKeyImg;
    [SerializeField] Sprite[] noobPanelKeySprites;

    [Header("Images Settings")]
    [SerializeField] Image[] keyToStartGameImg;
    [SerializeField] Sprite[] keyToStartGameSprites;
    [SerializeField] Image[] escapeKeyImg;
    [SerializeField] Sprite[] escapeKeySprites;

    CatchReferences references;
    Slider slider;
    PlayerInputSystem inputActions;

    int index;
    int gameDifficulty;

    bool isSwitchKeySprite;

    const float simulationBoardOffset = 120f;
    const float timeBetweenKeySwap = 0.07f;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        slider = GetComponentInChildren<Slider>();

        inputActions = new();
        inputActions.UISimuBoard.OpenBoard.performed += ctx => OnEnableBoard();
        inputActions.UISimuBoard.CloseBoard.performed += ctx => OnDisableBoard();
        inputActions.UISimuBoard.DownArrow.performed += ctx => OnGoDown();
        inputActions.UISimuBoard.UpArrow.performed += ctx => OnGoUp();
        inputActions.UISimuBoard.LeftArrow.performed += ctx => OnDisableSimulation();
        inputActions.UISimuBoard.RightArrow.performed += ctx => OnEnableSimulation();
        inputActions.UISimuBoard.NoobMode.performed += ctx => OnEnableNoobMode();
        inputActions.UISimuBoard.LaunchGame.performed += OnLaunchGame;

        inputActions.UISimuBoard.Enable();
    }

    private void OnDestroy()
    {
        inputActions.UISimuBoard.Disable();
    }

    private void Start()
    {
        ResfreshGameDifficulty();
        RefreshSelectedSimulation();
        RefreshNoobModePanel();
        RefreshWinStreak();

        EnableSimulations();

        simulationBoard.SetActive(false);

        launchSimulationPanel[0].SetActive(false);
        launchSimulationPanel[1].SetActive(false);

        noobModePanel[0].SetActive(false);
        noobModePanel[1].SetActive(false);

        controllerEscapeImg.SetActive(false);

        pressKey.SetActive(false);
    }

    private void Update()
    {
        RefreshUI();
    }

    private void OnLaunchGame(InputAction.CallbackContext context)
    {
        if (!simulationBoard.activeInHierarchy) return;

        selectionAS.Play();

        selectionAS.Play();

        StartCoroutine(SwitchKeySprite("Start"));
    }

    public void OnEnableBoard()
    {
        if (!simulationBoard.activeInHierarchy && pressKey.activeInHierarchy && !references.GetPauseMenu().GetInPauseMenu())
        {
            selectionAS.Play();

            StartCoroutine(EnableSimulationBoard(true));
        }
    }

    public void OnDisableBoard()
    {
        if (!simulationBoard.activeInHierarchy) return;

        selectionAS.Play();

        StartCoroutine(SwitchKeySprite("Escape"));
    }

    public void OnGoDown()
    {
        if (!simulationBoard.activeInHierarchy) return;

        index++;

        if (index == simulations.Length)
        {
            index = 0;
        }

        selectionAS.Play();

        RefreshSelectedSimulation();
        RefreshContentRectTransform(false);

        slider.value = index;
    }

    public void OnGoUp()
    {
        if (!simulationBoard.activeInHierarchy) return;

        index--;

        if (index < 0)
        {
            index = simulations.Length - 1;
        }

        selectionAS.Play();

        RefreshSelectedSimulation();
        RefreshContentRectTransform(true);

        slider.value = index;
    }

    public void OnEnableSimulation()
    {
        if (!simulationBoard.activeInHierarchy) return;
        if (locked[index].activeInHierarchy) return;
        if (simulations[index].GetComponent<SimulationSettings>().IsSimulationLevelMax()) return;

        simulations[index].GetComponent<SimulationSettings>().AddSimulationLevel();

        ResfreshGameDifficulty();

        selectionAS.Play();
    }

    public void OnDisableSimulation()
    {
        if (!simulationBoard.activeInHierarchy) return;
        if (locked[index].activeInHierarchy) return;
        if (simulations[index].GetComponent<SimulationSettings>().IsSimulationLevelMin()) return;

        simulations[index].GetComponent<SimulationSettings>().RemoveSimulationLevel();

        ResfreshGameDifficulty();

        selectionAS.Play();
    }

    public void OnEnableNoobMode()
    {
        if (!simulationBoard.activeInHierarchy) return;

        references.GetGameManager().ChangeNoobMode();

        RefreshNoobModePanel();

        selectionAS.Play();

        StartCoroutine(SwitchKeySprite("noob mode"));
    }

    private void RefreshContentRectTransform(bool up)
    {
        if (!up)
        {
            if (index >= 4)
            {
                if (index > simulations.Length - 4) return;

                contentRectTransform.offsetMax -= new Vector2(0, -simulationBoardOffset);
            }
            else
            {
                contentRectTransform.offsetMax = Vector2.zero;
            }
        }
        else
        {
            if (index < simulations.Length - 4)
            {
                if (index <= 2) return;

                contentRectTransform.offsetMax -= new Vector2(0, simulationBoardOffset);
            }
            else
            {
                contentRectTransform.offsetMax = new Vector2(0, (simulations.Length - 7) * simulationBoardOffset);
            }
        }
    }

    private void RefreshUI()
    {
        if (!keyboardEscapeImg.activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            launchSimulationPanel[0].SetActive(true);
            launchSimulationPanel[1].SetActive(false);

            noobModePanel[0].SetActive(true);
            noobModePanel[1].SetActive(false);

            keyboardEscapeImg.SetActive(true);
            controllerEscapeImg.SetActive(false);
        }
        else if (!controllerEscapeImg.activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            launchSimulationPanel[0].SetActive(false);
            launchSimulationPanel[1].SetActive(true);

            noobModePanel[0].SetActive(false);
            noobModePanel[1].SetActive(true);

            keyboardEscapeImg.SetActive(false);
            controllerEscapeImg.SetActive(true);
        }

        if (references.GetGameManager().GetChangeUI() && !isSwitchKeySprite)
        {
            keyToStartGameImg[1].sprite = keyToStartGameSprites[4];
            escapeKeyImg[1].sprite = escapeKeySprites[4];
            noobPanelKeyImg[1].sprite = noobPanelKeySprites[4];
        }
        else if (!isSwitchKeySprite)
        {
            keyToStartGameImg[1].sprite = keyToStartGameSprites[2];
            escapeKeyImg[1].sprite = escapeKeySprites[2];
            noobPanelKeyImg[1].sprite = noobPanelKeySprites[2];
        }
    }

    public IEnumerator EnableSimulationBoard(bool state)
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

        isSwitchKeySprite = false;

        if (state == true)
        {
            ResfreshGameDifficulty();
        }

        references.GetPlayerController().StopPlayer(true);

        simulationBoard.SetActive(state);
    }

    private void RefreshSelectedSimulation()
    {
        for (int i = 0; i < simulations.Length; i++)
        {
            if (index == i)
            {
                simulations[i].GetComponent<SimulationSettings>().EnableArrows();

                RefreshInformationPanel(i);
            }
            else
            {
                simulations[i].GetComponent<SimulationSettings>().DisableArrows();
            }
        }
    }

    private void RefreshInformationPanel(int index)
    {
        SimulationSettings simulationSettings = simulations[index].GetComponent<SimulationSettings>();

        if (simulationSettings.GetSimulationReccordToUnlocked() > references.GetGameManager().GetSimulationReccord())
        {
            informationPanelRectTransform.gameObject.SetActive(false);
        }
        else
        {
            informationPanelRectTransform.gameObject.SetActive(true);
        }

        informationPanelTxt.text = simulationSettings.GetSimulationDefinition();

        LayoutRebuilder.ForceRebuildLayoutImmediate(informationPanelRectTransform);
    }

    private void RefreshNoobModePanel()
    {
        if (references.GetGameManager().GetKeyboardControl())
        {
            if (references.GetGameManager().GetNoobMode())
            {
                noobModePanel[0].GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.9686275f, 0.5960785f, 0.1411765f);
            }
            else
            {
                noobModePanel[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
        else
        {
            if (references.GetGameManager().GetNoobMode())
            {
                noobModePanel[1].GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.9686275f, 0.5960785f, 0.1411765f);
            }
            else
            {
                noobModePanel[1].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    private void RefreshWinStreak()
    {
        winStreakTxt.text = references.GetGameManager().GetWinStreak().ToString();
    }

    private void ResfreshGameDifficulty()
    {
        gameDifficulty = 0;

        foreach (GameObject simulation in simulations)
        {
            gameDifficulty += simulation.GetComponent<SimulationSettings>().GetCurrentSimulationLevel();
        }

        gameDifficultyTxt.text = "<b>" + gameDifficulty.ToString() + " - <color=#F79824>" + references.GetGameManager().GetSimulationReccord() + "</color></b>";
    }

    private void EnableSimulations()
    {
        for (int i = 0; i < simulations.Length; i++)
        {
            SimulationSettings simulationSettings = simulations[i].GetComponent<SimulationSettings>();

            if (simulationSettings.GetSimulationReccordToUnlocked() > references.GetGameManager().GetSimulationReccord())
            {
                simulations[i].transform.GetChild(1).gameObject.SetActive(false);
                locked[i].SetActive(true);

                if (simulationSettings.GetSimulationReccordToUnlocked() <= 1)
                {
                    locked[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked (Need : simulation <b><color=#F79824>"
                        + simulationSettings.GetSimulationReccordToUnlocked() + "</b></color>)";
                }
                else
                {
                    locked[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked (Need : simulations <b><color=#F79824>"
                        + simulationSettings.GetSimulationReccordToUnlocked() + "</b></color>)";
                }
            }
            else
            {
                simulations[i].transform.GetChild(1).gameObject.SetActive(true);
                locked[i].SetActive(false);

                if (i == simulations.Length - 1)
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
            SteamUserStats.GetAchievement("ACH_UNLOCK_ALL_SIMULATIONS", out bool achievementUnlock);

            if (!achievementUnlock)
            {
                FindObjectOfType<Achievements>().SetAchievement("ACH_UNLOCK_ALL_SIMULATIONS");

                SteamUserStats.SetAchievement("ACH_UNLOCK_ALL_SIMULATIONS");
                SteamUserStats.StoreStats();
            }
        }
    }

    private IEnumerator SwitchKeySprite(string index)
    {
        isSwitchKeySprite = true;

        if (index == "Escape")
        {
            if (references.GetGameManager().GetKeyboardControl())
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

            StartCoroutine(EnableSimulationBoard(false));
        }
        else if (index == "Start")
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyToStartGameImg[0].sprite = keyToStartGameSprites[1];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToStartGameImg[0].sprite = keyToStartGameSprites[0];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    keyToStartGameImg[1].sprite = keyToStartGameSprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToStartGameImg[1].sprite = keyToStartGameSprites[4];
                }
                else
                {
                    keyToStartGameImg[1].sprite = keyToStartGameSprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToStartGameImg[1].sprite = keyToStartGameSprites[2];
                }
            }

            StartGame();
        }
        else if (index == "noob mode")
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                noobPanelKeyImg[0].sprite = noobPanelKeySprites[1];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                noobPanelKeyImg[0].sprite = noobPanelKeySprites[0];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    noobPanelKeyImg[1].sprite = noobPanelKeySprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    noobPanelKeyImg[1].sprite = noobPanelKeySprites[4];
                }
                else
                {
                    noobPanelKeyImg[1].sprite = noobPanelKeySprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    noobPanelKeyImg[1].sprite = noobPanelKeySprites[2];
                }
            }
        }

        isSwitchKeySprite = false;
    }

    private void StartGame()
    {
        references.GetSimulationsPlaceHolder().SaveAllSimulationsSettings();

        StartCoroutine(EnableSimulationBoard(false));

        references.GetGameManager().SetCurrentSimulationDifficulty(gameDifficulty);

        StartCoroutine(references.GetGameManager().StartGame());
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

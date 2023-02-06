using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SimulationSettings : MonoBehaviour, ISaveable
{
    [SerializeField] SimulationIndex simulationIndex;
    [SerializeField] string simulationName;
    [SerializeField] TextMeshProUGUI simulationNameTxt;
    [SerializeField] TextMeshProUGUI penalityTxt;
    [SerializeField] [TextArea] string simulationDefinition;
    [SerializeField] string[] simulationsPenalties;
    [SerializeField] bool oneLevelSimulation;
    [SerializeField] int simulationReccordToUnlocked;
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundSprites;
    [SerializeField] Image notSelected;

    [Header("Selection Key Settings")]
    [SerializeField] Image keyToEnable;
    [SerializeField] Image keyToDisable;
    [SerializeField] Sprite[] rightArrow;
    [SerializeField] Sprite[] leftArrow;

    GameManager gameManager;

    int currentSimulationeLevel;
    const float timeBetweenKeySwap = 0.07f;
    bool isSwapKeySprite;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        ResfreshSimulationPenalty();
    }

    private void Start()
    {
        if (oneLevelSimulation)
        {
            penalityTxt.enabled = false;
        }
        else
        {
            penalityTxt.enabled = true;
        }
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
            keyToEnable.sprite = rightArrow[0];
            keyToDisable.sprite = leftArrow[0];
        }
        else if (!gameManager.GetKeyboardControl())
        {
            if (gameManager.GetChangeUI())
            {
                keyToEnable.sprite = rightArrow[4];
                keyToDisable.sprite = leftArrow[4];
            }
            else
            {
                keyToEnable.sprite = rightArrow[2];
                keyToDisable.sprite = leftArrow[2];
            }
        }
    }

    public void AddSimulationLevel()
    {
        if (oneLevelSimulation && currentSimulationeLevel == 1) return; 
        if (!oneLevelSimulation && currentSimulationeLevel == simulationsPenalties.Length - 1) return;
        
        currentSimulationeLevel++;

        if (simulationIndex == SimulationIndex.ActivityTimerMalus)
        {
            foreach (SimulationSettings item in FindObjectsOfType<SimulationSettings>())
            {
                if (item.simulationIndex == SimulationIndex.EnableTimerActivity)
                {
                    item.AddSimulationLevel();
                }
            }
        } 
        else if (simulationIndex == SimulationIndex.HeartStalkerActivityBonus || simulationIndex == SimulationIndex.HeartStalkerTimer)
        {
            foreach (SimulationSettings item in FindObjectsOfType<SimulationSettings>())
            {
                if (item.simulationIndex == SimulationIndex.EnableHeartStalker)
                {
                    item.AddSimulationLevel();
                }
            }
        }

        ResfreshSimulationPenalty();

        StartCoroutine(SwitchKeySprite(true));
    }

    public void RemoveSimulationLevel()
    {
        if (currentSimulationeLevel == 0) return;

        if (simulationIndex != SimulationIndex.TheEnd)
        {
            foreach (SimulationSettings item in FindObjectsOfType<SimulationSettings>())
            {
                if (item.simulationIndex == SimulationIndex.TheEnd)
                {
                    item.RemoveSimulationLevel();
                }
            }
        }

        currentSimulationeLevel--;

        ResfreshSimulationPenalty();

        StartCoroutine(SwitchKeySprite(false));
    }

    private void ResfreshSimulationPenalty()
    {
        if (!oneLevelSimulation)
        {
            penalityTxt.text = simulationsPenalties[currentSimulationeLevel].ToString();

            if (currentSimulationeLevel > 0)
            {
                simulationNameTxt.text = "<color=#F79824>" + simulationName + "</color>";
            }
            else
            {
                simulationNameTxt.text = "<color=#FFFFFF>" + simulationName + "</color>";
            }
        }
        else
        {
            if (currentSimulationeLevel == 1)
            {
                simulationNameTxt.text = "<color=#F79824>" + simulationName + "</color>";
            }
            else
            {
                simulationNameTxt.text = "<color=#FFFFFF>" + simulationName + "</color>";
            }
        }
    }

    public void DisableArrows()
    {
        keyToEnable.enabled = false;
        keyToDisable.enabled = false;

        notSelected.enabled = true;

        background.sprite = backgroundSprites[0];
    }

    public void EnableArrows()
    {
        keyToEnable.enabled = true;
        keyToDisable.enabled = true;

        notSelected.enabled = false;

        background.sprite = backgroundSprites[1];
    }

    private IEnumerator SwitchKeySprite(bool isRightArrow)
    {
        isSwapKeySprite = true;

        if (isRightArrow)
        {
            if (gameManager.GetKeyboardControl())
            {
                keyToEnable.sprite = rightArrow[1];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToEnable.sprite = rightArrow[0];
            }
            else
            {
                if (gameManager.GetChangeUI())
                {
                    keyToEnable.sprite = rightArrow[5];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToEnable.sprite = rightArrow[4];
                }
                else
                {
                    keyToEnable.sprite = rightArrow[3];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToEnable.sprite = rightArrow[2];
                }
            }
        }
        else
        {
            if (gameManager.GetKeyboardControl())
            {
                keyToDisable.sprite = leftArrow[1];

                yield return new WaitForSeconds(timeBetweenKeySwap);

                keyToDisable.sprite = leftArrow[0];
            }
            else
            {
                if (gameManager.GetChangeUI())
                {
                    keyToDisable.sprite = leftArrow[5];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToDisable.sprite = leftArrow[4];
                }
                else
                {
                    keyToDisable.sprite = leftArrow[3];

                    yield return new WaitForSeconds(timeBetweenKeySwap);

                    keyToDisable.sprite = leftArrow[2];
                }
            }
        }

        isSwapKeySprite = false;
    }

    public string GetSimulationDefinition()
    {
        return simulationDefinition;
    }

    public int GetSimulationReccordToUnlocked()
    {
        return simulationReccordToUnlocked;
    }

    public int GetCurrentSimulationLevel()
    {
        return currentSimulationeLevel;
    }

    public bool IsSimulationLevelMax()
    {
        if (currentSimulationeLevel == simulationsPenalties.Length - 1 || currentSimulationeLevel == 1 && oneLevelSimulation) return true;

        return false;
    }

    public bool IsSimulationLevelMin()
    {
        if (currentSimulationeLevel == 0) return true;

        return false;
    }

    public SimulationIndex GetSimulationIndex()
    {
        return simulationIndex;
    }

    public object CaptureState()
    {
        return currentSimulationeLevel;
    }

    public void RestoreState(object state)
    {
        currentSimulationeLevel = (int)state;
    }
}

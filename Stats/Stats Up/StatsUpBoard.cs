using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatsUpBoard : MonoBehaviour
{
    [Serializable]
    private class PropositionSettings
    {
        public StatsUpIndex statsUpIndex;
        public float normalValue;
        public float epicValue;
        public float chanceForEpic;
        [HideInInspector] public bool isEpicValue;
        [TextArea] public string propositionTxt;
        public bool showIt; //Only for debug
    }

    [Serializable]
    private class StatsPool
    {
        public StatsUpPool statsUpPool;
        public PropositionSettings[] propositionSettings;
    }

    [SerializeField] GameObject statsBoard;
    [SerializeField] StatsPool[] statsPool;
    [SerializeField] GameObject[] propositions;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] AudioSource takenAS;
    [SerializeField] Image[] keyToPressImg;
    [SerializeField] Sprite[] keyToPressSprites;
    [SerializeField] Image[] panelImg;
    [SerializeField] Sprite[] panelSprite;
    [SerializeField] RectTransform statsUpRectTransform;
    [SerializeField] Image[] notSelected;
    [SerializeField] GameObject[] keyToClose;
    [SerializeField] Image[] keyToCloseImgs;
    [SerializeField] Sprite[] keyToCloseSprites;
    [SerializeField] TextInfoSpawner playerTextInfoSpawner;
    [SerializeField] GameObject statsUpIndication;
    [SerializeField] GameObject[] keyToOpenBoard;
    [SerializeField] Image[] keyToOpenBoardImgs;
    [SerializeField] Sprite[] keyToOpenBoardSprites;
    [SerializeField] bool cantUseStatsUpBoard;
    [SerializeField] bool tutoStatsUp;

    CatchReferences references;
    PlayerInputSystem inputActions;

    int selectionIndex;
    float currentChanceForStatsUp = 5;
    const float timeBetweenKeySpriteSwap = 0.07f;
    bool isSwapKeySprite;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.Dialogues.CancelDialogue.performed += ctx => OnCancelDialogue();
        inputActions.Dialogues.AcceptProposition.performed += ctx => OnAcceptProposition();
        inputActions.Dialogues.NavDown.performed += ctx => OnNavDown();
        inputActions.Dialogues.NavUp.performed += ctx => OnNavUp();
        inputActions.Dialogues.OpenDialogues.performed += ctx => OnOpenDialogues();
    }

    private void OnDestroy()
    {
        inputActions.Dialogues.Disable();
    }

    private void Start()
    {
        EnableStatsUpBoard(false);
    }

    private void Update()
    {
        if (cantUseStatsUpBoard) return;

        if (references.GetRoomPool())
        {
            if (!references.GetRoomPool().GetCurrentRoom().GetCanExitRoom() && statsUpIndication.activeInHierarchy)
            {
                statsUpIndication.SetActive(false);
                return;
            }
        }
        else
        {
            if (!references.GetRoomPoolTest().GetCurrentRoom().GetCanExitRoom() && statsUpIndication.activeInHierarchy)
            {
                statsUpIndication.SetActive(false);
                return;
            }
        }

        RefreshUI();
    }

    private void SetupPropositions()
    {
        int lastPropositionIndex = -1;
        int lastStatPoolIndex = -1;

        for (int i = 0; i < propositions.Length; i++)
        {
            string propositionValueTxt;

            int statPoolIndex;
            int propositionIndex;
            int epicChance = UnityEngine.Random.Range(1, 101);

            while (true)
            {
                statPoolIndex = PickStatPool();

                propositionIndex = UnityEngine.Random.Range(0, statsPool[statPoolIndex].propositionSettings.Length);

                if (statPoolIndex == lastStatPoolIndex)
                {
                    while (lastPropositionIndex == propositionIndex)
                    {
                        propositionIndex = UnityEngine.Random.Range(0, statsPool[statPoolIndex].propositionSettings.Length);
                    }
                }

                if ((statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.DamageForPassive ||
                    statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.HealthForPassive)
                    && !references.GetPassiveObject().CanDeleteSlot()) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.DivideTrinketCharge
                    && references.GetTrinket().GetCurrentTrinket() == TrinketIndex.None) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.RegainArmor
                    && references.GetPlayerStatistics().GetArmor() < 1) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.RemoveCurse
                    && references.GetCurse().GetCurrentCurse() == 0) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.Heal
                    && references.GetPlayerHealth().GetCurrentHealth() == references.GetPlayerStatistics().GetBaseHealth()) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.CantLoseNextKeySeries
                    && references.GetGameManager().GetCantLoseNextKeySeries()) continue;

                else if ((statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.ArmorForDamage
                    || statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.ArmorForHealth)
                    && references.GetPlayerStatistics().GetArmor() < 1) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.AddPassiveSlot
                    && references.GetPassiveDisplay().GetPassiveSlots().Count == 5) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.CantLoseAllKeySeries
                    && references.GetGameManager().GetCantLoseAllKeySeries()) continue;

                else if ((statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.HealthForRoomsCleared
                    || statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.DamageForRoomsCleared)
                    && references.GetSector().GetRoomsCleared() < references.GetSector().GetRoomsInTotal() / 2) continue;

                else if ((statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.HealthForRoomsRemaining
                    || statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.DamageForRoomsRemaining)
                    && references.GetSector().GetRoomsCleared() > references.GetSector().GetRoomsInTotal() / 2) continue;

                else if (statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex == StatsUpIndex.RemoveSimulation
                    && references.GetGameManager().GetCurrentSimulationDifficulty() == 0) continue;
                
                break;
            }

            lastPropositionIndex = propositionIndex;
            lastStatPoolIndex = statPoolIndex;

            if (i == 0)
            {
                for (int y = 0; y < statsPool.Length; y++)
                {
                    for (int z = 0; z < statsPool[y].propositionSettings.Length; z++)
                    {
                        if (statsPool[y].propositionSettings[z].showIt)
                        {
                            propositionIndex = z;
                            lastPropositionIndex = z;

                            statPoolIndex = y;
                            lastStatPoolIndex = y;
                        }
                    }
                }
            }

            if (epicChance <= statsPool[statPoolIndex].propositionSettings[propositionIndex].chanceForEpic)
            {
                propositionValueTxt = "<color=#D65108>+" + statsPool[statPoolIndex].propositionSettings[propositionIndex].epicValue.ToString() + "</color>";

                statsPool[statPoolIndex].propositionSettings[propositionIndex].isEpicValue = true;
            }
            else
            {
                propositionValueTxt = "<color=#3152A6>+" + statsPool[statPoolIndex].propositionSettings[propositionIndex].normalValue.ToString() + "</color>";

                statsPool[statPoolIndex].propositionSettings[propositionIndex].isEpicValue = false;
            }

            propositions[i].name = statsPool[statPoolIndex].propositionSettings[propositionIndex].statsUpIndex.ToString();

            TextMeshProUGUI propositionTxt = propositions[i].GetComponentInChildren<TextMeshProUGUI>();
            propositionTxt.text = statsPool[statPoolIndex].propositionSettings[propositionIndex].propositionTxt.Replace("N/A", propositionValueTxt);

            propositionTxt.text += "<br>" + PickPoolName(statPoolIndex);
        }
    }

    private int PickStatPool()
    {
        if (tutoStatsUp)
        {
            return 0;
        }

        int statPool;
        int poolIndex = UnityEngine.Random.Range(1, 101);

        if (poolIndex <= 50)
        {
            statPool = 0;
        }
        else if (poolIndex <= 80)
        {
            statPool = 1;
        }
        else if (poolIndex <= 95)
        {
            statPool = 2;
        }
        else
        {
            statPool = 3;
        }

        return statPool;
    }

    private string PickPoolName(int statPoolIndex)
    {
        string poolName = "";

        if (statPoolIndex == 0)
        {
            poolName += "(Normal)";
        }
        else if (statPoolIndex == 1)
        {
            poolName += "(<color=#3152A6>Rare</color>)";
        }
        else if (statPoolIndex == 2)
        {
            poolName += "(<color=#6B1380>Epic</color>)";
        }
        else
        {
            poolName += "(<color=#D65108>Legendary</color>)";
        }

        return poolName;
    }

    private void RefreshUI()
    {
        if (isSwapKeySprite) return;

        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].gameObject.SetActive(true);
            keyToPressImg[1].gameObject.SetActive(true);

            keyToPressImg[2].gameObject.SetActive(false);
            keyToPressImg[3].gameObject.SetActive(false);

            keyToClose[0].SetActive(true);
            keyToClose[1].SetActive(false);

            keyToOpenBoard[0].SetActive(true);
            keyToOpenBoard[1].SetActive(false);
        }
        else
        {
            keyToPressImg[0].gameObject.SetActive(false);
            keyToPressImg[1].gameObject.SetActive(false);

            keyToPressImg[2].gameObject.SetActive(true);
            keyToPressImg[3].gameObject.SetActive(true);

            keyToClose[0].SetActive(false);
            keyToClose[1].SetActive(true);

            keyToOpenBoard[0].SetActive(false);
            keyToOpenBoard[1].SetActive(true);

            if (references.GetGameManager().GetChangeUI())
            {
                keyToCloseImgs[1].sprite = keyToCloseSprites[4];

                keyToOpenBoardImgs[1].sprite = keyToOpenBoardSprites[1];
            }
            else
            {
                keyToCloseImgs[1].sprite = keyToCloseSprites[2];

                keyToOpenBoardImgs[1].sprite = keyToOpenBoardSprites[0];
            }
        }

        RefreshSelectedProposition();

        LayoutRebuilder.ForceRebuildLayoutImmediate(statsUpRectTransform);
    }

    private void RefreshSelectedProposition()
    {
        for (int i = 0; i < propositions.Length; i++)
        {
            if (selectionIndex == i)
            {
                if (references.GetGameManager().GetKeyboardControl())
                {
                    propositions[i].transform.GetChild(1).GetComponent<Image>().sprite = keyToPressSprites[0];
                }
                else
                {
                    if (references.GetGameManager().GetChangeUI())
                    {
                        propositions[i].transform.GetChild(2).GetComponent<Image>().sprite = keyToPressSprites[4];
                    }
                    else
                    {
                        propositions[i].transform.GetChild(2).GetComponent<Image>().sprite = keyToPressSprites[2];
                    }
                }

                panelImg[i].sprite = panelSprite[0];
                notSelected[i].enabled = false;
            }
            else
            {
                if (references.GetGameManager().GetKeyboardControl())
                {
                    propositions[i].transform.GetChild(1).GetComponent<Image>().sprite = keyToPressSprites[1];
                }
                else
                {
                    if (references.GetGameManager().GetChangeUI())
                    {
                        propositions[i].transform.GetChild(2).GetComponent<Image>().sprite = keyToPressSprites[5];
                    }
                    else
                    {
                        propositions[i].transform.GetChild(2).GetComponent<Image>().sprite = keyToPressSprites[3];
                    }
                }

                panelImg[i].sprite = panelSprite[1];
                notSelected[i].enabled = true;
            }
        }
    }

    private void OnOpenDialogues()
    {
        if (!statsUpIndication.activeInHierarchy) return;

        statsUpIndication.SetActive(false);
        EnableStatsUpBoard(true);
    }

    private void OnNavUp()
    {
        if (!statsBoard.activeInHierarchy) return;
        
        selectionAS.Play();

        selectionIndex--;

        if (selectionIndex == -1)
        {
            selectionIndex = propositions.Length - 1;
        }

        RefreshSelectedProposition();
    }

    private void OnNavDown()
    {
        if (!statsBoard.activeInHierarchy) return;

        selectionAS.Play();

        selectionIndex++;

        if (selectionIndex == propositions.Length)
        {
            selectionIndex = 0;
        }

        RefreshSelectedProposition();
    }

    private void OnCancelDialogue()
    {
        if (!statsBoard.activeInHierarchy) return;

        StartCoroutine(SwitchKeySprite(false));

        selectionAS.Play();
    }

    private void OnAcceptProposition()
    {
        if (!statsBoard.activeInHierarchy) return;

        StartCoroutine(SwitchKeySprite(true));

        takenAS.Play();
    }

    private void GiveBonus()
    {
        string htmlColor = "";

        //Normal
        if (propositions[selectionIndex].name == StatsUpIndex.Heal.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.Heal)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainGlitchs.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainGlitchs)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetGlitchDisplay().AddGlitch((int)item.epicValue);
                        }
                        else
                        {
                            references.GetGlitchDisplay().AddGlitch((int)item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainMaxHP.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainMaxHP)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainMaxArmor.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainMaxArmor)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddArmorPNJBonus(item.epicValue);
                            references.GetPlayerHealth().RegainArmor(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddArmorPNJBonus(item.normalValue);
                            references.GetPlayerHealth().RegainArmor(item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.RegainArmor.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.RegainArmor)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerHealth().RegainArmor(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerHealth().RegainArmor(item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.AddRoomToSector.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.AddRoomToSector)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetRoomPool().AddRoomToSector((int)item.epicValue);
                        }
                        else
                        {
                            references.GetRoomPool().AddRoomToSector((int)item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.RemoveCurse.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.RemoveCurse)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetCurse().RemoveCurse(item.epicValue);
                        }
                        else
                        {
                            references.GetCurse().RemoveCurse(item.normalValue);
                        }
                    }
                }
            }
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.CantLoseNextKeySeries.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.CantLoseNextKeySeries)
                    {
                        references.GetGameManager().SetCantLoseNextKeySeries(true);
                    }
                }
            }
        }

        //Rare
        else if (propositions[selectionIndex].name == StatsUpIndex.GainDamage.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainDamage)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue);
                        }
                    }

                    references.GetFighter().UpdateStats();
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainMaxHPLostDamage.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainMaxHPLostDamage)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue);
                        }

                        references.GetPlayerStatistics().DivideDamage();
                        references.GetFighter().UpdateStats();
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainMaxHPForLimitedTime.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainMaxHPForLimitedTime)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddHealthForLimitedTime(item.epicValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddHealthForLimitedTime(item.normalValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue);
                        }
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.HealthForPassive.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.HealthForPassive)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue);
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue);
                        }

                        references.GetPassiveObject().RemovePassiveSlot(false);
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.DamageForPassive.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.DamageForPassive)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue);
                        }

                        references.GetPassiveObject().RemovePassiveSlot(false);
                        references.GetFighter().UpdateStats();
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainExperience.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainExperience)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetExperience().GainExperience((int)item.epicValue);
                        }
                        else
                        {
                            references.GetExperience().GainExperience((int)item.normalValue);
                        }
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.ArmorForDamage.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.ArmorForDamage)
                    {
                        float armorPoint = references.GetPlayerStatistics().GetArmor();

                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue * armorPoint);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue * armorPoint);
                        }
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.ArmorForHealth.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.ArmorForHealth)
                    {
                        float armorPoint = references.GetPlayerStatistics().GetArmor();

                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue * armorPoint);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue * armorPoint);
                        }
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.NextRoomIsClear.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.NextRoomIsClear)
                    {
                        references.GetSector().SetNextRoomIsClear(true);
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainRandomTrinketButLostPassive.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainRandomTrinketButLostPassive)
                    {
                        references.GetTrinket().GainRandomTrinket(true);
                    }
                }
            }

            htmlColor = "#3152A6";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainRandomPassiveButLostPassive.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainRandomPassiveButLostPassive)
                    {
                        references.GetPassiveObject().GainRandomPassive(true);
                    }
                }
            }

            htmlColor = "#3152A6";
        }

        //Epic
        else if (propositions[selectionIndex].name == StatsUpIndex.GainDamageForLimitedTime.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainDamageForLimitedTime)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddDamageForLimitedTime(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddDamageForLimitedTime(item.normalValue);
                        }
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainDamageLostMaxHP.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainDamageLostMaxHP)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue);
                        }

                        references.GetPlayerStatistics().DivideHP();

                        references.GetPlayerHealth().TakeDamage(references.GetPlayerHealth().GetCurrentHealth() / 2, true, true, false, true, false, true);
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.DivideTrinketCharge.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.DivideTrinketCharge)
                    {
                        references.GetTrinket().DivideChargeValue(true);
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.KillForHealth.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.KillForHealth)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue * references.GetSector().GetNumberOfKills());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue * references.GetSector().GetNumberOfKills());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue * references.GetSector().GetNumberOfKills());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue * references.GetSector().GetNumberOfKills());
                        }
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.KillForDamage.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.KillForDamage)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue * references.GetSector().GetNumberOfKills());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue * references.GetSector().GetNumberOfKills());
                        }

                        references.GetFighter().UpdateStats();
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.NextRoomNPC.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.NextRoomNPC)
                    {
                        references.GetRoomPool().NextRoomNPC();
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.AddPassiveSlot.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.AddPassiveSlot)
                    {
                        references.GetPassiveObject().AddPassiveSlot();
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainRandomPassive.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainRandomPassive)
                    {
                        references.GetPassiveObject().GainRandomPassive(false);
                    }
                }
            }

            htmlColor = "#6B1380";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.GainRandomTrinket.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainRandomTrinket)
                    {
                        references.GetTrinket().GainRandomTrinket(false);
                    }
                }
            }

            htmlColor = "#6B1380";
        }

        //Legendary
        else if (propositions[selectionIndex].name == StatsUpIndex.GainSecondChance.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.GainSecondChance)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerHealth().AddSecondChance(item.epicValue);
                        }
                        else
                        {
                            references.GetPlayerHealth().AddSecondChance(item.normalValue);
                        }
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.DamageForRoomsRemaining.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.DamageForRoomsRemaining)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue * references.GetSector().GetRoomsRemaining());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue * references.GetSector().GetRoomsRemaining());
                        }

                        references.GetFighter().UpdateStats();
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.HealthForRoomsCleared.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.HealthForRoomsCleared)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue * references.GetSector().GetRoomsCleared());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue * references.GetSector().GetRoomsCleared());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue * references.GetSector().GetRoomsCleared());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue * references.GetSector().GetRoomsCleared());
                        }
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.DamageForRoomsCleared.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.DamageForRoomsCleared)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.epicValue * references.GetSector().GetRoomsCleared());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusDamage(item.normalValue * references.GetSector().GetRoomsCleared());
                        }

                        references.GetFighter().UpdateStats();
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.HealthForRoomsRemaining.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.HealthForRoomsRemaining)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.epicValue * references.GetSector().GetRoomsRemaining());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.epicValue * references.GetSector().GetRoomsRemaining());
                        }
                        else
                        {
                            references.GetPlayerStatistics().AddBonusHealth(item.normalValue * references.GetSector().GetRoomsRemaining());
                            references.GetPlayerHealth().RefreshCurrentHealth(item.normalValue * references.GetSector().GetRoomsRemaining());
                        }
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.CantLoseAllKeySeries.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.CantLoseAllKeySeries)
                    {
                        references.GetGameManager().SetCantLoseAllKeySeries();
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.RemoveSimulation.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.RemoveSimulation)
                    {
                        references.GetSimulationsPlaceHolder().RemoveSimulationLevel();
                    }
                }
            }

            htmlColor = "#D65108";
        }
        else if (propositions[selectionIndex].name == StatsUpIndex.RemoveSectorRooms.ToString())
        {
            foreach (StatsPool statPool in statsPool)
            {
                foreach (PropositionSettings item in statPool.propositionSettings)
                {
                    if (item.statsUpIndex == StatsUpIndex.RemoveSectorRooms)
                    {
                        if (item.isEpicValue)
                        {
                            references.GetRoomPool().RemoveSectorRooms((int)item.epicValue);
                        }
                        else
                        {
                            references.GetRoomPool().RemoveSectorRooms((int)item.normalValue);
                        }
                    }
                }
            }

            htmlColor = "#D65108";
        }

        Color textColor = new();
        string statsUp = "Stats Up";

        foreach (string statsUpIndex in Enum.GetNames(typeof(StatsUpIndex)))
        {
            if (statsUpIndex == propositions[selectionIndex].name)
            {
                foreach (StatsPool statPool in statsPool)
                {
                    foreach (PropositionSettings item in statPool.propositionSettings)
                    {
                        if (item.statsUpIndex.ToString() == propositions[selectionIndex].name)
                        {
                            if (item.isEpicValue)
                            {
                                ColorUtility.TryParseHtmlString(htmlColor, out textColor);

                                statsUp += "+";
                            }
                            else
                            {
                                ColorUtility.TryParseHtmlString(htmlColor, out textColor);
                            }
                        }
                    }
                }
            }
        }

        playerTextInfoSpawner.SpawnNPCBonus(statsUp, textColor);

        EnableStatsUpBoard(false);
    }

    public void EnableStatsUpBoard(bool state)
    {
        references.GetPlayerController().disableControl = state;
        references.GetPlayerController().SetIsInActivity(state);

        if (state)
        {
            references.GetPlayerController().StopPlayer(true);
            SetupPropositions();

            selectionIndex = 0;

            RefreshSelectedProposition();
        }
        else
        {
            statsUpIndication.SetActive(false);

            inputActions.Dialogues.Disable();
        }

        statsBoard.SetActive(state);
    }

    public void EnableStatsUpIndication()
    {
        if (cantUseStatsUpBoard) return;

        inputActions.Dialogues.Enable();

        statsUpIndication.SetActive(true);
    }

    public float GetCurrentChanceForStatsUp()
    {
        return currentChanceForStatsUp;
    }

    public void IncreaseChanceToStatsUp()
    {
        currentChanceForStatsUp += 12.5f;

        if (currentChanceForStatsUp > 100)
        {
            currentChanceForStatsUp = 100;
        }
    }

    public void ResetCurrentChanceForStatsUp()
    {
        currentChanceForStatsUp = 5;
    }

    public void CantUseStatsUpBoard(bool state)
    {
        cantUseStatsUpBoard = state;

        EnableStatsUpIndication();
    }

    public bool IsStatsUpBoardOpen()
    {
        if (statsBoard.activeInHierarchy) return true;
        else return false;
    }

    private IEnumerator SwitchKeySprite(bool selection)
    {
        isSwapKeySprite = true;

        if (references.GetGameManager().GetKeyboardControl())
        {
            if (selection)
            {
                keyToPressImg[0].sprite = keyToPressSprites[1];
                keyToPressImg[1].sprite = keyToPressSprites[1];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyToPressImg[0].sprite = keyToPressSprites[0];
                keyToPressImg[1].sprite = keyToPressSprites[0];

                GiveBonus();
            }
            else
            {
                keyToCloseImgs[0].sprite = keyToCloseSprites[1];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyToCloseImgs[0].sprite = keyToCloseSprites[0];

                EnableStatsUpBoard(false);
            }
        }
        else
        {
            if (references.GetGameManager().GetChangeUI())
            {
                if (selection)
                {
                    keyToPressImg[2].sprite = keyToPressSprites[5];
                    keyToPressImg[3].sprite = keyToPressSprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    keyToPressImg[2].sprite = keyToPressSprites[4];
                    keyToPressImg[3].sprite = keyToPressSprites[4];

                    GiveBonus();
                }
                else
                {
                    keyToCloseImgs[1].sprite = keyToCloseSprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    keyToCloseImgs[1].sprite = keyToCloseSprites[4];

                    EnableStatsUpBoard(false);
                }
            }
            else
            {
                if (selection)
                {
                    keyToPressImg[2].sprite = keyToPressSprites[3];
                    keyToPressImg[3].sprite = keyToPressSprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    keyToPressImg[2].sprite = keyToPressSprites[2];
                    keyToPressImg[3].sprite = keyToPressSprites[2];

                    GiveBonus();
                }
                else
                {
                    keyToCloseImgs[1].sprite = keyToCloseSprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    keyToCloseImgs[1].sprite = keyToCloseSprites[2];

                    EnableStatsUpBoard(false);
                }
            }
        }

        isSwapKeySprite = false;
    }
}

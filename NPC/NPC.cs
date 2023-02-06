using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Serializable]
    private class PropositionSettings
    {
        [Header("Passive")]
        public PassiveIndex passiveIndex;
        public string passiveName;

        [Header("Trinket")]
        public TrinketIndex trinketIndex;
        public string trinketName;

        [Header("Miscs")]
        [TextArea] public string propositionTxt;
        public float chanceForEpic;
        public DialogueSO endDialogue;
        public bool isAlreadyTaken;
        [HideInInspector] public bool isEpicValue;
    }

    [SerializeField] NPCIndex npcIndex;
    [SerializeField] GameObject propositions;
    [SerializeField] GameObject[] proposition;
    [SerializeField] PropositionSettings[] propositionSettings;
    [SerializeField] RectTransform dialogueRectTransform;
    [SerializeField] RectTransform propositionsRectTransform;
    [SerializeField] Image[] keyToPressImg;
    [SerializeField] Sprite[] keyToPressSprites;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] AudioSource takenAS;
    [SerializeField] DialogueSO nothingTakenDialogue;
    [SerializeField] LaunchDialogue launchDialogue;
    [SerializeField] TextMeshProUGUI[] npcNames;
    [SerializeField] bool isStoryNPC;

    CatchReferences references;
    DialogueSO endDialogue;
    PlayerInputSystem inputActions;

    int selectionIndex;
    bool isNothingToTake;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.Dialogues.CancelDialogue.performed += ctx => OnCancelDialogue();
        inputActions.Dialogues.AcceptProposition.performed += ctx => OnAcceptProposition();
        inputActions.Dialogues.NavDown.performed += ctx => OnNavDown();
        inputActions.Dialogues.NavUp.performed += ctx => OnNavUp();

        inputActions.Dialogues.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Dialogues.Disable();
    }

    private void OnEnable()
    {
        if (!isStoryNPC)
        {
            SetupPropositions();
        }

        EnablePropositions(false);
    }

    private void SetupPropositions()
    {
        int lastPropositionSettingsIndex = -1;

        for (int i = 0; i < proposition.Length; i++)
        {
            bool propositionFinded = false;
            int propositionSettingsIndex = -1;
            int propositionsRemaining = 3;

            foreach (var item in propositionSettings)
            {
                if (item.passiveIndex != PassiveIndex.None)
                {
                    if (references.GetPassiveObject().GetPassiveObject(item.passiveIndex))
                    {
                        item.isAlreadyTaken = true;

                        propositionsRemaining--;
                    }
                }
                else if (item.trinketIndex != TrinketIndex.None)
                {
                    if (references.GetTrinket().GetCurrentTrinket() == item.trinketIndex)
                    {
                        item.isAlreadyTaken = true;

                        propositionsRemaining--;
                    }
                }
            }

            if (propositionsRemaining == 1)
            {
                proposition[i].SetActive(false);

                i++;
            }
            else if (propositionsRemaining == 0)
            {
                isNothingToTake = true;

                break;
            }

            while (!propositionFinded)
            {
                propositionSettingsIndex = UnityEngine.Random.Range(0, propositionSettings.Length);
                
                while (lastPropositionSettingsIndex == propositionSettingsIndex)
                {
                    propositionSettingsIndex = UnityEngine.Random.Range(0, propositionSettings.Length);
                }

                if (propositionSettings[propositionSettingsIndex].passiveIndex != PassiveIndex.None
                    && !propositionSettings[propositionSettingsIndex].isAlreadyTaken)
                {
                    propositionFinded = true;
                }
                else if (propositionSettings[propositionSettingsIndex].trinketIndex != TrinketIndex.None
                    && !propositionSettings[propositionSettingsIndex].isAlreadyTaken)
                {
                    propositionFinded = true;
                }
            }

            lastPropositionSettingsIndex = propositionSettingsIndex;

            string propositionValueTxt = "";
            int epicChance = UnityEngine.Random.Range(0, 101);

            if (propositionSettings[propositionSettingsIndex].passiveIndex != PassiveIndex.None)
            {
                propositionValueTxt = "<color=#386641>" + propositionSettings[propositionSettingsIndex].passiveName + "</color>";

                proposition[i].name = propositionSettings[propositionSettingsIndex].passiveIndex.ToString();
            }
            else if (propositionSettings[propositionSettingsIndex].trinketIndex != TrinketIndex.None)
            {
                if (epicChance <= propositionSettings[propositionSettingsIndex].chanceForEpic)
                {
                    propositionValueTxt = "<color=#D65108>" + propositionSettings[propositionSettingsIndex].trinketName + "+</color>";

                    propositionSettings[propositionSettingsIndex].isEpicValue = true;
                }
                else
                {
                    propositionValueTxt = "<color=#3152A6>" + propositionSettings[propositionSettingsIndex].trinketName + "</color>";

                    propositionSettings[propositionSettingsIndex].isEpicValue = false;
                }

                proposition[i].name = propositionSettings[propositionSettingsIndex].trinketIndex.ToString();
            }

            TextMeshProUGUI propositionTxt = proposition[i].transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            propositionTxt.text = propositionValueTxt;

            if (references.GetSimulationsPlaceHolder().GetDisableUI())
            {
                propositionTxt.text = "<color=#C126B5>???</color>";
            }
        }
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (references.GetGameManager().GetKeyboardControl())
        {
            keyToPressImg[0].gameObject.SetActive(true);
            keyToPressImg[1].gameObject.SetActive(true);

            keyToPressImg[2].gameObject.SetActive(false);
            keyToPressImg[3].gameObject.SetActive(false);
        }
        else
        {
            keyToPressImg[0].gameObject.SetActive(false);
            keyToPressImg[1].gameObject.SetActive(false);

            keyToPressImg[2].gameObject.SetActive(true);
            keyToPressImg[3].gameObject.SetActive(true);
        }

        if (references.GetGameManager().GetChangeUI())
        {
            keyToPressImg[2].sprite = keyToPressSprites[1];
            keyToPressImg[3].sprite = keyToPressSprites[1];
        }
        else
        {
            keyToPressImg[2].sprite = keyToPressSprites[0];
            keyToPressImg[3].sprite = keyToPressSprites[0];
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(propositionsRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueRectTransform);
    }

    private void OnNavUp()
    {
        if (!propositions.activeInHierarchy) return;
        if (!proposition[0].activeInHierarchy) return;

        selectionAS.Play();

        selectionIndex--;

        if (selectionIndex == -1)
        {
            selectionIndex = proposition.Length - 1;
        }

        RefreshSelectedProposition();
    }

    private void OnNavDown()
    {
        if (!propositions.activeInHierarchy) return;
        if (!proposition[0].activeInHierarchy) return;

        selectionAS.Play();

        selectionIndex++;

        if (selectionIndex == proposition.Length)
        {
            selectionIndex = 0;
        }

        RefreshSelectedProposition();
    }

    private void OnCancelDialogue()
    {
        if (!propositions.activeInHierarchy) return;

        selectionAS.Play();

        EnablePropositions(false);

        StartCoroutine(launchDialogue.EndDialogue(nothingTakenDialogue, false));
    }

    private void OnAcceptProposition()
    {
        if (!propositions.activeInHierarchy) return;

        takenAS.Play();

        GiveBonus();

        EnablePropositions(false);

        StartCoroutine(launchDialogue.EndDialogue(endDialogue, false));
    }

    private void GiveBonus()
    {
        //Alchemist
        if (proposition[selectionIndex].name == PassiveIndex.Chemotherapy.ToString()) //1-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.Chemotherapy)
                {
                    if (references.GetPassiveObject().HaveEmptySlot() != -1)
                    {
                        references.GetPlayerHealth().TakeDamage(references.GetPlayerHealth().GetCurrentHealth() - 1, true, true, false, true, true);
                    }

                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.Chemotherapy);

                    references.GetPlayerHealth().RefreshBaseHealth();

                    references.GetFighter().UpdateStats();

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.BloodTransfusion.ToString()) // 1-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.BloodTransfusion)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.BloodTransfusion);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.Leukemia.ToString()) //1-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.Leukemia)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.Leukemia, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.Leukemia, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Bard
        else if (proposition[selectionIndex].name == PassiveIndex.LetTheMusicPlay.ToString()) //2-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.LetTheMusicPlay)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.LetTheMusicPlay);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.PentatonicMinor.ToString()) //2-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.PentatonicMinor)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.PentatonicMinor);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.BoringSong.ToString()) //2-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.BoringSong)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.BoringSong, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.BoringSong, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Blacksmith
        else if (proposition[selectionIndex].name == PassiveIndex.SteelBandage.ToString()) //3-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.SteelBandage)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.SteelBandage);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.BloodForge.ToString()) //3-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.BloodForge)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.BloodForge);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.ScrapsOfMetal.ToString()) //3-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.ScrapsOfMetal)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.ScrapsOfMetal, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.ScrapsOfMetal, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Chef
        else if (proposition[selectionIndex].name == PassiveIndex.HotSpicy.ToString()) //4-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.HotSpicy)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.HotSpicy);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.HotFatty.ToString()) //4-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.HotFatty)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.HotFatty);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.MidnightSnack.ToString()) //4-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.MidnightSnack)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.MidnightSnack, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.MidnightSnack, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Farmer
        else if (proposition[selectionIndex].name == PassiveIndex.HeartSeed.ToString()) //5-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.HeartSeed)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.HeartSeed);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.CactusSeed.ToString()) //5-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.CactusSeed)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.CactusSeed);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.StrongRoots.ToString()) //5-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.StrongRoots)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.StrongRoots, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.StrongRoots, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Miner
        else if (proposition[selectionIndex].name == PassiveIndex.GlitchedKey.ToString()) //7-1
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.GlitchedKey)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.GlitchedKey);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.BedrockArmor.ToString()) //7-2
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.BedrockArmor)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.BedrockArmor);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.NotchedPickaxe.ToString()) //7-3
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.NotchedPickaxe)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.NotchedPickaxe, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.NotchedPickaxe, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        //Priest
        else if (proposition[selectionIndex].name == PassiveIndex.Purification.ToString()) //8-4
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.Purification)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.Purification);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == PassiveIndex.HylianShield.ToString()) //8-5
        {
            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.passiveIndex == PassiveIndex.HylianShield)
                {
                    references.GetPassiveObject().SetPassiveObject(PassiveIndex.HylianShield);

                    endDialogue = item.endDialogue;
                }
            }
        }
        else if (proposition[selectionIndex].name == TrinketIndex.FavorOfGod.ToString()) //8-6
        {

            foreach (PropositionSettings item in propositionSettings)
            {
                if (item.trinketIndex == TrinketIndex.FavorOfGod)
                {
                    if (item.isEpicValue)
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.FavorOfGod, true));
                    }
                    else
                    {
                        StartCoroutine(references.GetTrinket().SetTrinketIndex(TrinketIndex.FavorOfGod, false));
                    }

                    endDialogue = item.endDialogue;
                }
            }
        }

        SpawnTextInfo();

        references.GetGameManager().IncreaseNumberOfOfferAccepted();

        CheckAchievement();
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfOfferAccepted = references.GetGameManager().GetNumberOfOfferAccepted();

            if (numberOfOfferAccepted == 50)
            {
                SteamUserStats.GetAchievement("ACH_NPC_OFFERS", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_NPC_OFFERS");
                    SteamUserStats.SetAchievement("ACH_NPC_OFFERS");
                }
            }

            SteamUserStats.StoreStats();
        }
    }

    private void SpawnTextInfo()
    {
        Color textColor = new();

        foreach (string passive in Enum.GetNames(typeof(PassiveIndex)))
        {
            if (passive == proposition[selectionIndex].name)
            {
                ColorUtility.TryParseHtmlString("#386641", out textColor);
            }
        }

        foreach (string trinket in Enum.GetNames(typeof(TrinketIndex)))
        {
            if (trinket == proposition[selectionIndex].name)
            {
                foreach (PropositionSettings prop in propositionSettings)
                {
                    if (proposition[selectionIndex].name == prop.trinketIndex.ToString())
                    {
                        if (prop.isEpicValue)
                        {
                            ColorUtility.TryParseHtmlString("#D65108", out textColor);

                            proposition[selectionIndex].name += "+";
                        }
                        else
                        {
                            ColorUtility.TryParseHtmlString("#3152A6", out textColor);
                        }
                    }
                }
            }
        }

        references.GetPlayerStatistics().GetComponentInChildren<TextInfoSpawner>().SpawnNPCBonus(proposition[selectionIndex].name, textColor);
    }

    private void RefreshSelectedProposition()
    {
        if (!proposition[0].activeInHierarchy)
        {
            selectionIndex = 1;

            return;
        }

        for (int i = 0; i < proposition.Length; i++)
        {
            if (selectionIndex == i)
            {
                proposition[i].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true); //border

                //key to select
                if (references.GetGameManager().GetKeyboardControl())
                {
                    proposition[i].transform.GetChild(1).GetComponent<Image>().enabled = true;
                    proposition[i].transform.GetChild(2).GetComponent<Image>().enabled = false;
                }
                else
                {
                    proposition[i].transform.GetChild(1).GetComponent<Image>().enabled = false;
                    proposition[i].transform.GetChild(2).GetComponent<Image>().enabled = true;
                }
            }
            else
            {
                proposition[i].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

                proposition[i].transform.GetChild(1).GetComponent<Image>().enabled = false;
                proposition[i].transform.GetChild(2).GetComponent<Image>().enabled = false;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueRectTransform);
        }
    }

    public void EnablePropositions(bool state)
    {
        propositions.SetActive(state);

        if (state)
        {
            selectionIndex = 0;

            RefreshSelectedProposition();
        }
    }

    public bool GetIsNothingToTake()
    {
        return isNothingToTake;
    }

    public NPCIndex GetNPCIndex()
    {
        return npcIndex;
    }
}

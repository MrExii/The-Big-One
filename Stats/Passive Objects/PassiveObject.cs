using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveObject : MonoBehaviour
{
    [SerializeField] GameObject[] replaceUI;
    [SerializeField] List<PassiveIndex> currentPassiveObjects;
    [SerializeField] Image[] keyboardReplaceUIImg;
    [SerializeField] Image[] controllerReplaceUIImg;
    [SerializeField] Sprite[] keyboardSprites;
    [SerializeField] Sprite[] controllerSprites;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] PassiveIndex[] passiveIndices;

    CatchReferences references;
    PlayerInputSystem inputActions;

    PassiveIndex objectToReplace;

    float healthLost;

    const float bloodForgeDamageAmount = 0.1f;
    const float timeBetweenKeySpriteSwap = 0.05f;

    int cactusSeedCount;
    int heartSeedCount;

    bool purificationAlreadyUsed;
    bool isReplacedUI;
    bool isKeySwap;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();

        inputActions = new();
        inputActions.UIPassiveObjects.Replace1.performed += ctx => OnReplace1();
        inputActions.UIPassiveObjects.Replace2.performed += ctx => OnReplace2();
        inputActions.UIPassiveObjects.Replace3.performed += ctx => OnReplace3();
        inputActions.UIPassiveObjects.Replace4.performed += ctx => OnReplace4();
        inputActions.UIPassiveObjects.Replace5.performed += ctx => OnReplace5();

        inputActions.UIPassiveObjects.Enable();
    }

    private void OnDestroy()
    {
        inputActions.UIPassiveObjects.Disable();
    }

    private void Start()
    {
        //SetPassiveObject(PassiveIndex.BedrockArmor);
        //SetPassiveObject(PassiveIndex.HotFatty);
        //SetPassiveObject(PassiveIndex.HylianShield);
        //SetPassiveObject(PassiveIndex.BloodTransfusion);

        foreach (var ui in replaceUI)
        {
            ui.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isReplacedUI) return;

        RefreshUI();
    }

    private void OnReplace1()
    {
        if (!isReplacedUI) return;
        if (!keyboardReplaceUIImg[0].gameObject.activeInHierarchy && !controllerReplaceUIImg[0].gameObject.activeInHierarchy) return;

        ReplacePassiveObject(0);
    }

    private void OnReplace2()
    {
        if (!isReplacedUI) return;
        if (!keyboardReplaceUIImg[1].gameObject.activeInHierarchy && !controllerReplaceUIImg[1].gameObject.activeInHierarchy) return;

        ReplacePassiveObject(1);
    }

    private void OnReplace3()
    {
        if (!isReplacedUI) return;
        if (!keyboardReplaceUIImg[2].gameObject.activeInHierarchy && !controllerReplaceUIImg[2].gameObject.activeInHierarchy) return;

        ReplacePassiveObject(2);
    }

    private void OnReplace4()
    {
        if (!isReplacedUI) return;
        if (!keyboardReplaceUIImg[3].gameObject.activeInHierarchy && !controllerReplaceUIImg[3].gameObject.activeInHierarchy) return;

        ReplacePassiveObject(3);
    }

    private void OnReplace5()
    {
        if (!isReplacedUI) return;
        if (!keyboardReplaceUIImg[4].gameObject.activeInHierarchy && !controllerReplaceUIImg[4].gameObject.activeInHierarchy) return;

        ReplacePassiveObject(4);
    }

    private void RefreshUI()
    {
        if (!replaceUI[0].activeInHierarchy && references.GetGameManager().GetKeyboardControl())
        {
            replaceUI[0].SetActive(true);
            replaceUI[1].SetActive(false);
        }
        else if (!replaceUI[1].activeInHierarchy && !references.GetGameManager().GetKeyboardControl())
        {
            replaceUI[0].SetActive(false);
            replaceUI[1].SetActive(true);
        }

        if (references.GetGameManager().GetChangeUI() && !isKeySwap)
        {
            controllerReplaceUIImg[0].sprite = controllerSprites[8];
            controllerReplaceUIImg[1].sprite = controllerSprites[10];
            controllerReplaceUIImg[2].sprite = controllerSprites[12];
            controllerReplaceUIImg[3].sprite = controllerSprites[14];
        }
        else if (!isKeySwap)
        {
            controllerReplaceUIImg[0].sprite = controllerSprites[0];
            controllerReplaceUIImg[1].sprite = controllerSprites[2];
            controllerReplaceUIImg[2].sprite = controllerSprites[4];
            controllerReplaceUIImg[3].sprite = controllerSprites[6];
        }

        RefreshReplaceUI();
    }

    public void SetPassiveObject(PassiveIndex passiveIndex)
    {
        int index;
        PassiveIndex oldPassive;

        for (int i = 0; i < currentPassiveObjects.Count; i++) //Replace SteelBandage || BloodForge
        {
            if ((currentPassiveObjects[i] == PassiveIndex.SteelBandage && passiveIndex == PassiveIndex.BloodForge) ||
                (currentPassiveObjects[i] == PassiveIndex.BloodForge && passiveIndex == PassiveIndex.SteelBandage))
            {
                oldPassive = currentPassiveObjects[i];

                currentPassiveObjects[i] = passiveIndex;

                index = i;

                StartCoroutine(references.GetPassiveDisplay().SetPassiveSprite(passiveIndex, oldPassive, index));

                return;
            }
        }

        for (int i = 0; i < currentPassiveObjects.Count; i++)
        {
            if (currentPassiveObjects[i] == PassiveIndex.None && passiveIndex != PassiveIndex.None)
            {
                oldPassive = currentPassiveObjects[i];

                currentPassiveObjects[i] = passiveIndex;

                index = i;

                StartCoroutine(references.GetPassiveDisplay().SetPassiveSprite(passiveIndex, oldPassive, index));

                return;
            }
        }

        if (passiveIndex != PassiveIndex.None)
        {
            objectToReplace = passiveIndex;

            isReplacedUI = true;
        }
    }

    private void RefreshReplaceUI()
    {
        for (int i = 0; i < keyboardReplaceUIImg.Length; i++)
        {
            if (i >= currentPassiveObjects.Count)
            {
                keyboardReplaceUIImg[i].gameObject.SetActive(false);
                controllerReplaceUIImg[i].gameObject.SetActive(false);
            }
            else
            {
                keyboardReplaceUIImg[i].gameObject.SetActive(true);
                controllerReplaceUIImg[i].gameObject.SetActive(true);
            }
        }
    }

    private void ReplacePassiveObject(int index)
    {
        PassiveIndex oldPassive;

        StartCoroutine(SwitchKeySprite(index));

        isReplacedUI = false;

        oldPassive = currentPassiveObjects[index];

        currentPassiveObjects[index] = objectToReplace;

        StartCoroutine(references.GetPassiveDisplay().SetPassiveSprite(objectToReplace, oldPassive, index));

        selectionAS.Play();

        if (objectToReplace == PassiveIndex.Chemotherapy)
        {
            references.GetPlayerHealth().TakeDamage(references.GetPlayerHealth().GetCurrentHealth() - 1, true, true, false, true, true);
            references.GetPlayerHealth().RefreshBaseHealth();
        }
    }

    public bool SteelBandage()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.SteelBandage)
            {
                return true;
            }
        }

        return false;
    }

    public bool GlitchedKeys()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.GlitchedKey)
            {
                return true;
            }
        }

        return false;
    }

    public bool BloodForge()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.BloodForge)
            {
                return true;
            }
        }

        return false;
    }

    public float BloodForgeHealAmount()
    {
        return UnityEngine.Random.Range(2, 6);
    }

    public void BloodForgeAddDamage(float damage)
    {
        healthLost += damage;

        if (healthLost >= 10f)
        {
            healthLost = 0f;

            references.GetPlayerStatistics().AddBonusDamage(bloodForgeDamageAmount);
            references.GetFighter().UpdateStats();
        }
    }

    public bool Chemotherapy()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.Chemotherapy)
            {
                return true;
            }
        }

        return false;
    }

    public bool BloodTransfusion()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.BloodTransfusion)
            {
                return true;
            }
        }

        return false;
    }

    public int BloodTransfusionHeal()
    {
        return UnityEngine.Random.Range(3, 7);
    }

    public void LetTheMusicPlay()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.LetTheMusicPlay)
            {
                if (!references.GetRoomPool().GetCurrentRoom().GetHostileRoom()) return;

                references.GetPlayerStatistics().AddBonusHealth(5);
                references.GetPlayerHealth().Heal(5);
            }
        }
    }

    public bool HylianShield()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.HylianShield)
            {
                return true;
            }
        }

        return false;
    }

    public bool BedrockArmor()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.BedrockArmor)
            {
                return true;
            }
        }

        return false;
    }

    public bool HotSpicy()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.HotSpicy)
            {
                return true;
            }
        }

        return false;
    }

    public bool HotFatty()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.HotFatty)
            {
                return true;
            }
        }

        return false;
    }

    public bool PentatonicMinor()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.PentatonicMinor)
            {
                return true;
            }
        }

        return false;
    }

    public void CactusSeed()
    {
        for (int i = 0; i < currentPassiveObjects.Count; i++)
        {
            if (currentPassiveObjects[i] == PassiveIndex.CactusSeed)
            {
                cactusSeedCount++;

                if (cactusSeedCount == 4)
                {
                    cactusSeedCount = 0;

                    GetComponent<PlayerStatistics>().AddBonusDamage(1);

                    GetComponent<Fighter>().UpdateStats();
                }
            }
        }
    }

    public void HeartSeed()
    {
        for (int i = 0; i < currentPassiveObjects.Count; i++)
        {
            if (currentPassiveObjects[i] == PassiveIndex.HeartSeed)
            {
                heartSeedCount++;

                if (heartSeedCount == 4)
                {
                    heartSeedCount = 0;

                    GetComponent<PlayerStatistics>().AddBonusHealth(20);

                    GetComponent<PlayerHealth>().Heal(20);
                }
            }
        }
    }

    public bool Purification()
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (item == PassiveIndex.Purification && !purificationAlreadyUsed)
            {
                purificationAlreadyUsed = true;

                return true;
            }
        }

        return false;
    }

    public void ResetPurification()
    {
        purificationAlreadyUsed = false;
    }

    public void RemovePassiveSlot(bool forLimitedTime)
    {
        List<GameObject> passiveSlots = references.GetPassiveDisplay().GetPassiveSlots();

        int index = HaveEmptySlot();

        if (index == -1)
        {
            index = UnityEngine.Random.Range(0, currentPassiveObjects.Count);
        }

        currentPassiveObjects.RemoveAt(index);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            if (i == index)
            {
                passiveSlots[i].SetActive(false);
            }
        }

        references.GetPassiveDisplay().RemovePassiveSlot(index);

        if (forLimitedTime)
        {
            Invoke(nameof(AddPassiveSlot), 300f);
        }
    }

    public void AddPassiveSlot()
    {
        if (currentPassiveObjects.Count == 5) return;

        currentPassiveObjects.Add(PassiveIndex.None);

        references.GetPassiveDisplay().AddPassiveSlot();
    }

    public int HaveEmptySlot()
    {
        for (int i = 0; i < currentPassiveObjects.Count; i++)
        {
            if (currentPassiveObjects[i] == PassiveIndex.None)
            {
                return i;
            }
        }

        return -1;
    }

    public bool GetPassiveObject(PassiveIndex passiveObject)
    {
        foreach (PassiveIndex item in currentPassiveObjects)
        {
            if (passiveObject == item)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanDeleteSlot()
    {
        if (currentPassiveObjects.Count == 0) return false;
        else return true;
    }

    public void GainRandomPassive(bool buLostPassive)
    {
        int passiveIndex = UnityEngine.Random.Range(0, passiveIndices.Length);

        while (references.GetPassiveObject().GetPassiveObject(passiveIndices[passiveIndex]))
        {
            passiveIndex = UnityEngine.Random.Range(0, passiveIndices.Length);
        }

        if (passiveIndices[passiveIndex] == PassiveIndex.Chemotherapy)
        {
            references.GetPlayerHealth().TakeDamage(references.GetPlayerHealth().GetCurrentHealth() - 1, true, true, false, true, true);

            references.GetPlayerHealth().RefreshBaseHealth();
            references.GetFighter().UpdateStats();
        }

        SetPassiveObject(passiveIndices[passiveIndex]);

        if (buLostPassive)
        {
            RemovePassiveSlot(true);
        }
    }

    private IEnumerator SwitchKeySprite(int index)
    {
        isKeySwap = true;

        if (index == 0)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyboardReplaceUIImg[0].sprite = keyboardSprites[1];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyboardReplaceUIImg[0].sprite = keyboardSprites[0];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    controllerReplaceUIImg[0].sprite = controllerSprites[11];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[0].sprite = controllerSprites[10];
                }
                else
                {
                    controllerReplaceUIImg[0].sprite = controllerSprites[1];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[0].sprite = controllerSprites[0];
                }
            }
        }
        else if (index == 1)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyboardReplaceUIImg[1].sprite = keyboardSprites[3];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyboardReplaceUIImg[1].sprite = keyboardSprites[2];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    controllerReplaceUIImg[1].sprite = controllerSprites[13];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[1].sprite = controllerSprites[12];
                }
                else
                {
                    controllerReplaceUIImg[1].sprite = controllerSprites[3];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[1].sprite = controllerSprites[2];
                }
            }
        }
        else if (index == 2)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyboardReplaceUIImg[2].sprite = keyboardSprites[5];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyboardReplaceUIImg[2].sprite = keyboardSprites[4];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    controllerReplaceUIImg[2].sprite = controllerSprites[15];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[2].sprite = controllerSprites[14];
                }
                else
                {
                    controllerReplaceUIImg[2].sprite = controllerSprites[5];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[2].sprite = controllerSprites[4];
                }
            }
        }
        else if (index == 3)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyboardReplaceUIImg[3].sprite = keyboardSprites[7];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyboardReplaceUIImg[3].sprite = keyboardSprites[6];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    controllerReplaceUIImg[3].sprite = controllerSprites[17];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[3].sprite = controllerSprites[16];
                }
                else
                {
                    controllerReplaceUIImg[3].sprite = controllerSprites[7];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[3].sprite = controllerSprites[6];
                }
            }
        }
        else if (index == 4)
        {
            if (references.GetGameManager().GetKeyboardControl())
            {
                keyboardReplaceUIImg[4].sprite = keyboardSprites[9];

                yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                keyboardReplaceUIImg[4].sprite = keyboardSprites[8];
            }
            else
            {
                if (references.GetGameManager().GetChangeUI())
                {
                    controllerReplaceUIImg[4].sprite = controllerSprites[19];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[4].sprite = controllerSprites[18];
                }
                else
                {
                    controllerReplaceUIImg[4].sprite = controllerSprites[9];

                    yield return new WaitForSeconds(timeBetweenKeySpriteSwap);

                    controllerReplaceUIImg[4].sprite = controllerSprites[8];
                }
            }
        }

        isKeySwap = false;

        foreach (var ui in replaceUI)
        {
            ui.SetActive(false);
        }
    }
}

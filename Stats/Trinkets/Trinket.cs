using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Trinket : MonoBehaviour
{
    [System.Serializable]
    private class TrinketSettings
    {
        public TrinketIndex trinketIndex;
        public int chargeValue;
        public int epicChargeValue;
    }

    [SerializeField] TrinketIndex currentTrinket;
    [SerializeField] TextMeshProUGUI chargeValueTxt;
    [SerializeField] GameObject[] PNJ;
    [SerializeField] GameObject spawnVFX;
    [SerializeField] TrinketSettings[] trinketSettings;
    [SerializeField] GameObject glitchPrefab;
    [SerializeField] AudioSource selectionAS;
    [SerializeField] Image cantUseImg;
    [SerializeField] TextInfoSpawner textInfoSpawner;
    [SerializeField] TrinketIndex[] trinketIndices;

    CatchReferences references;
    PlayerInputSystem inputActions;
    Animator cantUseAnimator;

    Color color;

    int chargeValue;
    int currentChargeValue;

    bool isEpicValue;
    bool divideChargeValue;
    bool trinketSwaping;
    bool isFadeOut = true;
    bool isFadeIn;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "End Scene") return;

        references = FindObjectOfType<CatchReferences>();
        cantUseAnimator = cantUseImg.GetComponent<Animator>();

        inputActions = new();
        inputActions.Player.UseTrinket.performed += ctx => OnUse();

        inputActions.Player.Enable();

        ColorUtility.TryParseHtmlString("#3152A6", out color);

        cantUseImg.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "End Scene") return;

        inputActions.Player.Disable();
    }

    private void Start()
    {
        StartCoroutine(SetTrinketIndex(currentTrinket, isEpicValue));
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "End Scene") return;

        CantUseDisplay();
    }

    private void CantUseDisplay()
    {
        if (trinketSwaping && !isFadeOut && currentTrinket != TrinketIndex.None)
        {
            isFadeOut = true;
            isFadeIn = false;

            cantUseAnimator.SetTrigger("fadeOut");
        }

        if (trinketSwaping) return;

        if (references.GetRoomPool())
        {
            RoomPool();
        }
        else
        {
            RoomPoolTest();
        }
    }

    private void RoomPool()
    {
        if ((references.GetPlayerController().GetIsInActivity() && currentTrinket != TrinketIndex.FavorOfGod ||
                    !references.GetRoomPool().GetCurrentRoom().GetCanExitRoom() && currentTrinket == TrinketIndex.FavorOfGod)
                    && !isFadeIn && currentTrinket != TrinketIndex.None)
        {
            if (cantUseAnimator.enabled == false)
            {
                cantUseAnimator.enabled = true;
            }

            isFadeIn = true;
            isFadeOut = false;

            cantUseAnimator.SetTrigger("fadeIn");
        }
        else if ((!references.GetPlayerController().GetIsInActivity() && currentTrinket != TrinketIndex.FavorOfGod
            || references.GetRoomPool().GetCurrentRoom().GetCanExitRoom() && currentTrinket == TrinketIndex.FavorOfGod)
            && !isFadeOut && currentTrinket != TrinketIndex.None)
        {
            if (cantUseAnimator.enabled == false)
            {
                cantUseAnimator.enabled = true;
            }

            isFadeOut = true;
            isFadeIn = false;

            cantUseAnimator.SetTrigger("fadeOut");
        }
    }

    private void RoomPoolTest()
    {
        if ((references.GetPlayerController().GetIsInActivity() && currentTrinket != TrinketIndex.FavorOfGod ||
           !references.GetRoomPoolTest().GetCurrentRoom().GetCanExitRoom() && currentTrinket == TrinketIndex.FavorOfGod)
           && !isFadeIn && currentTrinket != TrinketIndex.None)
        {
            if (cantUseAnimator.enabled == false)
            {
                cantUseAnimator.enabled = true;
            }

            isFadeIn = true;
            isFadeOut = false;

            cantUseAnimator.SetTrigger("fadeIn");
        }
        else if ((!references.GetPlayerController().GetIsInActivity() && currentTrinket != TrinketIndex.FavorOfGod
            || references.GetRoomPoolTest().GetCurrentRoom().GetCanExitRoom() && currentTrinket == TrinketIndex.FavorOfGod)
            && !isFadeOut && currentTrinket != TrinketIndex.None)
        {
            if (cantUseAnimator.enabled == false)
            {
                cantUseAnimator.enabled = true;
            }

            isFadeOut = true;
            isFadeIn = false;

            cantUseAnimator.SetTrigger("fadeOut");
        }
    }

    private void OnUse()
    {
        if (references.GetPlayerController().GetIsInActivity()) return;
        if (references.GetPlayerController().disableControl) return;
        if (references.GetPlayerHealth().isDead) return;
        if (references.GetPauseMenu().GetInPauseMenu()) return;

        if (currentChargeValue == chargeValue)
        {
            if (currentTrinket == TrinketIndex.None) return;

            if (currentTrinket == TrinketIndex.Leukemia)
            {
                Alchemist();
            }
            else if (currentTrinket == TrinketIndex.BoringSong)
            {
                Bard();
            }
            else if (currentTrinket == TrinketIndex.ScrapsOfMetal)
            {
                BlackSmith();
            }
            else if (currentTrinket == TrinketIndex.MidnightSnack)
            {
                Chef();
            }
            else if (currentTrinket == TrinketIndex.StrongRoots)
            {
                Farmer();
            }
            else if (currentTrinket == TrinketIndex.NotchedPickaxe)
            {
                Miner();
            }
            else if (currentTrinket == TrinketIndex.FavorOfGod)
            {
                if (!references.GetRoomPool().GetCurrentRoom().GetCanExitRoom()) return;
                if (FindObjectOfType<NPC>())
                {
                    if (FindObjectOfType<NPC>().GetComponent<SpriteRenderer>().enabled == true) return; //Check if there already a PNJ spawned
                }
                else if (FindObjectOfType<Scroll>())
                {
                    if (FindObjectOfType<Scroll>().GetComponentInChildren<SpriteRenderer>().enabled == true) return; //Check if there a Scroll
                }

                Priest();
            }

            currentChargeValue = 0;
            RefreshChargeTxt();

            selectionAS.Play();

            DivideChargeValue(false);
        }
    }

    private void Alchemist()
    {
        foreach (EnemyHealth enemy in FindObjectsOfType<EnemyHealth>())
        {
            if (enemy.gameObject.activeInHierarchy && !enemy.GetIsDead())
            {
                float damage = GetComponent<Fighter>().GetDamage(false);
                bool isCritical = GetComponent<GenerateDamage>().GetIsCriticalDamage();

                enemy.TakeDamage(damage, isCritical, true);
                references.GetPlayerHealth().TakeDamage(damage / 2, false, true, false, true, true, true);

                break;
            }
        }
    }

    private void Bard()
    {
        foreach (Activity activity in FindObjectsOfType<Activity>())
        {
            if (activity.GetIsActivityCompleted()) continue;
            if (activity.gameObject.name == "Enemy - Skeleton King") continue;

            activity.FinishActivity();

            activity.GetComponent<EnemyController>().SetIsSleeping();

            activity.GetComponentInChildren<TextInfoSpawner>().SpawnNPCBonus("Sleeping...", color);
        }
    }

    private void BlackSmith()
    {
        references.GetPlayerStatistics().AddArmorPNJBonus(5);
        references.GetPlayerHealth().RegainArmor(5);
        references.GetPlayerHealth().DisplayCurrentArmor();

        textInfoSpawner.SpawnNPCBonus("Armor +5", color);
    }

    public void Chef()
    {
        float healAmount = UnityEngine.Random.Range(8, 16);

        GetComponent<PlayerHealth>().Heal(healAmount);
    }

    public void Farmer()
    {
        float lastDistance = Mathf.Infinity;
        EnemyController closestEnemy = null;

        foreach (EnemyController item in FindObjectsOfType<EnemyController>())
        {
            if (item.GetDistance() < lastDistance && !item.GetIsStrongRoots())
            {
                lastDistance = item.GetDistance();
                closestEnemy = item;
            }
        }

        if (closestEnemy)
        {
            closestEnemy.SetStrongRoots(true);

            closestEnemy.GetComponentInChildren<TextInfoSpawner>().SpawnNPCBonus("Imobilize", color);
        }
    }

    public void Miner()
    {
        GameObject pickupsPool = references.GetRoomPool().GetCurrentRoom().GetPickupsPool();

        int numberOfGlitchs = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < numberOfGlitchs; i++)
        {
            Vector2 instantiatePosition = RandomPointInBounds(pickupsPool.GetComponent<BoxCollider2D>().bounds);

            Instantiate(glitchPrefab, instantiatePosition, Quaternion.identity, pickupsPool.transform);
        }

        textInfoSpawner.SpawnNPCBonus("Mining", color);
    }

    private void Priest()
    {
        Vector3 spawnPosition = references.GetRoomPool().GetCurrentRoom().GetPNJSpawnPosition();
        int roomIndex = references.GetRoomPool().GetCurrentRoom().GetRoomIndex();

        if (roomIndex != -1)
        {
            Instantiate(PNJ[roomIndex], spawnPosition, Quaternion.identity);
        }
        else
        {
            int index = UnityEngine.Random.Range(0, PNJ.Length);

            Instantiate(PNJ[index], spawnPosition, Quaternion.identity);
        }

        Instantiate(spawnVFX, new Vector2(spawnPosition.x, spawnPosition.y + 1), Quaternion.identity);
        textInfoSpawner.SpawnNPCBonus("Favor Of God", color);
    }

    public void AddCharge()
    {
        if (currentTrinket == TrinketIndex.None) return;

        currentChargeValue++;

        if (currentChargeValue > chargeValue)
        {
            currentChargeValue = chargeValue;

            return;
        }

        RefreshChargeTxt();
    }

    public void RemoveCharge()
    {
        if (currentTrinket == TrinketIndex.None) return;
        if (!references.GetSimulationsPlaceHolder().GetDamageKeySeries()) return;

        currentChargeValue--;

        if (currentChargeValue < 0)
        {
            currentChargeValue = 0;
        }

        RefreshChargeTxt();
    }

    public IEnumerator SetTrinketIndex(TrinketIndex trinketIndex, bool isEpicValue)
    {
        trinketSwaping = true;
        currentChargeValue = 0;
        currentTrinket = trinketIndex;
        this.isEpicValue = isEpicValue;

        if (trinketIndex == TrinketIndex.None)
        {
            currentChargeValue = 0;
            chargeValue = 0;

            EnableTrinket(false);
        }
        else
        {
            foreach (TrinketSettings trinket in trinketSettings)
            {
                if (trinketIndex == trinket.trinketIndex)
                {
                    if (isEpicValue)
                    {
                        chargeValue = trinket.epicChargeValue;
                    }
                    else
                    {
                        chargeValue = trinket.chargeValue;
                    }

                    if (divideChargeValue)
                    {
                        chargeValue /= 2;
                    }

                    yield return references.GetTrinketDisplay().SetTrinketSprite(trinketIndex);
                }
            }

            EnableTrinket(true);
        }

        trinketSwaping = false;
    }

    public void DivideChargeValue(bool state)
    {
        if (currentTrinket != TrinketIndex.None)
        {
            if (state && !divideChargeValue)
            {
                chargeValue /= 2;

                divideChargeValue = true;
            }
            else if (!state && divideChargeValue)
            {
                chargeValue *= 2;

                divideChargeValue = false;
            }
        }

        if (currentChargeValue > chargeValue)
        {
            currentChargeValue = chargeValue;
        }

        RefreshChargeTxt();
    }

    private void EnableTrinket(bool state)
    {
        chargeValueTxt.enabled = state;

        if (state)
        {
            RefreshChargeTxt();
        }
    }

    private void RefreshChargeTxt()
    {
        if (currentTrinket == TrinketIndex.None)
        {
            chargeValueTxt.enabled = false;
        }
        else
        {
            chargeValueTxt.enabled = true;

            chargeValueTxt.text = currentChargeValue + "/" + chargeValue;
        }

        if (currentChargeValue == chargeValue)
        {
            references.GetTrinketDisplay().TrinketCharged(true, currentTrinket);
        }
        else
        {
            references.GetTrinketDisplay().TrinketCharged(false, currentTrinket);
        }

        references.GetTrinketDisplay().AddCharge();
    }

    public void RechargeTrinket()
    {
        currentChargeValue = chargeValue;

        RefreshChargeTxt();
    }

    public int GetCurrentChargeValue()
    {
        return currentChargeValue;
    }

    public void FullChargeTrinket()
    {
        currentChargeValue = chargeValue;

        RefreshChargeTxt();
    }

    public TrinketIndex GetCurrentTrinket()
    {
        return currentTrinket;
    }

    private Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y));
    }

    public void GainRandomTrinket(bool lostPassive)
    {
        int trinketIndex = UnityEngine.Random.Range(0, trinketIndices.Length);

        while (GetCurrentTrinket() == trinketIndices[trinketIndex])
        {
            trinketIndex = UnityEngine.Random.Range(0, trinketIndices.Length);
        }

        StartCoroutine(SetTrinketIndex(trinketIndices[trinketIndex], false));

        if (lostPassive)
        {
            references.GetPassiveObject().RemovePassiveSlot(true);
        }
    }
}

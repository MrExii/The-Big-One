using Steamworks;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float baseHealth;
    [SerializeField] TextMeshProUGUI healthTxt;
    [SerializeField] TextMeshProUGUI armorTxt;
    [SerializeField] Animator deathVFX;
    [SerializeField] GameObject[] damageVFX;
    [SerializeField] GameObject[] criticalDamageVFX;
    [SerializeField] GameObject[] poisonVFX;
    [SerializeField] ParticleSystem poisonParticle;
    [SerializeField] GameObject hitVFX;
    [SerializeField] Animator animator;
    [SerializeField] TextInfoSpawner textInfoSpawner;
    [SerializeField] ColorGradient colorGradient;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GenerateDamage generateDamage;
    [SerializeField] bool boss;

    CatchReferences references;
    EnemyDependency enemyDependency;

    float currentHealth;
    float baseArmor;
    float currentArmor;
    float timeSinceLastPoisonStart = Mathf.Infinity;
    float poisonEffectDuration = 10f;
    float deathAnimationDuration;

    const float poisonDamagePerSecond = 1.2f;
    const float healOnDeathPercentage = 0.2f;
    const float trappedTimer = 1.2f;
    const float healWeaponAmount = 5f;

    bool isDead;
    bool hitAlready;
    bool shieldUpgrade;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        enemyDependency = GetComponent<EnemyDependency>();
    }

    private void Start()
    {
        healthTxt.enabled = false;
        armorTxt.enabled = false;

        GetDeathAnimationClipDuration();
    }

    private void OnEnable()
    {
        if (gameObject.name != "Enemy - Skeleton King")
        {
            baseHealth *= references.GetSimulationsPlaceHolder().GetEnemyHealthMultiplier() + references.GetGameManager().GetRoomClearMultiplier(true);
        }
        else
        {
            baseHealth *= references.GetPlayerStatistics().GetBaseDamage();

            if (baseHealth > 800)
            {
                baseHealth = 800;
            }
        }

        baseArmor = baseHealth * references.GetSimulationsPlaceHolder().GetEnemyArmor();

        currentHealth = baseHealth;
        currentArmor = baseArmor;
    }

    private void Update()
    {
        Poison();
    }

    private void Poison()
    {
        if (isDead) return;
        if (timeSinceLastPoisonStart > poisonEffectDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1);
            poisonParticle.Stop();

            return;
        }

        if (!poisonParticle.isPlaying)
        {
            poisonParticle.Play();
            spriteRenderer.color = new Color(0, 1, 0);
        }

        float damage = poisonDamagePerSecond * Time.deltaTime;

        currentHealth -= damage;

        IsDead();
        UpdateHealthTxt();

        timeSinceLastPoisonStart += Time.deltaTime;
    }

    public void TakeDamage(float damage, bool isCriticalDamage, bool spawnVFX)
    {
        ProcessDamage(damage, isCriticalDamage, spawnVFX);
        IsDead();
        ApplyWeaponBonus();
        
        if (!enemyDependency.GetEnemyController().GetVillagerGhost())
        {
            references.GetCurse().RemoveCurse(damage * 0.2f);
        }

        if (isDead) return;

        if (spawnVFX && hitVFX)
        {
            GameObject instance = Instantiate(hitVFX, transform);

            if (spriteRenderer.flipX)
            {
                instance.transform.localScale = new(-1, 1, 1);
            }
        }
        
        if (hitAlready) return;

        StartCoroutine(EnemyHit());
    }

    private void IsDead()
    {
        if (currentHealth <= 0)
        {
            isDead = true;

            references.GetGameManager().IncreaseNumberOfKill();

            references.GetSector().IncreaseNumberOfKills();

            GetComponent<Activity>().FinishActivity();

            if (gameObject.name != "Enemy - Skeleton King")
            {
                GetComponent<Animator>().Rebind();
            }
            
            GetAllColliders(false);

            animator.enabled = true;

            if (gameObject.name != "Enemy - Skeleton King")
            {
                animator.SetTrigger("death");
            }

            healthTxt.enabled = false;
            armorTxt.enabled = false;

            float heal = 0;

            if (references.GetPassiveObject().BloodTransfusion())
            {
                heal += references.GetPassiveObject().BloodTransfusionHeal();
            }

            if (references.GetPassiveObject().BloodForge())
            {
                heal += references.GetPassiveObject().BloodForgeHealAmount();
            }

            references.GetPlayerHealth().Heal(heal);

            poisonParticle.Stop();

            Invoke(nameof(DisableGameObject), deathAnimationDuration);

            CheckAchievements();
        }
    }

    private void CheckAchievements()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfKill = references.GetGameManager().GetNumberOfKill();

            if (numberOfKill == 100)
            {
                SteamUserStats.GetAchievement("ACH_KILL_100", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_KILL_100");
                    SteamUserStats.SetAchievement("ACH_KILL_100");
                }
            }
            else if (numberOfKill == 1000)
            {
                SteamUserStats.GetAchievement("ACH_KILL_1000", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_KILL_1000");
                    SteamUserStats.SetAchievement("ACH_KILL_1000");
                }
            }
            else if (numberOfKill == 2000)
            {
                SteamUserStats.GetAchievement("ACH_KILL_2000", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_KILL_2000");
                    SteamUserStats.SetAchievement("ACH_KILL_2000");
                }
            }
            else if (numberOfKill == 5000)
            {
                SteamUserStats.GetAchievement("ACH_KILL_5000", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_KILL_5000");
                    SteamUserStats.SetAchievement("ACH_KILL_5000");
                }
            }

            if (gameObject.name == "Enemy - Skeleton King")
            {
                SteamUserStats.GetAchievement("ACH_SKELETON_KING", out bool achievementUnlock);

                if (!achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_SKELETON_KING");
                    SteamUserStats.SetAchievement("ACH_SKELETON_KING");
                }
            }

            SteamUserStats.StoreStats();
        }
    }

    private IEnumerator EnemyHit()
    {
        hitAlready = true;

        for (int i = 0; i < 8; i++)
        {
            if (isDead)
            {
                spriteRenderer.enabled = true;
                break;
            }
            
            spriteRenderer.enabled = !spriteRenderer.enabled;
            
            yield return new WaitForSeconds(0.125f);
        }

        hitAlready = false;
    }

    private void ApplyWeaponBonus()
    {
        if (references.GetFighter().GetWeaponType() == WeaponType.BrokenSword || 
            references.GetFighter().GetWeaponType() == WeaponType.GlitchedWeapon) return;
        if (!references.GetFighter().CanUsePower()) return;

        if (references.GetFighter().GetWeaponType() == WeaponType.AssassinDagger)
        {
            if (references.GetFighter().GetIsWeaponUpgraded() && poisonEffectDuration != poisonEffectDuration * 2)
            {
                poisonEffectDuration *= 2;
            }

            timeSinceLastPoisonStart = 0f;
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.ChainWhip)
        {
            if (enemyDependency.GetEnemyController().GetIsStrongRoots()) return;

            StartCoroutine(TrappedEnemy());
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.DoubleAxe)
        {
            foreach (EnemyHealth enemy in FindObjectsOfType<EnemyHealth>())
            {
                if (enemy.gameObject.activeInHierarchy && !enemy.GetIsDead())
                {
                    StartCoroutine(DoubleAxe(enemy));
                    
                    return;
                }
            }
            
            references.GetFighter().ResetCanUsePower();
        }
        else if (references.GetFighter().GetWeaponType() == WeaponType.StickOfTruth)
        {
            if (!references.GetFighter().GetIsWeaponUpgraded() && references.GetPlayerHealth().GetCurrentHealth() == 
                references.GetPlayerStatistics().GetBaseHealth())
            {
                references.GetFighter().ResetCanUsePower();
                return;
            }
            else if (references.GetFighter().GetIsWeaponUpgraded())
            {
                references.GetPlayerHealth().GetComponent<PlayerStatistics>().AddBonusHealth(1);
                references.GetPlayerHealth().DisplayCurrentHealth();
            }

            references.GetPlayerHealth().Heal(healWeaponAmount);
        }
    }

    private IEnumerator DoubleAxe(EnemyHealth enemy)
    {
        yield return new WaitForSeconds(0.4f);

        float damage = references.GetFighter().GetDamage(false);
        bool isCritical = references.GetFighter().GetComponent<GenerateDamage>().GetIsCriticalDamage();

        if (references.GetFighter().GetIsWeaponUpgraded())
        {
            enemy.TakeDamage(damage, isCritical, true);
        }
        else
        {
            enemy.TakeDamage(damage / 2, isCritical, true);
        }
    }

    private IEnumerator TrappedEnemy()
    {
        enemyDependency.GetEnemyController().SetStrongRoots(true);

        if (references.GetFighter().GetIsWeaponUpgraded()) yield return new WaitForSeconds(trappedTimer * 2);
        else yield return new WaitForSeconds(trappedTimer);

        enemyDependency.GetEnemyController().SetStrongRoots(false);
    }

    private void ProcessDamage(float damage, bool isCriticalDamage, bool spawnDamageVFX)
    {
        if (currentArmor > 0f)
        {
            currentArmor -= damage * 0.75f;
            currentHealth -= damage * 0.25f;

            if (currentArmor < 0)
            {
                currentHealth += currentArmor;

                armorTxt.enabled = false;
            }
            else
            {
                armorTxt.enabled = true;
            }
        }
        else
        {
            currentHealth -= damage;
        }

        if (spawnDamageVFX)
        {
            DamageVFX(isCriticalDamage);
        }

        textInfoSpawner.SpawnDamage(damage, isCriticalDamage, false);

        UpdateHealthTxt();
    }

    private void DamageVFX(bool isCriticalDamage)
    {
        if (references.GetFighter().GetWeaponType() != WeaponType.AssassinDagger)
        {
            GameObject damageVFXInstance;

            int index = UnityEngine.Random.Range(0, damageVFX.Length);

            if (isCriticalDamage)
            {
                damageVFXInstance = Instantiate(criticalDamageVFX[index], transform.position, Quaternion.identity, transform);
            }
            else
            {
                damageVFXInstance = Instantiate(damageVFX[index], transform.position, Quaternion.identity, transform);
            }

            if (spriteRenderer.flipX)
            {
                damageVFXInstance.transform.localScale *= new Vector2(-1, 1);
            }
        }
        else
        {
            int index = UnityEngine.Random.Range(0, poisonVFX.Length);

            GameObject poisonVFXInstance = Instantiate(poisonVFX[index], transform.position, Quaternion.identity, transform);

            if (spriteRenderer.flipX)
            {
                poisonVFXInstance.transform.localScale *= new Vector2(-1, 1);
            }
        }
    }

    private void UpdateHealthTxt()
    {
        if (!references.GetGameManager().GetNoobMode() || references.GetSimulationsPlaceHolder().GetDisableUI()) return;
        if (isDead)
        {
            EnableHealthAndArmorTxt(false);
            return;
        }

        EnableHealthAndArmorTxt(true);

        float health = currentHealth / baseHealth;
        float armor = currentArmor / baseArmor;

        healthTxt.text = Mathf.CeilToInt(health * 100).ToString() + "%";
        armorTxt.text = Mathf.CeilToInt(armor * 100).ToString() + "%";

        healthTxt.color = colorGradient.UpdateColorGradient(health);
    }

    public void IncreaseHealth(float amount)
    {
        shieldUpgrade = true;

        baseHealth *= amount;
        currentHealth = baseHealth;
    }

    public void DecreaseHealth(float amount)
    {
        shieldUpgrade = false;

        currentHealth /= amount;

        UpdateHealthTxt();
    }

    private void GetAllColliders(bool state)
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = state;
        }
    }

    public bool GetShieldUpgrade()
    {
        return shieldUpgrade;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public GenerateDamage GetGenerateDamage()
    {
        return generateDamage;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetBaseHealth()
    {
        return baseHealth;
    }

    public void EnableHealthAndArmorTxt(bool state)
    {
        if (!references.GetGameManager().GetNoobMode() || references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        if (state && currentHealth == baseHealth)
        {
            return;
        }

        healthTxt.enabled = state;

        if (currentArmor <= 0)
        {
            armorTxt.enabled = false;
        }
        else
        {
            armorTxt.enabled = state;
        }
    }

    private void DisableGameObject()
    {
        if (boss) return;

        if (deathVFX)
        {
            if (spriteRenderer.flipX)
            {
                deathVFX.transform.localPosition = new(deathVFX.transform.localPosition.x * -1, deathVFX.transform.localPosition.y);
            }

            deathVFX.enabled = true;

            deathVFX.GetComponentInChildren<AudioSource>().Play();
        }

        spriteRenderer.enabled = false;
    }

    private void GetDeathAnimationClipDuration()
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Death")
            {
                deathAnimationDuration = clip.length;
            }
        }
    }
}

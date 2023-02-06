using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IUpdateStats
{
    [SerializeField] float timeBetweenHit;
    [SerializeField] GameObject hiVFX;
    [SerializeField] GameObject deathVFX;
    [SerializeField] Transform deathVFXPosition;
    [SerializeField] bool invincible;
    [SerializeField] ParticleSystem healVFX;
    [SerializeField] AudioSource hitAS;
    [SerializeField] AudioClip[] hitClips;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TextInfoSpawner textInfoSpawner;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AnimationClip reviveClip;

    [HideInInspector] public bool isDead = false;

    CatchReferences references;

    float currentHealth;
    float currentArmor;
    float timeSinceLastHit = Mathf.Infinity;
    float secondChance;

    bool isBleeding;
    bool hitAlready;
    bool damageTaken;

    const float bleedingDamagePerSecond = 1.5f;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
    }

    private void Start()
    {
        DisplayCurrentHealth();
        DisplayCurrentArmor();

        if (references.GetHealthDisplay())
        {
            references.GetHealthDisplay().Bleeding(false);
        }
    }

    private void Update()
    {
        Bleeding();
        HealthBonusForLimitedTime();

        timeSinceLastHit += Time.deltaTime;
    }

    private void HealthBonusForLimitedTime()
    {
        if (isDead) return;
        if (references.GetPlayerStatistics().GetHealthBonusDivider() == 0) return;
        if (references.GetRoomPool().GetCurrentRoom().GetCanExitRoom()) return;

        float health = references.GetPlayerStatistics().GetHealthBonusDivider() * Time.deltaTime * -1;

        references.GetPlayerStatistics().AddHealthForLimitedTime(health);

        RefreshCurrentHealth(health);
    }

    private void Bleeding()
    {
        if (SceneManager.GetActiveScene().name == "Tuto") return;
        if (isDead) return;
        if (invincible) return;
        if (references.GetPassiveObject().SteelBandage())
        {
            references.GetHealthDisplay().Bleeding(false);

            return;
        }

        if (isBleeding || references.GetPassiveObject().BloodForge())
        {
            float damage = bleedingDamagePerSecond * Time.deltaTime;

            if (references.GetPassiveObject().BloodForge())
            {
                references.GetPassiveObject().BloodForgeAddDamage(damage);
            }

            currentHealth -= damage;
            DisplayCurrentHealth();

            if (references.GetHealthDisplay())
            {
                references.GetHealthDisplay().Bleeding(true);
            }

            IsDead();
        }
        else
        {
            if (references.GetHealthDisplay())
            {
                references.GetHealthDisplay().Bleeding(false);
            }
        }
    }

    public void RefreshBaseHealth()
    {
        currentHealth = references.GetPlayerStatistics().GetBaseHealth();

        DisplayCurrentHealth();
    }

    private void RefreshBaseArmor()
    {
        currentArmor = references.GetPlayerStatistics().GetArmor();

        DisplayCurrentArmor();
    }

    public void RefreshCurrentHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > references.GetPlayerStatistics().GetBaseHealth())
        {
            currentHealth = references.GetPlayerStatistics().GetBaseHealth();
        }

        DisplayCurrentHealth();
    }

    public void AddSecondChance(float amount)
    {
        secondChance += amount;
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;
        if (currentHealth == references.GetPlayerStatistics().GetBaseHealth()) return;
        if (healAmount <= 0) return;

        healVFX.Play();

        currentHealth += healAmount;

        if (currentHealth > references.GetPlayerStatistics().GetBaseHealth())
        {
            textInfoSpawner.SpawnHeal(healAmount - (currentHealth - references.GetPlayerStatistics().GetBaseHealth()));

            currentHealth = references.GetPlayerStatistics().GetBaseHealth();
        }
        else
        {
            textInfoSpawner.SpawnHeal(healAmount);
        }

        StopBleeding();

        DisplayCurrentHealth();
    }

    public void HealBetweenRoom()
    {
        if (references.GetPlayerStatistics().GetRegenPvBetweenRoom() == 0f) return;

        float healAmount = references.GetPlayerStatistics().GetRegenPvBetweenRoom();

        currentHealth += healAmount;

        if (currentHealth > references.GetPlayerStatistics().GetBaseHealth())
        {
            textInfoSpawner.SpawnHeal(healAmount - (currentHealth - references.GetPlayerStatistics().GetBaseHealth()));

            currentHealth = references.GetPlayerStatistics().GetBaseHealth();
        }
        else
        {
            textInfoSpawner.SpawnHeal(healAmount);
        }

        DisplayCurrentHealth();
    }

    public void RegainArmor(float amount)
    {
        currentArmor += amount;

        if (currentArmor > references.GetPlayerStatistics().GetArmor())
        {
            currentArmor = references.GetPlayerStatistics().GetArmor();
        }
        else if (references.GetPlayerStatistics().GetArmor() == 0)
        {
            references.GetPlayerStatistics().AddArmorPNJBonus(amount);
        }

        DisplayCurrentArmor();
    }

    public void TakeDamage(float damage, bool isCritical, bool hitAnyway, bool isBleeding, bool spawnFX, bool spawnTextInfo, bool ignoreRoll=false)
    {
        if (invincible) return;
        if (isDead) return;
        if (references.GetPlayerController().isRolling && !ignoreRoll) return;
        if (timeSinceLastHit < timeBetweenHit && !hitAnyway) return;

        if (!damageTaken)
        {
            damageTaken = true;
        }

        if (healVFX.isPlaying)
        {
            healVFX.Stop();
        }

        if (isBleeding)
        {
            this.isBleeding = true;
        }

        timeSinceLastHit = 0f;

        ProcessDamage(damage, spawnTextInfo);

        if (IsDead()) return;

        if (spawnFX)
        {
            Instantiate(hiVFX, transform);
            HitSFX();
        }
        
        if (spawnTextInfo)
        {
            textInfoSpawner.SpawnDamage(damage, isCritical, false);
        }

        if (!hitAlready && spawnFX)
        {
            StartCoroutine(PlayerHit());
        }

        DisplayCurrentHealth();
        DisplayCurrentArmor();
    }

    private void HitSFX()
    {
        int index = UnityEngine.Random.Range(0, hitClips.Length);

        hitAS.PlayOneShot(hitClips[index]);
    }

    private bool IsDead()
    {
        if (isDead) return true;

        if (currentHealth <= 0 && secondChance == 0)
        {
            isDead = true;

            StopEnemies();

            references.GetPlayerController().StopPlayer(true);

            animator.SetTrigger("death");

            DisplayCurrentHealth();
            DisplayCurrentArmor();

            CheckAchievement();

            references.GetGameManager().IncreaseNumberOfDeath();

            StartCoroutine(ReturnHome());
        }
        else if (currentHealth <= 0 && secondChance > 0)
        {
            StartCoroutine(Revive());

            if (references.GetCurse().GetCurrentCurse() == 100)
            {
                references.GetCurse().RemoveCurse(50);
            }

            return true;
        }

        return isDead;
    }

    private void StopEnemies()
    {
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.StopEnemy();
        }
    }

    private void CheckAchievement()
    {
        if (references.GetGameManager().GetNoobMode()) return;
        if (references.GetGameManager().GetDisableSteamworks()) return;

        if (SteamManager.Initialized)
        {
            int numberOfDeath = references.GetGameManager().GetNumberOfDeath();

            if (numberOfDeath == 100)
            {
                SteamUserStats.GetAchievement("ACH_DEATH", out bool achievementUnlock);

                if (achievementUnlock)
                {
                    references.GetAchievements().SetAchievement("ACH_DEATH");
                    SteamUserStats.SetAchievement("ACH_DEATH");
                }
            }

            SteamUserStats.StoreStats();
        }
    }

    public void SwitchInvincibility()
    {
        invincible = !invincible;
    }

    private IEnumerator ReturnHome()
    {
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);

        while (Time.timeScale > 0.5f)
        {
            Time.timeScale -= Time.deltaTime;
        }

        yield return new WaitForSeconds(1f);

        Time.timeScale = 1f;

        if (spriteRenderer.flipX)
        {
            deathVFXPosition.localPosition = new(deathVFXPosition.localPosition.x * -1, deathVFXPosition.localPosition.y);
        }

        Instantiate(deathVFX, new Vector2(deathVFXPosition.position.x, deathVFXPosition.position.y + 1.5f), Quaternion.identity);

        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.4f);

        yield return FindObjectOfType<DeathFadeDisplay>().DeathFade();

        yield return StartCoroutine(references.GetGameManager().ReturnHome(true));
    }

    private IEnumerator Revive()
    {
        references.GetPlayerController().disableControl = true;
        references.GetPlayerController().SetIsInActivity(true);
        references.GetPlayerController().StopPlayer(true);

        StopBleeding();

        invincible = true;

        DisplayCurrentHealth();
        DisplayCurrentArmor();

        animator.SetTrigger("death");

        yield return new WaitForSeconds(1f);

        animator.SetTrigger("revive");

        yield return new WaitForSeconds(reviveClip.length);

        currentHealth = references.GetPlayerStatistics().GetBaseHealth() / 2;

        DisplayCurrentHealth();

        secondChance--;

        references.GetPlayerController().disableControl = false;
        references.GetPlayerController().SetIsInActivity(false);

        yield return StartCoroutine(PlayerHit());

        invincible = false;
    }

    private IEnumerator PlayerHit()
    {
        hitAlready = true;

        for (int i = 0; i < 10; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(0.1f);
        }

        hitAlready = false;
    }

    private void ProcessDamage(float damage, bool spawnTextInfo)
    {
        if (currentArmor > 0f)
        {
            if (!references.GetPassiveObject().BedrockArmor())
            {
                currentArmor -= damage * 0.60f;
            }
            
            currentHealth -= damage * 0.40f;

            if (currentArmor < 0)
            {
                currentHealth += currentArmor;
            }
        }
        else
        {
            currentHealth -= damage;
        }

        if (references.GetPassiveObject().BloodForge())
        {
            references.GetPassiveObject().BloodForgeAddDamage(damage);
        }

        if (references.GetPlayerController().GetCursed() && damage != Mathf.Infinity)
        {
            references.GetCurse().AddCurse(damage * 0.2f, spawnTextInfo);
        }
    }

    public void DisplayCurrentHealth()
    {
        if (references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        if (references.GetHealthDisplay())
        {
            references.GetHealthDisplay().RefreshHealth(currentHealth, references.GetPlayerStatistics().GetBaseHealth());
        }
    }

    public void DisplayCurrentArmor()
    {
        if (references.GetSimulationsPlaceHolder().GetDisableUI()) return;

        if (references.GetHealthDisplay())
        {
            references.GetHealthDisplay().RefreshArmor(currentArmor, references.GetPlayerStatistics().GetArmor());
        }
    }

    public float GetSecondChance()
    {
        return secondChance;
    }

    public void StopBleeding()
    {
        if (references.GetPassiveObject().BloodForge()) return;

        if (isBleeding)
        {
            isBleeding = false;
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetCurrentArmor()
    {
        return currentArmor;
    }

    public bool GetDamageTaken()
    {
        return damageTaken;
    }

    public void UpdateStats()
    {
        RefreshBaseHealth();
        RefreshBaseArmor();

        secondChance = references.GetPlayerStatistics().GetSecondChance();
    }
}

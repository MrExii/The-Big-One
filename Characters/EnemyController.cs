using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Controller Settings")]
    [SerializeField] PolygonCollider2D bodyCollider;
    [SerializeField] float contactDamage;
    [SerializeField] Collider2D attackCollider;
    [SerializeField] float attackDamage;
    [SerializeField] bool bleedingAttack;
    [SerializeField] GameObject exclamation;
    [SerializeField] AnimationClip exclamationAnimation;
    [SerializeField] bool villagerGhost;
    [SerializeField] bool orc;
    [SerializeField] bool blockCanAttack;

    CatchReferences references;
    EnemyDependency enemyDependency;

    Vector2 startPosition = new();

    bool canAttack;
    bool enableActivity = true;
    bool isStrongRoots;
    bool isSleeping;
    bool exclamationPop;
    bool stopEnemy;
    bool rebindAnimator;

    private void Awake()
    {
        references = FindObjectOfType<CatchReferences>();
        enemyDependency = GetComponent<EnemyDependency>();

        startPosition = transform.position;

        contactDamage *= references.GetSimulationsPlaceHolder().GetEnemyDamageMultiplier();
        attackDamage *= references.GetSimulationsPlaceHolder().GetEnemyDamageMultiplier();

        DisableExclamation();
    }

    private void LateUpdate()
    {
        if (stopEnemy)
        {
            if (!rebindAnimator)
            {
                enemyDependency.GetEnemyHealth().GetAnimator().Rebind();

                rebindAnimator = true;
            }
            
            return;
        }
        if (enemyDependency.GetEnemyHealth().GetIsDead()) return;

        CheckCollision();

        if (isStrongRoots || isSleeping)
        {
            enemyDependency.GetEnemyHealth().GetAnimator().enabled = false;
            return;
        }
        else
        {
            enemyDependency.GetEnemyHealth().GetAnimator().enabled = true;
        }

        if (canAttack && !references.GetPlayerHealth().isDead)
        {
            if(!exclamationPop && exclamation)
            {
                exclamation.SetActive(true);
                Invoke(nameof(DisableExclamation), exclamationAnimation.length);

                exclamationPop = true;
            }

            Attack();
        }
        else
        {
            Idle();
        }

        if (attackCollider)
        {
            SwitchColliders();
        }
    }

    //Virtual
    public virtual void Idle()
    {

    }

    public virtual void Attack()
    {
        
    }
    //

    //Getter
    public float GetDistance()
    {
        return Vector2.Distance(transform.position, references.GetPlayerController().transform.position);
    }

    public GameObject GetTarget()
    {
        return references.GetPlayerController().gameObject;
    }

    public Animator GetAnimator()
    {
        return enemyDependency.GetEnemyHealth().GetAnimator();
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return enemyDependency.GetEnemyHealth().GetSpriteRenderer();
    }

    public bool GetCanAttack()
    {
        if (blockCanAttack)
        {
            return false;
        }

        return canAttack;
    }

    public Vector2 GetStartPosition()
    {
        return startPosition;
    }

    public bool GetEnableActivity()
    {
        return enableActivity;
    }

    public EnemyHealth GetEnemyHealth()
    {
        return enemyDependency.GetEnemyHealth();
    }

    public GenerateDamage GetGenerateDamage()
    {
        return enemyDependency.GetEnemyHealth().GetGenerateDamage();
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public PolygonCollider2D GetBodyCollider()
    {
        return bodyCollider;
    }

    public bool GetIsStrongRoots()
    {
        return isStrongRoots;
    }

    public bool GetIsSleeping()
    {
        return isSleeping;
    }

    public PlayerController GetPlayerController()
    {
        return references.GetPlayerController();
    }

    public PlayerHealth GetPlayerHealth()
    {
        return references.GetPlayerHealth();
    }

    public CameraShake GetCameraShake()
    {
        return references.GetCameraShake();
    }

    public EndGame GetEndGame()
    {
        return references.GetEndGame();
    }

    public bool GetVillagerGhost()
    {
        return villagerGhost;
    }

    public SimulationsPlaceHolder GetSimulationsPlaceHolder()
    {
        return references.GetSimulationsPlaceHolder();
    }

    public GameManager GetGameManager()
    {
        return references.GetGameManager();
    }

    public SectorTimer GetSectorTimer()
    {
        return references.GetSectorTimer();
    }
    //

    public void DisableExclamation()
    {
        if (!exclamation) return;

        exclamation.SetActive(false);
    }

    private void CheckCollision()
    {
        if (contactDamage == 0 && !villagerGhost) return;

        if (bodyCollider.IsTouching(references.GetPlayerController().GetBodyCollider()))
        {
            float damage = enemyDependency.GetEnemyHealth().GetGenerateDamage().GetRandomDamage(contactDamage, false, false);
            bool isCritical = enemyDependency.GetEnemyHealth().GetGenerateDamage().GetIsCriticalDamage();

            if (orc && references.GetPlayerController().isRolling)
            {
                references.GetPlayerHealth().TakeDamage(damage, isCritical, false, false, true, true, true);
                references.GetPlayerController().StopPlayer(false);
            }
            else
            {
                references.GetPlayerHealth().TakeDamage(damage, isCritical, false, false, true, true);
            }

            if (villagerGhost && !references.GetPlayerController().isRolling)
            {
                enemyDependency.GetEnemyHealth().TakeDamage(Mathf.Infinity, true, false);

                references.GetCurse().AddCurse(10, true);
            }
        }
    }

    public void UnBlockCanAttack()
    {
        blockCanAttack = false;
    }

    public void AddEnemyToList(GameObject enemy)
    {
        references.GetEnemyPool().AddEnemyToList(enemy);
    }

    //Setters
    public void SetCanAttack(bool state)
    {

        canAttack = state;
    }

    public void SetEnableActivity(bool state)
    {
        enableActivity = state;
    }

    public void SetStrongRoots(bool state)
    {
        isStrongRoots = state;
    }

    public void SetIsSleeping()
    {
        isSleeping = true;
    }
    //

    public void FlipSprite(float xPosition)
    {
        if (transform.position.x < xPosition)
        {
            enemyDependency.GetEnemyHealth().GetSpriteRenderer().flipX = true;
        }
        else
        {
            enemyDependency.GetEnemyHealth().GetSpriteRenderer().flipX = false;
        }
    }

    public void StopEnemy()
    {
        stopEnemy = true;
    }

    //use by Attack Animation
    public void AttackEvent()
    {
        if (attackCollider.IsTouching(GetTarget().GetComponent<PlayerController>().GetBodyCollider()))
        {
            float damage = enemyDependency.GetEnemyHealth().GetGenerateDamage().GetRandomDamage(attackDamage, false, false);
            bool isCritical = enemyDependency.GetEnemyHealth().GetGenerateDamage().GetIsCriticalDamage();
            
            references.GetPlayerHealth().TakeDamage(damage, isCritical, false, bleedingAttack, true, true);
        }
    }

    private void SwitchColliders()
    {
        if (enemyDependency.GetEnemyHealth().GetSpriteRenderer().flipX)
        {
            attackCollider.transform.localScale = new Vector3(-1, 1, 1);
            bodyCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            attackCollider.transform.localScale = new Vector3(1, 1, 1);
            bodyCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
